using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using RedditSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Media;

namespace RPuller
{
    class Controller
    {
        private MainWindow MainWindow { get; set; }
        internal int DLDelay { get; set; }
        internal int AmountToFetch { get; set; }

        public Controller(MainWindow main)
        {
            MainWindow = main;
            ImageHistory.Initialize();
            DLDelay = 10;
            AmountToFetch = 1;
        }

        public void StartFetch(string subReddit)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                var reddit = new Reddit();
                List<Subreddit> subs = StringToSubreddits(subReddit).ToList();
                string[] imgurDomains = { "i.imgur.com", "imgur.com" };
                //string[] types = { ".jpg", ".gif", ".png", ".bmp" };

                List<PulledImage> queue = new List<PulledImage>();
                int current = 0;
                int contingousAmount = AmountToFetch;
                bool hit = false;
                
                try
                {
                    while (true)
                    {
                        foreach (Subreddit sub in subs)
                        {
                            queue.Clear();

                            foreach (var post in sub.Hot.Skip(current).Take(contingousAmount))
                            {
                                SetCurrentStatus(post.Url.AbsoluteUri, "Fetching Link");

                                if (imgurDomains.Any(d => post.Domain.EndsWith(d)))
                                {
                                    hit = true;
                                    var imgurResolver = new ImgurResolver(post.Url);

                                    string[] resolvedUris = imgurResolver.ResolveURI().ToArray();
                                    foreach (string res in resolvedUris)
                                        queue.Add(new PulledImage(res));
                                }

                                PutLine(post.Url.AbsoluteUri);
                            }

                            current += contingousAmount;

                            SaveImages(queue);

                            if (hit)
                            {
                                for (int i = DLDelay; i >= 0; i--)
                                {
                                    SetCurrentStatus("Sleeping " + i + "/" + DLDelay, "After-Download-Delay");
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    PutLine(e.Message);
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private IEnumerable<Subreddit> StringToSubreddits(string link)
        {
            Reddit red = new Reddit();
            List<Subreddit> subs = new List<Subreddit>();

            string[] splitted = link.Split(new char[] { '+' });

            foreach(string split in splitted)
                subs.Add(red.GetSubreddit(split));

            return subs.AsEnumerable();
        }

        private void SaveImages(IEnumerable<PulledImage> pulledImages)
        {
            foreach (PulledImage pulled in pulledImages)
            {
                pulled.Download();
                ImageHistory.Add(pulled.Link);
            }
        }

        private void PutLine(object line)
        {
            var log = Log.Get();
            log.PutLine(line);
            MainWindow.ResponseList.Dispatcher.Invoke(() =>
            {
                MainWindow.ResponseList.Text += line.ToString() + "\r\n";
            });
        }

        private void SetCurrentStatus(string status, string statusGroup)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.CurrentAction.Text = statusGroup;
                MainWindow.CurrentFile.Text = status;
            });
        }

        private void SetProgress(int progress)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.CurrentProgress.Value = progress;
            });
        }
    }
}

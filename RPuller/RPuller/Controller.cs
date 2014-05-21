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
        internal bool Save { get; set; }
        internal int DLDelay { get; set; }
        internal int AmountToFetch { get; set; }

        public Controller(MainWindow main)
        {
            MainWindow = main;
            ImageHistory.Initialize();
            DLDelay = 10;
            AmountToFetch = 1;
            Save = false;
        }

        public void StartFetch(string subReddit)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                var reddit = new Reddit();
                Subreddit sub = reddit.GetSubreddit(subReddit);
                string[] domains = { "i.imgur.com", "imgur.com" };
                string[] types = { ".jpg", ".gif", ".png", ".bmp" };

                List<string> queue = new List<string>();
                int current = 0;
                int contingousAmount = AmountToFetch;

                while (true)
                {
                    queue.Clear();

                    foreach (var post in sub.Hot.Skip(current).Take(contingousAmount))
                    {
                        SetCurrentStatus(post.Url.AbsoluteUri, "Fetching Link");
                        var uris = new List<string>();

                        if (domains.Any(d => post.Domain.EndsWith(d)))
                        {
                            var imgurResolver = new ImgurResolver(post.Url);
                            uris.AddRange(imgurResolver.ResolveURI());
                        }
                        else
                            uris.Add(post.Url.AbsoluteUri);

                        if (!ImageHistory.AddRange(uris))
                            foreach (string item in uris)
                                if (!ImageHistory.Add(item))
                                    Debug.WriteLine(item + " could not be added, already contained");
                                else
                                    queue.Add(item);
                        else
                            queue.AddRange(uris);

                        Debug.WriteLine(post.Url.AbsoluteUri);
                    }

                    current += contingousAmount;

                    DisplayImages(queue);

                    if (Save)
                        SaveImages(queue);

                    Thread.Sleep(DLDelay);
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void SaveImages(IEnumerable<string> uris)
        {
            foreach (string uri in uris)
            {
                var image = new BitmapImage(new Uri(uri));
                image.DownloadCompleted += (sender, args) =>
                {
                    string absolute = image.UriSource.AbsoluteUri;

                    Guid id = Guid.NewGuid();
                    BitmapEncoder encoder = null;
                    string location = "";

                    if (absolute.EndsWith("jpg") || absolute.EndsWith("jpeg"))
                    {
                        encoder = new JpegBitmapEncoder();
                        location = id.ToString() + ".jpg";
                    }
                    else if (absolute.EndsWith("png"))
                    {
                        encoder = new PngBitmapEncoder();
                        location = id.ToString() + ".png";
                    }
                    else if (absolute.EndsWith("bmp"))
                    {
                        encoder = new BmpBitmapEncoder();
                        location = id.ToString() + ".bmp";
                    }
                    else if (absolute.EndsWith("gif"))
                    {
                        encoder = new GifBitmapEncoder();
                        location = id.ToString() + ".gif";
                    }
                    else
                    {
                        encoder = new BmpBitmapEncoder();
                        location = id.ToString() + ".bmp";
                    }

                    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image));

                    using (var filestream = new FileStream(location, FileMode.Create))
                    {
                        encoder.Save(filestream);
                        filestream.Flush();
                    }
                };
            }
        }

        private void DisplayImages(IEnumerable<string> uris)
        {
            ClearList();

            int cur = 0;
            double step = 100.0 / uris.Count();

            foreach (string url in uris)
            {
                MainWindow.ResponseList.Dispatcher.Invoke(() =>
                {
                    SetProgress((int)(cur += (int)(step + 0.5d)));
                    AddItem(url);
                    MainWindow.ResponseListScrollView.ScrollToBottom();
                }, System.Windows.Threading.DispatcherPriority.Normal);
                Thread.Sleep(2000);
            }
        }

        private void ClearList()
        {
            MainWindow.ResponseList.Dispatcher.Invoke(() =>
            {
                MainWindow.ResponseList.Children.Clear();
            });
        }

        private void AddItem(string url)
        {
            MainWindow.ResponseList.Dispatcher.Invoke(() =>
            {
                var bitImg = new BitmapImage(new Uri(url));
                SetCurrentStatus(url, "Inserting Image");
                var img = new Image();
                img.Source = bitImg;
                img.MaxHeight = 250;
                MainWindow.ResponseList.Children.Add(img);
            });
        }

        private void SetCurrentStatus(string status, string statusGroup)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.CurrentAction.Text = status;
                MainWindow.CurrentFile.Text = statusGroup;
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

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

namespace RPuller
{
    class Controller
    {
        private MainWindow MainWindow { get; set; }
        private bool FetchComplete { get; set; }

        public Controller(MainWindow main)
        {
            MainWindow = main;
            FetchComplete = true;
        }

        public void StartFetch()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                var reddit = new Reddit();
                Subreddit sub = reddit.GetSubreddit("gaming");

                foreach (var post in sub.Hot.TakeWhile((p) => { return true; }))
                {
                    Thread.Sleep(2000);

                    FetchComplete = false;

                    MainWindow.ResponseList.Dispatcher.Invoke(() =>
                    {
                        MainWindow.ResponseList.Children.Clear();
                        var img = new Image();
                        img.Source = new BitmapImage(post.Url);
                        img.MaxHeight = 500;
                        MainWindow.ResponseList.Children.Add(img);

                        FetchComplete = true;
                    }, System.Windows.Threading.DispatcherPriority.Normal);

                    Debug.WriteLine(post.Url.AbsoluteUri);

                    while (!FetchComplete) Thread.Sleep(100);
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}

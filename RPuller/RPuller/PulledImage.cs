using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace RPuller
{
    class PulledImage
    {
        private static int Counter = 0;

        public string Link { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool IsDownloaded { get; private set; }
        private BitmapImage Image { get; set; }

        public PulledImage(string link)
        {
            Link = link;
            Path = "./PulledImages/";
            Name = "/" + Counter + GetFileType();
            Counter++;
        }

        public void Download()
        {
            try
            {
                if(!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                WebClient client = new WebClient();
                client.DownloadFile(Link, Path + Name);
                IsDownloaded = true;
            }
            catch (Exception)
            {
                IsDownloaded = false;
                if (File.Exists(Path))
                    File.Delete(Path);
            }
        }

        public string GetFileType()
        {
            string extension = System.IO.Path.GetExtension(Link);

            int index = extension.IndexOf("?");
            if(index != -1)
            {
                extension = extension.Substring(0, index);
            }

            return extension;
        }
    }
}

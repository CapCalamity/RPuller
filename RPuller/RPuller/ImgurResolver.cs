using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace RPuller
{
    class ImgurResolver : IResolver
    {
        private Uri BaseURI { get; set; }

        public ImgurResolver(Uri uri)
        {
            BaseURI = uri;
        }

        private IEnumerable<string> ResolveAlbum()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(GetSource());

            var items = doc.DocumentNode.SelectNodes("//div[@class='image']//div[contains(concat(' ', @class, ' '), ' item ')]//a");

            var hrefs = new List<string>(items.Count);

            foreach (var item in items)
                //foreach (var attribute in item.Attributes)
                if (new string[] { "href", "target" }.All(i => item.Attributes.Contains(i)))
                    hrefs.Add("http:" + item.Attributes["href"].Value);

            return hrefs;
        }

        private string ResolveImage()
        {
            string response = GetSource();

            var reg = new Regex("src=\"(?<Path>.*" + BaseURI.AbsolutePath + ".*?)\"");
            Match mat = reg.Match(response);

            if (mat.Success)
            {
                return "http:" + mat.Groups["Path"].Value;
            }
            else
            {
                return "";
            }
        }

        private string GetSource()
        {
            var client = new WebClient();
            return client.DownloadString(BaseURI.AbsoluteUri);
        }

        public IEnumerable<string> ResolveURI()
        {
            if (new string[] { ".jpg", ".jpeg", ".gif", ".bmp", ".png" }.Any(t => BaseURI.AbsolutePath.EndsWith(t)))
            {
                return new string[] { BaseURI.AbsoluteUri };
            }
            //if it is an album
            if (BaseURI.AbsoluteUri.Contains("/a/"))
            {
                return ResolveAlbum();
            }
            else
            {
                return new string[] { ResolveImage() };
            }
        }
    }
}

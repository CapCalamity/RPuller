using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPuller
{
    public class Log
    {
        private static Log Instance = null;
        private static string Path = "./Log.txt";
        private static StreamWriter Stream = null;

        private Log()
        {
            Stream = new StreamWriter(new FileStream(Path, FileMode.Create));
        }

        public static Log Get()
        {
            if (Instance == null)
                Instance = new Log();
            return Instance;
        }

        public void Put(object content)
        {
            Stream.Write(content);
        }

        public void PutLine(object line)
        {
            Stream.WriteLine(line);
        }
    }
}

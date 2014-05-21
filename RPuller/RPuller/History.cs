using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPuller
{
    static class ImageHistory
    {
        //May be replaced with ConcurrentDictionary<string, byte> for thread-safety
        //uses byte as value to waste least possible amount of ram
        private static HashSet<string> History = null;
        private static bool Initialized = false;

        public static void Initialize()
        {
            History = new HashSet<string>();
            Initialized = true;
        }

        public static bool Contains(string item)
        {
            return History.Contains(item);
        }

        public static bool Add(string item)
        {
            return History.Add(item);
        }

        /// <summary>
        /// Attempts to add a range of items to the History, but will not do so if any one item is already contained.
        /// </summary>
        /// <returns>true if all items could be added, otherwise false</returns>
        public static bool AddRange(IEnumerable<string> items)
        {
            foreach (string i in items)
                if (History.Contains(i))
                    return false;

            foreach (string i in items)
                History.Add(i);
            return true;
        }

        public static int Count()
        {
            return History.Count;
        }
    }
}

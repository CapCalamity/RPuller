using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPuller
{
    interface IResolver
    {
        IEnumerable<string> ResolveURI();
    }
}

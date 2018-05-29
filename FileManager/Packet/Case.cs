using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketLibrary
{
    [Serializable]
    public class Case : Packet
    {
        public string caseName;
        public Case(string caseName)
        {
            this.caseName = caseName;
        }
    }
}

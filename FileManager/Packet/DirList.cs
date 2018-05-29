using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketLibrary
{
    [Serializable]
    public class DirList : Packet
    {
        public string[] dirList;
        public DirList(string[] dirList)
        {
            this.dirList = dirList;
        }
    }
}

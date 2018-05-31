using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketLibrary
{
    [Serializable]
    public class DirInfo : Packet
    {
        public string dirName;
        public DirInfo(string dirName)
        {
            this.dirName = dirName;
        }
    }
}

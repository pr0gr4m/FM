using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketLibrary
{
    [Serializable]
    public class ACK : Packet
    {
        public bool isOK;
        public ACK(bool isOK = true)
        {
            this.isOK = isOK;
        }
    }
}

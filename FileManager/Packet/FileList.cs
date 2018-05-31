using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketLibrary
{
    [Serializable]
    public class FileList : Packet
    {
        public FileMeta[] fileList;
        public FileList(FileMeta[] fileList)
        {
            this.fileList = fileList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketLibrary
{
    [Serializable]
    public class FileMeta : Packet
    {
        public long fileLength;
        public string fileName;
        public string md5Hash;
        public string lastModified;

        public FileMeta(long fileLength, string fileName, string md5Hash)
        {
            this.fileLength = fileLength;
            this.fileName = fileName;
            this.md5Hash = md5Hash;
        }

        public FileMeta(long fileLength, string fileName, string md5Hash,
            string lastModified) : this(fileLength, fileName, md5Hash)
        {
            this.lastModified = lastModified;
        }
    }
}

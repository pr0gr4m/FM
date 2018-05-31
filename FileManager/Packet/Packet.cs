using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace PacketLibrary
{
    public enum PacketType
    {
        ACK = 0,
        Case,
        CaseSelected,
        FileMeta,
        ReqDirList,
        ReqFileList,
        EOP     // End of Packet
    }

    public static class PUtility
    {
        public const int BUF_LEN = 4096;
        public const int PORT_NUM = 18888;
        public static string CalculateMD5(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            throw new Exception("There is no NIC for IPv4");
        }
        public static void CompressWithVersion(string fileName)
        {
            FileInfo fileToCompress = new FileInfo(fileName);

            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) &
                    FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(Path.GetFileNameWithoutExtension(fileToCompress.FullName) +
                        "_" + DateTime.Now.ToString("yyMMddHHmmss") + fileToCompress.Extension + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                            CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
            if (File.Exists(Path.GetFileNameWithoutExtension(fileToCompress.FullName) + ".gpg"))
            {
                File.Move(Path.GetFileNameWithoutExtension(fileToCompress.FullName) + ".gpg",
                    Path.GetFileNameWithoutExtension(fileToCompress.FullName) + "_" + DateTime.Now.ToString("yyMMddHHmmss") + ".gpg");
            }
        }
    }

    [Serializable]
    public class Packet
    {
        public int Length;
        public int Type;

        public Packet()
        {
            this.Length = 0;
            this.Type = 0;
        }

        public static byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(PUtility.BUF_LEN);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }

        public static Object Deserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream(PUtility.BUF_LEN);
            foreach (byte b in bt)
            {
                ms.WriteByte(b);
            }

            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            Object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
}

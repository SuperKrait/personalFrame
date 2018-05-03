using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetModel.NetMgr.Tcp
{
    public class DataWriter : IDisposable
    {
        private MemoryStream mem;
        private bool isClose = false;
        //private BinaryWriter writter;
        public DataWriter()
        {
            mem = new MemoryStream();
            //writter = new BinaryWriter(mem);
        }

        public void WriteInt16(short data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteBytes(newData);
        }

        public void WriteInt32(int data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteBytes(newData);
            //writter.Write(data);
            //writter.Flush();
        }

        public void WriteInt64(long data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteBytes(newData);
            //writter.Write(data);
            //writter.Flush();

        }

        public void WriteString(string data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteBytes(newData);
            //writter.Write(data);
            //writter.Flush();
        }

        public void WriteSbyte(sbyte data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteBytes(newData);
            //writter.Write(data);
            //writter.Flush();
        }

        public void WriteBoolean(bool data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteBytes(newData);
            //writter.Write(data);
            //writter.Flush();
        }

        public void WriteByte(byte data)
        {
            byte[] newData = new byte[1];
            newData[0] = data;
            WriteBytes(newData);
        }

        public void WriteFile(byte[] data)
        {
            long size = data.LongLength;
            WriteInt64(size);
            WriteBytes(data);
        }

        [Obsolete("仅限于特殊的byte数组，写入流请用WriteFile")]
        public void WriteBytes(byte[] data)
        {
            mem.Write(data, 0, data.Length);
            mem.Flush();
        }

        public Stream GetData()
        {
            if (mem.Length <= 0)
                return null;
            else
                return mem;
        }

        public void Dispose()
        {
            //writter.Close();
            isClose = true;
            mem.Close();
            mem = null;
        }
        ~DataWriter()
        {
            if(!isClose)
                Dispose();
        }
    }
}

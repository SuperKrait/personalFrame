using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetModel.NetMgr.Tcp
{
    public class DataReaderCommon : IDisposable
    {
        BinaryReader reader;
        private bool isClose = false;
        public DataReaderCommon(Stream stream)
        {
            reader = new BinaryReader(stream);
        }

        public short ReadInt16()
        {
            return reader.ReadInt16();
        }
        public int ReadInt32()
        {
            return reader.ReadInt32();
        }
        public long ReadInt64()
        {
            return reader.ReadInt64();
        }
        public string ReadString()
        {
            return reader.ReadString();
        }
        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }
        public int ReadSbyte()
        {
            return reader.ReadSByte();
        }
        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public Stream ReadFile()
        {
            long fileSize = ReadInt64();
            byte[] data = new byte[fileSize];
            MemoryStream mem = new MemoryStream(data);
            long beenWriteSize = 1;
            int bufferSize = 1024;
            byte[] tmpData = null;

            while (beenWriteSize <= fileSize)
            {
                if (beenWriteSize + bufferSize > fileSize)
                {
                    tmpData = reader.ReadBytes((int)(fileSize - beenWriteSize));
                    mem.Write(tmpData, 0, tmpData.Length);
                    beenWriteSize = fileSize + 1;
                }
                else
                {
                    tmpData = reader.ReadBytes(bufferSize);
                    mem.Write(tmpData, 0, tmpData.Length);
                    beenWriteSize += bufferSize;
                }
                tmpData = null;
            }
            return mem;
        }

        [Obsolete("仅限于特殊的byte数组，读取流请用ReadFile")]
        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        public void Dispose()
        {
            isClose = true;
            reader.Close();
        }

        ~DataReaderCommon()
        {
            if(!isClose)
                Dispose();
        }
    }
}

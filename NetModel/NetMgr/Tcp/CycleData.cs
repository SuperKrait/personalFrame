using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace NetModel.NetMgr.Tcp
{
    /// <summary>
    /// 将数据拆分成包的工具类
    /// 包的格式---|包ID(8位)|时间戳(8位)|所有包数据总长(8位)|Data(1004位)|---包总长度是packetSize
    /// </summary>
    public class CycleData : IDisposable
    {
        /// <summary>
        /// 包体总大小(\个byte)
        /// </summary>
        private int packetSize = 1024;
        /// <summary>
        /// 包头大小(包ID+时间戳+包长度)
        /// </summary>
        private int headSize = 24;

        private object objLock = new object();

        public CycleData(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] tmpData = null;
            int count = packetSize;
            while (count > 0/*count >= bufferSize*/)
            {
                tmpData = CreateNewData();
                count = stream.Read(tmpData, 0, tmpData.Length - headSize);
                length += count;
                //if (count < bufferSize)
                //    break;
                GetPackageData(tmpData, count);
                list.Add(tmpData);
            }
            //if (count > 0)
            //{
            //    byte[] tmpEndData = new byte[count];
            //    for (int i = 0; i < count; i++)
            //    {
            //        tmpEndData[i] = tmpData[i];
            //    }
            //    tmpData = null;
            //    list.Add(tmpEndData);
            //}
            
        }

        public CycleData(byte[] data)
        {
            int count = data.Length;
            byte[] tmpData = null;

            for(int i = 0; count > packetSize; i++)
            {
                tmpData = CreateNewData();
                for (int j = 0; j < tmpData.Length; j++)
                {
                    tmpData[j] = data[i * packetSize + j];
                }
                length += packetSize;
                count -= packetSize;
                GetPackageData(tmpData, packetSize);
                list.Add(tmpData);
            }
            tmpData = CreateNewData();

            for (int j = 0; j < count; j++)
            {
                tmpData[j] = data[data.Length - tmpData.Length + j];
            }
            length += count;
            GetPackageData(tmpData, count);
            list.Add(tmpData);
        }

        private void GetPackageData(byte[] data, int dataRealCount)
        {
            for (int i = headSize + dataRealCount - 1; i >= headSize; i--)
            {
                data[i] = data[i - headSize];
            }
            for (int i = 0; i < headSize; i++)
            {
                data[i] = 0;
            }            
            
        }

        private long length = 0;
        public long Length
        {
            get
            {
                return length;
            }
        }

        private int pIndex = 0;
        public int PIndex
        {
            get
            {
                return pIndex;
            }
            set
            {
                if (value > list.Count - 1|| value < 0)
                    throw new ArgumentException("数组指定包超出数组长度");
                pIndex = value;
            }
        }
        //public byte this[int index]
        //{
        //    get {
        //        return data[index];
        //    }
        //    set
        //    {
        //        if (index >= data.Count)
        //        {
        //            byte[] nulArr = new byte[index - data.Count + 1];
        //            data.AddRange(nulArr);
        //        }
        //        data[index] = value;
        //    }
        //}

        /// <summary>
        /// 单个包最大值，不可以超过int上限
        /// </summary>
        private List<byte[]> list = new List<byte[]>();

        public IEnumerator<byte[]> GetData(long packageId)
        {
            for (; pIndex < list.Count; pIndex++)
            {
                byte[] tmpData = list[pIndex];
                //设置包id
                byte[] idData = BitConverter.GetBytes(packageId);
                int index = 0;
                for (; index < 8; index++)
                {
                    tmpData[index] = idData[index];
                }

                //设置时间戳
                long javaTimeData = System.DateTime.Now.Ticks.GetJavaTime();
                byte[] time = System.BitConverter.GetBytes(javaTimeData);
                for (int j = 0; index < 16; index++, j++)
                {
                    tmpData[index] = time[j];
                }
                //设置包的数据总大小
                byte[] lengthData = BitConverter.GetBytes(length);
                for (int j = 0; index < 16; index++, j++)
                {
                    tmpData[index] = lengthData[j];
                }
                yield return tmpData;
            }
        }
        private byte[] CreateNewData(int bufferSize = 0)
        {
            int tmpBufferSize = bufferSize == 0 ? this.packetSize : bufferSize;
            byte[] data = new byte[tmpBufferSize];
            list.Add(data);
            return data;
        }

        public void Dispose()
        {
            objLock = null;
            list.Clear();
        }

        ~CycleData()
        {
            Dispose();
        }
    }
}

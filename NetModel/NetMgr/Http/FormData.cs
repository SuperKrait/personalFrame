﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetModel.NetMgr.Http
{
    /// <summary>
    /// 表单类
    /// </summary>
    public class FormData : IDisposable
    {
        ~FormData()
        {
            Dispose();
        }
        /// <summary>
        /// 表单KeyValue值列表
        /// 参数名+参数值
        /// </summary>
        private Dictionary<string, string> keyValueDic = new Dictionary<string, string>();
        /// <summary>
        /// 表单文件值列表
        /// 参数名+contentType|文件路径
        /// </summary>
        private List<KeyValuePair<string, string>> fileList = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// 添加表单KeyValue值
        /// </summary>
        /// <param name="key">表单key值</param>
        /// <param name="customValue">表单对应的value值</param>
        public void AddKeyValue(string key, string customValue)
        {
            if (keyValueDic.ContainsKey(key))
            {
                keyValueDic[key] = customValue;
            }
            else
            {
                keyValueDic.Add(key, customValue);
            }
        }
        /// <summary>
        /// 表单KeyValue列表
        /// </summary>
        /// <param name="dic"></param>
        public void AddKeyValue(IDictionary<string, string>dic)
        {
            foreach (var pair in dic)
            {
                AddKeyValue(pair.Key, pair.Value);
            }
        }
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="key">表单Key值</param>
        /// <param name="filePath">格式是ContentType|文件路径.例如:image/jpeg|d:\test.jpg</param>
        public void AddFile(string key, string filePath)
        {
            KeyValuePair<string, string> item = new KeyValuePair<string, string>(key, filePath);
            fileList.Add(item);
        }
        /// <summary>
        /// 表单添加文件
        /// </summary>
        /// <param name="dic">表单文件列表 Key值 + ContentType|文件路径</param>
        public void AddFile(IDictionary<string, string> dic)
        {
            foreach (var pair in dic)
            {
                KeyValuePair<string, string> item = new KeyValuePair<string, string>(pair.Key, pair.Value);
                fileList.Add(item);
            }
        }

        /// <summary>
        /// 将表单写入Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <return>表单的Boundary</return>
        public void GetWordsFromData(Stream stream, string boundary)
        {
            if (stream == null)
                return;

            string beginBoundary = "--" + boundary + "\r\n";
            string endBoundary = "--" + boundary + "--\r\n";
            string builder = "";
            byte[] data;
            string item;
            foreach (var pairs in keyValueDic)
            {
                item = string.Format(beginBoundary + "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n", pairs.Key, pairs.Value);
                data = Encoding.UTF8.GetBytes(item);
                stream.Write(data, 0, data.Length);
                builder += item;
            }


            string filePartHeader =
                beginBoundary + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                 "Content-Type: " + /*application/octet-stream*/ "{2}" + "\r\n\r\n";

            for (int i = 0; i < fileList.Count; i++)
            {
                string[] arrList = fileList[i].Value.Split('|');
                string contentType = arrList[0];
                string fileName = arrList[1];
                fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                item = string.Format(filePartHeader, fileList[i].Key, fileName, contentType);
                data = Encoding.UTF8.GetBytes(item);
                stream.Write(data, 0, data.Length);
                builder += item;
                data = File.ReadAllBytes(arrList[1]);
                stream.Write(data, 0, data.Length);
                builder += "文件内容";

                string str = "\r\n";
                data = Encoding.UTF8.GetBytes(str);
                stream.Write(data, 0, data.Length);
                builder += str;

            }

            data = Encoding.UTF8.GetBytes(endBoundary);
            stream.Write(data, 0, data.Length);
            builder += endBoundary;

            Console.WriteLine(builder);
        }

        public void Dispose()
        {
            keyValueDic.Clear();
            fileList.Clear();
        }
    }
}

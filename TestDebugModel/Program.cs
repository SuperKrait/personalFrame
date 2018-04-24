using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.EventMgr;
using Common.ThreadPool;
using System.Threading;
using System.IO;
using LocolFileModel.CustomFileModel;
using NetModel.NetMgr.Http;
using System.Net;
using DataModel;
using LitJson;

namespace TestDebugModel
{
    class Program
    {
        private static byte[] myData;
        private static byte[] CusData
        {
            get
            {
                return myData;
            }
            set
            {
                myData = value;
                LoadFile(myData);
            }
        }

        static void Main(string[] args)
        {
            //string path = @"E:\素材小样";
            string path = @"E:\素材小样";
            string[] dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] tmpdirs = Directory.GetDirectories(dirs[i]);
                for (int j = 0; j < tmpdirs.Length; j++)
                {
                    string addFont;
                    if ((j + 1) < 10)
                    {
                        addFont = "0" + (j + 1);
                    }
                    else
                        addFont = j.ToString();
                    string nameFront = tmpdirs[j].Substring(0, tmpdirs[j].LastIndexOf('\\'));
                    string nameMiddle = "\\" + nameFront.Substring(nameFront.LastIndexOf('\\') + 1, 2) +  addFont;
                    string nameend = tmpdirs[j].Substring(tmpdirs[j].LastIndexOf('\\') + 3);
                    try
                    {
                        Directory.Move(tmpdirs[j], nameFront + nameMiddle + nameend);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.ToString());
                    }
                    
                }
            }
            Console.WriteLine("好了");
            Console.ReadKey();
        }


        //static void Main(string[] args)
        //{
        //    //string str = "";
        //    //for (int i = 0; i < 10000; i++)
        //    //{
        //    //    str += ("我是最帅的!" + i + "\r\n");
        //    //}
        //    //byte[] data = Encoding.UTF8.GetBytes(str);
        //    //int index = 0;

        //    //FileMgrSimple.WriteFileCycle(@"d:\测试循环写入.txt",
        //    //    delegate()
        //    //    {
        //    //        List<byte> tmpData = new List<byte>();
        //    //        for (int i = 0; i < 10; i++)
        //    //        {
        //    //            if(index < data.Length)
        //    //                tmpData.Add(data[index++]);
        //    //        }
        //    //        if (tmpData.Count > 0)
        //    //            return tmpData.ToArray();
        //    //        else
        //    //            return null;
        //    //    }
        //    //);
        //    //Console.WriteLine("0");

        //    //HttpWebResponse reponseGet = HttpHelper.CreateGetHttpResponse(@"http://d930c1f7.ngrok.io/net_login.jsp", 50000, null, null, null,  ErrorMsgCallBack);

        //    //List<byte> listGet = new List<byte>();
        //    //if (reponseGet != null)
        //    //{
        //    //    using (Stream stream = reponseGet.GetResponseStream())
        //    //    {
        //    //        //int index = 0;
        //    //        byte[] buffer = new byte[1024];
        //    //        while (true)
        //    //        {
        //    //            int count = stream.Read(buffer, 0, buffer.Length);
        //    //            for (int i = 0; i < count; i++)
        //    //            {
        //    //                listGet.Add(buffer[i]);
        //    //            }
        //    //            if (count < buffer.Length)
        //    //            {
        //    //                break;
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //string cookie = reponseGet.Headers["Set-Cookie"];
        //    //Console.WriteLine("cookie = " + cookie);
        //    //Console.ReadKey();

        //    //string[] cookiesInfo = cookie.Split(';');
        //    //string sessionTmp = "";
        //    //for (int i = 0; i < cookiesInfo.Length; i++)
        //    //{
        //    //    if (cookiesInfo[i].IndexOf("JSESSIONID=") == 0)
        //    //    {
        //    //        sessionTmp = cookiesInfo[i];
        //    //    }
        //    //}


        //    //Dictionary<string, string> dic = new Dictionary<string, string>();
        //    //dic.Add("username", "ssc");
        //    //dic.Add("password", "111111");

        //    //Dictionary<HttpRequestHeader, string> headers = new Dictionary<HttpRequestHeader, string>();
        //    //headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
        //    //headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.9");
        //    //headers.Add(HttpRequestHeader.UserAgent, @"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
        //    //headers.Add(HttpRequestHeader.Accept, "*/*");
        //    //headers.Add(HttpRequestHeader.Cookie, sessionTmp);

        //    //Dictionary<string, string> strHeaders = new Dictionary<string, string>();
        //    //strHeaders.Add("X-Requested-With", @"XMLHttpRequest");
        //    //strHeaders.Add("Content-Disposition", "form-data");


        //    //using (FormData formData = new FormData())
        //    //{
        //    //    formData.AddKeyValue(dic);
        //    //    HttpWebResponse reponse = HttpHelper.HttpPostData(@"http://d930c1f7.ngrok.io/user/login", 50000, formData, headers, strHeaders, null, ErrorMsgCallBack);            

        //    //    List<byte> list = new List<byte>();
        //    //    if (reponse != null)
        //    //    {
        //    //        using (Stream stream = reponse.GetResponseStream())
        //    //        {
        //    //            //int index = 0;
        //    //            byte[] buffer = new byte[1024];
        //    //            while (true)
        //    //            {
        //    //                int count = stream.Read(buffer, 0, buffer.Length);
        //    //                for (int i = 0; i < count; i++)
        //    //                {
        //    //                    list.Add(buffer[i]);
        //    //                }
        //    //                if (count < buffer.Length)
        //    //                {
        //    //                    break;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //    Console.WriteLine(Encoding.UTF8.GetString(list.ToArray()));
        //    //    reponse.Close();
        //    //}

        //    //using (FormData formData = new FormData())
        //    //{
        //    //    dic = new Dictionary<string, string>();
        //    //    dic.Add("type", "1");

        //    //    headers = new Dictionary<HttpRequestHeader, string>();
        //    //    headers.Add(HttpRequestHeader.ContentType, "application/x-jpg");
        //    //    headers.Add(HttpRequestHeader.Cookie, sessionTmp);
        //    //    headers.Add(HttpRequestHeader.UserAgent, @"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");


        //    //    strHeaders = new Dictionary<string, string>();

        //    //    formData.AddKeyValue(dic);
        //    //    formData.AddFile("fileName", @"d:\场景A_产品A.jpg");
        //    //    HttpWebResponse reponse1 = HttpHelper.HttpPostData(@"http://d930c1f7.ngrok.io/panorama/batchUpload", 50000, formData, headers, strHeaders, null, ErrorMsgCallBack);
        //    //    List<byte> list = new List<byte>();
        //    //    if (reponse1 != null)
        //    //    {
        //    //        using (Stream stream = reponse1.GetResponseStream())
        //    //        {
        //    //            byte[] buffer = new byte[1024];
        //    //            while (true)
        //    //            {
        //    //                int count = stream.Read(buffer, 0, buffer.Length);
        //    //                for (int i = 0; i < count; i++)
        //    //                {
        //    //                    list.Add(buffer[i]);
        //    //                }
        //    //                if (count < buffer.Length)
        //    //                {
        //    //                    break;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //    Console.WriteLine(Encoding.UTF8.GetString(list.ToArray()));
        //    //    Console.WriteLine("发送完毕");
        //    //    Console.ReadKey();
        //    //}

        //    //try
        //    //{
        //    //    DataCenter.initAll(@"C:\Users\admin\Desktop\测试文件夹测试版");
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine("错误代码，以及错误目录是:" + DataCenter.GetErrorPath());
        //    //    Console.ReadKey();
        //    //}
        //    //JsonData dataScene = JsonMapper.ToJson(DataCenter.GetSceneList());
        //    //FileMgrSimple.WriteFile(FileType.Txt, @"C:\Users\admin\Desktop\测试文件夹测试版\场景数据.txt", Encoding.UTF8.GetBytes(dataScene.ToJson()));
        //    //JsonData dataProduct = JsonMapper.ToJson(DataCenter.GetProductList());
        //    //FileMgrSimple.WriteFile(FileType.Txt, @"C:\Users\admin\Desktop\测试文件夹测试版\产品数据.txt", Encoding.UTF8.GetBytes(dataProduct.ToJson()));
        //    //JsonData dataPano = JsonMapper.ToJson(DataCenter.GetPanoList());
        //    //FileMgrSimple.WriteFile(FileType.Txt, @"C:\Users\admin\Desktop\测试文件夹测试版\全景图数据.txt", Encoding.UTF8.GetBytes(dataPano.ToJson()));
        //}

        private static void ErrorMsgCallBack(string str)
        {
            Console.WriteLine("error======" + str + "\r\n");
        }
        static IEnumerator ShowMyTest()
        {
            int t = 0;
            while (true)
            {
                yield return t;
                t++;
                if (t > 3)
                    break;
            }
        }

        static void ErrorHandler(string errorCode)
        {
            Console.WriteLine(errorCode);
        }
        static void LoadFile(byte[] data)
        {
            string str = System.Text.Encoding.Default.GetString(data);
            Console.Write(str);
        }

        static void ShowLog(object obj)
        {
            EventSystemMgr.SentEvent("MyTest", obj.ToString());
        }

        static void SendTest(string eId, params object[] obj)
        {
            Console.WriteLine(obj[0].ToString());
        }
    }
}

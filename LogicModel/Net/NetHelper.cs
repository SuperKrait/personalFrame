using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModel.NetMgr.Http;
using System.Net;
using System.IO;
using System.Collections;
using LocolFileModel.CustomFileModel;
using System.Diagnostics;

namespace LogicModel.Net
{
    public class NetHelper
    {
        public delegate void RequestFunc();

        private Dictionary<HttpRequestHeader, string> headers;
        private Dictionary<string, string> headerCustom;

        public Dictionary<HttpRequestHeader, string> DefaultHeaders
        {
            get
            {
                if (headers == null)
                {
                    headers = new Dictionary<HttpRequestHeader, string>();
                    headers.Add(HttpRequestHeader.Accept, "*/*");
                    //headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                    headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.9");
                    headers.Add(HttpRequestHeader.UserAgent, @"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                    headers.Add(HttpRequestHeader.Connection, "keep-alive");
                    //headers.Add(HttpRequestHeader.Host, "www.youtecloud.com");
                    //headers.Add(HttpRequestHeader.Referer, "https://www.youtecloud.com/excleupload/");
                }
                return headers;
            }

            set
            {
                headers = value;
            }
        }
        public Dictionary<HttpRequestHeader, string> DefaultHeadersClone
        {
            get
            {
                Dictionary<HttpRequestHeader, string> myHeader = new Dictionary<HttpRequestHeader, string>();
                foreach (var pairs in DefaultHeaders)
                {
                    myHeader.Add(pairs.Key, pairs.Value);
                }
                return myHeader;
            }
        }

        public Dictionary<string, string> HeaderCustom
        {
            get
            {
                if (headerCustom == null)
                {
                    headerCustom = new Dictionary<string, string>();
                    headerCustom.Add("Origin", "https://www.youtecloud.com");
                    headerCustom.Add("X-Requested-With", "XMLHttpRequest");
                }
                return headerCustom;
            }
            set
            {
                headerCustom = value;
            }
        }

        public Dictionary<string, string> HeaderCustomClone
        {
            get
            {
                Dictionary<string, string> myHeader = new Dictionary<string, string>();
                foreach (var pairs in HeaderCustom)
                {
                    myHeader.Add(pairs.Key, pairs.Value);
                }
                return myHeader;
            }
        }

        private static void SendRequest(string url, SetNetMsg callBack, RequestFunc request)
        {
            Common.ThreadPool.ThreadPoolMgrSimple.Start(
                    delegate ()
                    {
                        request();
                    },
                    0,
                    delegate (string msg)
                    {
                        callBack(NetEnum.FAILED, url, msg);
                    }
                    );
        }

        

        public void GetSessionId(string url, SetNetMsg callBack)
        {
            SendRequest(url, callBack, delegate()
            {
                Dictionary<HttpRequestHeader, string> headers = DefaultHeadersClone;
                headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
                Dictionary<string, string> customHeader = HeaderCustomClone;

                HttpWebResponse reponseGet;

                reponseGet = HttpHelper.CreatePostHttpResponse(url, null, 10000, headers, customHeader, null, null, delegate (string errorUrl, string msg)
                {
                    callBack(NetEnum.FAILED, errorUrl, msg);
                });

                if (reponseGet == null)
                {
                    return;
                }

                List<byte> listGet = new List<byte>();
                try
                {
                    if (reponseGet != null)
                    {
                        using (Stream stream = reponseGet.GetResponseStream())
                        {
                            byte[] buffer = new byte[1024];
                            while (true)
                            {
                                int count = stream.Read(buffer, 0, buffer.Length);
                                for (int i = 0; i < count; i++)
                                {
                                    listGet.Add(buffer[i]);
                                }
                                if (count < buffer.Length)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (reponseGet != null)
                    {
                        reponseGet.Close();
                    }
                    callBack(NetEnum.FAILED, url, e.ToString());
                    return;
                }

                if (reponseGet != null)
                {
                    reponseGet.Close();
                }

                string cookie = reponseGet.Headers["Set-Cookie"];

                string[] cookiesInfo = cookie.Split(';');
                string sessionTmp = "";
                for (int i = 0; i < cookiesInfo.Length; i++)
                {
                    if (cookiesInfo[i].IndexOf("JSESSIONID=") == 0)
                    {
                        sessionTmp = cookiesInfo[i];
                    }
                }
                if (!string.IsNullOrEmpty(sessionTmp))
                {
                    callBack(NetEnum.SUCCEED, url, sessionTmp);
                }
                else
                    callBack(NetEnum.FAILED, url, "没有获取到服务器的SessionId");
            }
            );            
        }
        
        public void SendByForm(string url, string sessionId, IDictionary<string, string> keyValueDic, List<KeyValuePair<string, string>> fileList, SetNetMsg callBack, SetUploadSpeedHandle uploadSpeedHandler = null)
        {
            SendRequest(url, callBack, delegate ()
            {
                Dictionary<HttpRequestHeader, string> headers = DefaultHeadersClone;
                Dictionary<string, string> customHeader = HeaderCustomClone;
                //headers.Add(HttpRequestHeader.Cookie, sessionId);

                FormData data = new FormData();

                if (keyValueDic != null)
                    data.AddKeyValue(keyValueDic);

                if (fileList != null)
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        data.AddFile(fileList[i].Key, fileList[i].Value);
                    }

                HttpWebResponse reponseGet = null;
                reponseGet = HttpHelper.HttpPostData(url, 300000, data, headers, customHeader, null, delegate (string errorUrl, string msg)
                {
                    if (reponseGet != null)
                    {
                        reponseGet.Close();
                    }
                    reponseGet = null;
                    callBack(NetEnum.FAILED, errorUrl, msg);
                },
                delegate(string upLoadUrl, int uploadSpeed)
                {
                    if(uploadSpeedHandler != null)
                        uploadSpeedHandler(upLoadUrl, uploadSpeed);
                }
                );

                if (reponseGet == null)
                {
                    return;
                }

                List<byte> listGet = new List<byte>();
                try
                {
                    if (reponseGet != null)
                    {
                        using (Stream stream = reponseGet.GetResponseStream())
                        {
                            byte[] buffer = new byte[1024];
                            while (true)
                            {
                                int count = stream.Read(buffer, 0, buffer.Length);
                                for (int i = 0; i < count; i++)
                                {
                                    listGet.Add(buffer[i]);
                                }
                                if (count < buffer.Length)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (reponseGet != null)
                        reponseGet.Close();
                    callBack(NetEnum.FAILED, url, e.ToString());
                }

                if(reponseGet != null)
                    reponseGet.Close();

                Debug.Print("+++++++++listGet.Count=" + listGet.Count + "++++++++++++++++++");
                if (listGet.Count > 0)
                {
                    string msg = string.Empty;
                    try
                    {
                        msg = Encoding.UTF8.GetString(listGet.ToArray());
                    }
                    catch (Exception e)
                    {
                        msg = e.ToString();
                    }
                    callBack(NetEnum.SUCCEED, url, msg);
                }
                else
                {
                    callBack(NetEnum.SUCCEED, url, string.Empty);
                }
            });
        }

        public void SendByGet(string url, string sessionId, SetNetMsg callBack)
        {
            SendRequest(url, callBack, delegate ()
            {
                Dictionary<HttpRequestHeader, string> headers = DefaultHeadersClone;
                //headers.Add(HttpRequestHeader.Cookie, sessionId);
                Dictionary<string, string> customHeader = HeaderCustomClone;
                HttpWebResponse reponseGet = null;
                reponseGet = HttpHelper.CreatePostHttpResponse(url, null, 10000, headers, customHeader, null, null, delegate (string errorUrl, string msg)
                {
                    if (reponseGet != null)
                        reponseGet.Close();
                    reponseGet = null;
                    callBack(NetEnum.FAILED, errorUrl, msg);
                });

                if (reponseGet == null)
                {
                    return;
                }

                List<byte> listGet = new List<byte>();

                try
                {
                    if (reponseGet != null)
                    {
                        using (Stream stream = reponseGet.GetResponseStream())
                        {
                            byte[] buffer = new byte[1024];
                            while (true)
                            {
                                int count = stream.Read(buffer, 0, buffer.Length);
                                for (int i = 0; i < count; i++)
                                {
                                    listGet.Add(buffer[i]);
                                }
                                if (count < buffer.Length)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (reponseGet != null)
                        reponseGet.Close();
                    callBack(NetEnum.FAILED, url, e.ToString());
                    return;
                }
                if(reponseGet != null)
                    reponseGet.Close();
                callBack(NetEnum.SUCCEED, url, listGet.Count > 0 ? Encoding.UTF8.GetString(listGet.ToArray()) : string.Empty);
            }
            );
        }

        public void GetErrorFile(string url, string sessionId, SetNetMsg callBack, string errorPath)
        {
            SendRequest(url, callBack, delegate ()
            {
                Dictionary<HttpRequestHeader, string> headers = DefaultHeadersClone;
                //headers.Add(HttpRequestHeader.Cookie, sessionId);
                Dictionary<string, string> customHeader = HeaderCustomClone;
                HttpWebResponse reponseGet = null;
                reponseGet = HttpHelper.CreatePostHttpResponse(url, null, 300000, headers, customHeader, null, null, delegate (string errorUrl, string msg)
                {
                    if (reponseGet != null)
                        reponseGet.Close();
                    reponseGet = null;
                    callBack(NetEnum.FAILED, errorUrl, msg);
                });

                if (reponseGet == null)
                {
                    return;
                }

                try
                {
                    if (reponseGet != null)
                    {
                        using (Stream stream = reponseGet.GetResponseStream())
                        {
                            byte[] buffer = new byte[1024];
                            int count = 1024;
                            FileMgrSimple.WriteFileCycle(errorPath,
                                delegate ()
                                {                                    
                                    count = stream.Read(buffer, 0, buffer.Length);
                                    if (count >= 1024)
                                    {
                                        return buffer;
                                    }
                                    else if (count > 0)
                                    {
                                        byte[] data = new byte[count];
                                        for (int i = 0; i < count; i++)
                                        {
                                            data[i] = buffer[i];
                                        }
                                        return data;
                                    }
                                    else
                                        return null;
                                }
                                );
                        }                            
                    }
                }
                catch (Exception e)
                {
                    if (reponseGet != null)
                        reponseGet.Close();
                    callBack(NetEnum.FAILED, url, e.ToString());
                    return;
                }
                if (reponseGet != null)
                    reponseGet.Close();
                callBack(NetEnum.SUCCEED, url, "下载成功");
            }
            );
        }

    }
    /// <summary>
    /// 访问当前地址的回调
    /// </summary>
    /// <param name="code">放心信息代码</param>
    /// <param name="url">访问地址</param>
    /// <param name="msg">返回值</param>
    public delegate void SetNetMsg(NetEnum code, string url, string msg);
    /// <summary>
    /// 返回当前上传速度
    /// </summary>
    /// <param name="url">上传地址</param>
    /// <param name="kbCount">上传速度</param>
    public delegate void SetUploadSpeedHandle(string url, int bCount);
}

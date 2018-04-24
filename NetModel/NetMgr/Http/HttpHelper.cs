using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;

namespace NetModel.NetMgr.Http
{
    public static class HttpHelper
    {
        public const int bufferSize = 1024 * 1024;

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">Post地址</param>
        /// <param name="parameters">传参</param>
        /// <param name="timeout">超时时间（毫秒整数）</param>
        /// <param name="header">常用头</param>
        /// <param name="headerCustom">自定义头</param>
        /// <param name="dataCustom">自定义数据</param>
        /// <param name="cookies"></param>
        /// <param name="errorHandler">报错回调</param>
        /// <returns></returns>
        public static HttpWebResponse CreatePostHttpResponse(string url, 
            IDictionary<string, string> parameters, 
            int timeout, 
            IDictionary<HttpRequestHeader, string> header,
            IDictionary<string, string> headerCustom,
            byte[] dataCustom,
            CookieCollection cookies,
            NetErrorHandle errorHandler
            )
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            //request.ContentType = "image/jpeg";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            WebHeaderCollection tmpHeader = new WebHeaderCollection();

            request.Timeout = timeout;

            if (header != null)
            {
                foreach (var pair in header)
                    tmpHeader.Add(pair.Key, pair.Value);
            }
            if (headerCustom != null)
            {
                foreach (var pair in headerCustom)
                    tmpHeader.Add(pair.Key, pair.Value);
            }
            string[] keys = tmpHeader.AllKeys;
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "Accept":
                        request.Accept = tmpHeader[i];
                        tmpHeader.Remove("Accept");
                        break;
                    case "Connection":
                        //request.Connection = tmpHeader[i];
                        tmpHeader.Remove("Connection");
                        break;
                    case "Content-Length":
                        request.ContentLength = long.Parse(tmpHeader[i]);
                        tmpHeader.Remove("Content-Length");
                        break;
                    case "Content-Type":
                        request.ContentType = tmpHeader[i];
                        tmpHeader.Remove("Content-Type");
                        break;
                    case "Expect":
                        request.Expect = tmpHeader[i];
                        tmpHeader.Remove("Expect");
                        break;
                    case "Date":
                        request.Date = System.DateTime.Parse(tmpHeader[i]);
                        tmpHeader.Remove("Date");
                        break;
                    case "Host":
                        request.Host = tmpHeader[i];
                        tmpHeader.Remove("Host");
                        break;
                    case "If-Modified-Since":
                        request.IfModifiedSince = System.DateTime.Parse(tmpHeader[i]);
                        tmpHeader.Remove("If-Modified-Since");
                        break;
                    case "Range":
                        string[] rangeArr = tmpHeader[i].Split('_');
                        request.AddRange(int.Parse(rangeArr[0]), int.Parse(rangeArr[1]));
                        tmpHeader.Remove("Range");
                        break;
                    case "Referer":
                        request.Referer = tmpHeader[i];
                        tmpHeader.Remove("Referer");
                        break;
                    case "Transfer-Encoding":
                        request.TransferEncoding = tmpHeader[i];
                        tmpHeader.Remove("Transfer-Encoding");
                        break;
                    case "User-Agent":
                        request.UserAgent = tmpHeader[i];
                        tmpHeader.Remove("User-Agent");
                        break;
                }
            }
            request.Headers = tmpHeader;
            //request.coo

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
                
            }

            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    if(data != null && data.Length > 0)
                        stream.Write(data, 0, data.Length);
                    if(dataCustom != null && dataCustom.Length > 0)
                        stream.Write(dataCustom, 0, dataCustom.Length);
                }

                //byte[] data = new byte[bufferSize];
                //while (true)
                //{
                //    stream.Seek(0, SeekOrigin.Begin);
                //    int count = stream.Read(data, 0, data.Length);
                //    if (count > 0)
                //        streamRequest.Write(data, 0, count);
                //    else
                //        break;
                //    Thread.Sleep(0);
                //}
            }

            HttpWebResponse reponse = null;
            try
            {
                reponse = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.ProtocolViolationException e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.Net.WebException e)
            {
                HttpWebResponse errorReponse = e.Response as HttpWebResponse;
                errorHandler(url, e.ToString() + "\r\n" + GetErrorReponseContent(errorReponse));
            }
            catch (System.InvalidOperationException e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.NotSupportedException e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.Exception e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            return reponse;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }


        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreateGetHttpResponse(string url, 
            int timeout, 
            IDictionary<HttpRequestHeader, string> header,
            IDictionary<string, string> headerCustom,
            CookieCollection cookies,
            NetErrorHandle errorHandler)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            request.Timeout = timeout;

            WebHeaderCollection tmpHeader = new WebHeaderCollection();

            if (header != null)
            {
                foreach (var pair in header)
                    tmpHeader.Add(pair.Key, pair.Value);
            }
            if (headerCustom != null)
            {
                foreach (var pair in headerCustom)
                    tmpHeader.Add(pair.Key, pair.Value);
            }
            string[] keys = tmpHeader.AllKeys;
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "Accept":
                        request.Accept = tmpHeader[i];
                        tmpHeader.Remove("Accept");
                        break;
                    case "Connection":
                        //request.Connection = tmpHeader[i];
                        tmpHeader.Remove("Connection");
                        break;
                    case "Content-Length":
                        request.ContentLength = long.Parse(tmpHeader[i]);
                        tmpHeader.Remove("Content-Length");
                        break;
                    case "Content-Type":
                        request.ContentType = tmpHeader[i];
                        tmpHeader.Remove("Content-Type");
                        break;
                    case "Expect":
                        request.Expect = tmpHeader[i];
                        tmpHeader.Remove("Expect");
                        break;
                    case "Date":
                        request.Date = System.DateTime.Parse(tmpHeader[i]);
                        tmpHeader.Remove("Date");
                        break;
                    case "Host":
                        request.Host = tmpHeader[i];
                        tmpHeader.Remove("Host");
                        break;
                    case "If-Modified-Since":
                        request.IfModifiedSince = System.DateTime.Parse(tmpHeader[i]);
                        tmpHeader.Remove("If-Modified-Since");
                        break;
                    case "Range":
                        string[] rangeArr = tmpHeader[i].Split('_');
                        request.AddRange(int.Parse(rangeArr[0]), int.Parse(rangeArr[1]));
                        tmpHeader.Remove("Range");
                        break;
                    case "Referer":
                        request.Referer = tmpHeader[i];
                        tmpHeader.Remove("Referer");
                        break;
                    case "Transfer-Encoding":
                        request.TransferEncoding = tmpHeader[i];
                        tmpHeader.Remove("Transfer-Encoding");
                        break;
                    case "User-Agent":
                        request.UserAgent = tmpHeader[i];
                        tmpHeader.Remove("User-Agent");
                        break;
                }
            }
            request.Headers = tmpHeader;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            HttpWebResponse reponse = null;
            try
            {
                reponse = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.ProtocolViolationException e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.Net.WebException e)
            {
                HttpWebResponse errorReponse = e.Response as HttpWebResponse;
                errorHandler(url, e.ToString() + "\r\n" + GetErrorReponseContent(errorReponse));
            }
            catch (System.InvalidOperationException e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.NotSupportedException e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.Exception e)
            {
                errorHandler(url, e.ToString());
                return null;
            }
            return reponse;
        }

        /// <summary>
        /// 表单提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <param name="form">表单文件</param>
        /// <param name="speedCallBack">当前下载速度回调,毫秒级别回调</param>
        /// <returns></returns>
        public static HttpWebResponse HttpPostData(string url,
            int timeOut,
            FormData form,
            IDictionary<HttpRequestHeader, string> header,
            IDictionary<string, string> headerCustom,
            CookieCollection cookies,
            NetErrorHandle errorHandler,
            GetNetSpeed speedCallBack = null
            )
        {
            Debug.Print("发送开始" + url);
            HttpWebRequest request;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Timeout = timeOut;
            request.Method = "POST";

            WebHeaderCollection tmpHeader = new WebHeaderCollection();

            if (header != null)
            {
                foreach (var pair in header)
                    tmpHeader.Add(pair.Key, pair.Value);
            }
            if (headerCustom != null)
            {
                foreach (var pair in headerCustom)
                    tmpHeader.Add(pair.Key, pair.Value);
            }
            string[] keys = tmpHeader.AllKeys;
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "Accept":
                        request.Accept = tmpHeader[i];
                        tmpHeader.Remove("Accept");
                        break;
                    case "Connection":
                        //request.Connection = tmpHeader[i];
                        tmpHeader.Remove("Connection");
                        break;
                    case "Content-Length":
                        request.ContentLength = long.Parse(tmpHeader[i]);
                        tmpHeader.Remove("Content-Length");
                        break;
                    case "Content-Type":
                        request.ContentType = tmpHeader[i];
                        tmpHeader.Remove("Content-Type");
                        break;
                    case "Expect":
                        request.Expect = tmpHeader[i];
                        tmpHeader.Remove("Expect");
                        break;
                    case "Date":
                        request.Date = System.DateTime.Parse(tmpHeader[i]);
                        tmpHeader.Remove("Date");
                        break;
                    case "Host":
                        request.Host = tmpHeader[i];
                        tmpHeader.Remove("Host");
                        break;
                    case "If-Modified-Since":
                        request.IfModifiedSince = System.DateTime.Parse(tmpHeader[i]);
                        tmpHeader.Remove("If-Modified-Since");
                        break;
                    case "Range":
                        string[] rangeArr = tmpHeader[i].Split('_');
                        request.AddRange(int.Parse(rangeArr[0]), int.Parse(rangeArr[1]));
                        tmpHeader.Remove("Range");
                        break;
                    case "Referer":
                        request.Referer = tmpHeader[i];
                        tmpHeader.Remove("Referer");
                        break;
                    case "Transfer-Encoding":
                        request.TransferEncoding = tmpHeader[i];
                        tmpHeader.Remove("Transfer-Encoding");
                        break;
                    case "User-Agent":
                        request.UserAgent = tmpHeader[i];
                        tmpHeader.Remove("User-Agent");
                        break;
                }
            }
            request.Headers = tmpHeader;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            string boundary = "-----------" + DateTime.Now.Ticks.ToString("x");

            request.ContentType = "multipart/form-data; boundary=" + boundary;

            MemoryStream stream = new MemoryStream();
            form.GetWordsFromData(stream, boundary);
            request.ContentLength = stream.Length;

            try
            {
                using (Stream streamRequest = request.GetRequestStream())
                {
                    byte[] data = new byte[bufferSize];
                    stream.Seek(0, SeekOrigin.Begin);
                    int dataTotolCount = 0;
                    while (true)
                    {                                 
                        int count = stream.Read(data, 0, data.Length);
                        dataTotolCount = dataTotolCount + count;
                        if (count > 0)
                            streamRequest.Write(data, 0, count);
                        else
                            break;
                        Thread.Sleep(0);
                        if (speedCallBack != null)
                        {
                            speedCallBack(url, count);
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                stream.Close();
                errorHandler(url, e.ToString());
                return null;
            }


            HttpWebResponse reponse = null;
            
            try
            {
                reponse = request.GetResponse() as HttpWebResponse;
                stream.Close();
            }
            catch (System.Net.ProtocolViolationException e)
            {
                stream.Close();
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.Net.WebException e)
            {
                stream.Close();
                HttpWebResponse errorReponse = e.Response as HttpWebResponse;
                errorHandler(url, e.ToString() + "\r\n" + GetErrorReponseContent(errorReponse));
            }
            catch (System.InvalidOperationException e)
            {
                stream.Close();
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.NotSupportedException e)
            {
                stream.Close();
                errorHandler(url, e.ToString());
                return null;
            }
            catch (System.Exception e)
            {
                stream.Close();
                errorHandler(url, e.ToString());
                return null;
            }
            Debug.Print("发送完毕" + url);
            return reponse;
        }

        private static string GetErrorReponseContent(HttpWebResponse errorReponse)
        {
            List<byte> listGet = new List<byte>();
            
            if (errorReponse != null)
            {
                using (Stream stream = errorReponse.GetResponseStream())
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
            if (listGet.Count <= 0)
                return string.Empty;
            return (Encoding.UTF8.GetString(listGet.ToArray()));            
        }
    }
    

    public delegate void NetErrorHandle(string url, string errorMsg);
    public delegate void GetNetSpeed(string url, int bCount);
}

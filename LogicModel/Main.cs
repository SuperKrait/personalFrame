using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ThreadPool;
using LogicModel.UI;
using DataModel;
using DataModel.CustomDataManager;
using System.IO;
using DataModel.FileDataManager;
using LogicModel.Excel;
using LogicModel.Net;
using LitJson;
using System.Threading;
using Common.EventMgr;
using LocolFileModel.CustomFileModel;
using System.Diagnostics;
using ZipModel;

namespace LogicModel
{
    public  class Main
    {
        private Main()
        {
            ;
        }
        ~Main()
        {
            Destory();
        }

        private static Main _instance;
        public static Main Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Main();
                return _instance;
            }
        }            

        #region 模块相关

        private  void ModelInit()
        {
            Common.ThreadPool.ThreadPoolMgrSimple.Init(100, 20, 5);
        }

        private  void ModelDestory()
        {
            Common.ThreadPool.ThreadPoolMgrSimple.Destory();
        }
        #endregion

        #region 数据相关

        public bool GeneratePanoData(string dirPath, bool isGetThumbnailTex = false, int width = 0, int height = 0)
        {
            UIHelper.ShowUploadUI();
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 0);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "数据准备中");            
            try
            {
                DataCenter.initAll(dirPath);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 30);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "Excel表生成中...，如有提示覆盖，请点击确认");
                GenExcel();
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 60);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源提取中...");
                CreateSourceDir();
                
            }
            catch(Exception e)
            {
                string error = DataCenter.GetErrorPath();
                if (!string.IsNullOrEmpty(error))
                    SetErrorMsg(error);
                else
                    SetErrorMsg("请重新点击开始\r\n" + e.ToString());
                return false;                
            }

            string errorCode = MoveSource(isGetThumbnailTex, width, height);
            if (string.IsNullOrEmpty(errorCode))
            {
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 100);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源准备完毕准备，请选择是否压缩上传...");                
            }
            else
            {
                SetErrorMsg(errorCode);
                return false;
            }
            Thread.Sleep(2000);
            UIHelper.CloseUploadUI();
            EventSystemMgr.SentEvent(EventSystemConst.MainBtnEnable);
            return true;

        }

        private  SceneInfo GetScene(string sceneId)
        {
            return DataCenter.GetSceneDic()[sceneId];
        }

        private  ProductInfo GetProduct(string productId)
        {
            return DataCenter.GetProductDic()[productId];
        }

        private string tmpRootDir = "Data";
        private  string tmpTexDirPath = "TmpPano";
        private  string tmpSceneDirPath = "TmpScene";
        private  string tmpProductDirPath = "TmpProduct";
        private  string excelPath = System.Environment.CurrentDirectory + "\\tmpExcel.xlsx";
        private string zipPath = "Data.zip";

        public  void GenExcel()
        {
            if (File.Exists(excelPath))
            {
                File.Delete(excelPath);
            }
            ExcelHelper excel = new ExcelHelper(excelPath);
            foreach (var pairs in DataCenter.GetPanoDic())
            {            
                SceneInfo scene = GetScene(pairs.Value.relateSceneIds[0]);
                excel.WriteKey("场景类目", scene.sceneType[0]);
                excel.WriteKey("场景名称", scene.Name);

                for (int j = 0; j < 3; j++)
                {
                    if (j < scene.flag.Count)
                        excel.WriteKey("场景标签" + (j + 1), scene.flag[j].flagTypeName + ":" + scene.flag[j].flagValue);
                    else
                        excel.WriteKey("场景标签" + (j + 1), "/");
                }


                ProductInfo product = GetProduct(pairs.Value.relateProductIds[0]);
                excel.WriteKey("商品类目", product.productType);
                excel.WriteKey("商品名称", product.Name);

                for (int j = 0; j < 6; j++)
                {
                    if (j < product.flag.Count)
                        excel.WriteKey("商品标签" + (j + 1), product.flag[j].flagTypeName + ":" + product.flag[j].flagValue);
                    else
                        excel.WriteKey("商品标签" + (j + 1), "/");
                }
                //excel.WriteKey("商品标签1", "8");
                //excel.WriteKey("商品标签2", "9");
                //excel.WriteKey("商品标签3", "10");
                //excel.WriteKey("商品标签4", "11");
                //excel.WriteKey("商品标签5", "12");
                //excel.WriteKey("商品标签6", "13");
                excel.WriteKey("商品金额", "0/件");
                excel.WriteKey("商品使用数量", "1");
                excel.WriteKey("商品说明", Encoding.UTF8.GetString(File.ReadAllBytes(product.ProductContentPath)));

            }
            excel.QuitExcel();           
            
        }

        public  void CreateSourceDir()
        {
            //UIHelper.ShowUploadUI();
            if (Directory.Exists(tmpTexDirPath))
            {
                Debug.Print("删除文件夹" + tmpTexDirPath);
                DeleteDir(tmpTexDirPath);                
            }
            if (Directory.Exists(tmpSceneDirPath))
            {
                Debug.Print("删除文件夹" + tmpSceneDirPath);
                DeleteDir(tmpSceneDirPath);
            }
            if (Directory.Exists(tmpProductDirPath))
            {
                Debug.Print("删除文件夹" + tmpProductDirPath);
                DeleteDir(tmpProductDirPath);
            }
            Debug.Print("创建文件夹" + tmpTexDirPath);
            Directory.CreateDirectory(tmpTexDirPath);
            Debug.Print("创建文件夹" + tmpSceneDirPath);
            Directory.CreateDirectory(tmpSceneDirPath);
            Debug.Print("创建文件夹" + tmpProductDirPath);
            Directory.CreateDirectory(tmpProductDirPath);
            Debug.Print("创建文件夹完毕" );


        }

        private void DeleteDir(string path)
        {
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
            string[] dir = Directory.GetDirectories(path);
            for (int i = 0; i < dir.Length; i++)
            {
                DeleteDir(dir[i]);
                Directory.Delete(dir[i]);
            }
        }

        public  string MoveSource(bool isGetThumbnailTex, int width, int height)
        {
            foreach (var pairs in DataCenter.GetSceneDic())
            {
                if (string.IsNullOrEmpty(pairs.Value.ThumbnailPath))
                {
                    return "场景名称--->" + pairs.Value.Name + "<---缩略图找不到";
                }
                CustomFileInfo customFileInfo = DataCenter.GetFileDic()[pairs.Value.ThumbnailPath];
                string newPath = tmpSceneDirPath + "\\" + pairs.Value.Name + customFileInfo.ExName;


                if (isGetThumbnailTex)
                {
                    System.Drawing.Image image = PictureModel.PictureHelper.GetImage(File.ReadAllBytes(pairs.Value.ThumbnailPath));
                    if (image.Width <= width && image.Height <= height)
                    {
                        File.Copy(pairs.Value.ThumbnailPath, newPath, true);
                        pairs.Value.ThumbnailPath = newPath;
                    }
                    else
                    {
                        image = PictureModel.PictureHelper.GetThumbnail(image, width, height);
                        byte[] data = PictureModel.PictureHelper.GetBytes(image, customFileInfo.ExName);
                        FileMgrSimple.WriteFile(FileType.Texture, newPath, data);
                    }
                }
                else
                {
                    File.Copy(pairs.Value.ThumbnailPath, newPath, true);
                    pairs.Value.ThumbnailPath = newPath;
                }
            }

            foreach (var pairs in DataCenter.GetProductDic())
            {
                if (string.IsNullOrEmpty(pairs.Value.ThumbnailPath))
                {
                    return "产品名称名称--->" + pairs.Value.Name + "<---缩略图找不到";
                }


                CustomFileInfo customFileInfo = DataCenter.GetFileDic()[pairs.Value.ThumbnailPath];
                string newPath = tmpProductDirPath + "\\" + pairs.Value.Name + customFileInfo.ExName;

                if (isGetThumbnailTex)
                {
                    System.Drawing.Image image = PictureModel.PictureHelper.GetImage(File.ReadAllBytes(pairs.Value.ThumbnailPath));
                    if (image.Width <= width && image.Height <= height)
                    {
                        File.Copy(pairs.Value.ThumbnailPath, newPath, true);
                        pairs.Value.ThumbnailPath = newPath;
                    }
                    else
                    {
                        image = PictureModel.PictureHelper.GetThumbnail(image, width, height);
                        byte[] data = PictureModel.PictureHelper.GetBytes(image, customFileInfo.ExName);
                        FileMgrSimple.WriteFile(FileType.Texture, newPath, data);
                    }
                }
                else
                {
                    File.Copy(pairs.Value.ThumbnailPath, newPath, true);
                    pairs.Value.ThumbnailPath = newPath;
                }
            }

            foreach (var pairs in DataCenter.GetPanoDic())
            {
                if (string.IsNullOrEmpty(pairs.Value.PanoTexturePath))
                {
                    return "产品名称名称--->" + pairs.Value.Name + "<---图找不到";
                }
                

                CustomFileInfo customFileInfo = DataCenter.GetFileDic()[pairs.Value.PanoTexturePath];
                string newPath = tmpTexDirPath + "\\" + pairs.Value.Name + ".jpg";

                //if (isGetThumbnailTex)
                //{
                //    System.Drawing.Image image = PictureModel.PictureHelper.GetImage(File.ReadAllBytes(pairs.Value.PanoTexturePath));
                //    image = PictureModel.PictureHelper.GetThumbnail(image, width, height);
                //    byte[] data = PictureModel.PictureHelper.GetBytes(image, ".jpg");
                //    FileMgrSimple.WriteFile(FileType.Texture, newPath, data);
                //}
                //else
                //{
                File.Copy(pairs.Value.PanoTexturePath, newPath, true);
                pairs.Value.PanoTexturePath = newPath;
                //}
            }
            return string.Empty;
        }



        #endregion

        #region 网络相关

        private  string serverIp = @"https://www.youtecloud.com/";
        private  string sessionId = "";
        private  NetHelper httpHelper = new NetHelper();

        /// <summary>
        /// 测试服务器是否开启，并且获取session
        /// </summary>
        public  void TestServerOpened()
        {            
            httpHelper.GetSessionId(serverIp + @"net_login.jsp",
                delegate (NetEnum code, string url, string msg)
                {
                    if (code == NetEnum.SUCCEED)
                    {
                        sessionId = msg;                        
                    }
                    else
                    {
                        SetErrorMsg("网络不太好，请检查该Url能打开，并且最好重启本工具" + "url= " + url + "\r\n" + "msg= " + msg);
                    }
                }
            );
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public  void Login(string username, string password)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                SetErrorMsg("请先测试服务器！");
                EventSystemMgr.SentEvent(EventSystemConst.LoginFailed);
                return;
            }

            Dictionary<string, string> keyDic = new Dictionary<string, string>();
            keyDic.Add("username", username);
            keyDic.Add("password", password);
            httpHelper.SendByForm(serverIp + @"user/login", sessionId, keyDic, null,
                delegate (NetEnum code, string url, string msg)
                {
                    if (code == NetEnum.SUCCEED)
                    {
                        JsonData json = JsonMapper.ToObject(msg);
                        if ((bool)json["success"])
                        {
                            //UIHelper.ShowMainUI();
                            UIHelper.CloseLoginUI(true);                            
                        }
                        else
                        {
                            SetErrorMsg(json["msg"].ToString());
                            EventSystemMgr.SentEvent(EventSystemConst.LoginFailed);
                        }
                    }
                    else
                    {
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                        EventSystemMgr.SentEvent(EventSystemConst.LoginFailed);
                    }
                },
                new SetUploadSpeedHandle(GetUploadSpeed)
                );
        }

        public void UploadZip()
        {
            UIHelper.ShowUploadUI();
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 30);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源正在上传，请耐心等待...");

            Dictionary<string, string> keyValueDic = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> fileList = new List<KeyValuePair<string, string>>();

            KeyValuePair<string, string> item = new KeyValuePair<string, string>("file", "application/x-zip-compressed|" + zipPath);

            fileList.Add(item);

            httpHelper.SendByForm(serverIp + @"excleupload/readExcle", sessionId,
                keyValueDic, fileList,
                delegate (NetEnum code, string url, string msg)
                {
                    //Debug.Print("============收到消息1===============" + System.DateTime.Now);
                    if (code == NetEnum.SUCCEED)
                    {
                        if (msg[0] == '{')
                        {
                            JsonData json = JsonMapper.ToObject(msg);
                            if ((bool)json["success"])
                            {
                                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 100);
                                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源上传成功...");
                                Thread.Sleep(2000);
                                EventSystemMgr.SentEvent(EventSystemConst.MainBtnEnable);
                                UIHelper.CloseUploadUI();
                            }
                            else
                            {

                                string errorMsg = json["msg"].ToString();
                                SetErrorMsg("url= " + url + "\r\n" + "msg= " + errorMsg);
                                //httpHelper.GetErrorFile(serverIp + @"excleupload/dowloaderror", sessionId,
                                //delegate (NetEnum tmpCode, string errorUrl, string Msg)
                                //{
                                //    if (code == NetEnum.SUCCEED)
                                //    {
                                //        SetErrorMsg("生成的Excel有问题，请检查每一个生成的资源文件夹，以及Excel表,报错的error.xls已经下载完毕\r\n" + errorMsg);
                                //    }
                                //    else
                                //    {
                                //        SetErrorMsg("生成的Excel有问题，请检查每一个生成的资源文件夹，以及Excel表\r\n" + Msg);

                                //        //isSucceed = false;
                                //        //threadSwitch.Set();
                                //        //SetErrorMsg("服务器访问出错，请重新上传，若还失败，请重新启动本工具!" + "url= " + url + "\r\n" + "msg= " + msg);
                                //    }

                                //}, @"error.xls");
                            }
                        }
                        else
                        {
                            SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                        }
                    }
                    else
                    {
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                    }
                    

                },
                new SetUploadSpeedHandle(GetUploadSpeed)
                );
            
        }


        /*关闭之前的上传逻辑
         * 2018/04/20
         * 
         * 
        /// <summary>
        /// 上传数据
        /// </summary>
        public  void UploadFile()
        {
            StartLogUpSpeed();
            Debug.Print("开始获取速度成功!");
            System.Threading.ManualResetEvent threadSwitch = new System.Threading.ManualResetEvent(false);
            bool isSucceed = true;

            //EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 30);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "正在访问服务器请稍后...");

            #region 服务器要求逻辑

            //服务器要求必须先访问该地址，才可以进行上传逻辑
            httpHelper.SendByGet(serverIp + @"excleupload/", sessionId,
                delegate (NetEnum code, string url, string msg)
                {
                    if (code == NetEnum.SUCCEED)
                    {
                        threadSwitch.Set();
                    }
                    else
                    {
                        isSucceed = false;
                        threadSwitch.Set();
                        SetErrorMsg("服务器访问出错，请重新上传，若还失败，请重新启动本工具!" + "url= " + url + "\r\n" + "msg= " + msg);
                    }

                });
            threadSwitch.WaitOne();

            //失败返回
            if (!isSucceed)
                return;

            #endregion            

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 35);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "正在上传excel验证请稍后...");

            List<KeyValuePair<string, string>> fileList;//文件列表
            Dictionary<string, string> keyDic;//keyValue字典

            #region 上传Excel验证

            threadSwitch.Reset();
            //写入excel文件
            fileList = new List<KeyValuePair<string, string>>();
            fileList.Add(new KeyValuePair<string, string>("excelfile", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet|" + excelPath));
            //写入excel中所需值
            keyDic = new Dictionary<string, string>();

            //写入所有全景图\平面图的名称
            string panoList = "";
            List<PanoInfo> panoInfo = new List<PanoInfo>();
            foreach (var pairs in DataCenter.GetPanoDic())
            {
                panoList += pairs.Value.Name + ",";
                panoInfo.Add(pairs.Value);
            }
            panoList = panoList.Substring(0, panoList.Length - 1);
            keyDic.Add("panoramaNames", panoList);

            //写入所有产品名称
            string productList = "";
            List<ProductInfo> productInfo = new List<ProductInfo>();
            foreach (var pairs in DataCenter.GetProductDic())
            {
                productList += pairs.Value.Name + ",";
                productInfo.Add(pairs.Value);
            }
            productList = productList.Substring(0, productList.Length - 1);
            keyDic.Add("productNames", productList);

            //写入所有场景名称
            string sceneList = "";
            List<SceneInfo> sceneInfo = new List<SceneInfo>();
            foreach (var pairs in DataCenter.GetSceneDic())
            {
                sceneList += pairs.Value.Name + ",";
                sceneInfo.Add(pairs.Value);
            }
            sceneList = sceneList.Substring(0, sceneList.Length - 1);
            keyDic.Add("sceneNames", sceneList);

            
            //发送协议
            httpHelper.SendByForm(serverIp + @"excleupload/readExcle", sessionId,
                keyDic, fileList,
                delegate (NetEnum code, string url, string msg)
                {
                    //Debug.Print("============收到消息1===============" + System.DateTime.Now);
                    if (code == NetEnum.SUCCEED)
                    {
                        JsonData json = JsonMapper.ToObject(msg);
                        if ((bool)json["success"])
                        {
                            threadSwitch.Set();
                        }
                        else
                        {
                            isSucceed = false;
                            threadSwitch.Set();
                            string errorMsg = json["msg"].ToString();

                            httpHelper.GetErrorFile(serverIp + @"excleupload/dowloaderror", sessionId,
                            delegate (NetEnum tmpCode, string errorUrl, string Msg)
                            {
                                if (code == NetEnum.SUCCEED)
                                {
                                    SetErrorMsg("生成的Excel有问题，请检查每一个生成的资源文件夹，以及Excel表,报错的error.xls已经下载完毕\r\n" + errorMsg);
                                }
                                else
                                {
                                    SetErrorMsg("生成的Excel有问题，请检查每一个生成的资源文件夹，以及Excel表\r\n" + Msg);

                                    //isSucceed = false;
                                    //threadSwitch.Set();
                                    //SetErrorMsg("服务器访问出错，请重新上传，若还失败，请重新启动本工具!" + "url= " + url + "\r\n" + "msg= " + msg);
                                }

                            }, @"error.xls");
                        }
                    }
                    else
                    {
                        isSucceed = false;
                        threadSwitch.Set();
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                    }
                    

                },
                new SetUploadSpeedHandle(GetUploadSpeed)
                );
            //Debug.Print("============收到消息2===============" + System.DateTime.Now);
            threadSwitch.WaitOne();
            //失败返回
            if (!isSucceed)
                return;
            threadSwitch.Reset();
            #endregion
            //Debug.Print("============收到消息3===============" + System.DateTime.Now);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 40);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "excel验证成功，正在上传全景图，可能时间比较久，请耐心等待...");
            //Debug.Print("============收到消息4===============" + System.DateTime.Now);
            #region 上传全景图
            object countLock = new object();
            int totolCount = panoInfo.Count;
            int errorCount = 0;
            for (int i = 0; i < panoInfo.Count; i++)
            {

                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, (40 + i * 100 / totolCount / 4));
                if(errorCount == 0)
                    EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, string.Format("正在上传全景图:{0}，请耐心等待...", panoInfo[i].Name));
                else
                    EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, string.Format("上传全景图:{0}失败，正在第{1}次重试，请耐心等待...", panoInfo[i].Name, errorCount));
                fileList = new List<KeyValuePair<string, string>>();
                fileList.Add(new KeyValuePair<string, string>("file", "image/jpeg|" + panoInfo[i].PanoTexturePath));

                keyDic = new Dictionary<string, string>();
                keyDic.Add("type", panoInfo[i].PanoType);

                int indexJ = i;

                httpHelper.SendByForm(serverIp + @"excleupload/panoramaAdd", sessionId,
                keyDic, fileList,
                delegate (NetEnum code, string url, string msg)
                {
                   if (code == NetEnum.SUCCEED)
                   {
                        try
                        {
                            errorCount = 0;
                            JsonData json = JsonMapper.ToObject(msg);
                            if ((bool)json["success"])
                            {
                                //EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, string.Format("上传全景图:{0}成功！", panoInfo[indexJ].Name));
                                threadSwitch.Set();
                            }
                            else
                            {
                                threadSwitch.Set();
                                //isSucceed = false;
                                //isSucceed = false;
                                //threadSwitch.Set();
                                SetWarning("上传全景图失败   url= " + url + "\r\n" + "msg= " + msg);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Print("上传全景图返回有误+++++++++++++++++++++++++++++++++" + msg);
                            threadSwitch.Set();
                        }
                        
                    }
                   else
                   {
                        if (errorCount < 3)
                        {
                            threadSwitch.Set();
                            i--;
                            errorCount++;
                        }
                        else
                        {
                            isSucceed = false;
                            threadSwitch.Set();
                            SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                        }
                   }                 

                },
                new SetUploadSpeedHandle(GetUploadSpeed));
                //Debug.Print("============收到消息i===============" + System.DateTime.Now + "=========" + i);


                threadSwitch.WaitOne();
                if (!isSucceed)
                    return;
                threadSwitch.Reset();

            }

            //Debug.Print("============收到消息over===============" + System.DateTime.Now);
            //threadSwitch.WaitOne();
            //if (!isSucceed)
                return;
            #endregion

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 65);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "全景图上传完毕，正在上传商品图，请耐心等待...");

            #region 上传产品图
            //threadSwitch.Reset();

            fileList = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < productInfo.Count; i++)
            {
                string contentType = string.Empty;
                CustomFileInfo item = new CustomFileInfo(productInfo[i].ThumbnailPath, "");
                if (item.ExName == ".jpg")
                {
                    contentType = "image/jpeg|";
                }
                else if (item.ExName == ".png")
                {
                    contentType = "image/png|";
                }
                else
                {
                    SetErrorMsg("产品图" + item.FileName + item.ExName + "不是jpg或者png格式，请转换格式后使用");
                    return;
                }
                fileList.Add(new KeyValuePair<string, string>("productionfile", contentType + productInfo[i].ThumbnailPath));
            }
            threadSwitch.Reset();
            for (int i = 0; i < 1; i++)
            {
                if(errorCount > 0)
                    EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, string.Format("商品图上传失败，正在第{0}次重试，请耐心等待...", errorCount));
                httpHelper.SendByForm(serverIp + @"excleupload/productAdd", sessionId, null, fileList,
                    delegate (NetEnum code, string url, string msg)
                    {
                        if (code == NetEnum.SUCCEED)
                        {
                            try
                            {
                                JsonData json = JsonMapper.ToObject(msg);
                                if ((bool)json["success"])
                                {
                                    errorCount = 0;
                                    threadSwitch.Set();
                                }
                                else
                                {
                                    //isSucceed = false;
                                    errorCount = 3;
                                    SetWarning("商品上传失败  url= " + url + "\r\n" + "msg= " + msg);
                                    threadSwitch.Set();
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Print("商品图上传成功个，但是返回有误+++++++++++++++++++" + msg);
                                threadSwitch.Set();
                            }
                        }
                        else
                        {
                            if (errorCount < 3)
                            {
                                i--;
                                errorCount++;
                            }
                            {
                                isSucceed = false;
                                threadSwitch.Set();
                                SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                            }
                        }
                        

                    },
                    new SetUploadSpeedHandle(GetUploadSpeed)
                    );
                threadSwitch.WaitOne();
                threadSwitch.Reset();
            }

            if (!isSucceed)
                return;
            #endregion

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 80);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "商品图上传完毕，正在上传封面图，再耐心点就快好了...");
            #region 上传场景
            
            fileList = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < sceneInfo.Count; i++)
            {
                string contentType = string.Empty;
                CustomFileInfo item = new CustomFileInfo(sceneInfo[i].ThumbnailPath, "");
                if (item.ExName == ".jpg")
                {
                    contentType = "image/jpeg|";
                }
                else if (item.ExName == ".png")
                {
                    contentType = "image/png|";
                }
                else
                {
                    SetErrorMsg("场景图" + item.FileName + item.ExName + "不是jpg或者png格式，请转换格式后使用");
                    return;
                }
                fileList.Add(new KeyValuePair<string, string>("scenefile", contentType + sceneInfo[i].ThumbnailPath));
            }

            for (int i = 0; i < 1; i++)
            {
                if (errorCount > 0)
                    EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, string.Format("封面图上传失败，正在第{0}次重试，请耐心等待...", errorCount));
                httpHelper.SendByForm(serverIp + @"excleupload/sceneAdd", sessionId, null, fileList,
                    delegate (NetEnum code, string url, string msg)
                    {
                        if (code == NetEnum.SUCCEED)
                        {
                            try
                            {
                                JsonData json = JsonMapper.ToObject(msg);
                                if ((bool)json["success"])
                                {
                                    errorCount = 0;
                                    threadSwitch.Set();
                                }
                                else
                                {
                                    errorCount = 3;
                                    SetWarning("场景上传出现问题，请检查数据资源是否有误!url= " + url + "\r\n" + "msg= " + msg);
                                    threadSwitch.Set();

                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Print("上传场景图成功，但是返回有误++++++++++", msg);
                                threadSwitch.Set();
                            }
                        }
                        else
                        {
                            if (errorCount < 3)
                            {
                                errorCount++;
                                i--;
                            }
                            isSucceed = false;
                            threadSwitch.Set();
                            SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                        }
                        

                    },
                    new SetUploadSpeedHandle(GetUploadSpeed)
                    );
                threadSwitch.WaitOne();
                threadSwitch.Reset();
            }


            if (!isSucceed)
                return;
            #endregion


            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 95);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "所有图片上传完毕，正在发布...");
            #region 发布所有信息
            httpHelper.SendByGet(serverIp + @"excledownload/publishAll", sessionId,
                delegate (NetEnum code, string url, string msg)
                {
                    if (code == NetEnum.SUCCEED)
                    {
                        
                    }
                    else
                    {
                        isSucceed = false;
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                    }
                });
            #endregion

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 100);
            if (isSucceed)
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "恭喜上传成功！");
            else
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "发布失败，请手动发布，或者重新上传，或者联系管理员！");

            threadSwitch.Set();
            EndLogUpSpeed();
            Debug.Print("获取速度停止成功!");
            Thread.Sleep(2000);
            UIHelper.CloseUploadUI();
            EventSystemMgr.SentEvent(EventSystemConst.MainBtnEnable);

        }
        */


        private object speedHandleLock = new object();
        private DateTime lastTime = DateTime.Now;
        /// <summary>
        /// 获取上传速度
        /// </summary>
        private void GetUploadSpeed(string url, int speed)
        {
            lock (speedHandleLock)
            {
                lastTime = System.DateTime.Now;
                Debug.Print(string.Format("当前url-->{0}上传速度为{1}B,时间是{2}", url, speed, lastTime.ToString()));
            }
        }
        



        #endregion

        #region 压缩相关

        /// <summary>
        /// 压缩成数据包
        /// </summary>
        public void CompressByZIP()
        {
            UIHelper.ShowUploadUI();
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 0);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源准备完毕准备，正在压缩请稍后...");
            ThreadPoolMgrSimple.Start(
                delegate ()
                {

                    if (!Directory.Exists(tmpTexDirPath))
                    {
                        SetErrorMsg("没有找到准备好的全景图文件夹 = " + tmpTexDirPath);
                        return;
                    }
                    else if (!Directory.Exists(tmpSceneDirPath))
                    {
                        SetErrorMsg("没有找到准备好的封面图文件夹 = " + tmpSceneDirPath);
                        return;
                    }
                    else if (!Directory.Exists(tmpProductDirPath))
                    {
                        SetErrorMsg("没有找到准备好的商品图文件夹 = " + tmpProductDirPath);
                        return;
                    }
                    else if (!File.Exists(excelPath))
                    {
                        SetErrorMsg("没有找到准备好的excel = " + excelPath);
                        return;
                    }

                    string excelFileName = excelPath.Substring(excelPath.LastIndexOf('\\') + 1);
                    try
                    {
                        if (Directory.Exists(tmpRootDir))
                        {
                            Directory.Delete(tmpRootDir, true);
                        }

                        Directory.CreateDirectory(tmpRootDir);

                        Directory.Move(tmpTexDirPath, tmpRootDir + "\\" + tmpTexDirPath);

                        Directory.Move(tmpSceneDirPath, tmpRootDir + "\\" + tmpSceneDirPath);

                        Directory.Move(tmpProductDirPath, tmpRootDir + "\\" + tmpProductDirPath);

                        
                        File.Move(excelPath, tmpRootDir + "\\" + excelFileName);

                        EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 50);
                        EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "图量较大，压缩时间可能比较久，请耐心等候...");

                        if (File.Exists(zipPath))
                        {
                            File.Delete(zipPath);
                        }

                        if (!ZipModel.ZipHelper.ZipDirectory(tmpRootDir, zipPath, ""))
                        {
                            SetErrorMsg("添加压缩文件失败，请检查文件是否有缺失。");
                        }
                        else
                        {
                            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 100);
                            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "恭喜压缩完毕成功!");
                        }
                    }
                    catch(Exception e)
                    {
                        if (Directory.Exists(tmpRootDir + "\\" + tmpTexDirPath))
                        {
                            Directory.Move(tmpRootDir + "\\" + tmpTexDirPath, tmpTexDirPath);
                        }

                        if (Directory.Exists(tmpRootDir + "\\" + tmpSceneDirPath))
                        {
                            Directory.Move(tmpRootDir + "\\" + tmpSceneDirPath, tmpSceneDirPath);
                        }

                        if (Directory.Exists(tmpRootDir + "\\" + tmpProductDirPath))
                        {
                            Directory.Move(tmpRootDir + "\\" + tmpProductDirPath, tmpProductDirPath);
                        }

                        if (File.Exists(tmpRootDir + "\\" + excelFileName))
                        {
                            File.Move(tmpRootDir + "\\" + excelFileName, excelPath);
                        }

                        if (File.Exists(zipPath))
                        {
                            File.Delete(zipPath);
                        }
                        SetErrorMsg("文件操作出了问题，正在回退操作，请稍后" + e.ToString());

                    }

                    Thread.Sleep(2000);
                    UIHelper.CloseUploadUI();
                    EventSystemMgr.SentEvent(EventSystemConst.MainBtnEnable);
                },
                2, delegate (string msg)
                {
                    SetErrorMsg(msg);
                });

        }

        #endregion


        #region 日志以及错误相关
        private  object errorObj = new object();
        public  void SetErrorMsg(string errorMsg)
        {
            lock (errorObj)
            {
                UIHelper.ShowErrorUI(errorMsg);
                Console.WriteLine("============error =============================\r\n " + errorMsg);

                EventSystemMgr.SentEvent(EventSystemConst.MainBtnEnable);
                UIHelper.CloseUploadUI();
            }
        }
        private  object warnObj = new object();
        public  void SetWarning(string warnMsg)
        {
            lock (warnObj)
            {
                Console.WriteLine("============warning =============================\r\n " + warnMsg);
            }
        }

        #endregion

        #region 初始化

        public  void Init()
        {
            UIHelper.ShowInitUI();
            ModelInit();
            Thread.Sleep(500);
            UIHelper.CloseInitUI();
            UIHelper.ShowLoginUI();            
        }

        #endregion

        #region 程序销毁相关

        public  void Destory()
        {
            ModelDestory();
        }

        #endregion
    }
}

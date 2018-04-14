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

        public bool GeneratePanoData(string dirPath)
        {
            UIHelper.ShowUploadUI();
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 0);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "数据准备中");            
            try
            {
                DataCenter.initAll(dirPath);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 10);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "Excel表生成中...，如有提示覆盖，请点击确认");
                GenExcel();
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 20);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源提取中...");
                CreateSourceDir();
                
            }
            catch(Exception e)
            {
                string error = DataCenter.GetErrorPath();
                if (!string.IsNullOrEmpty(error))
                    SetErrorMsg(error);
                else
                    SetErrorMsg(e.ToString());
                return false;                
            }

            string errorCode = MoveSource();
            if (string.IsNullOrEmpty(errorCode))
            {
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 30);
                EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "资源准备完毕准备上传...");
                Thread.Sleep(1000);
                return true;
            }
            else
            {
                SetErrorMsg(errorCode);
                return false;
            }

        }

        private  SceneInfo GetScene(string sceneId)
        {
            return DataCenter.GetSceneDic()[sceneId];
        }

        private  ProductInfo GetProduct(string productId)
        {
            return DataCenter.GetProductDic()[productId];
        }

        private  string tmpTexDirPath = "TmpPano";
        private  string tmpSceneDirPath = "TmpScene";
        private  string tmpProductDirPath = "TmpProduct";
        private  string excelPath = System.Environment.CurrentDirectory + "\\tmpExcel.xlsx";

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
                excel.WriteKey("商品说明", Encoding.Default.GetString(File.ReadAllBytes(product.ProductContentPath)));

            }
            excel.QuitExcel();           
            
        }

        public  void CreateSourceDir()
        {
            if (Directory.Exists(tmpTexDirPath))
            {
                Directory.Delete(tmpTexDirPath, true);
            }
            if (Directory.Exists(tmpSceneDirPath))
            {
                Directory.Delete(tmpSceneDirPath, true);
            }
            if (Directory.Exists(tmpProductDirPath))
            {
                Directory.Delete(tmpProductDirPath, true);
            }
            Directory.CreateDirectory(tmpTexDirPath);
            Directory.CreateDirectory(tmpSceneDirPath);
            Directory.CreateDirectory(tmpProductDirPath);
        }

        public  string MoveSource()
        {
            foreach (var pairs in DataCenter.GetSceneDic())
            {
                if (string.IsNullOrEmpty(pairs.Value.ThumbnailPath))
                {
                    return "场景名称--->" + pairs.Value.Name + "<---缩略图找不到";
                }
                File.Copy(pairs.Value.ThumbnailPath, tmpSceneDirPath + "\\" + pairs.Value.Name + DataCenter.GetFileDic()[pairs.Value.ThumbnailPath].ExName, true);
                pairs.Value.ThumbnailPath = tmpSceneDirPath + "\\" + pairs.Value.Name + DataCenter.GetFileDic()[pairs.Value.ThumbnailPath].ExName;
            }

            foreach (var pairs in DataCenter.GetProductDic())
            {
                if (string.IsNullOrEmpty(pairs.Value.ThumbnailPath))
                {
                    return "产品名称名称--->" + pairs.Value.Name + "<---缩略图找不到";
                }
                File.Copy(pairs.Value.ThumbnailPath, tmpProductDirPath + "\\" + pairs.Value.Name + DataCenter.GetFileDic()[pairs.Value.ThumbnailPath].ExName, true);
                pairs.Value.ThumbnailPath = tmpProductDirPath + "\\" + pairs.Value.Name + DataCenter.GetFileDic()[pairs.Value.ThumbnailPath].ExName;
            }

            foreach (var pairs in DataCenter.GetPanoDic())
            {
                if (string.IsNullOrEmpty(pairs.Value.PanoTexturePath))
                {
                    return "产品名称名称--->" + pairs.Value.Name + "<---图找不到";
                }
                File.Copy(pairs.Value.PanoTexturePath, tmpTexDirPath + "\\" + pairs.Value.Name + ".jpg"/*DataCenter.GetFileDic()[pairs.Value.PanoTexturePath].ExName*/, true);
                pairs.Value.PanoTexturePath = tmpTexDirPath + "\\" + pairs.Value.Name + ".jpg";
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
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
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
                }
                );
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        public  void UploadFile()
        {            
            System.Threading.ManualResetEvent threadSwitch = new System.Threading.ManualResetEvent(false);
            bool isSucceed = true;

            //EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 30);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "正在访问服务器请稍后...");

            #region 服务器要求逻辑

            //服务器要求必须先访问该地址，才可以进行上传逻辑
            httpHelper.SendByGet(serverIp + @"excleupload/", sessionId,
                delegate (NetEnum code, string url, string msg)
                {
                    threadSwitch.Set();
                    if (code == NetEnum.SUCCEED)
                    {

                    }
                    else
                    {
                        isSucceed = false;
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
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
                    if (code == NetEnum.SUCCEED)
                    {
                        JsonData json = JsonMapper.ToObject(msg);
                        if ((bool)json["success"])
                        {

                        }
                        else
                        {
                            isSucceed = false;
                            string errorMsg = json["msg"].ToString();
                            SetErrorMsg(errorMsg);
                        }
                        threadSwitch.Set();
                    }
                    else
                    {
                        isSucceed = false;
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                    }
                });
            threadSwitch.WaitOne();
            //失败返回
            if (!isSucceed)
                return;
            threadSwitch.Reset();
            #endregion

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 40);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "excel验证成功，正在上传全景图，可能时间比较久，请耐心等待...");

            #region 上传全景图
            object countLock = new object();
            int totolCount = panoInfo.Count;
            for (int i = 0; i < panoInfo.Count; i++)
            {
                fileList = new List<KeyValuePair<string, string>>();
                fileList.Add(new KeyValuePair<string, string>("file", "image/jpeg|" + panoInfo[i].PanoTexturePath));

                keyDic = new Dictionary<string, string>();
                keyDic.Add("type", panoInfo[i].PanoType);

                httpHelper.SendByForm(serverIp + @"excleupload/panoramaAdd", sessionId,
                keyDic, fileList,
                delegate (NetEnum code, string url, string msg)
                {
                    lock (countLock)
                    {
                        totolCount--;
                    }
                    if (totolCount <= 0)
                    {
                        threadSwitch.Set();
                    }
                    if (code == NetEnum.SUCCEED)
                   {                        
                        JsonData json = JsonMapper.ToObject(msg);
                       if ((bool)json["success"])
                       {
                           
                       }
                       else
                       {
                            //isSucceed = false;
                            SetWarning(json["msg"].ToString());
                        }
                        
                    }
                   else
                   {
                       isSucceed = false;
                       SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                   }
                });
                
            }
            threadSwitch.WaitOne();
            if (!isSucceed)
                return;
            #endregion

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 65);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "全景图上传完毕，正在上传商品图，请耐心等待...");

            #region 上传产品图
            threadSwitch.Reset();

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
            httpHelper.SendByForm(serverIp + @"excleupload/productAdd", sessionId, null, fileList,
                delegate (NetEnum code, string url, string msg)
                {
                    if (code == NetEnum.SUCCEED)
                    {                       
                        JsonData json = JsonMapper.ToObject(msg);
                        if ((bool)json["success"])
                        {
                            
                        }
                        else
                        {
                            //isSucceed = false;
                            SetWarning(json["msg"].ToString());
                        }
                        threadSwitch.Set();
                    }
                    else
                    {
                        isSucceed = false;
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                    }
                });
            threadSwitch.WaitOne();

            if (!isSucceed)
                return;
            #endregion

            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressBar, 80);
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "全景图上传完毕，正在上传封面图，再耐心点就快好了...");
            #region 上传场景
            threadSwitch.Reset();
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

            
            httpHelper.SendByForm(serverIp + @"excleupload/sceneAdd", sessionId, null, fileList,
                delegate (NetEnum code, string url, string msg)
                {
                    if (code == NetEnum.SUCCEED)
                    {
                        JsonData json = JsonMapper.ToObject(msg);
                        if ((bool)json["success"])
                        {

                        }
                        else
                        {
                            //isSucceed = false;
                            SetWarning(json["msg"].ToString());
                        }
                        threadSwitch.Set();
                    }
                    else
                    {
                        isSucceed = false;
                        SetErrorMsg("url= " + url + "\r\n" + "msg= " + msg);
                    }
                });

            threadSwitch.WaitOne();

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
            EventSystemMgr.SentEvent(EventSystemConst.UpdateUploadProgressLable, "恭喜上传成功！");

            Thread.Sleep(2000);
            UIHelper.CloseUploadUI();

            EventSystemMgr.SentEvent(EventSystemConst.MainBtnEnable);

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

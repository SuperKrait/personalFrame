using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.EventMgr;
using Common.ThreadPool;
using LogicModel;
using System.Threading;
namespace UI
{
    static class Program
    {
        static ManualResetEvent MainThreadMgr = new ManualResetEvent(false);
        static event MainThreadCall callList;

        static bool isMainOpen = false;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            RegisterPanel();
            LogicModel.Main.Instance.Init();
            MainThreadMgr.WaitOne();
            MainThreadMgr.Reset();

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            loginPanel = new LoginForm();
            Application.Run(loginPanel);

            if (!isMainOpen)
            {
                LogicModel.Main.Instance.Destory();
                return;
            }
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            mainPanel = new MainForm();
            Application.Run(mainPanel);
        }

        #region 注册界面开关属性

        private static void RegisterPanel()
        {
            EventSystemMgr.RegisteredEvent(EventSystemConst.OpenPanel_InitPanel, OpenInitPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.OpenPanel_LoginPanel, OpenLoginPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.OpenPanel_MainPanel, OpenMainPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.OpenPanel_UploadPanel, OpenUploadPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.OpenPanel_ErrorPanel, ShowErrorPanel);


            EventSystemMgr.RegisteredEvent(EventSystemConst.ClosePanel_InitPanel, CloseInitPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.ClosePanel_LoginPanel, CloseLoginPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.ClosePanel_MainPanel, CloseMainPanel);
            EventSystemMgr.RegisteredEvent(EventSystemConst.ClosePanel_UploadPanel, CloseUploadPanel);

        }

        #endregion

        #region 初始化界面
        private static InitForm initPanel;

        private static void OpenInitPanel(string eId, params object[] objs)
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            initPanel = new InitForm();
            Application.Run(initPanel);
        }

        private static void CloseInitPanel(string eId, params object[] objs)
        {
            if(initPanel != null)
                initPanel.CloseForm();
        }
        #endregion

        #region 登录界面

        private static LoginForm loginPanel;

        private static void OpenLoginPanel(string eId, params object[] objs)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //loginPanel = new LoginForm();
            //Application.Run(loginPanel);
            MainThreadMgr.Set();
        }

        private static void CloseLoginPanel(string eId, params object[] objs)
        {
            if(!isMainOpen)
                isMainOpen = (bool)objs[0];

            if (loginPanel != null)
                loginPanel.CloseForm();
        }

        #endregion

        #region 主界面

        private static MainForm mainPanel;

        private static void OpenMainPanel(string eId, params object[] objs)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //mainPanel = new MainForm();
            //Application.Run(mainPanel);
            
            //MainThreadMgr.Set();
        }

        private static void CloseMainPanel(string eId, params object[] objs)
        {
            
            mainPanel.CloseForm();
        }       

        #endregion

        #region 上传界面

        private static UploadForm upLoadPanel;

        private static void OpenUploadPanel(string eId, params object[] objs)
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            upLoadPanel = new UploadForm();
            Application.Run(upLoadPanel);
        }

        private static void CloseUploadPanel(string eId, params object[] objs)
        {
            if(upLoadPanel != null)
                upLoadPanel.CloseForm();
        }

        #endregion

        #region 报错界面

        private static void ShowErrorPanel(string eId, params object[] objs)
        {
            string msg = objs[0].ToString();
            MessageBox.Show(msg);
            CloseUploadPanel(string.Empty);
        }

        #endregion

        private static void GetDirPath(MainThreadCall mainCall)
        {
            if (mainCall != null)
            {
                callList += mainCall;
            }
        }

        
    }
    public delegate void MainThreadCall();
}

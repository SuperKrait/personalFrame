using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.EventMgr;
using System.Threading;
using LogicModel;

namespace UI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            Init();
            EventSystemMgr.RegisteredEvent(EventSystemConst.UpdateMainUI, SendUICmd);
            EventSystemMgr.RegisteredEvent(EventSystemConst.LoginFailed, LoginFailed);
            //EventSystemMgr.SentEvent(EventSystemConst.MainThreadSwitch);
            //注册当前form关闭的事件
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OnApplicationClose);
            Main.Instance.TestServerOpened();
        }



        /// <summary>
        /// 当前窗口关闭激活事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationClose(object sender, FormClosingEventArgs e)
        {
            EventSystemMgr.SentEvent(EventSystemConst.ClosePanel_LoginPanel, false);
            //loginBtn.Enabled = true;
            EventSystemMgr.UnRegisteredEvent(EventSystemConst.UpdateMainUI, SendUICmd);
            Clear();
        }

        public void CloseForm()
        {
            CallUpdateUIHandle handle = new CallUpdateUIHandle(
                delegate (string test1, object test2)
                {
                    this.Dispose();
                });
            if (this.InvokeRequired)
            {
                this.Invoke(handle, string.Empty, null);
            }
            else
            {
                handle(string.Empty, null);
            }
        }
        #region testBox
        private void ShowTestTxt(object obj)
        {
            string str = obj.ToString();
            TestTBX.Text += str + "\r\n";
        }

        private void ChangeLoginBtn(object obj)
        {
            loginBtn.Enabled = (bool)obj;
        }


        #endregion

        #region 通用UI更新模块

        private delegate void UIUseHandle(object obj);
        /// <summary>
        /// UI激活字典
        /// </summary>
        private Dictionary<string, UIUseHandle> uiDic = new Dictionary<string, UIUseHandle>();
        //private Queue<KeyValuePair<string, object>> uiCmdQueue = new Queue<KeyValuePair<string, object>>();
        //private bool isStart = false;
        //private bool isUpdate = true;

        /// <summary>
        /// UI线程锁
        /// </summary>
        private object commonObj = new object();
        private delegate void CallUpdateUIHandle(string uiName, object obj);
        /// <summary>
        /// UI激活方法
        /// </summary>
        private CallUpdateUIHandle updateUIHandler;

        private void Init()
        {
            updateUIHandler = CallUIHandler;
            RegisterUI();
        }
        /// <summary>
        /// 注册UI界面
        /// </summary>
        public void RegisterUI()
        {
            AddUI(UITypeConst.TestTbx, ShowTestTxt);
            AddUI(UITypeConst.loginBtn, ChangeLoginBtn);
        }
       
        public void SendUICmd(string eId, params object[] objs)
        {
            string uiName = objs[0].ToString();
            object obj = null;
            if (objs.Length > 1)
            {
                obj = objs[1];
            }
            lock (commonObj)
            {
                KeyValuePair<string, object> pair = new KeyValuePair<string, object>(uiName, obj);
                //uiCmdQueue.Enqueue(pair);
                if (this.InvokeRequired)
                {
                    this.Invoke(updateUIHandler, pair.Key, pair.Value);
                }
                else
                {
                    updateUIHandler(pair.Key, pair.Value);
                }
            }
            
        }

        //private void UpdateUI()
        //{
        //    while (isUpdate)
        //    {
        //        lock (commonObj)
        //        {
        //            while (uiCmdQueue.Count > 0)
        //            {
        //                KeyValuePair<string, object> pair = uiCmdQueue.Dequeue();
        //                CallUIHandler(pair.Key, pair.Value);
        //            }
        //        }
        //        Thread.Sleep(0);
        //    }
        //}

        /// <summary>
        /// 注册UI方法
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="handler"></param>
        private void AddUI(string uiName, UIUseHandle handler)
        {
            uiDic.Add(uiName, handler);
        }
        /// <summary>
        /// 调用UI方法
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="obj"></param>
        private void CallUIHandler(string uiName, object obj)
        {
            UIUseHandle handler;
            if (uiDic.TryGetValue(uiName, out handler))
            {
                if (handler != null)
                {
                    handler(obj);
                    return;
                }
            }
            SetUIError(uiName, obj);
        }
        /// <summary>
        /// UI界面报错方法，使用的时候补全即可
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="obj"></param>
        private void SetUIError(string uiName, object obj)
        {
            //等待添加UI报错提示
        }

        private void Clear()
        {
            lock (commonObj)
            {
                //isUpdate = false;
                //uiCmdQueue.Clear();
                uiDic.Clear();
            }
        }
        #endregion

        private void LoginFailed(string eId, params object[] objs)
        {
            EventSystemMgr.SentEvent(EventSystemConst.UpdateMainUI, UITypeConst.loginBtn, true);
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            Main.Instance.Login(usernameTbx.Text, passwordTbx.Text);
            loginBtn.Enabled = false;
        }

        private void forgetBtn_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("http://blog.csdn.net/testcs_dn");
        }
    }
}

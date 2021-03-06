﻿using Common.EventMgr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicModel;

namespace UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Init();
            EventSystemMgr.RegisteredEvent(EventSystemConst.UpdateMainUI, SendUICmd);
            EventSystemMgr.RegisteredEvent(EventSystemConst.MainBtnEnable, EnableMainBtn);
            //EventSystemMgr.SentEvent(EventSystemConst.MainThreadSwitch);
            //注册当前form关闭的事件
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OnApplicationClose);

        }
        /// <summary>
        /// 当前窗口关闭激活事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationClose(object sender, FormClosingEventArgs e)
        {
            EventSystemMgr.UnRegisteredEvent(EventSystemConst.UpdateMainUI, SendUICmd);
            EventSystemMgr.UnRegisteredEvent(EventSystemConst.MainBtnEnable, EnableMainBtn);
            Main.Instance.Destory();
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


        #endregion

        #region 通用UI更新模块

        private delegate void UIUseHandle(object obj);
        /// <summary>
        /// UI激活字典
        /// </summary>
        private Dictionary<string, UIUseHandle> uiDic = new Dictionary<string, UIUseHandle>();
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
            AddUI(UITypeConst.mainBtns, EnableBtn);
        }

        /// <summary>
        /// 执行UI方法
        /// </summary>
        /// <param name="eId"></param>
        /// <param name="objs"></param>
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

        private void EnableMainBtn(string eId, params object[] objs)
        {
            EventSystemMgr.SentEvent(EventSystemConst.UpdateMainUI, UITypeConst.mainBtns, null);
        }        

        private void EnableBtn(object obj)
        {
            startBtn.Enabled = true;
            scanBtn.Enabled = true;
            compressBtn.Enabled = true;
            uploadBtn.Enabled = true;
        }
        private void DisableBtn(object obj)
        {
            startBtn.Enabled = false;
            scanBtn.Enabled = false;
            compressBtn.Enabled = false;
            uploadBtn.Enabled = false;
        }

        private void scanBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dirTbx.Text = dialog.SelectedPath;
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dirTbx.Text))
            {
                return;
            }
            else
            {
                bool isGetThumbnail = false;
                int width = 0;
                int height = 0;
                if (!string.IsNullOrEmpty(widthTbx.Text) && !string.IsNullOrEmpty(heightTbx.Text))
                {
                    try
                    {
                        width = int.Parse(widthTbx.Text);
                        height = int.Parse(heightTbx.Text);
                        isGetThumbnail = true;
                    }
                    catch (System.ArgumentNullException exception)
                    {
                        EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_ErrorPanel, "请输入正确的宽高值，或者清空宽高值，使用默认宽高比");
                        return;
                    }
                    catch (System.FormatException exception)
                    {
                        EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_ErrorPanel, "请输入正确的宽高值，或者清空宽高值，使用默认宽高比");
                        return;
                    }
                    catch (System.OverflowException exception)
                    {
                        EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_ErrorPanel, "请输入正确的宽高值，或者清空宽高值，使用默认宽高比");
                        return;
                    }
                }

                DisableBtn(null);

                Main.Instance.GeneratePanoData(dirTbx.Text, isGetThumbnail, width, height);
                //{
                //    //Main.Instance();
                //}
            }
        }

        private void compressBtn_Click(object sender, EventArgs e)
        {
            DisableBtn(null);
            Main.Instance.CompressByZIP();
        }

        private void uploadBtn_Click(object sender, EventArgs e)
        {
            DisableBtn(null);
            Main.Instance.UploadZip();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ThreadPool;
using Common.EventMgr;

namespace LogicModel.UI
{
    public static class UIHelper
    {
        public static void ShowInitUI()
        {
            ThreadPoolMgrSimple.Start(delegate ()
            {
                EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_InitPanel);
            }, 2, 
            delegate(string msg)
            {
                ShowErrorUI(msg);
            });
        }

        public static void CloseInitUI()
        {
            EventSystemMgr.SentEvent(EventSystemConst.ClosePanel_InitPanel);
        }

        public static void ShowLoginUI()
        {
            ThreadPoolMgrSimple.Start(delegate ()
            {
                EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_LoginPanel);
            }, 2,
            delegate (string msg)
            {
                ShowErrorUI(msg);
            });
        }

        public static void CloseLoginUI(bool isMainPanelOpen)
        {
            EventSystemMgr.SentEvent(EventSystemConst.ClosePanel_LoginPanel, isMainPanelOpen);
        }

        public static void ShowMainUI()
        {
            ThreadPoolMgrSimple.Start(delegate ()
            {
                EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_MainPanel);
            }, 2,
            delegate (string msg)
            {
                ShowErrorUI(msg);
            });
        }

        public static void ShowUploadUI()
        {
            ThreadPoolMgrSimple.Start(delegate ()
            {
                EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_UploadPanel);
            }, 2,
            delegate (string msg)
            {
                CloseUploadUI();
                ShowErrorUI(msg);
            });
        }

        public static void CloseUploadUI()
        {
            EventSystemMgr.SentEvent(EventSystemConst.ClosePanel_UploadPanel);
        }

        public static void ShowErrorUI(string msg)
        {
            EventSystemMgr.SentEvent(EventSystemConst.OpenPanel_ErrorPanel, msg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventMgr
{
    /// <summary>
    /// 常用事件注册常量
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    public class EventSystemConst
    {
        #region UI相关

        public const string OpenPanel_InitPanel = "OpenPanel_InitPanel";
        public const string OpenPanel_LoginPanel = "OpenPanel_LoginPanel";
        public const string OpenPanel_MainPanel = "OpenPanel_MainPanel";
        public const string OpenPanel_UploadPanel = "OpenPanel_UploadPanel";

        public const string ClosePanel_InitPanel = "ClosePanel_InitPanel";
        public const string ClosePanel_LoginPanel = "ClosePanel_LoginPanel";
        public const string ClosePanel_MainPanel = "ClosePanel_MainPanel";
        public const string ClosePanel_UploadPanel = "ClosePanel_UploadPanel";

        public const string OpenPanel_ErrorPanel = "OpenPanel_ErrorPanel";


        public const string LoginFailed = "LoginFailed";

        public const string MainBtnEnable = "MainBtnEnable";

        /// <summary>
        /// UI上主逻辑继续执行
        /// </summary>
        public const string MainThreadSwitch = "MainThreadSwitch";
        /// <summary>
        /// 在UI上的测试框中输出
        /// </summary>
        public const string UpdateMainUI = "UpdateUI";
        public const string UpdateUploadUI = "UpdateUI";



        public const string UpdateUploadProgressBar = "UpdateUploadProgressBar";
        public const string UpdateUploadProgressLable = "UpdateUploadProgressLable";
        #endregion

        #region 测试相关

        #endregion
    }
}

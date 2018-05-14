using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace QuickAgent.ViewModel
{
    class MainWindowVM : INotifyPropertyChanged
    {
        #region 单例
        private static MainWindowVM mainWindowVM = null;
        private static readonly object lockObj = new object();
        public static MainWindowVM GetInstance()
        {
            if (null == mainWindowVM)
            {
                lock (lockObj)
                {
                    if (null == mainWindowVM)
                    {
                        mainWindowVM = new MainWindowVM();
                    }
                }
            }
            return mainWindowVM;
        }
        private MainWindowVM() { }
        #endregion

        /// <summary>
        /// 是否签入
        /// </summary>
        private bool isEnter = false;
        public bool IsEnter
        {
            get
            {
                return isEnter;
            }
            set
            {
                if (isEnter != value)
                {
                    isEnter = value; OnPropertyChanged("IsEnter");
                }
            }
        }

        /// <summary>
        /// 是否登陆
        /// </summary>
        private bool isLogin = false;
        public bool IsLogin
        {
            get
            {
                return isLogin;
            }
            set
            {
                if (isLogin != value)
                {
                    isLogin = value; OnPropertyChanged("IsLogin");
                }
            }
        }

        /// <summary>
        /// 未登录
        /// </summary>
        private Visibility loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {
            get
            {
                return loginVisibility;
            }
            set
            {
                if (loginVisibility != value)
                {
                    loginVisibility = value; OnPropertyChanged("LoginVisibility");
                }
            }
        }

        private Visibility logoutVisibility = Visibility.Collapsed;
        public Visibility LogoutVisibility
        {
            get
            {
                return logoutVisibility;
            }
            set
            {
                if (logoutVisibility != value)
                {
                    logoutVisibility = value; OnPropertyChanged("LogoutVisibility");
                }
            }
        }

        /// <summary>
        /// 是否录制
        /// </summary>
        private bool isRecord = false;
        public bool IsRecord
        {
            get
            {
                return isRecord;
            }
            set
            {
                if (isRecord != value)
                {
                    isRecord = value; OnPropertyChanged("IsRecord");
                }
            }
        }

        /// <summary>
        /// 工号
        /// </summary>
        private string jobNumber = string.Empty;
        public string JobNumber
        {
            get { return jobNumber; }
            set
            {
                if (value != jobNumber)
                {
                    jobNumber = value; OnPropertyChanged("JobNumber");
                }
            }
        }

        /// <summary>
        /// 主叫号码
        /// </summary>
        private string callerNumber = string.Empty;
        public string CallerNumber
        {
            get { return callerNumber; }
            set
            {
                if (value != callerNumber)
                {
                    callerNumber = value; OnPropertyChanged("CallerNumber");
                }
            }
        }

        /// <summary>
        /// 被叫号码
        /// </summary>
        private string calledNumber = string.Empty;
        public string CalledNumber
        {
            get { return calledNumber; }
            set
            {
                if (value != calledNumber)
                {
                    calledNumber = value; OnPropertyChanged("CalledNumber");
                }
            }
        }

        /// <summary>
        /// 座席状态图片
        /// </summary>
        private string agentStateIcon = "/CCCBar2;component/Resource/QuickTellerImage/Images/Default/StateLeave.png";
        public string AgentStateIcon
        {
            get
            {
                return agentStateIcon;
            }
            set
            {
                if (value != agentStateIcon)
                {
                    agentStateIcon = value; OnPropertyChanged("AgentStateIcon");
                }
            }
        }

        /// <summary>
        /// 当前座席状态图片
        /// </summary>
        private string curAgentStateIcon = "/CCCBar2;component/Resource/QuickTellerImage/Images/Default/StateLeave.png";
        public string CurAgentStateIcon
        {
            get
            {
                return curAgentStateIcon;
            }
            set
            {
                if (value != curAgentStateIcon)
                {
                    curAgentStateIcon = value; OnPropertyChanged("CurAgentStateIcon");
                }
            }
        }

        //是否保持
        private bool isHold;//加载页面时都是不可用状态
        public bool IsHold
        {
            get { return isHold; }
            set
            {
                isHold = value; OnPropertyChanged("IsHold");
            }
        }
        //是否取保持
        private bool isUNHold;//加载页面时都是不可用状态
        public bool IsUNHold
        {
            get { return isUNHold; }
            set
            {
                isUNHold = value; OnPropertyChanged("IsUNHold");
            }
        }
        //是否静音
        private Boolean isMute;
        public Boolean IsMute
        {
            get { return isMute; }
            set
            {
                isMute = value; OnPropertyChanged("IsMute");
            }
        }
        //是否取静音
        private Boolean isUNMute;
        public Boolean IsUNMute
        {
            get { return isUNMute; }
            set
            {
                isUNMute = value; OnPropertyChanged("IsUNMute");
            }
        }

        /// <summary>
        /// 屏幕顶部
        /// </summary>
        private bool isTopChecked = true;
        public bool IsTopChecked
        {
            get
            {
                return isTopChecked;
            }
            set
            {
                if (value != isTopChecked)
                {
                    isTopChecked = value; OnPropertyChanged("IsTopChecked");
                }
            }
        }

        /// <summary>
        /// 屏幕底部
        /// </summary>
        private bool isBottomChecked = false;
        public bool IsBottomChecked
        {
            get
            {
                return isBottomChecked;
            }
            set
            {
                if (value != isBottomChecked)
                {
                    isBottomChecked = value; OnPropertyChanged("IsBottomChecked");
                }
            }
        }

        private string commonPrompt = string.Empty;
        public string CommonPrompt
        {
            get
            {
                return commonPrompt;
            }
            set
            {
                if (value != commonPrompt)
                {
                    commonPrompt = value; OnPropertyChanged("CommonPrompt");
                }
            }
        }

        private string agentState = new CommonText().StateInfo_Leave;
        public string AgentState
        {
            get
            {
                return agentState;
            }
            set
            {
                if (value != agentState)
                {
                    agentState = value; OnPropertyChanged("AgentState");
                }
            }
        }

        private double controlBarHeight = 0;
        public double ControlBarHeight
        {
            get
            {
                return controlBarHeight;
            }
            set
            {
                if (value != controlBarHeight)
                {
                    controlBarHeight = value; OnPropertyChanged("ControlBarHeight");
                }
            }
        }

        private double controlBarWidth = 0;
        public double ControlBarWidth
        {
            get
            {
                return controlBarWidth;
            }
            set
            {
                if (value != controlBarWidth)
                {
                    controlBarWidth = value; OnPropertyChanged("ControlBarWidth");
                }
            }
        }

        private double componentHeight = 0;
        public double ComponentHeight
        {
            get
            {
                return componentHeight;
            }
            set
            {
                if (value != componentHeight)
                {
                    componentHeight = value; OnPropertyChanged("ComponentHeight");
                }
            }
        }

        private double componentWidth = 0;
        public double ComponentWidth
        {
            get
            {
                return componentWidth;
            }
            set
            {
                if (value != componentWidth)
                {
                    componentWidth = value; OnPropertyChanged("ComponentWidth");
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(value));
            }
        }
        #endregion
    }
}

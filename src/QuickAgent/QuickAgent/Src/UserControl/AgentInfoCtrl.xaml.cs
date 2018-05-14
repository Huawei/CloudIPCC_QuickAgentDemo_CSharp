using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HuaweiAgentGateway;
using QuickAgent.Src.Common;

namespace QuickAgent.Src.UserControl
{
    /// <summary>
    /// 座席信息用户控件
    /// </summary>
    public partial class AgentInfoCtrl : System.Windows.Controls.UserControl
    {
        public AgentInfoCtrl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 用户控件数据源
        /// </summary>
        private List<AgentStateInfo> _lstInfo { set; get; }

        /// <summary>
        /// 当前选择的座席
        /// </summary>
        public AgentStateInfo SelectedAgent
        {
            get
            {
                if (null == _lstInfo || lstAgent.SelectedIndex == -1 || _lstInfo.Count < lstAgent.SelectedIndex + 1)
                {
                    return null;
                }
                return _lstInfo[lstAgent.SelectedIndex];
            }
        }

        public string WorkNo
        {
            get
            {
                if (null == lstAgent.SelectedItem)
                {
                    return string.Empty;
                }
                var item = lstAgent.SelectedItem as AgentStateInfoWrapper;
                if (null == item)
                {
                    return string.Empty;
                }
                return item.WorkNo;
            }
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        public void SetSource(List<AgentStateInfo> lstInfo)
        {
            if (null == lstInfo) return;
            this._lstInfo = lstInfo;
            lstAgent.ItemsSource = lstInfo.Select(item => new AgentStateInfoWrapper(item)).ToList();
        }

        public void SetSourceEx(List<AgentStateInfo> lstInfo)
        {
            UIViewHelper.SelectorResetSource(lstAgent, lstInfo);
        }
    }

    /// <summary>
    /// 座席信息包装类，用于界面显示
    /// </summary>
    class AgentStateInfoWrapper : AgentStateInfo
    {
        private const string TALK_STATUS = "7";

        /// <summary>
        /// 班组名称
        /// </summary>
        public string GroupName { set; get; }

        /// <summary>
        /// 座席姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 工号
        /// </summary>
        public string WorkNo { set; get; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// 第一行描述信息
        /// </summary>
        public string Content { set; get; }

        private Dictionary<string, string> m_agentStatusPicDict = new Dictionary<string, string>()
        {
            {"0","/QuickAgent;component/Resource/Images/icon2-_117.png"},
            {"1","/QuickAgent;component/Resource/Images/icon2-_119.png"},
            {"2","/QuickAgent;component/Resource/Images/icon2-_117.png"},
            {"3","/QuickAgent;component/Resource/Images/icon2-_205.png"},
            {"4","/QuickAgent;component/Resource/Images/icon2-_119.png"},
            {"5","/QuickAgent;component/Resource/Images/icon2-_243.png"},
            {"6","/QuickAgent;component/Resource/Images/icon2-_119.png"},
            {"7","/QuickAgent;component/Resource/Images/icon2-_201.png"},  
            {"8","/QuickAgent;component/Resource/Images/icon2-_207.png"},  
        };

        private Dictionary<string, string> m_talkStatusPicDic = new Dictionary<string, string>()
        {
            {"2","/QuickAgent;component/Resource/Images/icon2-_121.png"},
            {"3","/QuickAgent;component/Resource/Images/icon2-_123.png"},
            {"4","/QuickAgent;component/Resource/Images/icon2-_125.png"},
            {"5","/QuickAgent;component/Resource/Images/icon2-_201.png"},
        };

        public AgentStateInfoWrapper() { }

        public AgentStateInfoWrapper(AgentStateInfo info)
        {
            this.GroupName = string.IsNullOrEmpty(info.groupName) ? LanguageResource.FindResourceMessageByKey("agentinfo_nogroup") : info.groupName;
            this.WorkNo = info.workno;
            this.status = info.status;
            this.Name = info.name;
            this.Content = LanguageResource.FindResourceMessageByKey("agentinfo_id") + info.workno;

            if (TALK_STATUS.Equals(info.status))
            {
                Url = (info.ctiStatus != null && m_talkStatusPicDic.ContainsKey(info.ctiStatus)) ?
                    m_talkStatusPicDic[info.ctiStatus] : "/QuickAgent;component/Resource/Images/icon2-_201.png";
            }
            else
            {
                Url = m_agentStatusPicDict.ContainsKey(info.status) ? m_agentStatusPicDict[info.status] 
                    : "/QuickAgent;component/Resource/Images/icon2-_117.png";
            }
        }
    }
}

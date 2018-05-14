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
using System.Windows.Shapes;
using QuickAgent.Src.Common;
using HuaweiAgentGateway;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 外呼 的交互逻辑
    /// </summary>
    /// <remarks>
    /// AGW：媒体能力 3 种。0：音频，1：视频，2：默认。默认为 2。
    /// MCP：媒体能力 3 种。0：音频，1：视频，2：缺省能力，由主机与终端协商。默认是 2。
    /// </remarks>
    public partial class CallOutWindow : Window
    {
        #region  属性

        private static CallOutWindow _instance = null;

        private List<SkillInfo> _skillSrc = null;

        /// <summary>
        /// MCP 媒体能力
        /// </summary>
        private static Dictionary<int, string> DIC_MCP_MEDIA = new Dictionary<int, string>()
        {
            {0, "Audio"},
            {1, "Video"},
            {2, "Default"}
        };

        /// <summary>
        /// AGW 媒体能力
        /// </summary>
        private static Dictionary<int, string> DIC_AGW_MEDIA = new Dictionary<int, string>() 
        {
            {0, "Audio"},
            {1, "Video"},
            {2, "Default"}
        };

        /// <summary>
        /// 呼出检测模式（用于 MCP）
        /// 0：不检测
        /// 1：接通前检测
        /// 2：接通后检测
        /// </summary>
        public static int CallCheck { private set; get; }

        /// <summary>
        /// 主叫
        /// </summary>
        public static string CalledNumber { private set; get; }

        /// <summary>
        /// 被叫
        /// </summary>
        public static string CallerNumber { private set; get; }

        /// <summary>
        /// 媒体能力（当前写死音频 0）
        /// </summary>
        public static int MediaAbility { private set; get; }

        /// <summary>
        /// 外呼技能
        /// </summary>
        public static int SkillID { private set; get; }

        #endregion

        #region  方法

        public static CallOutWindow GetInstance()
        {
            if (null == _instance)
            {
                _instance = new CallOutWindow();
            }
            return _instance;
        }

        public CallOutWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            this.Closed += new EventHandler(Window_Close);

            #region  控件设置（技能，媒体能力）

            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("outcall_head"));
            cbxMedia.ItemsSource = BusinessAdapter.CurrentBusinessType == BusinessType.OCX ?
                DIC_MCP_MEDIA.Values : DIC_AGW_MEDIA.Values;
            cbxMedia.SelectedIndex = BusinessAdapter.CurrentBusinessType == BusinessType.OCX ?
                2 : 0;

            _skillSrc = BusinessAdapter.GetBusinessInstance().GetAgentSkills();
            if (null == _skillSrc || _skillSrc.Count == 0) return;
            // 查询技能，增加一项 "默认"
            _skillSrc.Insert(0, new SkillInfo() { name = LanguageResource.FindResourceMessageByKey("outcall_default"), id = "0", mediatype = "5" });
            cbxSkill.ItemsSource = _skillSrc.Select(item => item.name).ToList();
            cbxSkill.SelectedIndex = 0;

            #endregion

            #region  控件样式

            txtBlockSkill.Style = (Style)this.FindResource("txtBlockStyle");
            lblCalledNumber.Style = (Style)this.FindResource("txtBlockStyle");
            lblCallerNumber.Style = (Style)this.FindResource("txtBlockStyle");
            txtBlockMedia.Style = (Style)this.FindResource("txtBlockStyle");
            txtBlockCheck.Style = (Style)this.FindResource("txtBlockStyle");

            #endregion
        }

        #endregion

        #region  事件

        public void OK_Click(object sender, RoutedEventArgs e)
        {
            Button me = sender as Button;
            if (string.IsNullOrEmpty(txtCalledNumber.Text))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("calledNumberNotIsNull"));
                me.Focus();
            }
            else
            {
                int id = 0;
                if (_skillSrc != null && _skillSrc.Count > 0 && cbxSkill.SelectedIndex != -1 && Int32.TryParse(_skillSrc[cbxSkill.SelectedIndex].id, out id))
                {
                    SkillID = id;
                }

                CallCheck = chkNoCheck.IsChecked == true ? 0 : chkNoCheckBefore.IsChecked == true ? 1 : 2;
                CalledNumber = txtCalledNumber.Text;
                CallerNumber = txtCallerNumber.Text;
                MediaAbility = cbxMedia.SelectedIndex;
                this.DialogResult = true;
            }
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void Window_Close(object sender, EventArgs e)
        {
            _instance = null;
        }

        #endregion
    }
}

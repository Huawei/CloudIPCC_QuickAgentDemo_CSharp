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
using HuaweiAgentGateway.AgentGatewayEntity;
using HuaweiAgentGateway;
using System.ComponentModel;
using QuickAgent.Src.Common;

namespace QuickAgent.View
{
    /// <summary>
    /// 技能重设界面
    /// </summary>
    public partial class AgentRegisterWindow : Window
    {
        #region 属性

        /// <summary>
        /// 最终选择要签入的技能列表
        /// </summary>
        public List<SkillInfo> lstSelectSkills { private set; get; }

        /// <summary>
        /// 传入的原始数据源
        /// </summary>
        private List<SkillItem> _lstOriSkills { set; get; }

        #endregion

        #region  构造方法

        public AgentRegisterWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("login_head"));
        }

        public AgentRegisterWindow(IList<SkillInfo> items, string phoneNo)
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            _lstOriSkills = new List<SkillItem>();
            lstSelectSkills = new List<SkillInfo>();

            RadioButton_AutoRegister.Checked += RadioButton_AutoRegister_Checked;
            RadioButton_HandMovement.Checked += RadioButton_HandMovement_Checked;
            btnCancel.Click += Button_Cancel_Click;
            btnConfirm.Click += Button_Confirm_Click;

            foreach (SkillInfo item in items)
            {
                _lstOriSkills.Add(new SkillItem(item));
            }
            ListView_Skill.ItemsSource = _lstOriSkills;
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("login_head"));
        }

        #endregion

        #region  事件

        /// <summary>
        /// 自动签入事件。选择时技能列表不可更改，默认全选。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_AutoRegister_Checked(object sender, RoutedEventArgs e)
        {
            if (ListView_Skill != null)
            {
                ListView_Skill.IsEnabled = false;
                ListView_Skill.ItemsSource = null;

                if (_lstOriSkills != null)
                {
                    foreach (var item in _lstOriSkills)
                    {
                        item.IsItemSelected = true;
                    }
                }
                ListView_Skill.ItemsSource = null;
                ListView_Skill.ItemsSource = _lstOriSkills;
            }
        }

        /// <summary>
        /// 手动签入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_HandMovement_Checked(object sender, RoutedEventArgs e)
        {
            ListView_Skill.IsEnabled = true;
        }

        /// <summary>
        /// 技能列表取消选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBoxSkill_UnChecked(object sender, RoutedEventArgs e)
        {
            SkillSelectCheck();
        }

        /// <summary>
        /// 确认（先检查技能是否至少选择一个）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!SkillSelectCheck())
            {
                return;
            }

            foreach (SkillItem item in ListView_Skill.Items)
            {
                if (!item.isSelected) continue;
                lstSelectSkills.Add(new SkillInfo() { name = item.name, id = item.id });
            }

            this.DialogResult = true;
            Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        /// <summary>
        /// 技能选择检查，如果一项都没有选那么弹出窗口
        /// </summary>
        private bool SkillSelectCheck()
        {
            if (ListView_Skill.ItemsSource == null) return true;
            foreach (SkillItem item in ListView_Skill.ItemsSource)
            {
                if (item.IsItemSelected) return true;
            }
            MessageBox.Show(LanguageResource.FindResourceMessageByKey("pleaseSelectSkills"));
            return false;
        }

        #endregion
    }

    /// <summary>
    /// 技能类包装，增加一个是否选中属性（bool），默认状态是选中。
    /// </summary>
    class SkillItem : SkillInfo, INotifyPropertyChanged
    {
        public bool isSelected = true;
        public bool IsItemSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value; OnPropertyChanged("IsItemSelected");
                }
            }
        }

        public SkillItem() { }
        public SkillItem(SkillInfo item)
        {
            this.name = item.name;
            this.id = item.id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(value));
            }
        }
    }
}

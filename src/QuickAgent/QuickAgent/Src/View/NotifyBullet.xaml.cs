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
using HuaweiAgentGateway;
using QuickAgent.Src.Common;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 发布公告界面
    /// </summary>
    public partial class NotifyBullet : Window
    {
        #region 属性

        public string Msg { private set; get; }

        public string Parm { private set; get; }

        public MsgType MessageType { private set; get; }

        #endregion

        #region  方法

        public NotifyBullet()
        {
            InitialWindow();
            rdBtnWorkGroup.IsChecked = true;
            groupBoxSkill.IsEnabled = false;
        }

        public NotifyBullet(List<SkillInfo> lstSkills, List<WorkGroupData> lstWorkGroup)
        {
            InitialWindow();
            lstSkill.ItemsSource = lstSkills;
            lstGroup.ItemsSource = lstWorkGroup;
            grdSkills.IsEnabled = false;
            grdWorkGroup.IsEnabled = true;
            rdBtnWorkGroup.IsChecked = true;
        }

        private void InitialWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("bulletin_head"));
        }

        #endregion

        #region  事件

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Msg = rdBtnSkill.IsChecked == true ? txtBoxNotifySkill.Text : txtBoxNotifyGroup.Text;
            this.MessageType = rdBtnSkill.IsChecked == true ? MsgType.Skill : MsgType.WorkGroup;
            this.Parm = rdBtnSkill.IsChecked == true ? txtBoxSkill.Text : txtBoxWorkGroup.Text;
            Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void rdBtnSkill_Checked(object sender, RoutedEventArgs e)
        {
            if (rdBtnWorkGroup != null)
                rdBtnWorkGroup.IsChecked = false;
            if (grdWorkGroup != null)
                grdWorkGroup.IsEnabled = false;
            if (grdSkills != null)
                grdSkills.IsEnabled = true;
        }

        private void rdBtnWorkGroup_Checked(object sender, RoutedEventArgs e)
        {
            if (rdBtnSkill != null)
                rdBtnSkill.IsChecked = false;
            if (grdWorkGroup != null)
                grdWorkGroup.IsEnabled = true;
            if (grdSkills != null)
                grdSkills.IsEnabled = false;
        }

        private void lstGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lstGroup.SelectedItem as WorkGroupData;         
            txtBoxWorkGroup.Text = item == null ? string.Empty : item.WorkGroupName;
            //bool hasAgent = BusinessAdapter.GetBusinessInstance().HasAgentInGroup(item.WorkGourpId, item.MonitorNo);
            //if (!hasAgent)
            //    MessageBox.Show(LanguageResource.FindResourceMessageByKey("notifybullet_nogroup"));
        }

        private void lstSkill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lstSkill.SelectedItem as SkillInfo;
            txtBoxSkill.Text = item == null ? string.Empty : item.name;
        }

        #endregion
    }

    public enum MsgType
    {
        Skill = 0,
        WorkGroup = 1,
    }
}

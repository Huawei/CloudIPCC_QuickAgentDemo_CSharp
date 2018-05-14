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
using HuaweiAgentGateway.AgentGatewayEntity;
using QuickAgent.Src.Common;

namespace QuickAgent.Src.View.TextChat
{
    /// <summary>
    /// 文字交谈---呼叫转移
    /// </summary>
    public partial class TC_CallTrans : Window
    {
        public int TransAddrType { set; get; }

        public int TransType { set; get; }

        public bool IsConfirm { set; get; }

        public string Dest { set; get; }

        /// <summary>
        /// 1：表示转移地址为工号；10：表示技能队列
        /// </summary>
        private List<int> _lstTransAddrType = new List<int>() { 1, 10 };

        /// <summary>
        /// 0表示释放转。1表示成功转。
        /// </summary>
        private List<int> _lstTransType = new List<int>() { 0, 1 };

        public TC_CallTrans()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            var lstSkillsSrc = ((AgentGatewayBusiness)(BusinessAdapter.GetBusinessInstance())).QrySkillsOnAgentVDN();
            var lstItemsSkills = new List<AGWVdnSkills>();
            if (lstSkillsSrc != null && lstSkillsSrc.Count > 0)
            {
                foreach (var item in lstSkillsSrc)
                {
                    if (item.mediatype != 1) continue;
                    lstItemsSkills.Add(item);
                }
                lst_skills.ItemsSource = lstItemsSkills;
            }
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            TransAddrType = _lstTransAddrType[cb_transaddrtype.SelectedIndex];
            TransType = _lstTransType[1];
            IsConfirm = true;
            Dest = string.Empty;
            if (cb_transaddrtype.SelectedIndex == 0)
            {
                Dest = tb_workNo.Text;
            }
            else
            {
                if (lst_skills.SelectedItem != null)
                {
                    var item = lst_skills.SelectedItem as AGWVdnSkills;
                    Dest = item == null ? string.Empty : item.id + "";
                } 
            }
            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirm = false;
            Close();
        }
    }
}

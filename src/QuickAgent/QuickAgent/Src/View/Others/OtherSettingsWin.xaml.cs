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
using QuickAgent.View;
using QuickAgent.Src.Common;

namespace QuickAgent.Src.View.Others
{
    /// <summary>
    /// Interaction logic for OtherSettingsWin.xaml
    /// </summary>
    public partial class OtherSettingsWin : Window
    {
        #region  Fields

        /// <summary>
        /// current window instance
        /// </summary>
        private static OtherSettingsWin _instance { set; get; }

        /// <summary>
        /// get current phonelinkage set
        /// </summary>
        public bool IsPhonelinkage 
        {
            get 
            {
                return chk_PhoneLink.IsChecked.Value;
            }
        }

        #endregion

        #region  Methods

        public OtherSettingsWin()
        {
            InitializeComponent();
            _instance = this;
            LanguageResource.ChangeWindowLanguage(this);
            QryAndShowAgentSkillsInfo();
        }

        private void QryAndShowAgentSkillsInfo()
        {
            var iBuss = BusinessAdapter.GetBusinessInstance();
            if (null == iBuss)
            {
                return;
            }
            var lstSkills = iBuss.GetAgentSkills();
            if (null == lstSkills || lstSkills.Count == 0)
            {
                return;
            }
            var lstSignSkills = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).GetAgentSignedSkills();
            var lstSignSkillId = lstSignSkills == null || lstSignSkills.Count == 0 ?
                new List<string>() : lstSignSkills.Select(item => item.id).ToList();

            lstView_Skills.ItemsSource = lstSkills.Select(item =>
                new SkillItem()
                {
                    isSelected = lstSignSkillId.Contains(item.id),
                    id = item.id,
                    name = item.name
                }).ToList();
        }

        public static OtherSettingsWin GetInstance()
        {
            if (null == _instance)
            {
                _instance = new OtherSettingsWin();
            }
            return _instance;
        }

        public static void ClearInstance()
        {
            _instance = null;
        }

        public static bool ChekcInstanceExists()
        {
            return _instance != null;
        }

        /// <summary>
        /// check if any skill is selected
        /// </summary>
        /// <returns>
        /// false : no skill has been selected
        /// true : any skill has been selected
        /// </returns>
        private bool SkillSelectCheck()
        {
            foreach (SkillItem item in lstView_Skills.ItemsSource)
            {
                if (item.IsItemSelected) return true;
            }
            MessageBox.Show(LanguageResource.FindResourceMessageByKey("pleaseSelectSkills"));
            return false;
        }

        #endregion

        #region Events

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (lstView_Skills.ItemsSource == null)
            {
                return;
            }
            if (!SkillSelectCheck())
            {
                return;
            }

            var lstId = new List<int>();

            foreach (SkillItem item in lstView_Skills.Items)
            {
                if (!item.isSelected) continue;
                int id = 0;
                if (Int32.TryParse(item.id, out id))
                {
                    lstId.Add(id);
                }
            }
            var res = ((AgentGatewayBusiness)(BusinessAdapter.GetBusinessInstance())).ResetSkillEx(false, lstId, chk_PhoneLink.IsChecked.Value);
            if (AGWErrorCode.OK.Equals(res))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("otherset_resetskillsucc"));
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkBoxSkill_UnChecked(object sender, RoutedEventArgs e)
        {
            SkillSelectCheck();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion
    }
}

#define FORCEHTTPS

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
using QuickAgent.Common;
using QuickAgent.Src.Common;

namespace QuickAgent.View
{
    /// <summary>
    /// demo config window 
    /// </summary>
    public partial class ConfigWindow : Window
    {
        #region  Fields

        private const string MAIN_URL = "HuaweiServerUrl";

        private const string BACK_URL = "HuaweiBackupAddress";

        private const string AUTO_ANS = "HuaweiIsAutoAnswer";

        private const string ENTR_WRK = "HuaweiAutoEnterWork";

        private const string TIME_OUT = "HuaweiOutTime";

        private const string BUSS_TYP = "HuaweiBusinessType";

        private const string USAG_SSL = "UseSsl";

        private const string USAG_VOI = "UseVoice";

        private const string VOIC_DOM = "Domain";

        private const string PROG_ID = "ProgID";

        private const string DEFAULT_PROGID = "48";

        private const string DEFAULT_TIMEOUT = "10";

        private const string AGW_URLPARM = "{0}://{1}:{2}/agentgateway/resource";

        /// <summary>
        /// voice domain: used for C80 or public aws version
        /// </summary>
        private const string DEFAULT_VOICEDOMAIN = "cce.com";

        /// <summary>
        /// voice domain value: used for 6.1 or C10 version
        /// </summary>
        private const string C10_VOICEDOMAIN = "example.com";

        /// <summary>
        /// current agenttype
        /// 1,pc+phone
        /// 2,pc+phone video(not supported at commercial version)
        /// </summary>
        private Dictionary<string, string> m_dicAgentType = new Dictionary<string, string>()
        {
           {"PC+PHONE","4"},
           //{LanguageResource.FindResourceMessageByKey("set_pcphonevideo"), "11"}     
        };

        public Dictionary<string, string> Settings { get; set; }

        #endregion

        public ConfigWindow()
        { }

        public ConfigWindow(Dictionary<string, string> settings, AgentInfo agentInfo)
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("config_head"));
            cbAgentType.ItemsSource = m_dicAgentType.Keys;
            cbAgentType.SelectedIndex = 0;
            Settings = settings;
            LoadConfig();
            lblUseSsl.Visibility = Visibility.Collapsed;
            chkUseHttps.IsChecked = true;
            chkUseHttps.Visibility = Visibility.Collapsed;
            tbLongCall.Visibility = System.Windows.Visibility.Collapsed;
            chkLongCall.Visibility = System.Windows.Visibility.Collapsed;
            chkLongCall.IsChecked = false;
        }

        private string ValidateString(TextBox control, bool mandatory, string prompt)
        {
            var text = control.Text;

            if (prompt == null)
                prompt = "";

            if (string.IsNullOrEmpty(text) && mandatory)
            {
            }
            return text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var mainUrl = ValidateString(txtUrl, true, "Server");
                var backUrl = ValidateString(txtBoxBackUpAddress, false, "BackupServer");
                if (chkAgw.IsChecked.Value)
                {
                    var urlhead = chkUseHttps.IsChecked.Value ? "https" : "http";
                    mainUrl = string.Format(AGW_URLPARM, urlhead, mainUrl, tbSerPort.Text);
                    if (!string.IsNullOrEmpty(txtBoxBackUpAddress.Text) && !string.IsNullOrEmpty(tbSerPort1.Text))
                    {
                        backUrl = string.Format(AGW_URLPARM, urlhead, backUrl, tbSerPort1.Text);
                    }
                }

                if (null == Settings)
                {
                    Settings = new Dictionary<string, string>();
                }
                else
                {
                    Settings.Clear();
                }

                Settings.Add(MAIN_URL, ValidateString(txtUrl, true, "Server"));
                Settings.Add(BACK_URL, ValidateString(txtBoxBackUpAddress, false, "BackupServer"));
                Settings.Add(AUTO_ANS, (bool)chkAutoAnswer.IsChecked + "");
                Settings.Add(ENTR_WRK, (bool)chkAutoEnterWork.IsChecked + "");
                Settings.Add(TIME_OUT, txtOverTime.Text);
                var serType = chkOCX.IsChecked.Value ? "OCX" : "Agent";
                Settings.Add(BUSS_TYP, serType);
                Settings.Add(PROG_ID, tbProgID.Text);
                Settings.Add(USAG_SSL, chkUseHttps.IsChecked.Value + "");
                Settings.Add("tbSerPort", tbSerPort.Text);
                Settings.Add("tbSerPort1", tbSerPort1.Text);
                Settings.Add("mainUrl", mainUrl);
                Settings.Add("backUrl", backUrl);
                Settings.Add("agentType", m_dicAgentType[cbAgentType.Text]);

                Settings.Add(USAG_VOI, chkOpenEye.IsChecked.Value + "");
                Settings.Add("SipIP", tbxSIPIP.Text);
                Settings.Add("SipPort", tbxSIPPort.Text);
                Settings.Add("SipIP2", tbxSIPIP2.Text);
                Settings.Add("SipPort2", tbxSIPPort2.Text);
                Settings.Add("VoiceAccount", tbxName2.Text);
                Settings.Add(VOIC_DOM, tbDomain.Text);
                Settings.Add("LongCall", chkLongCall.IsChecked.Value + "");
                Settings.Add("ChkCerti", chk_certifile.IsChecked.Value + "");
                MainWindow.VoicePwd = tbxPass2.Password;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                Log4NetHelper.BaseLog("[config_excep] Msg:" + excep.Message);
            }
            finally
            {
                this.DialogResult = true;
            }
        }

        private string GetSetting(string name)
        {
            if (string.IsNullOrEmpty(name) || Settings == null)
            {
                return string.Empty;
            }
            string value = null;
            this.Settings.TryGetValue(name, out value);
            return value;
        }

        private void LoadConfig()
        {
            if (Settings != null && Settings.Count > 1)
            {
                txtUrl.Text = GetSetting(MAIN_URL);
                chkAutoAnswer.IsChecked = string.Compare(GetSetting(AUTO_ANS), "true", true) == 0;
                chkOCX.IsChecked = string.Compare(GetSetting(BUSS_TYP), "OCX", true) == 0;
                txtBoxBackUpAddress.Text = GetSetting(BACK_URL);
                chkAgw.IsChecked = !chkOCX.IsChecked;
                chkAutoEnterWork.IsChecked = string.Compare(GetSetting(ENTR_WRK), "true", true) == 0;
                txtOverTime.SetText(GetSetting(TIME_OUT));
                chkUseHttps.IsChecked = string.Compare(GetSetting(USAG_SSL), "true", true) == 0;
                chk_certifile.IsChecked = string.Compare(GetSetting("ChkCerti"),"true",true) == 0;
                tbSerPort.Text = GetSetting("tbSerPort");
                tbSerPort1.Text = GetSetting("tbSerPort1");

                int index = 0;
                foreach (var key in m_dicAgentType.Keys)
                {
                    if (m_dicAgentType[key] == GetSetting("agentType")) break;
                    index++;
                }
                cbAgentType.SelectedIndex = m_dicAgentType.Keys.Count == index ? 0 : index;

                chkOpenEye.IsChecked = string.Compare(GetSetting(USAG_VOI), "true", true) == 0;
                tbxSIPIP.Text = GetSetting("SipIP");
                tbxSIPPort.Text = GetSetting("SipPort");
                tbxSIPIP2.Text = GetSetting("SipIP2");
                tbxSIPPort2.Text = GetSetting("SipPort2");
                tbxName2.Text = GetSetting("VoiceAccount");
                tbDomain.Text = GetSetting(VOIC_DOM);
            }
            else
            {
                txtOverTime.SetText(DEFAULT_TIMEOUT);
                tbProgID.SetText(DEFAULT_PROGID);
                tbDomain.Text = MainWindow.Instance().Version == MainWindow.DemoVersion.C80 ? DEFAULT_VOICEDOMAIN : C10_VOICEDOMAIN;
                chk_certifile.IsChecked = true;
            }
        }

        private void chkOCX_Checked(object sender, RoutedEventArgs e)
        {
            ChangeUi(true);
        }

        private void chkAgw_Checked(object sender, RoutedEventArgs e)
        {
            ChangeUi(false);
        }

        private void ChangeUi(bool isMcp)
        {
            if (tbProgID != null)
                tbProgID.IsEnabled = isMcp;
            if (chkUseHttps != null)
                chkUseHttps.IsEnabled = !isMcp;
            if (tbSerPort != null)
                tbSerPort.IsEnabled = !isMcp;
            if (tbSerPort1 != null)
                tbSerPort1.IsEnabled = !isMcp;
        }
    }
}

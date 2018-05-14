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
using QuickAgent.Common;
using System.Runtime.InteropServices;
using HuaweiAgentGateway.Utils;

namespace QuickAgent.Src.View.Others
{
    /// <summary>
    /// Record Screen Window
    /// </summary>
    public partial class RecordWin : Window
    {
        #region  Fields

        /// <summary>
        /// record type: one screen
        /// </summary>
        private const int RCDTYPE_SINGLE = 1;

        /// <summary>
        /// record type: two screens
        /// </summary>
        private const int RCDTYPE_TWO = 2;

        /// <summary>
        /// screen number
        /// used at 'one screen record' only
        /// 0: main screen
        /// </summary>
        private const string SCREEN_MAIN = "0";

        /// <summary>
        /// screen number
        /// used at 'one screen record' only
        /// 1: sub screen
        /// </summary>
        private const string SCREEN_SUB = "1";

        /// <summary>
        /// business intance
        /// </summary>
        private AgentGatewayBusiness _agwBus { set; get; }

        /// <summary>
        /// vrc control instance
        /// </summary>
        private AxVRCCONTROLLib.AxVRCControl _vrc { set; get; }

        /// <summary>
        /// a property whether vrc has initalized
        /// </summary>
        private bool _isVrcInitalized { set; get; }

        /// <summary>
        /// current agentID
        /// </summary>
        private string _agentID { set; get; }

        /// <summary>
        /// vrc login type
        /// 2: agent
        /// </summary>
        private const int VRC_LOGINTYPE = 2;

        /// <summary>
        /// default screen size(height & width)
        /// </summary>
        /// <remarks>default size means record as pritical screen size(height & width)</remarks>
        private const string DEFAULT_SIZE = "0";

        #endregion

        #region  Methods

        /// <summary>
        /// window initial method
        /// </summary>
        public RecordWin()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }

        /// <summary>
        /// window initial method extentds
        /// </summary>
        /// <param name="vrc">axVrcControl instance</param>
        /// <param name="agentId">agentID</param>
        /// <param name="info">Vrc server ip and port</param>
        /// <remarks>info format: ip(port)</remarks>
        public RecordWin(AxVRCCONTROLLib.AxVRCControl vrc, string agentId)
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            this._agentID = agentId;
            this._vrc = vrc;
            _agwBus = BusinessAdapter.GetBusinessInstance() as AgentGatewayBusiness;
        }

        /// <summary>
        /// initial vrc component
        /// </summary>
        /// <param name="agentId">agengID</param>
        /// <returns>initail result</returns>
        private bool InitialVrcComponent(string agentId)
        {
            var iniRes = false;
            try
            {
                _isVrcInitalized = true;
                if (null == _agwBus || null == _vrc)
                {
                    return false;
                }
                var token = _agwBus.QryCurrentAgentRecordToken();
                if (string.IsNullOrEmpty(token))
                {
                    Log4NetHelper.ActionLog("Agw", "QryRecordToken", "failed");
                    return false;
                }

                var recordType = rb_single.IsChecked.Value == true ? RCDTYPE_SINGLE : RCDTYPE_TWO;
                var parm = string.Format("screenNo={0};recordType={1};mainScreenX={2};mainScreenY={3};subScreenX={4};subScreenY={5}",
                    tb_screenNo.Text, recordType, DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE);
                var setRes = _vrc.SetRecordParam(parm) + "";
                Log4NetHelper.ActionLog("Vrc", "SetRecordParam", setRes);
                int port = 0;
                Int32.TryParse(tb_localPort.Text, out port);
                var iniAgentRes = _vrc.InitializeAgentEx(tb_vrpList.Text + " ", VRC_LOGINTYPE, agentId, tb_localIP.Text, port, "", token) + "";
                Log4NetHelper.ActionLog("Vrc", "InitializeAgentEx", iniAgentRes);
                iniRes = AGWErrorCode.OK.Equals(iniAgentRes);
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Common", "StartRecordScreen", exc.Message);
                return false;
            }
            return iniRes;
        }

        #endregion

        #region  Events

        private void btn_initial_Click(object sender, RoutedEventArgs e)
        {
            var res = InitialVrcComponent(_agentID);
        }

        /// <summary>
        /// start record screen click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_start_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_localPort.Text) || string.IsNullOrEmpty(tb_localPort.Text) || null == _agwBus)
            {
                return;
            }
            var res = _agwBus.StartRecordScreen();
            Log4NetHelper.ActionLog("Agw", "StartRecordScreen", res);
        }

        /// <summary>
        /// stop record screen click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (null == _agwBus)
            {
                return;
            }
            var res = _agwBus.StopRecordScreen();
            Log4NetHelper.ActionLog("Agw", "StopRecordScreen", res);
        }

        /// <summary>
        /// window closing event
        /// </summary>
        /// <param name="sender">window</param>
        /// <param name="e">closing event</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null == _vrc || !_isVrcInitalized)
            {
                return;
            }
            try
            {
                _vrc.ExitAgent();
            }
            catch (COMException exc)
            {
                Log4NetHelper.ExcepLog("Vrc", "ExitAgent", exc.Message);
            }
        }

        #endregion
    }
}

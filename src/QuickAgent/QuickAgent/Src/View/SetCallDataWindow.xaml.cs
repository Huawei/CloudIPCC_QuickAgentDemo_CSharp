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
using QuickAgent.Common;
using QuickAgent.Constants;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// Set call data window
    /// </summary>
    public partial class SetCallDataWindow : Window
    {
        private List<CallInfo> m_CallInfo = null;

        public SetCallDataWindow()
        {
            InitialWindow();
        }

        public SetCallDataWindow(List<CallInfo> info)
        {
            InitialWindow();
            dgrdCallList.SetSource(info);
            m_CallInfo = info;
        }

        #region  按钮事件

        private void btnSet_Click(Object sender, EventArgs e)
        {
            if (m_CallInfo == null)
            {
                return;
            }
            if (dgrdCallList.SelectIndex == -1)
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("setdata_selecttalk"));
                return;
            }

            var item = m_CallInfo[dgrdCallList.SelectIndex];
            var res = BusinessAdapter.GetBusinessInstance().SetData(item.CallId + "", txtBoxData.Text, 5);
            Log4NetHelper.ActionLog("Common", "Vc_SetData", res);
            if (res.Equals(AGWErrorCode.OK))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("setDataSucc"));
            }
        }

        private void btnCancel_Click(Object sender, EventArgs e)
        {
            m_CallInfo = null;
            Close();
        }

        #endregion

        private void InitialWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("setdata_head"));
        }
    }


    /// <summary>
    /// 包装 callInfo 类，增加一项 "序号" 用于界面显示
    /// </summary>
    class CallInfoWrapper : CallInfo
    {
        public int Index { set; get; }

        public CallInfoWrapper()
        {
        }

        public CallInfoWrapper(CallInfo info)
        {
            this.Called = info.Called;
            this.Caller = info.Caller;
            this.CallId = info.CallId;
            this.MediaType = LanguageResource.FindResourceMessageByKey("normalVoiceCall");
        }
    }
}

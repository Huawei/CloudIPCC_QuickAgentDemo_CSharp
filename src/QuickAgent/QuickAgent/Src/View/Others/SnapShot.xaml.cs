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
using HuaweiAgentGateway.Entity.VoiceEntity;
using QuickAgent.Src.Common;
using HuaweiAgentGateway.Utils;
using QuickAgent.Common;
using HuaweiAgentGateway;
using System.Runtime.InteropServices;

namespace QuickAgent.Src.View.Others
{
    /// <summary>
    /// Interaction logic for SnapShot.xaml
    /// </summary>
    public partial class SnapShot : Window
    {
        private const string DEAULT_SIZE = "500";

        private VoiceOCXBusiness _voice { set; get; }

        private List<DeviceItem> _videoList { set; get; }

        public SnapShot()
        {
            InitializeComponent();
        }

        public SnapShot(VoiceOCXBusiness voice)
        {
            InitializeComponent();
            this._voice = voice;
            LanguageResource.ChangeWindowLanguage(this);
        }

        private void Btn_Qry_Click(object sender, RoutedEventArgs e)
        {
            if (null == _voice)
            {
                return;
            }

            try
            {
                var res = _voice.GetDeviceVideo();
                var item = JsonUtil.DeJson<DeviceList>(res);
                _videoList = item.GetDeviceList();
                if (null == _videoList || 0 == _videoList.Count)
                {
                    return;
                }
                UIViewHelper.SelectorResetSource(Cb_Device, _videoList.Select(dev => dev.deviceName).ToList());
                Cb_Device.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
        }

        private void Btn_Select_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.ShowDialog();
            tb_path.Text = dlg.FileName;
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (null == _voice || 0 == _videoList.Count || string.IsNullOrEmpty(tb_path.Text) || null == _videoList || -1 == Cb_Device.SelectedIndex)
            {
                return;
            }
            var item = _videoList[Cb_Device.SelectedIndex];
            if (null == item)
            {
                return;
            }
            var info = JsonUtil.ToJson(new VoiceSnapShot()
            {
                deviceIndex = item.deviceIndex,
                height = DEAULT_SIZE,
                width = DEAULT_SIZE,
                filePath = tb_path.Text
            });

            try
            {
                var res = _voice.SnapShot(info);
                Log4NetHelper.ActionLog("Voice", "SnapShot", res);
            }
            catch (COMException exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace QuickAgent.Src.Common
{
    public class HoldCallInfo : INotifyPropertyChanged
    {
        public string callId { get; set; }

        private string number = string.Empty;
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                if (value != number)
                {
                    number = value; OnPropertyChanged("Number");
                }
            }
        }

        private string callerNumber = string.Empty;
        public string CallerNumber
        {
            get
            {
                return callerNumber;
            }
            set
            {
                if (value != callerNumber)
                {
                    callerNumber = value; OnPropertyChanged("CallerNumber");
                }
            }
        }

        private string calledNumber = string.Empty;
        public string CalledNumber
        {
            get
            {
                return calledNumber;
            }
            set
            {
                if (value != calledNumber)
                {
                    calledNumber = value; OnPropertyChanged("CalledNumber");
                }
            }
        }

        private string mediaType = string.Empty;
        public string MediaType
        {
            get
            {
                return mediaType;
            }
            set
            {
                if (value != mediaType)
                {
                    mediaType = value; OnPropertyChanged("MediaType");
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(value));
            }
        }
        #endregion
    }
}

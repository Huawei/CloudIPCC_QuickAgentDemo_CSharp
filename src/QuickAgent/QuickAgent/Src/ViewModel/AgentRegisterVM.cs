using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace QuickAgent.ViewModel
{
    class AgentRegisterVM : ObservableCollection<Skill>
    {
        #region 单例
        private static AgentRegisterVM agentRegisterVM = null;
        private static readonly object lockObj = new object();
        public static AgentRegisterVM GetInstance()
        {
            if (null == agentRegisterVM)
            {
                lock (lockObj)
                {
                    if (null == agentRegisterVM)
                    {
                        agentRegisterVM = new AgentRegisterVM();
                    }
                }
            }
            return agentRegisterVM;
        }
        private AgentRegisterVM() { }
        #endregion

        public ObservableCollection<Skill> obsSkill = new ObservableCollection<Skill>();
      
    }
}

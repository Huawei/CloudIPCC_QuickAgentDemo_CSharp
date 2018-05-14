using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    /// <summary>
    /// business instance adapter,sets ang gets which business type is in use.
    /// current business type: 1,agentgateway  2,ocx
    /// </summary>
    public class BusinessAdapter
    {
        public static string ErrorMessage { set; get; }

        /// <summary>
        /// default business type is agw
        /// </summary>
        public static BusinessType CurrentBusinessType = BusinessType.AgentGateway;

        /// <summary>
        /// instance cache
        /// </summary>
        private static Dictionary<BusinessType, IBusiness> instanceCache = new Dictionary<BusinessType, IBusiness>();

        static BusinessAdapter()
        {
        }

        /// <summary>
        /// add business instance
        /// </summary>
        /// <param name="type"></param>
        public static void AddBusinessInstance(BusinessType type)
        {
            if (null == instanceCache)
            {
                instanceCache = new Dictionary<BusinessType, IBusiness>();
            }
            if (instanceCache.ContainsKey(type) && instanceCache[type] != null)
            {
                return;
            }
            try
            {
                if (type == BusinessType.AgentGateway)
                {
                    instanceCache.Add(BusinessType.AgentGateway, new AgentGatewayBusiness());
                }
                else if (type == BusinessType.OCX)
                {
                    instanceCache.Add(BusinessType.OCX, new OCXBusiness());
                }
            }
            catch (ArgumentException exc)
            {
                ErrorMessage = exc.Message;
            }
        }

        /// <summary>
        /// clear business instance 
        /// </summary>
        public static void ClearBusinessInstance()
        {
            if (null == instanceCache)
            {
                return;
            }
            instanceCache.Clear();
        }

        /// <summary>
        /// get current business instance
        /// </summary>
        /// <returns></returns>
        public static IBusiness GetBusinessInstance()
        {
            if (instanceCache.ContainsKey(CurrentBusinessType))
            {
                return instanceCache[CurrentBusinessType];
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// supported business types
    /// </summary>
    public enum BusinessType
    {
        AgentGateway,
        OCX,
    }
}

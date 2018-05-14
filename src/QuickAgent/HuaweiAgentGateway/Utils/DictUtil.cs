using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Utils
{
    /// <summary>
    /// helper class used for dictionary actions
    /// </summary>
    public static class DictUtil
    {
        public static void AddKey<T, T1>(IDictionary<T, T1> dict, T key, T1 value)
        {
            if (null == dict || null == key)
            {
                return;
            }
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }
}

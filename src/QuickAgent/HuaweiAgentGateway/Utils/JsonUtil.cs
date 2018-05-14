using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace HuaweiAgentGateway.Utils
{
    /// <summary>
    /// helper class used for json 
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// error message
        /// </summary>
        public static string ErrMsg { set; get; }

        /// <summary>
        ///  convert object to json
        /// </summary>
        /// <param name="obj">object that needs to be jsoned</param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// convert json to object
        /// not recommanded
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="obj">json object</param>
        /// <returns></returns>
        public static T DeJson<T>(string obj)
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj);
            return res;
        }

        /// <summary>
        /// convert json to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeJsonEx<T>(string obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T ret = default(T);
            try
            {
                ret = serializer.Deserialize<T>(obj);
            }
            catch (ArgumentException exc)
            {
                ErrMsg = exc.Message;
            }
            catch (InvalidOperationException inexc)
            {
                ErrMsg = inexc.Message;
            }
            serializer = null;
            return ret;
        }
    }
}

using System.Reflection;
using log4net;
using HuaweiAgentGateway;

namespace QuickAgent.Common
{
    /// <summary>
    /// log helper class
    /// </summary>
    public static class Log4NetHelper
    {
        /// <summary>
        /// action log format
        /// {0}: module name
        /// {1}: action name
        /// {2}: action result
        /// </summary>
        private static string _actionType = "[{0}_{1}] Rslt: {2}";

        /// <summary>
        /// json parse failed log
        /// {0}: module name
        /// {1}: action name
        /// </summary>
        private static string _jsonParseFailedType = "[{0}_{1}] Json Parse Failed.";

        /// <summary>
        /// exception log
        /// {0}: module name
        /// {1}: action name
        /// {2}: exception message
        /// </summary>
        private static string _excMsgType = "[{0}_{1}] Exce: {2}";

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog Log()
        {
            return log;
        }

        public static void BaseLog(string parm)
        {
            Log4NetHelper.Log().Info(parm);
        }

        /// <summary>
        /// log action
        /// </summary>
        /// <param name="moduleName">module name</param>
        /// <param name="actionName">action name</param>
        /// <param name="res">action result</param>
        public static void ActionLog(string moduleName, string actionName, string res)
        {
            Log4NetHelper.Log().Info(string.Format(_actionType, moduleName, actionName, res));
        }

        /// <summary>
        /// log json parse failed
        /// </summary>
        /// <param name="moduleName">module name</param>
        /// <param name="actionName">action name</param>
        public static void JsonParseFailed(string moduleName, string actionName)
        {
            Log4NetHelper.Log().Error(string.Format(_jsonParseFailedType, moduleName, actionName));
        }

        /// <summary>
        /// log exception info
        /// </summary>
        /// <param name="moduleName">module name</param>
        /// <param name="actionName">action name</param>
        /// <param name="msg">exception message</param>
        public static void ExcepLog(string moduleName, string actionName, string msg)
        {
            Log4NetHelper.Log().Error(string.Format(_excMsgType, moduleName, actionName, msg));
        }
    }
}
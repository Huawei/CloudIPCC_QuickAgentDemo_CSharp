using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public class AGWErrorCode
    {
        public const string OK = "0";

        public const string Empty = "-1";

        public const string SpecErr = "-2";

        public const string InvalidUserNameOrPassword = "100-004";

        public const string NotLogIn = "100-006";

        public const string InvalidAgentId = "100-007";

        public const string NoRight = "000-003";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public class LoginParam
    {
        public string password { get; set; }

        public string phonenum { get; set; }

        public bool autoanswer { get; set; }

        public bool autoenteridle { get; set; }

        public int status { get; set; }
    }
}

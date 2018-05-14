using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.AGWEntity
{
    public class TextChatRingParm
    {
        public string caller { set; get; }

        public bool isInnerCall { set; get; }

        public string called { set; get; }

        public string callId { set; get; }

        public string attachData { set; get; }

        public string realCaller { set; get; }

        public string userInfo { set; get; }

        public string displayName { set; get; }

        public string subMediaType { set; get; }
    }


    public class TextChatConnectParm
    {
        public string caller { set; get; }

        public string isInnerCall { set; get; }

        public string called { set; get; }

        public string uvid { set; get; }

        public string callId { set; get; }

        public string attachData { set; get; }
    }


    public class TextChatDisconnParm
    {
        public string callId { set; get; }
    }


    public class TextChatDataRecvParm
    {
        public string sender { set; get; }

        public string isInnerCall { set; get; }

        public string chatContent { set; get; }

        public string callId { set; get; }

        public string chatContentType { set; get; }
    }


    public class TextChatDataRecvExParm
    {
        public string sender { set; get; }

        public string senderTime { set; get; }

        public string isInnerCall { set; get; }

        public string sessionId { set; get; }

        public string chatContent { set; get; }

        public string callId { set; get; }

        public string chatId { set; get; }

        public string chatContentType { set; get; }
    }


    public class TextChatInnerParm
    {
        public string caller { set; get; }

        public string isInnerCall { set; get; }

        public string called { set; get; }

        public string callId { set; get; }
    }


    public class TextChatCallConfirmParm
    {
        public string result { set; get; }

        public string confirmer { set; get; }

        public string chatId { set; get; }
    }


    public class TextChatNoRealTmCallParm
    {
        public string caller { set; get; }

        public string dataSeqNo { set; get; }

        public string callData { set; get; }

        public string innerSid { set; get; }

        public string mediaType { set; get; }

        public string subMediaType { set; get; }
    }


    public class TextCharPostDataSuccParm
    {
        public string result { set; get; }

        public string confirmer { set; get; }

        public string callId { set; get; }

        public string chatId { set; get; }
    }


    public class TextChatPostDataFailParm
    {
        public string callId { set; get; }

        public string chatId { set; get; }
    }


    public class TCSendMsgParm
    {
        public string callId { set; get; }

        public string content { set; get; }

        public bool needCheck { set; get; }

        public int chatId { set; get; }
    }


    public class TCTransParm
    {
        public int addesstype { set; get; }

        public string destaddr { set; get; }

        public string callid { set; get; }

        public string attachdata { set; get; }

        public int mode { set; get; }
    }


    public class TCRes<T>
    {
        public string message { set; get; }

        public string retcode { set; get; }

        public TCEvent<T> @event { set; get; }
    }


    public class TCEvent<T>
    {
        public string eventType { set; get; }

        public string workNo { set; get; }

        public T content { set; get; }
    }


    public class TCUploadRes
    {
        public string filePath { set; get; }

        public string fileName { set; get; }

        public string chatId { set; get; }
    }


    public class TcSmsParm
    {
        public string skillId { set; get; }

        public string chatId { set; get; }

        public string chatReceiver { set; get; }

        public string content { set; get; }

        public string jauIP { set; get; }

        public string jauPort { set; get; }
    }


    public class TcCallContentRes
    {
        public string totalPageNo { set; get; }

        public string totalCount { set; get; }

        public string pageNo { set; get; }

        public string pageSize { set; get; }

        public List<CallContentData> weccCallContentList { set; get; }
    }


    public class CallContentData
    {
        public string id { set; get; }

        public string callId { set; get; }

        public string subMediaType { set; get; }

        public string sender { set; get; }

        public string reciver { set; get; }

        public string contentType { set; get; }

        public string content { set; get; }

        public string direction { set; get; }

        public string createTime { set; get; }

        public string confirmFlag { set; get; }

        public string status { set; get; }

        public object result { set; get; }
    }


    public class TcCallSessionRes
    {
        public string totalPageNo { set; get; }

        public string totalCount { set; get; }

        public string pageNo { set; get; }

        public string pageSize { set; get; }

        public List<CallSessionData> weccCallBillList { set; get; }
    }


    public class CallSessionData
    {
        public string id { set; get; }

        public string ccId { set; get; }

        public string vdnId { set; get; }

        public string callId { set; get; }

        public string innerCall { set; get; }

        public string mediaType { set; get; }

        public string subMediaType { set; get; }

        public string accessType { set; get; }

        public string caller { set; get; }

        public string called { set; get; }

        public string accessCode { set; get; }

        public string skillId { set; get; }

        public string realCaller { set; get; }

        public string callerName { set; get; }

        public string beginTime { set; get; }

        public string connectTime { set; get; }

        public string endTime { set; get; }

        public string releaseCause { set; get; }

        public string platformReleaseCause { set; get; }

        public string logDate { set; get; }

        public string callidNum { set; get; }

        public string remark { set; get; }

        public string receiveMsgNum { set; get; }

        public string sendMsgNum { set; get; }
    }


    public class CallSessionQryParm
    {
        public string startTime { set; get; }

        public string endTime { set; get; }

        public string pageNo { set; get; }

        public string pageSize { set; get; }

        public string callType { set; get; }

        public string caller { set; get; }

        public string called { set; get; }

        public string realCaller { set; get; }

        public string callerName { set; get; }

        public string mediaType { set; get; }

        public string subMediaType { set; get; }
    }


    public class RecordTokenParm
    {
        public string token { set; get; }
    }


    public class SmsMsgEvent
    {
        public string caller {set;get;}

        public string dataSeqNo {set;get;}

        public string callData {set;get;}

        public string innerSid {set;get;}

        public string mediaType {set;get;}

        public string subMediaType { set; get; }
    }
}

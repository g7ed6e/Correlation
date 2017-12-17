using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace Correlation.WcfHost
{
    public class CorrelationContextDispatchMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message message, IClientChannel channel, InstanceContext instanceContext)
        {
            string requestId = WebOperationContext.Current.IncomingRequest.Headers[CorrelationContext.RequestIdHeaderName];
            CorrelationContext.Current.RequestId = requestId;
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }
    }
}

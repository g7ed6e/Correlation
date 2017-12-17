using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace Correlation.WcfClient
{
    public class CorrelationContextClientMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            WebOperationContext.Current.OutgoingResponse.Headers[CorrelationContext.RequestIdHeaderName] = CorrelationContext.Current.RequestId;
            return null;
        }
    }
}

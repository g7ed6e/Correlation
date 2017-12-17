using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Correlation.WcfClient
{
    public class CorrelationContextEndpointBehavior :
        BehaviorExtensionElement,
        IEndpointBehavior
    {
        public override Type BehaviorType => typeof(CorrelationContextEndpointBehavior);

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new CorrelationContextClientMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        protected override object CreateBehavior()
        {
            return new CorrelationContextEndpointBehavior();
        }
    }
}

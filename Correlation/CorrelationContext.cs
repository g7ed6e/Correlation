using System;
#if NET45
using System.Runtime.Remoting.Messaging;
#elif NETSTANDARD13
using System.Threading;
#endif
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Correlation.WcfHost")]
[assembly: InternalsVisibleTo("Correlation.WcfClient")]
[assembly: InternalsVisibleTo("Correlation.WebApiHost")]

namespace Correlation
{
    /// <summary>
    /// Provide an entry point to the current request's  and current session's correlation id
    /// </summary>
    public class CorrelationContext
    {
        private static readonly CorrelationContext current = new CorrelationContext();

        private CorrelationContext()
        {

        }

        public static CorrelationContext Current { get { return current; } }

        public static readonly string DefaultRequestIdHeaderName = "x-correlation-request-id";
        public static readonly string DefaultSessionIdHeaderName = "x-correlation-session-id";
        private static readonly Func<string> DefaultRequestIdHeaderNameProvider = () => DefaultRequestIdHeaderName;
        private static readonly Func<string> DefaultSessionIdHeaderNameProvider = () => DefaultSessionIdHeaderName;
        private static readonly Func<string> DefaultCorrelationIdFactory = () => Guid.NewGuid().ToString();

        private static Func<string> requestIdHeaderNameProvider = DefaultRequestIdHeaderNameProvider;
        private static Func<string> sessionIdHeaderNameProvider = DefaultSessionIdHeaderNameProvider;
        private static Func<string> requestIdFactory = DefaultCorrelationIdFactory;
        private static Func<string> sessionIdFactory = DefaultCorrelationIdFactory;

        public static string RequestIdHeaderName
        {
            get { return GetValueFromDelegateOrDefault(requestIdHeaderNameProvider, DefaultRequestIdHeaderName); }
        }

        public static string SessionIdHeaderName
        {
            get { return GetValueFromDelegateOrDefault(sessionIdHeaderNameProvider, DefaultSessionIdHeaderName); }
        }

        /// <summary>
        /// Delegate responsible for providing the HTTP Header name of the request's correlation id.
        /// Defaults to <see cref="DefaultRequestIdHeaderName"/>
        /// </summary>
        public static Func<string> RequestIdHeaderNameProvider
        {
            internal get { return requestIdHeaderNameProvider; }
            set
            {
                if(value != null)
                {
                    requestIdHeaderNameProvider = value;
                }
            }
        }

        /// <summary>
        /// Delegate responsible for providing the HTTP Header name of the session's correlation id
        /// Defaults to <see cref="DefaultSessionIdHeaderName"/>
        /// </summary>
        public static Func<string> SessionIdHeaderNameProvider
        {
            internal get { return sessionIdHeaderNameProvider; }
            set
            {
                if(value != null)
                {
                    sessionIdHeaderNameProvider = value;
                }
            }
        }

        /// <summary>
        /// Delegate responsible for providing request's correlation id value when no HTTP Header found with name: <see cref="RequestIdHeaderNameProvider"/>
        /// Default to <code>System.Guid.NewGuid().ToString()</code>
        /// </summary>
        public static Func<string> RequestIdFactory
        {
            internal get { return requestIdFactory; }
            set
            {
                if(value != null)
                {
                    requestIdFactory = value;
                }
            }
        }

        /// <summary>
        /// Delegate responsible for providing session's correlation id value when no HTTP Header found with name: <see cref="SessionIdHeaderNameProvider"/>
        /// Default to <code>System.Guid.NewGuid().ToString()</code>
        /// </summary>
        public static Func<string> SessionIdFactory
        {
            internal get { return sessionIdFactory; }
            set
            {
                if (value != null)
                {
                    sessionIdFactory = value;
                }
            }
        }

#if NET45
        private static readonly string CorrelationContextContextPropertiesNamedDataSlot = "CorrelationContextContextPropertiesNamedDataSlot";
#elif NETSTANDARD13
        private static readonly AsyncLocal<ContextProperties> contextProperties = new AsyncLocal<ContextProperties>();
#endif
        private class ContextProperties
        {
            public string RequestId { get; private set; }
            public string SessionId { get; private set; }

            public ContextProperties(string requestId = null, string sessionId = null)
            {
                this.RequestId = requestId;
                this.SessionId = sessionId;
            }
        }

        private ContextProperties Properties
        {
            get
            {
#if NET45
                return CallContext.LogicalGetData(CorrelationContextContextPropertiesNamedDataSlot) as ContextProperties;
#elif NETSTANDARD13
                return contextProperties.Value;
#endif
            }
            set
            {
#if NET45
                CallContext.LogicalSetData(CorrelationContextContextPropertiesNamedDataSlot, value);
#elif NETSTANDARD13
                contextProperties.Value = value;
#endif
            }
        }

        /// <summary>
        /// Session's correlation id
        /// </summary>
        public string SessionId
        {
            get
            {
                return Properties?.SessionId;
            }
            internal set
            {
                if(string.IsNullOrEmpty(value))
                {
                    value = GetValueFromDelegateOrDefault(SessionIdFactory, DefaultCorrelationIdFactory.Invoke());
                }
                var properties = Properties != null ?
                    new ContextProperties(requestId: Properties?.RequestId, sessionId: value)
                    : new ContextProperties(sessionId: value);
                Properties = properties;
            }
        }

        private static string GetValueFromDelegateOrDefault(Func<string> @delegate, string @default)
        {
            System.Diagnostics.Debug.Assert(@default != null);
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(@default));
            try
            {
                return @delegate.Invoke();
            }
            catch
            {
                return @default;
            }
        }

        /// <summary>
        /// Request's correlation id
        /// </summary>
        public string RequestId
        {
            get
            {
                return Properties?.RequestId;
            }
            internal set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = GetValueFromDelegateOrDefault(requestIdFactory, DefaultCorrelationIdFactory.Invoke());
                }
                var properties = Properties != null ?
                    new ContextProperties(requestId: value, sessionId: Properties?.SessionId)
                    : new ContextProperties(requestId: value);
                Properties = properties;
            }
        }
    }
}

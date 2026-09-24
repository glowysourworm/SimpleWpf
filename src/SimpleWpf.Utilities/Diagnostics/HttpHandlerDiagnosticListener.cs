using System.Net.Http;
using System.Reflection;

using SimpleWpf.Extensions.Event;

namespace SimpleWpf.Utilities.Diagnostics
{
    internal class HttpHandlerDiagnosticListener : IObserver<KeyValuePair<string, object?>>
    {
        private const string REQUEST_START_KEY = "System.Net.Http.HttpRequestOut.Start";
        private const string REQUEST_STOP_KEY = "System.Net.Http.HttpRequestOut.Stop";

        private const string REQUEST_DATA_TYPE = "System.Net.Http.DiagnosticsHandler+ActivityStartData, System.Net.Http";
        private const string RESPONSE_DATA_TYPE = "System.Net.Http.DiagnosticsHandler+ActivityStopData, System.Net.Http";

        public event SimpleEventHandler<HttpRequestMessage> HttpRequestEvent;
        public event SimpleEventHandler<HttpResponseMessage> HttpResponseEvent;

        private static readonly PropertyInfo _requestProperty;
        private static readonly PropertyInfo _responseProperty;

        static HttpHandlerDiagnosticListener()
        {
            var requestDataType = Type.GetType(REQUEST_DATA_TYPE, throwOnError: true);
            var responseDataType = Type.GetType(RESPONSE_DATA_TYPE, throwOnError: true);

            _requestProperty = requestDataType.GetProperty("Request");
            _responseProperty = responseDataType.GetProperty("Response");
        }
        internal HttpHandlerDiagnosticListener()
        {

        }

        public void OnCompleted()
        {
        }
        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            // NOTE: Legacy applications can use "System.Net.Http.HttpRequest" and "System.Net.Http.Response"
            //
            if (value.Key == REQUEST_START_KEY)
            {
                // The type is private, so we need to use reflection to access it... (??)
                //
                var request = GetRequest(value.Value);

                if (this.HttpRequestEvent != null)
                    this.HttpRequestEvent(request);
            }
            else if (value.Key == REQUEST_STOP_KEY)
            {
                // The type is private, so we need to use reflection to access it... (??)
                //
                var response = GetResponse(value.Value);

                if (this.HttpResponseEvent != null)
                    this.HttpResponseEvent(response);
            }
        }

        private HttpRequestMessage GetRequest(object value)
        {
            return (HttpRequestMessage)_requestProperty.GetValue(value);
        }

        private static HttpResponseMessage GetResponse(object value)
        {
            return (HttpResponseMessage)_responseProperty.GetValue(value);
        }
    }
}

using System.Diagnostics;
using System.Net.Http;

using SimpleWpf.Extensions.Event;

namespace SimpleWpf.Utilities.Diagnostics
{
    public class HttpRequestResponseObserver : IDisposable, IObserver<DiagnosticListener>
    {
        private readonly HttpHandlerDiagnosticListener _diagnosticListener;
        private IDisposable _subscription;

        public event SimpleEventHandler<HttpRequestMessage> HttpRequestEvent;
        public event SimpleEventHandler<HttpResponseMessage> HttpResponseEvent;
        public event SimpleEventHandler<Exception> ErrorEvent;

        public HttpRequestResponseObserver()
        {
            _diagnosticListener = new HttpHandlerDiagnosticListener();

            _diagnosticListener.HttpRequestEvent += OnHttpRequestMessage;
            _diagnosticListener.HttpResponseEvent += OnHttpResponseEvent;
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "HttpHandlerDiagnosticListener" && _subscription == null)
            {
                _subscription = value.Subscribe(_diagnosticListener);
            }
        }

        public void OnCompleted()
        {

        }
        public void OnError(Exception error)
        {
            if (this.ErrorEvent != null)
                this.ErrorEvent(error);
        }
        public void Dispose()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }

        private void OnHttpRequestMessage(HttpRequestMessage sender)
        {
            if (this.HttpRequestEvent != null)
                this.HttpRequestEvent(sender);
        }

        private void OnHttpResponseEvent(HttpResponseMessage sender)
        {
            if (this.HttpResponseEvent != null)
                this.HttpResponseEvent(sender);
        }

        //private sealed class HttpHandlerDiagnosticListener : IObserver<KeyValuePair<string, object>>
        //{
        //    private static readonly Func<object, HttpRequestMessage> RequestAccessor = CreateGetRequest();
        //    private static readonly Func<object, HttpResponseMessage> ResponseAccessor = CreateGetResponse();

        //    public void OnCompleted() { }
        //    public void OnError(Exception error) { }

        //    public void OnNext(KeyValuePair<string, object> value)
        //    {
        //        // note: Legacy applications can use "System.Net.Http.HttpRequest" and "System.Net.Http.Response"
        //        if (value.Key == "System.Net.Http.HttpRequestOut.Start")
        //        {
        //            // The type is private, so we need to use reflection to access it.
        //            var request = RequestAccessor(value.Value);
        //            Console.WriteLine($"{request.Method} {request.RequestUri} {request.Version} (UserAgent: {request.Headers.UserAgent})");
        //        }
        //        else if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
        //        {
        //            // The type is private, so we need to use reflection to access it.
        //            var response = ResponseAccessor(value.Value);
        //            Console.WriteLine($"{response.StatusCode} {response.RequestMessage.RequestUri}");
        //        }
        //    }

        //    private static Func<object, HttpRequestMessage> CreateGetRequest()
        //    {
        //        var requestDataType = Type.GetType("System.Net.Http.DiagnosticsHandler+ActivityStartData, System.Net.Http", throwOnError: true);
        //        var requestProperty = requestDataType.GetProperty("Request");
        //        return (object o) => (HttpRequestMessage)requestProperty.GetValue(o);
        //    }

        //    private static Func<object, HttpResponseMessage> CreateGetResponse()
        //    {
        //        var requestDataType = Type.GetType("System.Net.Http.DiagnosticsHandler+ActivityStopData, System.Net.Http", throwOnError: true);
        //        var requestProperty = requestDataType.GetProperty("Response");
        //        return (object o) => (HttpResponseMessage)requestProperty.GetValue(o);
        //    }
        //}
    }

    //sealed class HttpEventListener : EventListener
    //{
    //    protected override void OnEventSourceCreated(EventSource eventSource)
    //    {
    //        switch (eventSource.Name)
    //        {
    //            case "System.Net.Http":
    //                EnableEvents(eventSource, EventLevel.Informational, EventKeywords.All);
    //                break;

    //                //// Enable EventWrittenEventArgs.ActivityId to correlate Start and Stop events
    //                //case "System.Threading.Tasks.TplEventSource":
    //                //    const EventKeywords TasksFlowActivityIds = (EventKeywords)0x80;
    //                //    EnableEvents(eventSource, EventLevel.LogAlways, TasksFlowActivityIds);
    //                //    break;
    //        }

    //        base.OnEventSourceCreated(eventSource);
    //    }

    //    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    //    {
    //        // note: Use eventData.ActivityId to correlate Start and Stop events
    //        if (eventData.EventId == 1) // eventData.EventName == "RequestStart"
    //        {
    //            var scheme = (string)eventData.Payload[0];
    //            var host = (string)eventData.Payload[1];
    //            var port = (int)eventData.Payload[2];
    //            var pathAndQuery = (string)eventData.Payload[3];
    //            var versionMajor = (byte)eventData.Payload[4];
    //            var versionMinor = (byte)eventData.Payload[5];
    //            var policy = (HttpVersionPolicy)eventData.Payload[6];

    //            Console.WriteLine($"{eventData.ActivityId} {eventData.EventName} {scheme}://{host}:{port}{pathAndQuery} HTTP/{versionMajor}.{versionMinor}");
    //        }
    //        else if (eventData.EventId == 2) // eventData.EventName == "RequestStop"
    //        {
    //            Console.WriteLine(eventData.ActivityId + " " + eventData.EventName);
    //        }
    //    }
    //}

    //internal sealed class HttpRequestsObserver : IDisposable, IObserver<DiagnosticListener>
    //{
    //    private IDisposable _subscription;

    //    public void OnNext(DiagnosticListener value)
    //    {
    //        if (value.Name == "HttpHandlerDiagnosticListener")
    //        {
    //            Debug.Assert(_subscription == null);
    //            _subscription = value.Subscribe(new HttpHandlerDiagnosticListener());
    //        }
    //    }

    //    public void OnCompleted()
    //    {
    //    }
    //    public void OnError(Exception error)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        _subscription?.Dispose();
    //    }

    //    private sealed class HttpHandlerDiagnosticListener : IObserver<KeyValuePair<string, object>>
    //    {
    //        private static readonly Func<object, HttpRequestMessage> RequestAccessor = CreateGetRequest();
    //        private static readonly Func<object, HttpResponseMessage> ResponseAccessor = CreateGetResponse();

    //        public void OnCompleted() { }
    //        public void OnError(Exception error) { }

    //        public void OnNext(KeyValuePair<string, object> value)
    //        {
    //            // note: Legacy applications can use "System.Net.Http.HttpRequest" and "System.Net.Http.Response"
    //            if (value.Key == "System.Net.Http.HttpRequestOut.Start")
    //            {
    //                // The type is private, so we need to use reflection to access it.
    //                var request = RequestAccessor(value.Value);
    //                Console.WriteLine($"{request.Method} {request.RequestUri} {request.Version} (UserAgent: {request.Headers.UserAgent})");
    //            }
    //            else if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
    //            {
    //                // The type is private, so we need to use reflection to access it.
    //                var response = ResponseAccessor(value.Value);
    //                Console.WriteLine($"{response.StatusCode} {response.RequestMessage.RequestUri}");
    //            }
    //        }

    //        private static Func<object, HttpRequestMessage> CreateGetRequest()
    //        {
    //            var requestDataType = Type.GetType("System.Net.Http.DiagnosticsHandler+ActivityStartData, System.Net.Http", throwOnError: true);
    //            var requestProperty = requestDataType.GetProperty("Request");
    //            return (object o) => (HttpRequestMessage)requestProperty.GetValue(o);
    //        }

    //        private static Func<object, HttpResponseMessage> CreateGetResponse()
    //        {
    //            var requestDataType = Type.GetType("System.Net.Http.DiagnosticsHandler+ActivityStopData, System.Net.Http", throwOnError: true);
    //            var requestProperty = requestDataType.GetProperty("Response");
    //            return (object o) => (HttpResponseMessage)requestProperty.GetValue(o);
    //        }
    //    }
    //}
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PlateHTTP.Core;



namespace PlateHTTP.Interfaces.Core {
    public interface IWebApplication {
        public WebApplicationConfig? Config { get; set; }
        public bool IsAlive { get; }
        public string? Prefix { get; }
        public Dictionary<string, Dictionary<string, Func<Request, Response, Task>>> Routes { get; }
        public Dictionary<string, IWebApplication> SubApplications { get; }
        public Logging.ILogger? Logger { get; }
        public StaticFiles.IStaticFileLoader? StaticLoader { get; }
        public object? TemplateLoader { get; set; }


        void EnableLogging(string minimumLogLevel);
        void UseLogger(Logging.ILogger logger);

        void UseStaticLoader(StaticFiles.IStaticFileLoader staticLoader);

        Task Invoke(HttpListenerRequest listenerRequest, HttpListenerResponse listenerResponse);

        // route functions
        void Route(string httpMethod, string path, Func<Request, Response, Task> callback);
        void Get(string path, Func<Request, Response, Task> callback);
        void Post(string path, Func<Request, Response, Task> callback);
        void Put(string path, Func<Request, Response, Task> callback);
        void Delete(string path, Func<Request, Response, Task> callback);

        // mount
        void Mount(string prefix, IWebApplication subApplication);

        void Start(string host, int port);
        void Stop();

        void Dispose();
    }
}

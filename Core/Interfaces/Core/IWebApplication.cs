using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using PlateHTTP.Core;



namespace PlateHTTP.Interfaces.Core {
    public interface IWebApplication {
        public WebApplicationConfig? Config { get; set; }
        public bool IsAlive { get; }
        public string? Prefix { get; }
        public Dictionary<string, Dictionary<string, Func<IRequest, IResponse, Task>>> Routes { get; }
        public Dictionary<string, IWebApplication> SubApplications { get; }
        public Logging.ILogger? Logger { get; }
        public StaticFiles.IStaticFileLoader? StaticLoader { get; }
        public Dictionary<string, object> Extensions { get; }


        void EnableLogging(string minimumLogLevel);
        void UseLogger(Logging.ILogger logger);

        void UseStaticLoader(StaticFiles.IStaticFileLoader staticLoader);

        Task Invoke(HttpListenerRequest listenerRequest, HttpListenerResponse listenerResponse);

        // route functions
        void Route(string httpMethod, string path, Func<IRequest, IResponse, Task> callback);
        void Get(string path, Func<IRequest, IResponse, Task> callback);
        void Post(string path, Func<IRequest, IResponse, Task> callback);
        void Put(string path, Func<IRequest, IResponse, Task> callback);
        void Delete(string path, Func<IRequest, IResponse, Task> callback);

        // mount
        void Mount(string prefix, IWebApplication subApplication);

        void Start(string host, int port);
        void Stop();

        void Dispose();
    }
}

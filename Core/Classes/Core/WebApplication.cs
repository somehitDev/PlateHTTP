using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlateHTTP.Interfaces.Core;
using PlateHTTP.Interfaces.Logging;
using PlateHTTP.Interfaces.StaticFiles;
using PlateHTTP.Enums;



namespace PlateHTTP.Core {
    public class WebApplication: IWebApplication {
        // private attributes
        private HttpListener? Listener;
        private string SafePrefix {
            get => this.Prefix ?? "";
        }

        // public attributes
        public WebApplicationConfig? Config { get; set; }
        public bool IsAlive { get; private set; }
        public string? Prefix { get; private set; }
        public Dictionary<string, Dictionary<string, Func<Request, Response, Task>>> Routes { get; private set; }
        public Dictionary<string, IWebApplication> SubApplications { get; private set; }
        public ILogger? Logger { get; private set; }
        public IStaticFileLoader? StaticLoader { get; private set; }
        public object? TemplateLoader { get; set; }

        // events
        public event Func<IWebApplication, Task>? OnStartup;
        public event Func<IWebApplication, Task>? OnShutdown;
        public event Func<Request, Response, Task>? BeforeRequest;
        public event Func<Request, Task>? AfterRequest;


        public WebApplication(WebApplicationConfig? config = null) {
            this.Config = config;

            this.IsAlive = false;
            this.Routes = new Dictionary<string, Dictionary<string, Func<Request, Response, Task>>>() {
                { "GET", new Dictionary<string, Func<Request, Response, Task>>() },
                { "POST", new Dictionary<string, Func<Request, Response, Task>>() },
                { "PUT", new Dictionary<string, Func<Request, Response, Task>>() },
                { "DELETE", new Dictionary<string, Func<Request, Response, Task>>() }
            };
            this.SubApplications = new Dictionary<string, IWebApplication>();

            this.Logger = null;
            this.StaticLoader = null;
            this.TemplateLoader = null;

            // map favicon default
            this.Get("/favicon.ico", async ( request, response ) => {
                await response.Send(Assembly.GetExecutingAssembly().GetManifestResourceStream("PlateHTTP.Assets.favicon.ico"), "image/x-icon", StatusCode.OK);
            });
        }

        public override string ToString() {
            if (string.IsNullOrEmpty(this.Prefix)) {
                return $"<WebApplication at {this.GetHashCode()}>";
            }
            else {
                return $"<WebApplication at {this.GetHashCode()} / Prefix:{this.Prefix}>";
            }
        }

        //== private functions
        // pattern search
        private ( bool, ReadOnlyDictionary<string, string> ) ParseUrlPattern(string httpMethod, string url) {
            var isMatch = false;
            var urlParams = new Dictionary<string, string>();

            foreach (var item in this.Routes[httpMethod]) {
                if (item.Key.Contains(":")) {
                    var pattern = this.SafePrefix + item.Key;
                    var patternIndexes = new List<int>();

                    int patternIndex = 0;
                    foreach (var part in item.Key.Split("/")) {
                        if (part.StartsWith(":")) {
                            pattern = pattern.Replace($"/{part}", "/*");
                            patternIndexes.Add(patternIndex);
                        }

                        patternIndex += 1;
                    }

                    var regex = new Regex(pattern);
                    if (regex.IsMatch(this.SafePrefix + url)) {
                        var patternParts = item.Key.Split("/");
                        var urlParts = item.Key.Split("/");

                        foreach (var index in patternIndexes) {
                            urlParams[patternParts[index].Substring(1)] = urlParts[index];
                        }
                    }
                }
                else {
                    if (item.Key == url) {
                        isMatch = true;
                        break;
                    }
                }
            }

            return ( isMatch, new ReadOnlyDictionary<string, string>(urlParams) );
        }
        // listener task
        private async Task CreateListenerTask() {
            while (this.IsAlive) {
                HttpListenerContext context = await this.Listener!.GetContextAsync();
                HttpListenerRequest req = context.Request;
                
                string reqUrl = req.Url!.LocalPath;

                if (reqUrl == "/shutdown") {
                    this.Stop();
                }
                else {
                    await this.Invoke(req, context.Response);
                }
            }
        }

        //== public functions
        // logger setup functions
        public void EnableLogging(string minimumLogLevel) {
            this.Logger = new Logging.Logger("PlateHTTP", minimumLogLevel);
        }
        public void UseLogger(ILogger logger) {
            this.Logger = logger;
        }
        // static file
        public void UseStaticLoader(IStaticFileLoader staticLoader) {
            this.StaticLoader = staticLoader;
        }

        public async Task Invoke(HttpListenerRequest listenerRequest, HttpListenerResponse listenerResponse) {
            // check url matches with subapplication prefix
            var prefix = "/" + listenerRequest.Url!.LocalPath.Split("/")[1];
            if (this.SubApplications.ContainsKey(prefix)) {
                await this.SubApplications[prefix].Invoke(listenerRequest, listenerResponse);
            }
            else {
                var request = new Request(listenerRequest, this.Prefix);
                var response = new Response(this, listenerResponse);

                var ( isMatch, urlParams ) = this.ParseUrlPattern(request.HttpMethod, request.Path);

                if (isMatch) {
                    request.SetUrlParams(urlParams);

                    try {
                        // call beforerequest event
                        if (this.BeforeRequest != null) {
                            await this.BeforeRequest.Invoke(request, response);
                        }

                        // call route callback
                        await this.Routes[request.HttpMethod][request.Path](request, response);

                        // call afterrequest event
                        if (this.AfterRequest != null) {
                            await this.AfterRequest.Invoke(request);
                        }

                        // log after job finished
                        if (!new string[] { "/favicon.ico" }.Contains(request.FullPath)) {
                            this.Logger?.Debug($"Endpoint [{request.HttpMethod}] `{request.FullPath}`");
                        }
                    }
                    catch (KeyNotFoundException e) {
                        if (!new string[] { "/favicon.ico" }.Contains(request.FullPath)) {
                            this.Logger?.Error($"Endpoint [{request.HttpMethod}] `{request.FullPath}` - 404 Not Found");
                            await response.RenderDefaultPage(StatusCode.NotFound, e.ToString());
                        }
                    }
                    catch (Exception e) {
                        this.Logger?.Error(e.Message);
                        await response.RenderDefaultPage(StatusCode.InternalServerError, e.ToString());
                    }
                }
                else {
                    await response.RenderDefaultPage(StatusCode.NotFound);
                    this.Logger?.Error($"Endpoint `{request.Path}` not found!");
                }

                response.Close();

                request.Dispose();
                response.Dispose();
            }
        }

        // route functions
        public void Route(string httpMethod, string path, Func<Request, Response, Task> callback) {
            var routeMap = this.Routes[httpMethod];

            // check path startswith "/"
            if (!path.StartsWith("/")) {
                this.Logger?.Except("routePoint must startswith `/`!");
            }

            if (routeMap.ContainsKey(path)) {
                this.Logger?.Except($"RoutePoint {path} already exists!");
            }

            routeMap[path] = callback;
        }
        public void Get(string path, Func<Request, Response, Task> callback) {
            this.Route("GET", path, callback);
        }
        public void Post(string path, Func<Request, Response, Task> callback) {
            this.Route("POST", path, callback);
        }
        public void Put(string path, Func<Request, Response, Task> callback) {
            this.Route("PUT", path, callback);
        }
        public void Delete(string path, Func<Request, Response, Task> callback) {
            this.Route("DELETE", path, callback);
        }

        // mount
        public void Mount(string prefix, IWebApplication subApplication) {
            var subApp = (WebApplication)subApplication;

            subApp.Prefix = prefix;
            if (subApp.Logger == null && this.Logger != null) {
                subApp.UseLogger(this.Logger);
            }
            if (subApp.StaticLoader == null && this.StaticLoader != null) {
                subApp.UseStaticLoader(this.StaticLoader);
            }
            if (subApp.Config == null && this.Config != null) {
                subApp.Config = this.Config;
            }
            subApp.Routes["GET"].Remove("/favicon.ico");

            this.SubApplications[prefix] = subApp;
        }

        // start
        public void Start(string host = "*", int port = 8000) {
            // create listener
            this.Listener = new HttpListener();

            // create base url
            var baseUrl = $"http://{host}:{port}/";
            this.Listener.Prefixes.Add(baseUrl);

            // start listener
            this.Listener.Start();

            // set is alive to true
            this.IsAlive = true;

            // call event
            this.OnStartup?.Invoke(this);

            // log through logger
            this.Logger?.Info($"Server listening on {baseUrl.Replace("*", "0.0.0.0")}");
            this.Logger?.Info("Press `Ctrl+C` to shutdown server...\n");

            // run listener task
            var ListenerTask = this.CreateListenerTask();
            ListenerTask.GetAwaiter().GetResult();

            // if closed, run close
            this.Stop();
        }
        // stop
        public void Stop() {
            // close task loop
            this.IsAlive = false;
            // close listener
            this.Listener!.Close();

            // call event
            this.OnShutdown?.Invoke(this);

            // dispose
            this.Dispose();
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using PlateHTTP.Interfaces.Core;



namespace PlateHTTP.Core {
    public class Request: Interfaces.Core.IRequest {
        // private attributes
        private HttpListenerRequest ListenerRequest;

        // public attributes
        public string Prefix { get; private set; }
        public string HttpMethod {
            get => this.ListenerRequest.HttpMethod;
        }
        public string Path {
            get {
                if (string.IsNullOrEmpty(this.Prefix)) {
                    return this.FullPath;
                }
                else {
                    var path = this.FullPath.Replace(this.Prefix, "");
                    if (string.IsNullOrEmpty(path)) {
                        return "/";
                    }
                    else {
                        return path;
                    }
                }
            }
        }
        public string FullPath {
            get => this.ListenerRequest.Url!.LocalPath;
        }
        public string Body {
            get {
                string body;
                using (var reader = new StreamReader(this.ListenerRequest.InputStream)) {
                    body = reader.ReadToEnd();
                }

                return body;
            }
        }
        public ReadOnlyDictionary<string, string> UrlParams { get; private set; }
        public ReadOnlyDictionary<string, string> QueryParams {
            get {
                var queryParams = new Dictionary<string, string>();
                foreach (var key in this.ListenerRequest.QueryString.AllKeys) {
                    queryParams[key!] = this.ListenerRequest.QueryString[key]!.ToString();
                }

                return new ReadOnlyDictionary<string, string>(queryParams);
            }
        }

        public static IRequest FromHttpRequest(HttpListenerRequest listenerRequest, ReadOnlyDictionary<string, string> urlParams, string? prefix) {
            return new Request(listenerRequest, urlParams, prefix);
        }

        public Request(HttpListenerRequest listenerRequest, ReadOnlyDictionary<string, string> urlParams, string? prefix) {
            this.ListenerRequest = listenerRequest;
            this.Prefix = prefix ?? string.Empty;
            this.UrlParams = urlParams;
        }

        internal void SetUrlParams(ReadOnlyDictionary<string, string> urlParams) {
            this.UrlParams = urlParams;
        }

        public ReadOnlyDictionary<string, dynamic> GetJSON() {
            try {
                var data = new Dictionary<string, dynamic>();
                foreach (var part in this.Body.Split("&")) {
                    var partItem = part.Split("=");
                    data[partItem[0].Trim()] = partItem[1].Trim();
                }

                return new ReadOnlyDictionary<string, dynamic>(data);
            }
            catch {
                return new ReadOnlyDictionary<string, dynamic>(new Dictionary<string, dynamic>());
            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}

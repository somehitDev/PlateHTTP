using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Newtonsoft.Json;



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

        public Request(HttpListenerRequest listenerRequest, string? prefix) {
            this.ListenerRequest = listenerRequest;
            this.Prefix = prefix ?? string.Empty;
            this.UrlParams = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        internal void SetUrlParams(ReadOnlyDictionary<string, string> urlParams) {
            this.UrlParams = urlParams;
        }

        public ReadOnlyDictionary<string, dynamic> GetJSON() {
            string jsonContent;
            using (var reader = new StreamReader(this.ListenerRequest.InputStream)) {
                jsonContent = reader.ReadToEnd();
            }

            return new ReadOnlyDictionary<string, dynamic>(
                JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent)
            );
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}

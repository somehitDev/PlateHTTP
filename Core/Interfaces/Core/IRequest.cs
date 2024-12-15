using System;
using System.Collections.ObjectModel;
using System.Net;



namespace PlateHTTP.Interfaces.Core {
    public interface IRequest {
        public string Prefix { get; }
        string HttpMethod { get; }
        string Path { get; }
        string FullPath { get; }
        string Body { get; }
        ReadOnlyDictionary<string, string> UrlParams { get; }
        ReadOnlyDictionary<string, string> QueryParams { get; }

        static IRequest FromHttpRequest(HttpListenerRequest listenerRequest, ReadOnlyDictionary<string, string> urlParams, string? prefix) => throw new NotImplementedException();

        ReadOnlyDictionary<string, dynamic> GetJSON();

        void Dispose();
    }
}

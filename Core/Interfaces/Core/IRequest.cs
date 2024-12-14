using System.Collections.ObjectModel;



namespace PlateHTTP.Interfaces.Core {
    public interface IRequest {
        public string Prefix { get; }
        string HttpMethod { get; }
        string Path { get; }
        string FullPath { get; }
        ReadOnlyDictionary<string, string> UrlParams { get; }
        ReadOnlyDictionary<string, string> QueryParams { get; }

        ReadOnlyDictionary<string, dynamic> GetJSON();

        void Dispose();
    }
}

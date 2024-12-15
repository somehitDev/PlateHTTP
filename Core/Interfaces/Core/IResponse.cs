using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PlateHTTP.Enums;



namespace PlateHTTP.Interfaces.Core {
    public interface IResponse {
        public IWebApplication WebApplication { get; }

        static IResponse FromHttpResponse(IWebApplication webApplication, HttpListenerResponse listenerResponse) => throw new NotImplementedException();

        Task Send(byte[] bytes, string contentType, Encoding? encoding = null, StatusCode statusCode = StatusCode.OK);
        Task Send(Stream stream, string contentType, StatusCode statusCode = StatusCode.OK);
        Task Send(string content, string contentType, StatusCode statusCode = StatusCode.OK);
        Task SendText(string text, StatusCode statusCode = StatusCode.OK);
        Task SendHTML(string html, StatusCode statusCode = StatusCode.OK);
        Task SendJSON(Dictionary<string, dynamic> dictionary, StatusCode statusCode = StatusCode.OK);
        Task SendJSON(ReadOnlyDictionary<string, dynamic> dictionary, StatusCode statusCode = StatusCode.OK);
        Task SendJSON(string jsonString, StatusCode statusCode = StatusCode.OK);

        void Redirect(string path, StatusCode statusCode);

        Task RenderDefaultPage(StatusCode statusCode, string? message = null);

        void Close();

        void Dispose();
    }
}

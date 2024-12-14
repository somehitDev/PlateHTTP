using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PlateHTTP.Enums;



namespace PlateHTTP.Interfaces.Core {
    public interface IResponse {
        public PlateHTTP.Core.WebApplication WebApplication { get; }

        Task Send(byte[] bytes, string contentType, Encoding? encoding, StatusCode statusCode);
        Task Send(Stream stream, string contentType, StatusCode statusCode);
        Task Send(string content, string contentType, StatusCode statusCode);
        Task SendText(string text, StatusCode statusCode);
        Task SendHTML(string html, StatusCode statusCode);
        Task SendJSON(Dictionary<string, dynamic> dictionary, StatusCode statusCode);

        void Redirect(string path, StatusCode statusCode);

        Task RenderDefaultPage(StatusCode statusCode, string? message);

        void Close();

        void Dispose();
    }
}

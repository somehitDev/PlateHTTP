using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlateHTTP.Enums;



namespace PlateHTTP.Core {
    public class Response: Interfaces.Core.IResponse {
        private HttpListenerResponse ListenerResponse;

        public WebApplication WebApplication { get; private set; }

        public Response(WebApplication webApplication, HttpListenerResponse listenerResponse): base() {
            this.WebApplication = webApplication;
            this.ListenerResponse = listenerResponse;
        }

        private async Task<string> RenderDefaultPageFromFile(StatusCode statusCode) {
            string templateString = "";
            if (this.WebApplication.Config == null || this.WebApplication.Config.DefaultPageRoot == null) {
                throw new WebException("Config or DefaultPageRoot is not set!");
            }
            else {
                var codePage = Path.Join(this.WebApplication.Config.DefaultPageRoot, $"{(int)statusCode}.html");
                if (File.Exists(codePage)) {
                    templateString = await File.ReadAllTextAsync(codePage);
                }
                else {
                    var templatePage = Path.Join(this.WebApplication.Config.DefaultPageRoot, "template.html");
                    if (!File.Exists(templatePage)) {
                        throw new WebException("Template files cannot found!");
                    }

                    templateString = await File.ReadAllTextAsync(templatePage);
                }
            }

            return await Task.FromResult(templateString);
        }
        private async Task<string> RenderDefaultPageFromResource(StatusCode statusCode) {
            string templateString;
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = $"PlateHTTP.Assets.DefaultPages.{(int)statusCode}.html";
            if (assembly.GetManifestResourceNames().Contains(resourceName)) {
                using (var stream = assembly.GetManifestResourceStream(resourceName)) {
                    using (var reader = new StreamReader(stream)) {
                        templateString = await reader.ReadToEndAsync();
                    }
                }
            }
            else {
                resourceName = "PlateHTTP.Assets.DefaultPages.template.html";
                using (var stream = assembly.GetManifestResourceStream(resourceName)) {
                    using (var reader = new StreamReader(stream)) {
                        templateString = await reader.ReadToEndAsync();
                    }
                }
            }

            return await Task.FromResult(templateString);
        }

        public async Task Send(byte[] bytes, string contentType, Encoding? encoding = null, StatusCode statusCode = StatusCode.OK) {
            this.ListenerResponse.ContentType = contentType;
            this.ListenerResponse.ContentEncoding = encoding ?? Encoding.UTF8;
            this.ListenerResponse.ContentLength64 = bytes.LongLength;
            this.ListenerResponse.StatusCode = (int)statusCode;

            await this.ListenerResponse.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        }
        public async Task Send(string content, string contentType, StatusCode statusCode = StatusCode.OK) {
            await this.Send(Encoding.UTF8.GetBytes(content), contentType, Encoding.UTF8, statusCode);
        }
        public async Task Send(Stream stream, string contentType, StatusCode statusCode = StatusCode.OK) {
            byte[] bytes;
            using (var reader = new BinaryReader(stream, Encoding.UTF8)) {
                bytes = reader.ReadBytes((int)stream.Length);
            }

            await this.Send(bytes, contentType, Encoding.UTF8, statusCode);
        }
        public async Task SendText(string content, StatusCode statusCode = StatusCode.OK) {
            await this.Send(content, "text/plain", statusCode);
        }
        public async Task SendHTML(string html, StatusCode statusCode = StatusCode.OK) {
            await this.Send(html, "text/html", statusCode);
        }
        public async Task SendJSON(Dictionary<string, dynamic> map, StatusCode statusCode = StatusCode.OK) {
            var content = JsonConvert.SerializeObject(map);
            await this.Send(content, "application/json", statusCode);
        }

        public void Redirect(string path, StatusCode statusCode = StatusCode.Redirect) {
            this.ListenerResponse.StatusCode = (int)statusCode;
            this.ListenerResponse.Redirect(path);
        }

        public async Task RenderDefaultPage(StatusCode statusCode, string? message = null) {
            string templateString;
            try {
                templateString = await this.RenderDefaultPageFromFile(statusCode);
            }
            catch {
                templateString = await this.RenderDefaultPageFromResource(statusCode);
            }

            await this.SendHTML(
                templateString.Replace(
                    "$codeNum", $"{(int)statusCode}"
                ).Replace(
                    "$codeText", statusCode.ToString()
                ).Replace(
                    "$message", message == null ? "" : $"<pre style=\"width:calc(100% - 40px);border:1px solid black;overflow-x:auto;\"><code>{message}</code></pre>"
                ),
                statusCode
            );
        }

        public void Close() {
            this.ListenerResponse.Close();
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}

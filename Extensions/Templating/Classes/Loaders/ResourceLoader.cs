using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Scriban;
using Scriban.Parsing;



namespace PlateHTTP.Extensions.Templating {
    public class ResourceTemplateLoader: AbstractTemplateLoader {
        private Assembly assembly;

        public string ResourcePrefix { get; private set; }

        public ResourceTemplateLoader(string? resourcePrefix = null): base() {
            this.assembly = Assembly.GetEntryAssembly();
            this.ResourcePrefix = resourcePrefix ?? "";
        }

        public override string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName) {
            return this.GetPath(templateName);
        }

        public override string GetPath(string templateName) {
            string? templatePath = null;
            foreach (var name in this.assembly.GetManifestResourceNames()) {
                var resourceSuffix = $"{this.ResourcePrefix}.{templateName}".Replace("..", ".");
                if (name.EndsWith(resourceSuffix)) {
                    templatePath = name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(templatePath)) {
                throw new FileNotFoundException("Template cannot found!");
            }
            
            return templatePath;
        }

        public override Task<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) {
            return this.LoadAsync(templatePath);
        }

        public override Task<string> LoadAsync(string templatePath) {
            string templateString;
            using (var stream = this.assembly.GetManifestResourceStream(templatePath)) {
                if (stream == null) {
                    throw new FileNotFoundException("Template cannot found!");
                }
                else {
                    using (var reader = new StreamReader(stream)) {
                        templateString = reader.ReadToEnd();
                    }
                }
            }

            return Task.FromResult(templateString);
        }
    }
}

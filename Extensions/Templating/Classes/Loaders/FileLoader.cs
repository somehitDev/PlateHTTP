using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;


namespace PlateHTTP.Extensions.Templating {
    public class FileTemplateLoader: AbstractTemplateLoader {
        public string TemplateRoot { get; private set; }

        public FileTemplateLoader(string templateRootPath): base() {
            this.TemplateRoot = Path.GetFullPath(templateRootPath);
        }

        public override string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName) {
            return this.GetPath(templateName);
        }
        public override string GetPath(string templateName) {
            var templatePath = Path.Join(this.TemplateRoot, templateName);
            if (!File.Exists(templatePath)) {
                throw new FileNotFoundException("Template cannot found!");
            }

            return templatePath;
        }

        public override Task<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) {
            return this.LoadAsync(templatePath);
        }
        public override Task<string> LoadAsync(string templatePath) {
            if (!File.Exists(templatePath)) {
                throw new FileNotFoundException("Template cannot found!");
            }

            return Task.FromResult(File.ReadAllText(templatePath));
        }
    }
}

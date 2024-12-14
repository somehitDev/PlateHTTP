using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using PlateHTTP.Interfaces.StaticFiles;



namespace PlateHTTP.StaticFiles {
    public class ResourceStaticLoader: IStaticFileLoader {
        private Assembly assembly;

        public string ResourcePrefix { get; private set; }

        public ResourceStaticLoader(string? resourcePrefix = null): base() {
            this.assembly = Assembly.GetEntryAssembly();
            this.ResourcePrefix = resourcePrefix ?? "";
        }

        public string GetPath(string partialPath) {
            string? staticFile = null;
            foreach (var name in this.assembly.GetManifestResourceNames()) {
                var resourceSuffix = $"{this.ResourcePrefix}.{partialPath}".Replace("/", ".").Replace("..", ".");
                if (name.EndsWith(resourceSuffix)) {
                    staticFile = name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(staticFile)) {
                throw new FileNotFoundException("Invalid static file!");
            }

            return staticFile;
        }

        public async Task<Stream> LoadAsync(string fullPath) {
            var memStream = new MemoryStream();
            using (var stream = this.assembly.GetManifestResourceStream(fullPath)) {
                if (stream == null) {
                    throw new FileNotFoundException("Invalid static file!");
                }
                else {
                    await stream.CopyToAsync(memStream);
                }
            }

            return await Task.FromResult(memStream);
        }
    }
}

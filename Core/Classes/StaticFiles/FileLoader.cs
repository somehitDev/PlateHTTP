using System.IO;
using System.Threading.Tasks;
using PlateHTTP.Interfaces.StaticFiles;


namespace PlateHTTP.StaticFiles {
    public class FileStaticLoader: IStaticFileLoader {
        public string Root { get; private set; }

        public FileStaticLoader(string staticRoot): base() {
            this.Root = Path.GetFullPath(staticRoot);
        }

        public string GetPath(string partialPath) {
            var staticFile = Path.Join(this.Root, partialPath);
            if (!File.Exists(staticFile)) {
                throw new FileNotFoundException("Invalid static file!");
            }

            return staticFile;
        }

        public async Task<Stream> LoadAsync(string fullPath) {
            if (!File.Exists(fullPath)) {
                throw new FileNotFoundException("Invalid static file!");
            }

            var memStream = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open)) {
                await memStream.CopyToAsync(memStream);
            }

            return await Task.FromResult(memStream);
        }
    }
}

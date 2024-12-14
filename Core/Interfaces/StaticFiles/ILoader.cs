using System.IO;
using System.Threading.Tasks;


namespace PlateHTTP.Interfaces.StaticFiles {
    public interface IStaticFileLoader {
        string GetPath(string partialPath);
        Task<Stream> LoadAsync(string fullPath);
    }
}

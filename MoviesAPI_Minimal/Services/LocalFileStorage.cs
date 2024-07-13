
namespace MoviesAPI_Minimal.Services
{
    public class LocalFileStorage(IWebHostEnvironment environment, 
        IHttpContextAccessor httpContext) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                return Task.CompletedTask;
            }
            var filename = Path.GetFileName(route);
            var fileDirectory = Path.Combine(environment.WebRootPath, container, filename);

            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var filename = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(environment.WebRootPath, extension);
            
            if (!Directory.Exists(folder)) 
            { 
                Directory.CreateDirectory(folder);
            }
            
            string route = Path.Combine(folder, extension);
            using (var ms =  new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }

            var scheme = httpContext.HttpContext!.Request.Scheme;
            var host = httpContext.HttpContext!.Request.Host;
            var url = $"{scheme}://{host}";
            var urlFile = Path.Combine(url, container, filename).Replace("\\", "/");

            return urlFile;
        }
    }
}

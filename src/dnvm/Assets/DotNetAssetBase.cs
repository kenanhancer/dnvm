using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Reporting;
using DotNet.Utils;

namespace DotNet.Assets
{
    public abstract class DotNetAssetBase : Asset
    {
        protected IAssetChannel Channel { get; } = new StableAssetChannel();

        private static readonly HttpClient DefaultHttpClient = new HttpClient();
        protected IReporter Reporter { get; }

        public DotNetAssetBase(IReporter reporter)
        {
            Reporter = reporter;
        }

        protected bool UninstallFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Reporter.Verbose($"Deleting '{path}'");
                try
                {
                    Directory.Delete(path, recursive: true);
                }
                catch (Exception ex)
                {
                    Reporter.Verbose(ex.Message);
                    Reporter.Error($"Failed to delete '{path}'. Try again or remove manually.");
                    return false;
                }
                Reporter.Verbose("Deleted");
            }
            else
            {
                Reporter.Verbose($"Skipping. '{path}' does not exist.");
            }
            return true;
        }

        protected async Task<bool> DownloadAndExtractAsync(string url, string destination, CancellationToken cancellationToken)
        {
            Reporter.Verbose($"Downloading from '{url}'");

            HttpResponseMessage result;
            try
            {
                result = await DefaultHttpClient.GetAsync(url, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                Reporter.Error($"Downloading asset failed: {ex.Message}");
                return false;
            }

            if (!result.IsSuccessStatusCode)
            {
                Reporter.Error($"Failed to download '{url}'");
                throw new InvalidOperationException("Installation failed");
            }

            var filename = Path.GetFileName(url);
            var ext = filename.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase)
                ? ".tar.gz"
                : Path.GetExtension(filename);

            using (var tmp = new TempFile(ext))
            using (var stream = new FileStream(tmp.Path, FileMode.Create))
            {
                await result.Content.CopyToAsync(stream);

                Reporter.Verbose($"Extracting '{filename}' to '{destination}'");

                return ArchiveUtility.Extract(tmp.Path, destination);
            }
        }
    }
}
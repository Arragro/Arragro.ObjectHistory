using Azure.Storage.Blobs;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Extentions
{
    public static class BlobClientExtentions
    {
        public static async Task<string> DownloadTextAsync(this BlobClient blobClient)
        {
            using (var ms = new MemoryStream())
            {
                await blobClient.DownloadToAsync(ms);
                ms.Position = 0;
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}

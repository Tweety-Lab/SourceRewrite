using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltralightNet;
using UltralightNet.Platform;

namespace VistaGUI.Preprocessor
{
    public class VistaFileSystem : IFileSystem
    {
        public string ResourcePathPrefix { get; set; }

        public VistaFileSystem(string resourcePathPrefix)
        {
            ResourcePathPrefix = resourcePathPrefix;
        }

        // File Exists
        public bool FileExists(string path)
        {
            string fullPath = Path.Combine(ResourcePathPrefix, path);
            return File.Exists(fullPath);
        }

        // Get File Mime Type
        public string GetFileMimeType(string path)
        {
            string fullPath = Path.Combine(ResourcePathPrefix, path);
            string extension = Path.GetExtension(fullPath).ToLower();

            return extension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".json" => "application/json",
                _ => "application/octet-stream" // Default binary type
            };
        }

        // Get File Charset
        public string GetFileCharset(string path)
        {
            return "UTF-8";
        }

        // Open File
        public unsafe ULBuffer OpenFile(string path)
        {
            string fullPath = Path.Combine(ResourcePathPrefix, path);
            byte[] fileData = File.ReadAllBytes(fullPath);
            fixed (byte* dataPtr = fileData)
            {
                return ULBuffer.CreateFromDataCopy((void*)dataPtr, (nuint)fileData.Length);
            }
        }

    }
}

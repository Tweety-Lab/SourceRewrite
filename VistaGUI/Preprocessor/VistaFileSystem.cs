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
        // Base path for resources
        public string ResourcePathPrefix { get; set; }

        public VistaFileSystem(string resourcePathPrefix)
        {
            ResourcePathPrefix = resourcePathPrefix;
        }

        // Check if a File Exists
        public bool FileExists(string path)
        {
            string fullPath = Path.Combine(ResourcePathPrefix, path);
            return File.Exists(fullPath);
        }

        // Get a File's Mime Type
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

        // Get a File's Charset
        public string GetFileCharset(string path)
        {
            return "UTF-8";
        }

        // Open a File in a buffer
        public unsafe ULBuffer OpenFile(string path)
        {
            string fullPath = Path.Combine(ResourcePathPrefix, path);

            // Read the file
            string fileContents = File.ReadAllText(fullPath, Encoding.UTF8);

            // Pre-process HTML
            if (Path.GetExtension(fullPath).ToLower() == ".html")
                fileContents = PreProccesor.ProcessHTML(fileContents);

            // Convert to byte array and return buffer
            byte[] fileData = Encoding.UTF8.GetBytes(fileContents);
            fixed (byte* dataPtr = fileData)
            {
                return ULBuffer.CreateFromDataCopy((void*)dataPtr, (nuint)fileData.Length);
            }
        }

    }
}

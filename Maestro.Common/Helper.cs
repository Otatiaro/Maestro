using System;
using System.Collections.Generic;
using Brotli;

namespace Maestro.Common
{
    public static class Helper
    {
        public static string ToC(this string input) => input.ToLowerInvariant().Replace(' ', '_');

        public static IEnumerable<string> IsC(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                yield return "cannot be empty";
            if (input.Contains(" "))
                yield return "cannot contain whitespace (replace by underscore or camel case)";
        }

        public static byte[] Compress(this byte[] data)
        {
            using (var msInput = new System.IO.MemoryStream(data))
            using (var msOutput = new System.IO.MemoryStream())
            using (var bs = new BrotliStream(msOutput, System.IO.Compression.CompressionMode.Compress))
            {
                bs.SetQuality(11);
                bs.SetWindow(22);
                msInput.CopyTo(bs);
                bs.Close();
                return msOutput.ToArray();
            }
        }

        public static byte[] Decompress(this byte[] data)
        {
            using (var msInput = new System.IO.MemoryStream(data))
            using (var bs = new BrotliStream(msInput, System.IO.Compression.CompressionMode.Decompress))
            using (var msOutput = new System.IO.MemoryStream())
            {
                bs.CopyTo(msOutput);
                msOutput.Seek(0, System.IO.SeekOrigin.Begin);
                return msOutput.ToArray();
            }
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Zxcvbn
{
    /// <summary>
    /// A few useful extension methods used through the Zxcvbn project.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Helper method to parse gzipped files.
        /// </summary>
        /// <param name="filename">The filename of the file to get the contents from.</param>
        /// <returns>An enumerable of lines of text in the resource.</returns>
        public static IEnumerable<string> ReadAllGzipLines(string filename)
        {
            using var file = File.OpenRead(filename);
            using var gzipStream = new GZipStream(file, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream);
            var line = reader.ReadLine();
            while (line != null)
            {
                yield return line;
                line = reader.ReadLine();
            }
        }

        /// <summary>
        /// Returns a list of the lines of text from an embedded resource in the assembly.
        /// </summary>
        /// <param name="resourceName">The name of the resource to get the contents from.</param>
        /// <returns>An enumerable of lines of text in the resource or null if the resource does not exist.</returns>
        public static IEnumerable<string> GetEmbeddedResourceLines(string resourceName)
        {
            var asm = typeof(Utility).GetTypeInfo().Assembly;
            if (!asm.GetManifestResourceNames().Contains(resourceName))
                return null; // Not an embedded resource
            return GetEmbeddedResourceLinesImpl(asm, resourceName);
        }

        /// <summary>
        /// Reverse a string in one call.
        /// </summary>
        /// <param name="str">String to reverse.</param>
        /// <returns>String in reverse.</returns>
        public static string StringReverse(this string str)
        {
            return new string(str.Reverse().ToArray());
        }

        /// <summary>
        /// Returns a list of the lines of text from an embedded resource in the assembly.
        /// </summary>
        /// <remarks>
        /// assumes the file exists.
        /// </remarks>
        /// <param name="asm">the assembly to take the resource from.</param>
        /// <param name="resourceName">the resource name to take content from.</param>
        /// <returns>An enumerable of lines of text in the resource.</returns>
        private static IEnumerable<string> GetEmbeddedResourceLinesImpl(Assembly asm, string resourceName)
        {
            using var stream = asm.GetManifestResourceStream(resourceName);
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using var text = new StreamReader(gzipStream);
            while (!text.EndOfStream)
            {
                yield return text.ReadLine();
            }
        }
    }
}

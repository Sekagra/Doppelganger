using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Doppelganger.Core
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns a list of all specified files in the given directory.
        /// </summary>
        /// <param name="dir">The directory which will be searched.</param>
        /// <param name="extensions">A list of file extensions as string (e.g. *.png) which specify what types will be collected.</param>
        /// <param name="options">Search options.</param>
        /// <returns>An array of FileInfo objects.</returns>
        public static FileInfo[] GetFiles(this DirectoryInfo dir, string[] extensions, SearchOption options)
        {
            var __files = new List<FileInfo>();
            for (int i = 0; i < extensions.Length; i++)
            {
                __files.AddRange(dir.GetFiles(extensions[i], options));
            }

            return __files.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doppelganger.Model;

namespace Doppelganger.Core
{
    /// <summary>
    /// Wraps providence for minor data, the program may needs
    /// </summary>
    internal static class DataProvider
    {
        /// <summary>
        /// Uses reflection to get all subclasses of 'ImageFinderBase' in the current assembly.
        /// </summary>
        /// <returns>A list of all subclass-types found in this assembly.</returns>
        internal static List<Type> GetImageFinders()
        {
            return typeof(ImageFinderBase).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ImageFinderBase))).ToList();
        }
    }
}

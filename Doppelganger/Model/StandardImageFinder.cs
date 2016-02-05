using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Doppelganger.Model
{
    class StandardImageFinder : ImageFinderBase
    {       
        #region Methods
            /// <summary>
            /// Searches for duplicates or similar images and groups them into a DuplicateImage-Instance.
            /// </summary>
            /// <param name="path">The directory to search through.</param>
            protected override IEnumerable<DuplicateImages> FindDuplicates(Dictionary<FileInfo, ulong> images)
            {
                //Find identical images
                //todo: AsParallel() safe to use here?
                var __groups = images.AsParallel().GroupBy(i => i.Value).Where(grp => grp.Count() > 1).ToList();

                return __groups.Select(group => new DuplicateImages(group.Select(e => new DuplicateImage(e.Key, e.Value))));
            }

            /// <summary>
            /// Creates a basic MD5 hash to determine identical files.
            /// </summary>
            protected override ulong GetHash(string file)
            {
                using (var __stream = File.OpenRead(file))
                    return (ulong)BitConverter.ToInt32(System.Security.Cryptography.MD5.Create().ComputeHash(__stream), 0);
            }
        #endregion
    }
}

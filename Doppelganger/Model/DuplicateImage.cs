using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Doppelganger.Model
{
    /// <summary>
    /// Sadly, FileInfo is sealed, so it's not possible to simply inherit from it and extend the few properties needed.
    /// </summary>
    public class DuplicateImage
    {
        #region Fields
            private int _imageWidth;
            private int _imageHeight;
        #endregion

        #region Properties
            public FileInfo Info { get; set; }
            public ulong Hash { get; set; }

            //Resolution in pixel (Lazy-Loading)
            //Note: Using Image.FromStream(stream, false, false) should be faster than Image.FromFile() as it doesn't load the whole data into the memory
            //(Source: http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/b54fbacd-364a-411f-b82d-4c488d3bfd8e and some StackOverflow post)
            public int ImageWidth 
            {
                get 
                { 
                    if(_imageWidth == 0)
                    {
                        var __strm = Info.OpenRead();
                        _imageWidth = Image.FromStream(__strm, false, false).Width;
                        __strm.Close(); //Important, otherwise the image will be in use by the application itself
                    }
                    return _imageWidth;
                }
            }
            public int ImageHeight
            {
                get
                {
                    if (_imageHeight == 0)
                    {
                        var __strm = Info.OpenRead();
                        _imageHeight = Image.FromStream(__strm, false, false).Height;
                        __strm.Close(); //Important, otherwise the image will be in use by the application itself
                    }
                    
                    return _imageHeight;
                }
            }
        #endregion

        #region Constructor
            public DuplicateImage(FileInfo fileInfo)
            {
                Info = fileInfo;
            }

            public DuplicateImage(FileInfo fileInfo, ulong hash) : this(fileInfo)
            {
                Hash = hash;
            }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Doppelganger.Model
{
    class PerceptualImageFinder : ImageFinderBase
    {
        #region Methods
            /// <summary>
            /// Searches for duplicates or similar images and groups them into a DuplicateImage-Instance.
            /// </summary>
            /// <param name="path">The directory to search through.</param>
            protected override IEnumerable<DuplicateImages> FindDuplicates(Dictionary<FileInfo, ulong> images)
            {
                if (images != null)
                {
                    var __distance = GetHammingDistance(images.First().Value, images.Last().Value);

                    //todo: Plainest approach, find something smarter...
                    var __copyList = new Dictionary<FileInfo, ulong>(images);
                    var __duplicates = new List<DuplicateImages>();

                    foreach (var __image in __copyList)
                    {
                        if (images.ContainsKey(__image.Key))
                        {
                            var __group = __copyList.Where(i => GetHammingDistance(__image.Value, i.Value) < 4).ToList();   //todo: Hamming distance max. 3
                            if (__group.Count() > 1) __duplicates.Add(new DuplicateImages(__group.Select(e => new DuplicateImage(e.Key, e.Value))));
                            __group.ForEach(e => images.Remove(e.Key)); //Remove from the list, so the images won't be found twice
                        }
                    }
                    return __duplicates;
                }

                return null;
            }

            /// <summary>
            /// Calculates a perceptual hash for the image.
            /// </summary>
            protected override ulong GetHash(string file)
            {
                //Load the image
                var __image = GetImage(file);

                //Shrink the image down to 8x8 pixel
                var __newImage = ChangeImageSize(__image, 8, 8);

                //Dispose the image to free up resources right away
                __image.Dispose();

                //Get the pixel colors as a greyscale (simply return the the new color values as byte[])
                var __greyscale = GetGreyscaleValues(__newImage);

                //Compute the mean value (this may be slower than adding up the average during the calculation process of the greyscale values)
                var __avgGrey = __greyscale.Average(b => Convert.ToInt32(b));

                //Create the hash based on every pixels value compared to the mean
                //(Bits 0 or 1, whether or not the value is greater or lower than the mean)
                ulong __hash = 0;

                #region Iterate through every bit and set the hash accordingly
                    for (int i = 0; i < 64; i++)
                        if (__greyscale[i] >= __avgGrey)
                            __hash |= (1UL << (63 - i));    //1UL is equal to (ulong)1
                            //Explanation: uses a 64 bit long 1 (00000000...0001), shifts the 1 to the current position; using an OR-operator guarantees all the other bit values to stay as is and causes only the new 1 to be written
                #endregion
                return __hash;
            }

            /// <summary>
            /// Returns the Image-object to an image file on the filesystem.
            /// </summary>
            /// <param name="path">The full path to the image.</param>
            /// <returns>The image as Bitmap-object.</returns>
            private Image GetImage(string path)
            {
                return new Bitmap(path);
            }

            /// <summary>
            /// Changes the size of the given image regardless of the ratio.
            /// </summary>
            /// <param name="image">The image to change.</param>
            /// <param name="width">The new width in pixel.</param>
            /// <param name="height">The new height in pixel.</param>
            /// <returns>The modified version of the image.</returns>
            private Bitmap ChangeImageSize(Image image, int width, int height)
            {
                return new Bitmap(image, width, height);

                #region Old slower solution:
                /*var __newImagae = new Bitmap(width, height);
                    var __graphics = Graphics.FromImage(__newImagae);
                    __graphics.CompositingQuality = CompositingQuality.HighQuality;
                    __graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    __graphics.SmoothingMode = SmoothingMode.HighQuality;
                    __graphics.DrawImage(image, 0, 0, width, height);

                    return __newImagae;*/
                #endregion
            }

            /// <summary>
            /// Determines the greyscale value for every pixel.
            /// </summary>
            /// <param name="image">The image to get the greyscale values from.</param>
            /// <returns>All greyscale values as one-dimensional array.</returns>
            private byte[] GetGreyscaleValues(Bitmap image)
            {
                byte[] __greyscaleImage = new byte[64];

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var __color = image.GetPixel(x, y);

                        //It is not important how the greyscale value is calculated as long as it is consistent. http://www.tannerhelland.com/3643/grayscale-image-algorithm-vb6/
                        //the cheapest conversion method
                        uint __grayValue = (uint)((__color.R + __color.G + __color.B) / 3);

                        //or the slightly better version (sloweR?!)
                        //uint __grayValue = (uint)(__color.R * 0.299 + __color.G * 0.587 + __color.B * 0.114);

                        __greyscaleImage[x + y * 8] = (byte)__grayValue;
                    }
                }

                return __greyscaleImage;
            }

            /// <summary>
            /// Calculates the Hamming Distance between both words.
            /// </summary>
            /// <returns>The Hamming Distance as integer value.</returns>
            private int GetHammingDistance(ulong hash1, ulong hash2)
            {
                ulong __word = hash1 ^ hash2;
                int __weight = 0;

                while (__word > 0)
                {
                    if ((__word & 1) == 1) __weight++;
                    __word >>= 1;
                }

                return __weight;
            }
        #endregion
    }
}

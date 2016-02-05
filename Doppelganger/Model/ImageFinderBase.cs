using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Doppelganger.Core;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Collections.Concurrent;

namespace Doppelganger.Model
{
    public abstract class ImageFinderBase : INotifyPropertyChanged
    {
        #region Fields
            private Progress _state;
        #endregion

        #region Properties
            /// <summary>
            /// Informs about the progress of the current search.
            /// </summary>
            public Progress State
            {
                get { return _state; }
                set { _state = value; OnPropertyChanged("State"); }
            }

            /// <summary>
            /// Include all subdirectories into the search process.
            /// </summary>
            public bool CheckSubdirectories { get; set; }

            /// <summary>
            /// Ignore files older than this date.
            /// </summary>
            public DateTime MinDate { get; set; }
        #endregion

        #region Constructor
            /// <summary>
            /// Creates an ImageFinder, that detect images files under a certain path and is able to caluclate perceptual hashes.
            /// </summary>
            public ImageFinderBase() 
            {
                CheckSubdirectories = true;
                State = new Progress();
            }
        #endregion

        #region Methods
            /// <summary>
            /// Searches for duplicates or similar images and groups them into a DuplicateImage-Instance.
            /// </summary>
            /// <param name="path">The directory to search through.</param>
            /// <param name="callback">Execute this after the search is done.</param>
            public List<DuplicateImages> Search(string path, Action<IEnumerable<DuplicateImages>> callback)
            {
                Task.Factory.StartNew(() =>
                {
                    return FindDuplicates(Scan(path, CheckSubdirectories, MinDate));
                }).ContinueWith(__task =>
                {
                    //Update the state
                    State.Current = "Done! (" + __task.Result.Count().ToString() + " duplicates found)";
                    
                    //Invoke the callback function with all groups
                    callback.Invoke(__task.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext());

                return null;
            }

            /// <summary>
            /// Searches for duplicates or similar images and groups them into a DuplicateImage-Instance.
            /// </summary>
            /// <param name="path">The directory to search through.</param>
            protected abstract IEnumerable<DuplicateImages> FindDuplicates(Dictionary<FileInfo, ulong> images);

            /// <summary>
            /// Calculates a hash specifically for the 
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            protected abstract ulong GetHash(string file);

            /// <summary>
            /// Scans the given path for image files and creates a hash for every element found.
            /// </summary>
            /// <param name="path">The directory to search through.</param>
            /// <param name="checkSubdirs">Include subdirectories?</param>
            /// <param name="minDate">Ignore files older than this date.</param>
            /// <returns>Returns a dictionary with the FileInfo-object of the file as key and its hash as value.</returns>
            private Dictionary<FileInfo, ulong> Scan(string path, bool checkSubdirs, DateTime minDate)
            {
                #region Scan subdirs or not
		            SearchOption __options;
                    if (checkSubdirs) __options = SearchOption.AllDirectories;
                    else __options = SearchOption.TopDirectoryOnly;
	            #endregion
                
                //Get all files
                var __files = new DirectoryInfo(path).GetFiles(new string[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp" }, __options).ToList();

                #region Filter according to a given date
                    if (minDate != null)
                        __files = __files.Where(f => DateTime.Compare(f.LastWriteTime, minDate) >= 0).ToList();
                #endregion

                State.Max = __files.Count;

                //todo: Because this is a parallel operation, the ConcurrentDictionary class is used instead of the standard Dictionary (which is not thread-safe) 
                //Critical issue, because doing this parallel saves a huge amount of time
                //var __images = new ConcurrentDictionary<FileInfo, ulong>();
                //__files.AsParallel().ForAll((f) => 
                var __images = new Dictionary<FileInfo, ulong>();
                __files.ToList().ForEach((f) => 
                {
                    //Calling the respective overridden method to create a hash (the case where the key already exists can't happen, so it's not implemented properly)
                    __images.Add(f, GetHash(f.FullName));
                    //Update the State to give information about the current image
                    State.Current = f.Name;
                    State.Count = __files.IndexOf(f) + 1;
                });
                return __images;
            }
        #endregion

        #region INotifyPropertyChanged Member
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                var __handler = this.PropertyChanged;
                if (__handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    __handler(this, e);
                }
            }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Doppelganger.Model
{
    /// <summary>
    /// Represents a single group of similar or identical image files.
    /// </summary>
    public class DuplicateImages : INotifyPropertyChanged
    {
        #region Fields
            private ObservableCollection<DuplicateImage> _images;
        #endregion

        #region Properties
            /// <summary>
            /// A user friendly name for the image group 
            /// (Simply return the filename of the first image with this hash that has been found.)
            /// </summary>
            public string DisplayName
            {
                get
                {
                    if (Images != null) 
                        return Images.First().Info.Name.ToString();
                    return string.Empty;
                }
            }

            /// <summary>
            /// All files in this group of similiar/identical images
            /// </summary>
            public ObservableCollection<DuplicateImage> Images 
            {
                get { return _images; }
                set { _images = value; OnPropertyChanged("Images"); }
            }
        #endregion

        #region Constructor
            /// <summary>
            /// Creates an object that represents a group of duplicate images.
            /// </summary>
            /// <param name="files">All files in this new group.</param>
            public DuplicateImages(IEnumerable<DuplicateImage> files)
            {
                Images = new ObservableCollection<DuplicateImage>(files);
            }
        #endregion

        #region Methods
            public bool Remove(DuplicateImage file)
            {
                try
                {
                    file.Info.Delete();     //Delete the file
                    Images.Remove(file);    //Delete the entry from the collection
                    //todo: Delete the whole DuplicateImages group from its parent list, if there is only one element left
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        #endregion

        #region OnPropertyChanged Member
            private void OnPropertyChanged(string property)
            {
                var handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(property));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

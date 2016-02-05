using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doppelganger.Core;
using Doppelganger.Model;
using System.IO;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;

namespace Doppelganger.ViewModel
{
    public class DuplicateImagesViewModel : ViewModelBase
    {
        #region Fields
            //Fields for every Commmand
            private RelayCommand _deleteCommand;
            private RelayCommand _openDirectoryCommand;
            private RelayCommand _deleteYesCommand;
            private RelayCommand _deleteNoCommand;
        
            private DuplicateImages _source;
            private DuplicateImage _selectedImage;
            private bool _isAskDeleteOn;
        #endregion

        #region Properties
            /// <summary>
            /// A user friendly name for the image group 
            /// (Simply return the filename of the first image with this hash that has been found.)
            /// </summary>
            public override string DisplayName
            {
                get { return _source.DisplayName; }
            }

            public IEnumerable<DuplicateImage> Images 
            {
                get { return _source.Images; } 
            }

            public DuplicateImage SelectedImage
            {
                get { return _selectedImage; }
                set { _selectedImage = value; OnPropertyChanged("SelectedImage"); }
            }

            public bool IsAskDeleteOn
            {
                get { return _isAskDeleteOn; }
                set { _isAskDeleteOn = value; OnPropertyChanged("IsAskDeleteOn"); }
            }
        #endregion

        #region Commands
            public ICommand DeleteCommand
            {
                get
                {
                    if (_deleteCommand == null)
                        _deleteCommand = new RelayCommand(c => AskDelete(c));

                    return _deleteCommand;
                }
            }

            public ICommand OpenDirectoryCommand
            {
                get
                {
                    if (_openDirectoryCommand == null)
                        _openDirectoryCommand = new RelayCommand(a => this.OpenDirectory(a));

                    return _openDirectoryCommand;
                }
            }

            public ICommand DeleteYesCommand
            {
                get
                {
                    if (_deleteYesCommand == null)
                        _deleteYesCommand = new RelayCommand(c => this.DeleteImage(c));

                    return _deleteYesCommand;
                }
            }

            public ICommand DeleteNoCommand
            {
                get
                {
                    if (_deleteNoCommand == null)
                        _deleteNoCommand = new RelayCommand(c => IsAskDeleteOn = false);

                    return _deleteNoCommand;
                }

            }
        #endregion

        #region Constructor
            /// <summary>
            /// Creates a new DuplicateImagesViewModel
            /// </summary>
            /// <param name="images">The set of duplicate images, that is represented.</param>
            public DuplicateImagesViewModel(DuplicateImages images)
            {
                _source = images;
            }
        #endregion

        #region Methods
            /// <summary>
            /// Is called when someone asks to delete an image.
            /// </summary>
            /// <param name="file">The file which is asked to be deleted.</param>
            public void AskDelete(object file)
            {
                //make sure the file that may be deleted is selected
                SelectedImage = file as DuplicateImage;

                IsAskDeleteOn = true;
            }

            /// <summary>
            /// Deletes the given file.
            /// </summary>
            /// <param name="file">The file which will be deleted.</param>
            public void DeleteImage(object file)
            {
                _source.Remove((DuplicateImage)file);
                IsAskDeleteOn = false;
            }
            
            /// <summary>
            /// Open the file's directory in a new explorer window.
            /// </summary>
            /// <param name="file">The file to show the containing directory of.</param>
            public void OpenDirectory(object file)
            {
                if (file as DuplicateImage != null)
                {
                    var __file = file as DuplicateImage;

                    //todo: this should be directed to the UI
                    var __explorer = new Process();
                    __explorer.StartInfo = new ProcessStartInfo("explorer.exe", __file.Info.Directory.FullName);
                    __explorer.Start();
                }
            }
        #endregion
    }
}

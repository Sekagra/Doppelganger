using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doppelganger.Core;
using Doppelganger.Model;
using System.Windows.Forms;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Doppelganger.ViewModel
{
    public class ImageFinderViewModel : ViewModelBase
    {
        #region Fields
            private ImageFinderBase _imageFinder;

            //Create fields for all properties that have make use of OnPropertyChanged
            private string _selectedPath;
            private ObservableCollection<DuplicateImagesViewModel> _duplicateImages;
            private DuplicateImagesViewModel _selectedDuplicateImages;
            private Type _selectedImageFinder;

            //Fields for every Commmand
            private RelayCommand _selectPathCommand;
            private RelayCommand _searchCommand;

            //Fields for internal use
            private bool _isSearching;
        #endregion

        #region Properties
            public Progress State
            {
                get { return _imageFinder.State; }
            }

            public string SelectedPath 
            {
                get { return _selectedPath; }
                set { _selectedPath = value; OnPropertyChanged("SelectedPath"); } 
            }

            public bool CheckSubdirectories
            {
                get { return _imageFinder.CheckSubdirectories; }
                set { _imageFinder.CheckSubdirectories = value; } 
            }

            public List<Type> ImageFinders { get; set; }
            public Type SelectedImageFinder 
            {
                get { return _selectedImageFinder; }
                set
                {
                    _selectedImageFinder = value; 
                    //Create a new ImageFinder as Model for this ViewModel
                    _imageFinder = (ImageFinderBase)Activator.CreateInstance(_selectedImageFinder);
                    //Notify changes in the Model (e.g. State)
                    _imageFinder.PropertyChanged += new PropertyChangedEventHandler((obj, e) => { OnPropertyChanged(e.PropertyName); });
                    //todo: dirty way to make sure the Properties are updated
                    OnPropertyChanged("CheckSubdirectories");
                    OnPropertyChanged("SelectedDate");
                    OnPropertyChanged("State");
                } 
            }
            
            public DateTime? SelectedDate
            {
                //Deal with the need of a Nullable<DateTime> here, instead of changing the model.
                get 
                {
                    if (_imageFinder.MinDate.Equals(DateTime.MinValue))
                        return null;
                    return _imageFinder.MinDate;
                }
                set 
                { 
                    if(value.HasValue)
                        _imageFinder.MinDate = value.Value; 
                } 
            }

            public ObservableCollection<DuplicateImagesViewModel> DuplicateImages 
            {
                get { return _duplicateImages; }
                set { _duplicateImages = value; OnPropertyChanged("DuplicateImages"); }
            }

            public DuplicateImagesViewModel SelectedDuplicateImages
            {
                get { return _selectedDuplicateImages; }
                set { _selectedDuplicateImages = value; OnPropertyChanged("SelectedDuplicateImages"); }
            }
        #endregion

        #region Constructor
            public ImageFinderViewModel()
            {
                //Get all available ImageFinders
                ImageFinders = DataProvider.GetImageFinders();
                SelectedImageFinder = ImageFinders.First();

                CheckSubdirectories = true;
            }
	    #endregion

        #region Commands
            public ICommand SelectPathCommand
            {
                get
                {
                    if (_selectPathCommand == null)
                        _selectPathCommand  = new RelayCommand(a => this.SelectPath(), c => this.CanSelectPath());
                    
                    return _selectPathCommand;
                }
            }

            public ICommand SearchCommand
            {
                get
                {
                    if (_searchCommand == null)
                        _searchCommand = new RelayCommand(a => this.Search(), c => this.CanSearch());

                    return _searchCommand;
                }
            }
        #endregion

        #region Methods
            /// <summary>
            /// Opens a FolderBrowserDialog.
            /// (Note: Using the FolderBrowserDialog here directly, slightly violates the rules MVVM.)
            /// </summary>
            public void SelectPath()
            {
                var __dialog = new FolderBrowserDialog();
                __dialog.ShowNewFolderButton = false;
                __dialog.Description = "Select the directory to search in...";

                var __result = __dialog.ShowDialog();
                if (__result == DialogResult.OK)
                    SelectedPath = __dialog.SelectedPath;
            }

            /// <summary>
            /// Determines whether or not a path can be selected right now.
            /// </summary>
            public bool CanSelectPath()
            {
                return !_isSearching;
            }

            /// <summary>
            /// Initiates the search for duplicate images under the given settings.
            /// </summary>
            public void Search()
            {
                _isSearching = true;

                //Clear the list while the new search is running
                if (DuplicateImages != null) DuplicateImages.Clear();

                //Scan and check the images directly
                _imageFinder.Search(SelectedPath, (__duplicates) => 
                { 
                    //Wrap all duplicate sets into a ViewModel and add all of them to the list
                    if(__duplicates != null)
                        DuplicateImages = new ObservableCollection<DuplicateImagesViewModel>(__duplicates.Select(d => new DuplicateImagesViewModel(d)));

                    _isSearching = false;

                    //Force the CommandManager to call all "CanExecute"-methods again (otherwise the 'Search'-button would stay disabled)
                    CommandManager.InvalidateRequerySuggested();
                });
            }

            /// <summary>
            /// Determines whether or not a search can be executed right now.
            /// </summary>
            public bool CanSearch()
            {
                return !_isSearching && SelectedPath != null && SelectedPath != string.Empty;
            }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Doppelganger.Model
{
    //a struct would cause trouble with the Binding (CS1612)
    public class Progress : INotifyPropertyChanged
    {
        private string _current;
        private int _count;
        private int _max;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Current 
        {
            get { return _current; }
            set { _current = value; PropertyChanged(this, new PropertyChangedEventArgs("Current")); } 
        }
        
        public int Count
        {
            get { return _count; }
            set { _count = value; PropertyChanged(this, new PropertyChangedEventArgs("Count")); }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; PropertyChanged(this, new PropertyChangedEventArgs("Max")); }
        }
    }
}

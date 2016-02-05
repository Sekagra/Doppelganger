using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace Doppelganger.Core
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Properties
            public virtual string DisplayName { get; protected set; }
            protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

            public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        
        #region Methods
            protected virtual void OnPropertyChanged(string propertyName)
            {
                this.VerifyPropertyName(propertyName);

                var __handler = this.PropertyChanged;
                if (__handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    __handler(this, e);
                }
            }

            [Conditional("DEBUG")]
            [DebuggerStepThrough]
            public void VerifyPropertyName(string propertyName)
            {
                // Verify that the property name matches a real,  
                // public, instance property on this object.
                if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                {
                    string msg = "Invalid property name: " + propertyName;

                    if (this.ThrowOnInvalidPropertyName)
                        throw new Exception(msg);
                    else
                        Debug.Fail(msg);
                }
            }
        #endregion   
    }
}

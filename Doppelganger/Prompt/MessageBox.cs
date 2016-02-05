using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Doppelganger.Prompt
{
    /// <summary>
    /// A special MessageBox class to wrap all message in order to be MVVM friendly.
    /// Thanks to: http://geekswithblogs.net/mukapu/archive/2010/03/12/user-prompts-messagebox-with-mvvm.aspx
    /// </summary>
    [DesignTimeVisible(false)]
    class MessageBox : Control, INotifyPropertyChanged
    {
        public MessageBox()
        {
            Visibility = Visibility.Hidden;
        }

        #region Fields
            private string _message;
        #endregion

        #region Properties
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// This property is used as a trigger in order to show and hide the MessageBox
            /// </summary>
            public static readonly DependencyProperty TriggerProperty = DependencyProperty.Register("Trigger", typeof(bool), typeof(MessageBox), new PropertyMetadata(new PropertyChangedCallback(OnVisibilityChanged)));
            public bool Trigger
            {
                get { return (bool)GetValue(TriggerProperty); }
                set { SetValue(TriggerProperty, value); }
            }

            //Properties for Yes, No, Cancel commands, which can be bound later on
            public static readonly DependencyProperty YesCommandProperty = DependencyProperty.Register("YesCommand", typeof(ICommand), typeof(MessageBox), new UIPropertyMetadata(null));
            public ICommand YesCommand
            {
                get { return (ICommand)GetValue(YesCommandProperty); }
                set { SetValue(YesCommandProperty, value); }
            }

            public static readonly DependencyProperty NoCommandProperty = DependencyProperty.Register("NoCommand", typeof(ICommand), typeof(MessageBox), new UIPropertyMetadata(null));
            public ICommand NoCommand
            {
                get { return (ICommand)GetValue(NoCommandProperty); }
                set { SetValue(NoCommandProperty, value); }
            }

            public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(MessageBox), new UIPropertyMetadata(null));
            public ICommand CancelCommand
            {
                get { return (ICommand)GetValue(CancelCommandProperty); }
                set { SetValue(CancelCommandProperty, value); }
            }

            public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(MessageBox), new UIPropertyMetadata(null));
            public object CommandParameter
            {
                get { return (object)GetValue(CommandParameterProperty); }
                set { SetValue(CommandParameterProperty, value); }
            }

            //Those can be either DependencyProperties or "standard" properties with INotifyPropertyChanged
            //Note: The Binding for both is a little different
            public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MessageBox), new UIPropertyMetadata(null));
            public string Title
            {
                get { return (string)GetValue(TitleProperty); }
                set { SetValue(TitleProperty, value); }
            }           
            
            public string Message
            {
                get { return _message; }
                set { _message = value; OnPropertyChanged("Message"); } 
            }
        #endregion

        #region Methods
            /// <summary>
            /// Set the control to visibile/invisible based on a given boolean
            /// </summary>
            private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var __this = (MessageBox)d;

                if (Convert.ToBoolean(e.NewValue))
                    __this.Visibility = Visibility.Visible;
                else
                    __this.Visibility = Visibility.Hidden;
            }

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

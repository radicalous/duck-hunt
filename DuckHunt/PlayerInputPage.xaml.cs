using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DuckHunt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerInputPage : Page
    {
        public class InputAsyncResult : IAsyncResult
        {
            public string Text
            {
                get;
                set;
            }
            public Object State
            {
                get;
                set;
            }
            public virtual object AsyncState 
            {
                get
                {
                    return State;
                }
            }

  
            public virtual WaitHandle AsyncWaitHandle 
            {
                get
                {
                    return null;
                }
            }

            public virtual bool CompletedSynchronously
            {
                get
                {
                    return false;
                }
            }

            public bool Completed
            {
                get;
                set;
            }

            public virtual bool IsCompleted
            {
                get
                {
                    return Completed;
                }
            }

           
        }
        UIElement previousPage;

        public string Text
        {
            get;
            set;
        }

        public UIElement PreviousPage
        {
            get
            {
                return previousPage;
            }
            set 
            {
                previousPage = value;
            }
        }
        public PlayerInputPage(UIElement previousPage1)
        {
            previousPage = previousPage1;
            this.InitializeComponent();
        }

        static PlayerInputPage singelen;

        AsyncCallback _callback = null;
        Object _state = null;

        public static void BeginShowKeyboardInput(
             string title,
             string description,
             string defaultText,
             AsyncCallback callback,
             Object state
        )
        {
            if (singelen == null)
            {
                singelen = new PlayerInputPage(null);
            }
            singelen.PreviousPage = Windows.UI.Xaml.Window.Current.Content;
            singelen._callback = callback;
            singelen._state = state;
            Windows.UI.Xaml.Window.Current.Content = singelen;
        }

        public static string EndShowKeyboardInput(IAsyncResult r)
        {
            InputAsyncResult result = (InputAsyncResult)r;
            return result.Text;
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            InputAsyncResult result = new InputAsyncResult();
            result.State = this._state;
            result.Text = this.InputText.Text;
            result.Completed = true;
            _callback(result);

            Text = this.InputText.Text;
            Window.Current.Content = previousPage;
            Window.Current.Activate();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}

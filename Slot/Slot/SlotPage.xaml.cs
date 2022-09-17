using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlotPage : Shell
    {
        public SlotPage()
        {
            InitializeComponent();
            this.BindingContext = App.LocalAuth;
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Remove("Auth");
            App.LocalAuth = null;
            App.Current.MainPage = new AuthorizationPage();
        }
    }
}
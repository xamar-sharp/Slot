using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IPPage : ContentPage
    {
        public IPPage()
        {
            InitializeComponent();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            Prefixes.IP = (sender as Entry).Text;
            if (Xamarin.Essentials.Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Local)
            {
                Connectivity.ConnectivityChanged += ((sender, args) =>
                {
                    if (!App.IsAuthorized)
                    {
                        if (App.CheckAuth())
                        {
                            App.Current.MainPage = new SlotPage();
                           App. IsAuthorized = true;
                        }
                    }
                });
                App.Current.MainPage = new TabbedPage() { Children = { new ExplorerPage(), new Streamer() } };
                App.IsAuthorized = false;
            }
            else
            {
                if (App.CheckAuth())
                {
                   App.Current.MainPage = new SlotPage();
                   App.IsAuthorized = true;
                }
                else
                {
                    App.IsAuthorized = false;
                }
            }
        }
    }
}
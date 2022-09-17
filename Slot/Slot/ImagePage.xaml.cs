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
    public partial class ImagePage : ContentPage
    {
        private bool _scaled;
        public ImageSource Source { get; set; }
        public ImagePage(string fromAPI=null,string fileName=null)
        {
            InitializeComponent();
            bool _isFromAPI = fromAPI != null;
            Title = _isFromAPI ? Resource.ImagePageTitle : fileName;
            Source = _isFromAPI ? ImageSource.FromUri(new Uri($"http://{Prefixes.IP}:5000/memen/" + fromAPI)) : ImageSource.FromFile(fileName);
            this.BindingContext = this;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (_scaled)
            {
                await (sender as Image).RelScaleTo(-1.25,1);
                _scaled = false;
            }
            else
            {
                await (sender as Image).RelScaleTo(1.25, 1);
                _scaled = true;
            }
        }
    }
}
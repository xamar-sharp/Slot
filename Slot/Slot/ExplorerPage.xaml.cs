using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExplorerPage : ContentPage
    {
        public IList<DriveInfoViewModel> Source { get; set; }
        public ExplorerPage()
        {
            InitializeComponent();
            Source = DriveInfoViewModel.Cast(System.IO.DriveInfo.GetDrives(),this);
            Title = Resource.DriveInfoTitle;
            this.BindingContext = this;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if ((e.Item as DriveInfoViewModel).IsReady)
            {
                (e.Item as DriveInfoViewModel).GoToDirectories.Execute(0);
            }
        }
    }
}
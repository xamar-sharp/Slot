using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectoriesPage : ContentPage
    {
        public ObservableCollection<DirectoryInfoModel> Source { get; set; }
        public DirectoriesPage(DriveInfoViewModel drive = null, IList<DirectoryInfoModel> initials = null)
        {
            if (drive is null && initials is null)
            {
                Environment.FailFast("error");
            }
            Source = drive is null ? new ObservableCollection<DirectoryInfoModel>(initials) : new ObservableCollection<DirectoryInfoModel>(new List<DirectoryInfoModel>(2) { new DirectoryInfoModel(drive.Root.FullName, this) });
            InitializeComponent();
            this.BindingContext = this;
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new IntermediatePage(e.Item as DirectoryInfoModel), false);
        }
    }
}
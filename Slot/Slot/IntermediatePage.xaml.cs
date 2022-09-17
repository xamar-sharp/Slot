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
    public partial class IntermediatePage : ContentPage
    {
        private readonly DirectoryInfoModel _model;
        public IntermediatePage(DirectoryInfoModel model)
        {
            InitializeComponent();
            _model = model;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            _model.GoToDirectories.Execute(this);
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            _model.GoToFiles.Execute(0);
        }
    }
}
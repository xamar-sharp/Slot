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
    public partial class CompressorPage : ContentPage
    {
        public CompressorViewModel Model { get; set; }
        public CompressorPage(string src)
        {
            InitializeComponent();
            Model = new CompressorViewModel(src);
            BindingContext = Model;
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                isDecompress.Text = Resource.Decompress;
            }
            else
            {
                isDecompress.Text = Resource.Compress;
            }
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(e.Value)
                Model.CompressType = Resource.Deflate;
        }

        private void RadioButton_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
                Model.CompressType = Resource.GZip;
        }

        private void RadioButton_CheckedChanged_2(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
                Model.CompressType = Resource.Brotli;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileInfoPage : ContentPage,INotifyable
    {
        public FileInfoModel Model { get; set; }
        private readonly Stopwatch _watch;
        public FileInfoPage(string fileName)
        {
            //Расширения разметки вызываются во время вызова InitializeComponent()
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.FileInfoTitle;
            Model = new FileInfoModel(fileName,this);
            this.BindingContext = Model;
        }
        public double Notify()
        {
            var ms = _watch.ElapsedMilliseconds;
            _watch.Reset();
            return ms;
        }
    }
}
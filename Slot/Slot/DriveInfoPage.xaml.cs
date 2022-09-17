using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Slot.ViewModels;
namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriveInfoPage : ContentPage, INotifyable
    {
        public DriveInfoViewModel Model { get; set; }
        private readonly Stopwatch _watch;
        public DriveInfoPage(DriveInfoViewModel model)
        {
            //Расширения разметки вызываются во время вызова InitializeComponent()
            _watch = Stopwatch.StartNew();
            InitializeComponent();
            Title = Resource.DriveInfoMessage;
            Model = model;
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
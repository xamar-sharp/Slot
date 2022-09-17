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
    public partial class DirectoryInfoPage : ContentPage,INotifyable
    {
        public DirectoryInfoModel Model { get; set; }
        private readonly Stopwatch _stopwatch;
        public DirectoryInfoPage(DirectoryInfoModel model)
        {
            _stopwatch = Stopwatch.StartNew();
            InitializeComponent();
            Model = model;
            this.BindingContext = Model;
        }
        public double Notify()
        {
            var val = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
            return val;
        }
    }
}
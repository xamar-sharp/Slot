using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Octane.Xamarin.Forms.VideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : ContentPage
    {
        public VideoSource Source { get; set; }
        private readonly bool _isFromAPI;
        private readonly string _prefix;
        private Stopwatch _watch;
        public VideoPage(string fromAPI = null, string fileName = null)
        {
            InitializeComponent();
            _isFromAPI = fromAPI != null;
            if (!_isFromAPI)
            {
                Title = fileName;
                Source = VideoSource.FromFile(fileName);
                _prefix = Title;
            }
            else
            {
                Title = Resource.VideoTitle;
                Source = VideoSource.FromUri($"http://{Prefixes.IP}:5000/memen/" + fromAPI);
                _prefix = Title;
            }
            this.BindingContext = this;
        }

        private void VideoPlayer_Paused(object sender, Octane.Xamarin.Forms.VideoPlayer.Events.VideoPlayerEventArgs e)
        {
            Title = _prefix + " " + e.Duration.ToString();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            _watch = Stopwatch.StartNew();
            while(_watch.ElapsedMilliseconds < 10000)
            {
                await (video.Parent as Frame).RelRotateTo(360, 5000);
            }
            _watch.Reset();
        }
    }
}
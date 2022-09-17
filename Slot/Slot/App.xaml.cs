using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.ViewModels;
namespace Slot
{
    public partial class App : Application
    {
        public readonly static System.Xml.XmlWriterSettings XmlWriterSettings = new System.Xml.XmlWriterSettings() { Async = true };
        public static new App Current { get => Application.Current as App; }
        public static bool IsAuthorized { get; set; }
        internal static ResponseModel LocalAuth { get; set; }
        public App()
        {
            InitializeComponent();
            MainPage = new IPPage();
        }
        internal static bool CheckAuth()
        {
            if (LocalAuth is null)
            {
                var resp = Preferences.Get("Auth", "EMPTY");
                if (resp == "EMPTY")
                {
                    App.Current.MainPage = new AuthorizationPage();
                    return false;
                }
                LocalAuth = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel>(resp);
            }
            return true;
        }
        internal static void SaveAuthInfo(ResponseModel model)
        {
            Preferences.Set("Auth", Newtonsoft.Json.JsonConvert.SerializeObject(model));
            LocalAuth = new ResponseModel() { Jwt = model.Jwt, Refresh = model.Refresh, UtcJwtExpiration = model.UtcJwtExpiration,Login=model.Login,Icon = model.Icon };
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
    public interface INotifyable
    {
        double Notify();
    }
 
    public sealed class AnalyticConverter : IValueConverter
    {
        public object Convert(object path, Type target, object param, CultureInfo culture)
        {
            if (path != null)
            {
                var val = path as DriveInfoViewModel;
                return 1 - System.Convert.ToDouble(val.GetAvailableFree()) / System.Convert.ToDouble(val.GetSize());
            }
            return null;
        }
        public object ConvertBack(object path, Type target, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class ExtensionToIconConverter : IValueConverter
    {
        private static readonly ImageSource _xml = ImageSource.FromResource("Slot.Extensions.Xml.png");
        private static readonly ImageSource _json = ImageSource.FromResource("Slot.Extensions.Json.png");
        private static readonly ImageSource _zip = ImageSource.FromResource("Slot.Extensions.Zip.png");
        private static readonly ImageSource _jpg = ImageSource.FromResource("Slot.Extensions.Jpg.png");
        private static readonly ImageSource _mp4 = ImageSource.FromResource("Slot.Extensions.Mp4.png");
        private static readonly ImageSource _mp3 = ImageSource.FromResource("Slot.Extensions.Mp3.png");
        private static readonly ImageSource _png = ImageSource.FromResource("Slot.Extensions.Png.png");
        private static readonly ImageSource _bin = ImageSource.FromResource("Slot.Streaming.binary.jpg");
        private static readonly ImageSource _br = ImageSource.FromResource("Slot.Extensions.Brotli.png");
        private static readonly ImageSource _deflate = ImageSource.FromResource("Slot.Extensions.Deflate.png");
        private static readonly ImageSource _gz = ImageSource.FromResource("Slot.Extensions.Gzip.png");
        private static readonly ImageSource _txt = ImageSource.FromResource("Slot.Extensions.Txt.png");
        private static readonly ImageSource _dat = ImageSource.FromResource("Slot.Extensions.Dat.png");

        public object Convert(object path, Type target, object param, CultureInfo culture)
        {
            switch (System.IO.Path.GetExtension(path as string))
            {
                case ".xml":
                    return _xml;
                case ".json":
                    return _json;
                case ".zip":
                    return _zip;
                case ".jpg":
                    return _jpg;
                case ".mp4":
                    return _mp4;
                case ".mp3":
                    return _mp3;
                case ".png":
                    return _png;
                case ".zz":
                    return _deflate;
                case ".gz":
                    return _gz;
                case ".br":
                    return _br;
                case ".txt":
                    return _txt;
                case ".dat":
                    return _dat;
                default:
                    return _bin;
            }
        }
        public object ConvertBack(object path, Type target, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class SpanToStringConverter : IValueConverter
    {
        public object Convert(object path,Type target,object param,CultureInfo info)
        {
            if((path as byte[]).Length == 0)
            {
                return null;
            }
            return string.Join(' ', (path as byte[]).Select(ent => { 
                return System.Convert.ToByte(ent); 
            }))+" ";
        }
        public object ConvertBack(object path, Type target, object param, CultureInfo info)
        {
            if(string.IsNullOrWhiteSpace(path as string))
            {
                return String.Empty;
            }
            return (path as string).Split(' ').Select(en => System.Convert.ToByte(en)).ToArray();
        }
    }
    [ContentProperty("Target")]
    public sealed class Notify : IMarkupExtension
    {
        public INotifyable Target { get; set; }
        public object ProvideValue(IServiceProvider provider)
        {
            return Resource.NotifyFooter + Target.Notify();
        }
    }
    [ContentProperty("Value")]
    public sealed class ByteArrayImg :IValueConverter
    {
        public object Convert(object path,Type target,object param,System.Globalization.CultureInfo info)
        {
            return ImageSource.FromStream(() => new System.IO.MemoryStream(path as byte[]));
        }
        public object ConvertBack(object path, Type target, object param, System.Globalization.CultureInfo info)
        {
            throw new ApplicationException();
        }
    }
    [ContentProperty("Name")]
    public sealed class ImgRes : IMarkupExtension
    {
        public string Name { get; set; }
        public object ProvideValue(IServiceProvider provider)
        {
            return ImageSource.FromResource($"Slot.{Name}");
        }
    }
    [ContentProperty("Name")]
    public sealed class VideoRes : IMarkupExtension
    {
        public string Name { get; set; }
        public object ProvideValue(IServiceProvider provider)
        {
            return Octane.Xamarin.Forms.VideoPlayer.VideoSource.FromResource($"Slot.{Name}");
        }
    }
    [ContentProperty("Name")]
    public sealed class Localizer : IMarkupExtension
    {
        public string Name { get; set; }
        public object ProvideValue(IServiceProvider provider)
        {
            return Resource.ResourceManager.GetString(Name);
        }
    }
    public static class Prefixes
    {
        public const string StoragePath = "/storage/emulated/0/Android/data/com.companyname.Slot/";
        public const string Xml = "Xml/";
        public const string Json = "Json/";
        public const string Compress = "Compress/";
        public const string Combine = "Combine/";
        public const string Text = "Text/";
        public const string Binary = "Binary/";
        public const string File = "Files/";
        public const int GigaByte = 1024 * 1024 * 1024;
        public const int KiloByte = MegaByte / 1024;
        public const int MegaByte = GigaByte / 1024;
        public static string IP;
        static Prefixes()
        {

        }
    }
}

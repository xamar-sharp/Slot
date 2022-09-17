using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
namespace Slot.ViewModels
{
    public sealed class StreamerViewModel : INotifyPropertyChanged
    {
        public ICommand PushFileCommand { get; set; }
        public ICommand PushXmlCommand { get; set; }
        public ICommand PushTextCommand { get; set; }
        public ICommand PushJsonCommand { get; set; }
        public ICommand PushCompressorCommand { get; set; }
        public ICommand PushBinaryCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Page _underlying;
        public StreamerViewModel(Page page)
        {
            _underlying = page;
            PushFileCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string val = await _underlying.DisplayPromptAsync(Resource.Message, Resource.DestinationPath, accept: Resource.Accept, cancel: Resource.Cancel, placeholder: Resource.DestinationPathExample, keyboard: Keyboard.Url);
                    if (val != Resource.Cancel)
                    {
                        val = Prefixes.StoragePath + Prefixes.File+ val;
                        var dir = Prefixes.StoragePath + Prefixes.File;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        await Match(Path.GetExtension(val), fileName: val, writeable: System.IO.File.Exists(val) ? false : true);
                    }
                });
            });
            PushXmlCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string val = await _underlying.DisplayPromptAsync(Resource.Message, Resource.DestinationPath, accept: Resource.Accept, cancel: Resource.Cancel, placeholder: Resource.DestinationPathExample, keyboard: Keyboard.Url);
                    if (val != Resource.Cancel)
                    {
                        val = Prefixes.StoragePath + Prefixes.Xml + val;
                        var dir = Prefixes.StoragePath + Prefixes.Xml;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        await Match(".xml", fileName: val, writeable: System.IO.File.Exists(val) ? false : true);
                    }
                });
            });
            PushTextCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string val = await _underlying.DisplayPromptAsync(Resource.Message, Resource.DestinationPath, accept: Resource.Accept, cancel: Resource.Cancel, placeholder: Resource.DestinationPathExample, keyboard: Keyboard.Url);
                    if (val != Resource.Cancel)
                    { 
                        val = Prefixes.StoragePath + Prefixes.Text + val;
                        var dir = Prefixes.StoragePath + Prefixes.Text;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        await Match(".txt", fileName: val, writeable: System.IO.File.Exists(val) ? false : true);
                    }
                });
            });
            PushJsonCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string val = await _underlying.DisplayPromptAsync(Resource.Message, Resource.DestinationPath, accept: Resource.Accept, cancel: Resource.Cancel, placeholder: Resource.DestinationPathExample, keyboard: Keyboard.Url);
                    if (val != Resource.Cancel)
                    {
                        val = Prefixes.StoragePath + Prefixes.Json + val;
                        var dir = Prefixes.StoragePath + Prefixes.Json;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        await Match(".json", fileName: val, writeable: System.IO.File.Exists(val) ? false : true);
                    }
                });
            });
            PushCompressorCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string val = await _underlying.DisplayPromptAsync(Resource.Message, Resource.DestinationPath, accept: Resource.Accept, cancel: Resource.Cancel, placeholder: Resource.DestinationPathExample, keyboard: Keyboard.Url);
                    if (val != Resource.Cancel)
                    {
                        await Match(".compress", fileName: val);
                    }
                });
            });
            PushBinaryCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string val = await _underlying.DisplayPromptAsync(Resource.Message, Resource.DestinationPath, accept: Resource.Accept, cancel: Resource.Cancel, placeholder: Resource.DestinationPathExample, keyboard: Keyboard.Url);
                    if (val != Resource.Cancel)
                    {
                        val = Prefixes.StoragePath + Prefixes.Binary + val;
                        var dir = Prefixes.StoragePath + Prefixes.Binary;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        await Match(".dat", fileName: val, writeable: System.IO.File.Exists(val) ? false : true);
                    }
                });
            });
        }
        public async Task Match(string extension, string fileName = null, string fromAPI = null, bool writeable = true)
        {
            switch (extension)
            {
                case ".xml":
                    await _underlying.Navigation.PushAsync(new XmlPage(fileName, writeable, fromAPI), false);
                    break;
                case ".json":
                    await _underlying.Navigation.PushAsync(new JsonPage(fileName, writeable, fromAPI), false);
                    break;
                case ".mp4":
                    await _underlying.Navigation.PushAsync(new VideoPage(fromAPI, fileName), false);
                    break;
                case ".mp3":
                    await _underlying.Navigation.PushAsync(new VideoPage(fromAPI, fileName), false);
                    break;
                case ".m4a":
                    await _underlying.Navigation.PushAsync(new VideoPage(fromAPI, fileName), false);
                    break;
                case ".jpg":
                    await _underlying.Navigation.PushAsync(new ImagePage(fromAPI, fileName), false);
                    break;
                case ".png":
                    await _underlying.Navigation.PushAsync(new ImagePage(fromAPI, fileName), false);
                    break;
                case ".compress":
                    await _underlying.Navigation.PushAsync(new CompressorPage(fileName), false);
                    break;
                case ".zz":
                    await _underlying.Navigation.PushAsync(new CompressorPage(fileName), false);
                    break;
                case ".br":
                    await _underlying.Navigation.PushAsync(new CompressorPage(fileName), false);
                    break;
                case ".gz":
                    await _underlying.Navigation.PushAsync(new CompressorPage(fileName), false);
                    break;
                case ".txt":
                    byte[] bufferTxt = null;
                    if (fromAPI != null)
                    {
                        bufferTxt = await NetCentre._client.GetByteArrayAsync($"http://{Prefixes.IP}:5000/memen/" + fromAPI);
                    }
                    await _underlying.Navigation.PushAsync(new TxtPage(fileName, writeable, bufferTxt), false);
                    break;
                case ".dat":
                    byte[] bufferDat = null;
                    if (fromAPI != null)
                    {
                        bufferDat = await NetCentre._client.GetByteArrayAsync($"http://{Prefixes.IP}:5000/memen/" + fromAPI);
                    }
                    await _underlying.Navigation.PushAsync(new DatPage(fileName, writeable, bufferDat), false);
                    break;
                default:
                    byte[] buffer = null;
                    if (fromAPI != null)
                    {
                        buffer = await NetCentre._client.GetByteArrayAsync($"http://{Prefixes.IP}:5000/memen/"+fromAPI);
                    }
                    await _underlying.Navigation.PushAsync(new FileBinPage(fileName, writeable, buffer), false) ;
                    break;
            }
        }
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

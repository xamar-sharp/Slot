using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using System.IO;
using Xamarin.Forms;
namespace Slot.ViewModels
{
    public sealed class BinViewModel : INotifyPropertyChanged, IAsyncDisposable
    {
        public string Handle { get { if (_stream is null) { return new IntPtr(0).ToString(); } else { return _stream.Handle.ToString(); } } set { } }
        public bool TextWasChanged { get; set; }
        public byte[] Bytes
        {
            get
            {
                if (_stream != null)
                {
                    if (_stream.Position == _stream.Length)
                    {
                        _stream.Seek(-1 * _stream.Position, SeekOrigin.Current);
                    }
                    var buffer = new byte[_stream.Length];
                    _stream.Read(buffer);
                    return buffer.ToArray();
                }
                else
                {
                    return _inMemory.ToArray();
                }
            }
            set {
                Bytes = value;
            }
        }
        public int Length { get => Bytes.Length; set { } }
        public ICommand InfoCommand { get; set; }
        public ICommand SeekCommand { get; set; }
        public ICommand SetLengthCommand { get; set; }
        public ICommand WriteBytesCommand { get; set; }
        public ICommand DoneCommand { get; set; }
        public ICommand ClipboardCommand { get; set; }
        private readonly FileInfoModel _underlying;
        private readonly FileStream _stream;
        private readonly MemoryStream _inMemory;
        public event PropertyChangedEventHandler PropertyChanged;
        public BinViewModel(string fileName, bool writeable,Page page, byte[] apiData = null)
        {
            if (writeable)
            {
                _stream = File.Create(fileName);
            }
            else
            {
                if (apiData is null)
                {
                    _underlying = new FileInfoModel(fileName, page);
                    _stream = File.OpenRead(fileName);
                }
                else
                {
                    _underlying = new FileInfoModel("", page);
                    _inMemory = new MemoryStream(apiData, true);
                }
            }
            InfoCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _underlying.InfoCommand.Execute(0);
                });
            });
            SetLengthCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var variant = await page.DisplayPromptAsync(Resource.Message, Resource.SetLength, placeholder: Resource.SetLength, maxLength: int.MaxValue.ToString().Length - 1, keyboard: Keyboard.Numeric, accept: Resource.Accept, cancel: Resource.Cancel);
                        if (variant != Resource.Cancel)
                        {
                            _stream.SetLength(Convert.ToInt32(variant));
                        }
                    }
                    catch
                    {
                        await page.DisplayAlert(Resource.Message, Resource.IOError, Resource.Cancel);
                    }
                });
            }, () => _stream != null);
            SeekCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var variant = await page.DisplayPromptAsync(Resource.Message, Resource.Seek, placeholder: Resource.Seek, maxLength: int.MaxValue.ToString().Length - 1, keyboard: Keyboard.Numeric, accept: Resource.Accept, cancel: Resource.Cancel);
                        if (variant != Resource.Cancel)
                        {
                            _stream.Seek(Convert.ToInt32(variant), SeekOrigin.Current);
                        }
                    }
                    catch
                    {
                        await page.DisplayAlert(Resource.Message, Resource.IOError, Resource.Cancel);
                    }
                });
            }, () => _stream.CanSeek);
            WriteBytesCommand = new Command((obj) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var variant = await page.DisplayPromptAsync(Resource.Message, Resource.WriteBytes, placeholder: Resource.WriteBytes, maxLength: long.MaxValue.ToString().Length - 1, keyboard: Keyboard.Text, accept: Resource.Accept, cancel: Resource.Cancel);
                        if (variant != Resource.Cancel)
                        {
                            var bytes = variant.Split(' ').Select((format) => Convert.ToByte(format)).ToArray();
                            await _stream.WriteAsync(bytes);
                            (obj as Editor).Text += (App.Current.Resources["fileConv"] as SpanToStringConverter).Convert(bytes, null, null, null);
                        }
                    }
                    catch(Exception ex)
                    {
                        await page.DisplayAlert(Resource.Message, Resource.IOError, Resource.Cancel);
                    }
                });
            }, (obj) => _stream.CanWrite);
            ClipboardCommand = new Command(async () =>
            {
                await Xamarin.Essentials.Clipboard.SetTextAsync(Encoding.UTF8.GetString(Bytes));
            });
            DoneCommand = new Command(async () =>
            {
                if (TextWasChanged)
                {
                    await File.WriteAllBytesAsync(fileName, Bytes);
                }
                await DisposeAsync();
                await page.Navigation.PopAsync(true);
            });
        }
        public async ValueTask DisposeAsync()
        {
            if (_stream != null)
            {
                await _stream.DisposeAsync();
            }
            else
            {
                await _inMemory.DisposeAsync();
            }
        }
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

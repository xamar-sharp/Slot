using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.IO;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.ComponentModel;
using System.Threading.Tasks;
namespace Slot.ViewModels
{
    public sealed class DatViewModel : INotifyPropertyChanged, IAsyncDisposable
    {
        public ICommand ClipboardCommand { get; set; }
        public ICommand ReadInt32 { get; set; }
        public ICommand ReadInt16 { get; set; }
        public ICommand ReadInt64 { get; set; }
        public ICommand ReadString { get; set; }
        public ICommand ReadDouble { get; set; }
        public ICommand ReadByte { get; set; }
        public ICommand ReadBoolean { get; set; }
        public ICommand DoneCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand ReadChar { get; set; }
        public ICommand WriteCommand { get; set; }
        private string _txt;
        public string Text { get => _txt; set { _txt = value; OnPropertyChanged(); } }
        public bool TextWasChanged { get; set; }
        private readonly Stream _stream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly FileInfoModel _underlying;
        public event PropertyChangedEventHandler PropertyChanged;
        public DatViewModel(string apiOrFileName, bool writeable, Page page, byte[] apiData = null)
        {
            if (writeable)
            {
                _stream = File.Create(apiOrFileName);
                _writer = new BinaryWriter(_stream);
            }
            else
            {
                if (apiData is null)
                {
                    _underlying = new FileInfoModel(apiOrFileName, page);
                    _stream = File.OpenRead(apiOrFileName);
                }
                else
                {
                    _underlying = new FileInfoModel("", page);
                    _stream = new MemoryStream(apiData, true);
                }
                _reader = new BinaryReader(_stream);
            }
            InfoCommand = new Command(() =>
            {
                _underlying.InfoCommand.Execute(0);
            });
            ClipboardCommand = new Command(async () =>
            {
                await Clipboard.SetTextAsync(Text);
            });
            DoneCommand = new Command(async () =>
            {
                if (TextWasChanged)
                {
                    await File.WriteAllBytesAsync(apiOrFileName, Encoding.UTF8.GetBytes(Text));
                }
                await DisposeAsync();
                await page.Navigation.PopAsync(true);
            });
            WriteCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var format = await page.DisplayPromptAsync(Resource.Message, Resource.EnterBinValue, placeholder: Resource.EnterBinValue, maxLength: 512, keyboard: Keyboard.Email, cancel: Resource.Cancel, accept: Resource.Accept);
                    if (format != Resource.Cancel)
                    {
                        var type = await page.DisplayActionSheet(Resource.EnterBinaryType, Resource.Cancel, Resource.Destructive, Resource.Int, Resource.Long, Resource.Double, Resource.String, Resource.Char, Resource.Byte, Resource.Bool);
                        MatchValue(format, type);
                    }
                });
            });
            ReadInt16 = new Command(() =>
            {
                Text += _reader.ReadInt16() + Environment.NewLine;
            });
            ReadDouble = new Command(() =>
            {
                Text += _reader.ReadDouble() + Environment.NewLine;
            });
            ReadInt32 = new Command(() =>
            {
                Text += _reader.ReadInt32() + Environment.NewLine;
            });
            ReadInt64 = new Command(() =>
            {
                Text += _reader.ReadInt64() + Environment.NewLine;
            });
            ReadByte = new Command(() =>
            {
                Text += _reader.ReadByte() + Environment.NewLine;
            });
            ReadString = new Command(() =>
            {
                Text += _reader.ReadString() + Environment.NewLine;
            });
            ReadChar = new Command(() =>
            {
                Text += _reader.ReadChar() + Environment.NewLine;
            });
            ReadBoolean = new Command(() =>
            {
                Text += _reader.ReadBoolean() + Environment.NewLine;
            });
        }
        public void MatchValue(string format, string type)
        {
            if (type != Resource.Cancel)
            {
                if (type == Resource.Int)
                {
                    _writer.Write(Convert.ToInt32(format));
                }
                else if (type == Resource.Bool)
                {
                    _writer.Write(Convert.ToBoolean(format));
                }
                else if (type == Resource.Double)
                {
                    _writer.Write(Convert.ToDouble(format.Replace('.',',')));
                }
                else if (type == Resource.Long)
                {
                    _writer.Write(Convert.ToInt64(format));
                }
                else if (type == Resource.Short)
                {
                    _writer.Write(Convert.ToInt16(format));
                }
                else if (type == Resource.Char)
                {
                    _writer.Write(Convert.ToChar(format));
                }
                else if (type == Resource.String)
                {
                    _writer.Write(Convert.ToString(format));
                }
                else
                {
                    _writer.Write(Convert.ToByte(format));
                }
                Text += format + Environment.NewLine;
            }
        }
        public async ValueTask DisposeAsync()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            await _stream.DisposeAsync();
        }
        public void OnPropertyChanged([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

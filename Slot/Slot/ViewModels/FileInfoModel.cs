using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
namespace Slot.ViewModels
{
    public sealed class FileInfoModel
    {
        private readonly FileInfo _info;
        public FileInfoModel(string fileName, Page input)
        {
            if (File.Exists(fileName))
                _info = new FileInfo(fileName);
            else
                _info = new FileInfo(Directory.EnumerateFiles(Prefixes.StoragePath, "*", SearchOption.AllDirectories).First());
            MoveCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        string destPath = await input.DisplayPromptAsync(Resource.IOMessage, Resource.MoveFile, placeholder: Resource.EnterAbsolutePath, maxLength: 1024, keyboard: Keyboard.Url, cancel: Resource.Cancel, accept: Resource.Accept);
                        if (destPath != Resource.Cancel)
                        {
                            Directory.Move(_info.FullName, destPath);
                        }
                    }
                    catch
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    }
                });
            });
            CompressCommand = new Command(async () =>
            {
                await new StreamerViewModel(input).Match(".compress", FullName, writeable: false);
            });
            CopyCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        string destPath = await input.DisplayPromptAsync(Resource.IOMessage, Resource.CopyFile, placeholder: Resource.EnterAbsolutePath, maxLength: 1024, keyboard: Keyboard.Url, cancel: Resource.Cancel, accept: Resource.Accept);
                        if (destPath != Resource.Cancel)
                        {
                            File.Copy(_info.FullName, destPath);
                        }
                    }
                    catch
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    }
                });
            });
            DeleteCommand = new Command((obj) =>
            {
                try
                {
                    File.Delete(_info.FullName);
                    (obj as FilesPage).Source.Remove(this);
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    });
                }
            });
            UnzipCommand = new Command(() =>
            {
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(_info.FullName, Path.GetDirectoryName(_info.FullName));
                }
                catch
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    });
                }
            }, () => _info.Extension == ".zip");
            InfoCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await input.Navigation.PushAsync(new FileInfoPage(FullName));
                });
            });
            ToAPICommand = new Command(async () =>
              {
                  if (!await SlotAPI.PostFile(new FileModel() { Data = await File.ReadAllBytesAsync(FullName), Extension = Extension }))
                  {
                      Device.BeginInvokeOnMainThread(async () =>
                      {
                          await input.DisplayAlert(Resource.Message, Resource.APIError, Resource.Accept);
                      });
                  }
              },()=>App.IsAuthorized);
        }
        public string Name { get { return _info.Name; } set { } }
        public string DirectoryName { get { return _info.DirectoryName; } set { } }
        public string FullName { get { return _info.FullName; } set { } }
        public string Extension { get { return _info.Extension; } set { } }
        public string Length
        {
            get
            {
                if (_info.Length >= Prefixes.GigaByte)
                {
                    return $"{_info.Length / Prefixes.GigaByte} GB";
                }
                else if (_info.Length >= Prefixes.MegaByte)
                {
                    return $"{_info.Length / Prefixes.MegaByte} MB";
                }
                else if (_info.Length >= Prefixes.KiloByte)
                {
                    return $"{_info.Length / Prefixes.KiloByte} KB";
                }
                else
                {
                    return $"{_info.Length} B";
                }
            }
            set { }
        }
        public DateTime CreationTime { get => _info.CreationTime; set { } }
        public DateTime LastAccessTime { get => _info.LastAccessTime; set { } }
        public DateTime LastWriteTime { get => _info.LastWriteTime; set { } }
        public string Attributes { get => _info.Attributes.ToString(); set { } }
        public ICommand MoveCommand { get; set; }
        public ICommand ToAPICommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand UnzipCommand { get; set; }
        public ICommand CompressCommand { get; set; }
    }
}

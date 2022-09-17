using System;
using System.IO.Compression;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace Slot.ViewModels
{
    public sealed class DirectoryInfoModel
    {
        private DirectoryInfo _info;
        public DirectoryInfoModel(string dirName, Page input)
        {
            _info = new DirectoryInfo(dirName);
            SubdirectoryCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string word = await input.DisplayPromptAsync(Resource.IOMessage, Resource.CreateSubdir, placeholder: Resource.CreateSubdir, accept: Resource.Accept, cancel: Resource.Cancel, maxLength: 64, keyboard: Keyboard.Text);
                    if (!Directory.Exists(Path.Combine(_info.FullName, word)))
                    {
                        try
                        {
                            var dir = _info.CreateSubdirectory(word);
                            await input.Navigation.PushAsync(new DirectoryInfoPage(new DirectoryInfoModel(dir.FullName, input)), false);
                        }
                        catch
                        {
                            await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                        }
                    }
                });
            });
            InfoCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await input.Navigation.PushAsync(new DirectoryInfoPage(this));
                });
            });
            GoToFiles = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        _info = _info.FullName == "/storage/emulated" ? new DirectoryInfo("/storage/emulated/0/") : _info;
                        _info = _info.FullName == "/data" ? new DirectoryInfo("/data/user/0/com.companyname.slot/") : _info;
                        await input.Navigation.PushAsync(new FilesPage( _info.GetFiles("*", SearchOption.TopDirectoryOnly).Select(ent => new FileInfoModel(ent.FullName, input)).ToList()), false);
                    }
                    catch (Exception ex)
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    }
                });
            });
            GoToDirectories = new Command((obj) =>
             {
                 Device.BeginInvokeOnMainThread(async () =>
                 {
                     try
                     {
                         _info = _info.FullName == "/storage/emulated" ? new DirectoryInfo("/storage/emulated/0/") : _info;
                         _info = _info.FullName == "/data" ? new DirectoryInfo("/data/user/0/com.companyname.slot/") : _info;
                         await input.Navigation.PushAsync(new DirectoriesPage(initials: _info.GetDirectories("*", SearchOption.TopDirectoryOnly).Select(ent => new DirectoryInfoModel(ent.FullName, obj as Page)).ToList()), false);
                     }
                     catch (Exception ex)
                     {
                         await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                     }
                 });
             });
            MoveCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        string destPath = await input.DisplayPromptAsync(Resource.IOMessage, Resource.MoveDirectory, placeholder: Resource.EnterAbsolutePath, maxLength: 1024, keyboard: Keyboard.Url, cancel: Resource.Cancel, accept: Resource.Accept);
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
            DeleteCommand = new Command((obj) =>
            {
                try
                {
                    Directory.Delete(_info.FullName, true);
                    (obj as DirectoriesPage).Source.Remove(this);
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    });
                }
            });
            ZipCommand = new Command(() =>
            {
                try
                {
                    ZipFile.CreateFromDirectory(_info.FullName, _info.FullName + ".zip");
                }
                catch
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await input.DisplayAlert(Resource.IOMessage, Resource.IOError, Resource.Accept);
                    });
                }
            });
        }
        public string Name { get { return _info.Name; } set { } }
        public string Parent { get { return _info.Parent.Name; } set { } }
        public string FullName { get { return _info.FullName; } set { } }
        public string Extension { get { return _info.Extension; } set { } }
        public string Root
        {
            get
            {
                return _info.Root.Name;
            }
            set { }
        }
        public int Count
        {
            get
            {
                try
                {
                    return _info.GetFileSystemInfos().Length;
                }
                catch
                {
                    return -1;
                }
            }
            set
            {

            }
        }
        public DateTime CreationTime { get => _info.CreationTime; set { } }
        public DateTime LastAccessTime { get => _info.LastAccessTime; set { } }
        public DateTime LastWriteTime { get => _info.LastWriteTime; set { } }
        public string Attributes { get => _info.Attributes.ToString(); set { } }
        public ICommand SubdirectoryCommand { get; set; }
        public ICommand GoToFiles { get; set; }
        public ICommand GoToDirectories { get; set; }
        public ICommand MoveCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ZipCommand { get; set; }

    }
}

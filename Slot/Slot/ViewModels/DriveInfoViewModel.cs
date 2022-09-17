using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
namespace Slot.ViewModels
{
    public sealed class DriveInfoViewModel
    {
        private readonly DriveInfo _underlying;
        public DriveInfoViewModel(DriveInfo info, Page page)
        {
            _underlying = info;
            InfoCommand = new Command(() =>
           {
               Device.BeginInvokeOnMainThread(async () =>
               {
                   await page.Navigation.PushAsync(new DriveInfoPage(this));
               });
           });
            GoToDirectories = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await page.Navigation.PushAsync(new DirectoriesPage(this), false);
                });
            });
        }
        public long GetAvailableFree() => _underlying.AvailableFreeSpace;
        public long GetSize() => _underlying.TotalSize;
        public bool IsReady { get => _underlying.IsReady; set { } }
        public string Name { get { return _underlying.Name; } set { } }
        /// <summary>
        /// Not supports on Android!!!
        /// </summary>
        public string VolumeLabel { get { if (_underlying.IsReady) { return _underlying.VolumeLabel; } return Resource.JsonUndefined; } set { _underlying.VolumeLabel = value; } }
        public string Size
        {
            get
            {
                if (_underlying.IsReady)
                {
                    if (_underlying.TotalSize >= Prefixes.MegaByte)
                    {
                        return $"{_underlying.TotalSize / Prefixes.MegaByte} MB";
                    }
                    else if (_underlying.TotalSize >= Prefixes.KiloByte)
                    {
                        return $"{_underlying.TotalSize / Prefixes.KiloByte} KB";
                    }
                    else
                    {
                        return $"{_underlying.TotalSize} B";
                    }
                }
                else
                {
                    return Resource.JsonUndefined;
                }
            }
            set { }
        }
        public string TotalFree
        {
            get
            {
                if (_underlying.IsReady)
                {
                    if (_underlying.TotalFreeSpace >= Prefixes.MegaByte)
                    {
                        return $"{_underlying.TotalFreeSpace / Prefixes.MegaByte} MB";
                    }
                    else if (_underlying.TotalFreeSpace >= Prefixes.KiloByte)
                    {
                        return $"{_underlying.TotalFreeSpace / Prefixes.KiloByte} KB";
                    }
                    else
                    {
                        return $"{_underlying.TotalFreeSpace} B";
                    }
                }
                return Resource.JsonUndefined;
            }
            set { }
        }
        public string AvailableFree
        {
            get
            {
                if (_underlying.IsReady)
                {
                    if (_underlying.AvailableFreeSpace >= Prefixes.MegaByte)
                    {
                        return $"{_underlying.AvailableFreeSpace / Prefixes.MegaByte} MB";
                    }
                    else if (_underlying.AvailableFreeSpace >= Prefixes.KiloByte)
                    {
                        return $"{_underlying.AvailableFreeSpace / Prefixes.KiloByte} KB";
                    }
                    else
                    {
                        return $"{_underlying.AvailableFreeSpace} B";
                    }
                }
                return Resource.JsonUndefined;
            }
            set { }
        }
        public string Format { get { if (_underlying.IsReady) { return _underlying.DriveFormat; } return Resource.JsonUndefined; } set { } }
        public string Type { get { if (_underlying.IsReady) { return _underlying.DriveType.ToString(); } return Resource.JsonUndefined; } set { } }
        public DirectoryInfo Root { get { if (_underlying.IsReady) { return _underlying.RootDirectory; } return null; } }
        public ICommand InfoCommand { get; set; }
        public ICommand GoToDirectories { get; set; }
        public static IList<DriveInfoViewModel> Cast(DriveInfo[] drives, Page page)
        {
            return drives.Select(ent => new DriveInfoViewModel(ent, page)).ToList();
        }
    }
}

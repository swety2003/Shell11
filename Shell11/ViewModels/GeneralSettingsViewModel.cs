using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iNKORE.UI.WPF.Modern;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using System;
using System.ComponentModel;

namespace Shell11.ViewModels
{
    public partial class GeneralSettingsViewModel : ObservableObject, IConfigurationChangeAware,IDisposable
    {
        private bool disposedValue;
        private PropertyChangedEventHandler handler;

        public GeneralSettingsViewModel()
        {
            handler = Settings.Subscribe(this);
        }
        public void HandleSettingChange(string setting)
        {
            switch (setting)
            {
                case "ColorTheme":
                    if (Settings.Instance.ColorTheme==0)
                    {
                        ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    }
                    else
                    {
                        ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    }
                    break;
            }
        }

        [RelayCommand]
        void OnUnload()
        {
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                Settings.UnSubscribe(handler);
                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~GeneralSettingsViewModel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
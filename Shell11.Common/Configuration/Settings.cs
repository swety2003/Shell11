using CommunityToolkit.Mvvm.ComponentModel;
using ManagedShell.AppBar;
using ManagedShell.Common.Enums;
using Microsoft.Extensions.Logging;
using Shell11.Common.Application.Contracts;
using System.ComponentModel;
using System.IO;
using System.Net.Http.Json;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shell11.Common.Configuration
{
    public sealed partial class Settings : ObservableObject
    {
        static ILogger<Settings>? _logger;
        private const string CONFIG_FILE = "config.json";
        public static Settings Instance { get; private set; } = new Settings();

        public static Settings Default => new Settings();

        public static void Load()
        {
            if (File.Exists(CONFIG_FILE))
            {
                _logger?.LogDebug("找到了配置文件");
                Instance = JsonSerializer.Deserialize<Settings>(File.ReadAllText(CONFIG_FILE)) ?? Default;
            }
            else
            {
                _logger?.LogWarning("无配置文件");

                Instance = Default;
            }
        }

        public static void Save()
        {
            File.WriteAllText(CONFIG_FILE, JsonSerializer.Serialize(Instance));
        }

        public static PropertyChangedEventHandler Subscribe(IConfigurationChangeAware objects)
        {
            PropertyChangedEventHandler handler = (s, e) =>
            {
                if (e == null || string.IsNullOrWhiteSpace(e.PropertyName)) return;

                objects.HandleSettingChange(e.PropertyName);
            };
            Instance.PropertyChanged += handler;

            return handler;
        }
        public static void UnSubscribe(PropertyChangedEventHandler handler)
        {
            Instance.PropertyChanged -= handler;
        }

        [ObservableProperty, JsonPropertyName("enableTaskBar")] bool enableTaskBar = true;
        [ObservableProperty, JsonPropertyName("autoHideTaskBar")] bool autoHideTaskBar = true;
        [ObservableProperty, JsonPropertyName("enableMenuBar")] bool enableMenuBar = true;
        [ObservableProperty, JsonPropertyName("autoHideMenuBar")] bool autoHideMenuBar = true;
        [ObservableProperty, JsonPropertyName("enableDesktop")] bool enableDesktop = true;
        [ObservableProperty, JsonPropertyName("enableMenuBarMultiMon")] bool enableMenuBarMultiMon;
        [ObservableProperty, JsonPropertyName("enableTaskbarMultiMon")] bool enableTaskbarMultiMon;
        [ObservableProperty, JsonPropertyName("taskbarMultiMonMode")] int taskbarMultiMonMode;
        [ObservableProperty, JsonPropertyName("autoHideShowDelayMs")] int autoHideShowDelayMs = 200;
        [ObservableProperty, JsonPropertyName("taskbarMode")] int taskbarMode = 2;
        [ObservableProperty, JsonPropertyName("enableTaskbarThumbnails")] bool enableTaskbarThumbnails = true;
        [ObservableProperty, JsonPropertyName("showTaskbarLabels")] bool showTaskbarLabels = true;
        [ObservableProperty, JsonPropertyName("taskbarIconSize")] IconSize taskbarIconSize = IconSize.Medium;
        [ObservableProperty, JsonPropertyName("showTaskbarBadges")] bool showTaskbarBadges;
        [ObservableProperty, JsonPropertyName("taskbarMiddleClick")] int taskbarMiddleClick;
        [ObservableProperty, JsonPropertyName("taskbarEdge")] AppBarEdge taskbarEdge;
        [ObservableProperty, JsonPropertyName("pinnedNotifyIcons")] string[] pinnedNotifyIcons = new string[] 
        { "7820ae76-23e3-4229-82c1-e41cb67d5b9c","7820ae75-23e3-4229-82c1-e41cb67d5b9c","7820ae74-23e3-4229-82c1-e41cb67d5b9c","7820ae73-23e3-4229-82c1-e41cb67d5b9c" };
        [ObservableProperty, JsonPropertyName("taskbarGroupingStyle")] int taskbarGroupingStyle = 1;
    }
}

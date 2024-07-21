using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage.Streams;
using WindowsMediaController;
using static WindowsMediaController.MediaManager;

namespace Shell11.MenuBarExtensions.ViewModels
{
    public partial class MediaViewModel : ObservableObject
    {
        [ObservableProperty]
        string songTitle;

        [ObservableProperty]
        Visibility show = Visibility.Collapsed;

        [ObservableProperty]
        bool playing;

        [ObservableProperty]
        string id;

        [ObservableProperty]
        ImageSource coverSource;

        [ObservableProperty]
        string artist;

        private readonly MediaManager mediaManager;

        public MediaViewModel()
        {
            mediaManager = new MediaManager();

            mediaManager.OnAnySessionOpened += MediaManager_OnAnySessionOpened;
            mediaManager.OnAnySessionClosed += MediaManager_OnAnySessionClosed;
            mediaManager.OnFocusedSessionChanged += MediaManager_OnFocusedSessionChanged;
            mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;
            mediaManager.OnAnyPlaybackStateChanged += MediaManager_OnAnyPlaybackStateChanged;
            //mediaManager.OnAnyTimelinePropertyChanged += MediaManager_OnAnyTimelinePropertyChanged;

            mediaManager.Start();
        }
        MediaManager.MediaSession? activeSession;

        [RelayCommand]
        private async void Back()
        {
            if (activeSession == null)
                return;

            await activeSession.ControlSession.TrySkipPreviousAsync();
            UpdateUI();
        }

        [RelayCommand]
        private async void PlayPause()
        {
            if (activeSession == null)
                return;

            var controlsInfo = activeSession.ControlSession.GetPlaybackInfo().Controls;

            if (controlsInfo.IsPauseEnabled == true)
                await activeSession.ControlSession.TryPauseAsync();
            else if (controlsInfo.IsPlayEnabled == true)
                await activeSession.ControlSession.TryPlayAsync();
            UpdateUI();
        }

        [RelayCommand]
        private async void Forward()
        {
            if (activeSession == null)
                return;

            await activeSession.ControlSession.TrySkipNextAsync();

            UpdateUI();
        }

        async void UpdateUI()
        {
            if (activeSession != null)
            {
                var mediaProperties = await activeSession.ControlSession.TryGetMediaPropertiesAsync();


                SongTitle = mediaProperties.AlbumTitle;
                CoverSource = Helper.GetThumbnail(mediaProperties.Thumbnail);
                Artist = mediaProperties.Artist;
                Id = activeSession.Id;

            }
        }

        private void MediaManager_OnAnyPlaybackStateChanged(MediaManager.MediaSession mediaSession, GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo)
        {
            if (mediaSession != activeSession)
            {
                return;
            }
            Playing = playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
        }

        private void MediaManager_OnAnyMediaPropertyChanged(MediaManager.MediaSession mediaSession, GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                UpdateUI();
            });
        }

        private void MediaManager_OnFocusedSessionChanged(MediaManager.MediaSession mediaSession)
        {
            activeSession = mediaSession;
        }

        private void MediaManager_OnAnySessionClosed(MediaManager.MediaSession mediaSession)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (mediaSession == activeSession)
                {
                    SongTitle = "";
                    CoverSource = null;
                    Show = Visibility.Collapsed;
                    activeSession = null;
                }
            });
        }

        private void MediaManager_OnAnySessionOpened(MediaManager.MediaSession mediaSession)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Show = Visibility.Visible;

                activeSession = mediaSession;

                UpdateUI();
            });
        }

        internal static class Helper
        {
            internal static BitmapImage? GetThumbnail(IRandomAccessStreamReference Thumbnail, bool convertToPng = true)
            {
                if (Thumbnail == null)
                    return null;

                var thumbnailStream = Thumbnail.OpenReadAsync().GetAwaiter().GetResult();
                byte[] thumbnailBytes = new byte[thumbnailStream.Size];
                using (DataReader reader = new DataReader(thumbnailStream))
                {
                    reader.LoadAsync((uint)thumbnailStream.Size).GetAwaiter().GetResult();
                    reader.ReadBytes(thumbnailBytes);
                }

                byte[] imageBytes = thumbnailBytes;

                if (convertToPng)
                {
                    using var fileMemoryStream = new System.IO.MemoryStream(thumbnailBytes);
                    Bitmap thumbnailBitmap = (Bitmap)Bitmap.FromStream(fileMemoryStream);

                    if (!thumbnailBitmap.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                    {
                        using var pngMemoryStream = new System.IO.MemoryStream();
                        thumbnailBitmap.Save(pngMemoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageBytes = pngMemoryStream.ToArray();
                    }
                }

                var image = new BitmapImage();
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }

                return image;
            }
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage.Streams;
using WindowsMediaController;

namespace Shell11.MenuBarExtensions.ViewModels
{
    public partial class MediaViewModel : ObservableObject, IDisposable
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
        private async Task Back()
        {
            if (activeSession == null)
                return;

            await activeSession.ControlSession.TrySkipPreviousAsync();
            //UpdateUI();
        }

        [RelayCommand]
        private async Task PlayPause()
        {
            if (activeSession == null)
                return;

            var controlsInfo = activeSession.ControlSession.GetPlaybackInfo().Controls;

            if (controlsInfo.IsPauseEnabled == true)
                await activeSession.ControlSession.TryPauseAsync();
            else if (controlsInfo.IsPlayEnabled == true)
                await activeSession.ControlSession.TryPlayAsync();
            //UpdateUI();
        }

        [RelayCommand]
        private async Task Forward()
        {
            if (activeSession == null)
                return;

            await activeSession.ControlSession?.TrySkipNextAsync();

            //UpdateUI();
        }

        async Task UpdateUI()
        {
            if (activeSession != null)
            {
                try
                {

                    var mediaProperties = await activeSession.ControlSession.TryGetMediaPropertiesAsync();


                    SongTitle = string.IsNullOrEmpty(mediaProperties.AlbumTitle) ? activeSession.Id : mediaProperties.AlbumTitle;
                    Artist = mediaProperties.Artist;
                    Id = activeSession.Id;
                    CoverSource = await Helper.GetThumbnail(mediaProperties.Thumbnail);
                }
                catch (Exception ex)
                {
                    // todo 
                }

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
                await UpdateUI();
            });
        }

        private void MediaManager_OnFocusedSessionChanged(MediaManager.MediaSession mediaSession)
        {
            activeSession = mediaSession;
        }

        ObservableCollection<MediaManager.MediaSession> sessions = new ObservableCollection<MediaManager.MediaSession>();

        private void MediaManager_OnAnySessionClosed(MediaManager.MediaSession mediaSession)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                sessions.Remove(mediaSession);
                if (mediaSession == activeSession && sessions.Count == 0)
                {
                    SongTitle = "";
                    CoverSource = null;
                    Show = Visibility.Collapsed;
                    activeSession = null;
                }
                else
                {
                    activeSession = sessions.First();
                    await UpdateUI();
                }
            });
        }

        private void MediaManager_OnAnySessionOpened(MediaManager.MediaSession mediaSession)
        {
            sessions.Add(mediaSession);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                Show = Visibility.Visible;

                activeSession = mediaSession;

                await UpdateUI();
            });
        }

        public void Dispose()
        {
            mediaManager.OnAnySessionOpened -= MediaManager_OnAnySessionOpened;
            mediaManager.OnAnySessionClosed -= MediaManager_OnAnySessionClosed;
            mediaManager.OnFocusedSessionChanged -= MediaManager_OnFocusedSessionChanged;
            mediaManager.OnAnyMediaPropertyChanged -= MediaManager_OnAnyMediaPropertyChanged;
            mediaManager.OnAnyPlaybackStateChanged -= MediaManager_OnAnyPlaybackStateChanged;
            mediaManager.Dispose();
        }

        internal static class Helper
        {
            internal static async Task<BitmapImage?> GetThumbnail(IRandomAccessStreamReference? thumbnail, bool convertToPng = false)
            {
                if (thumbnail == null)
                    return null;


                using var thumbnailStream = await thumbnail.OpenReadAsync();
                await using var stream = thumbnailStream.AsStreamForRead();

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
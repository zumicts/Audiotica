﻿using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Audiotica.Core.Exceptions;
using Audiotica.Core.Extensions;
using Audiotica.Core.Windows.Helpers;
using Audiotica.Database.Models;
using Audiotica.Database.Services.Interfaces;
using Audiotica.Windows.Common;
using Audiotica.Windows.Services.Interfaces;
using Audiotica.Windows.Views;
using Autofac;

namespace Audiotica.Windows.Controls
{
    public sealed partial class TrackViewer : INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof (bool), typeof (TrackViewer), null);

        public static readonly DependencyProperty IsCatalogProperty =
            DependencyProperty.Register("IsCatalog", typeof (bool), typeof (TrackViewer), null);

        public static readonly DependencyProperty IsQueueProperty =
            DependencyProperty.Register("IsQueue", typeof (bool), typeof (TrackViewer), null);

        public static readonly DependencyProperty QueueIdProperty =
            DependencyProperty.Register("QueueId", typeof(string), typeof(TrackViewer), null);

        private bool _isPlaying;

        private Track _track;

        public TrackViewer()
        {
            InitializeComponent();
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected

        {
            get { return (bool) GetValue(IsSelectedProperty); }

            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsCatalog

        {
            get { return (bool) GetValue(IsCatalogProperty); }

            set { SetValue(IsCatalogProperty, value); }
        }

        public bool IsQueue

        {
            get { return (bool) GetValue(IsQueueProperty); }

            set { SetValue(IsQueueProperty, value); }
        }

        public string QueueId
        {
            get { return (string)GetValue(QueueIdProperty); }

            set { SetValue(QueueIdProperty, value); }
        }

        public Track Track
        {
            get { return _track; }
            set
            {
                _track = value;
                Bindings.Update();
                TrackChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void TrackChanged()
        {
            var player = App.Current.Kernel.Resolve<IPlayerService>();

            if (Track == null)
                IsPlaying = false;
            else
            {
                if (IsQueue && QueueId != null)
                    IsPlaying = player.CurrentQueueId == QueueId;
                else if (!IsQueue && player.CurrentQueueTrack?.Track != null)
                    IsPlaying = (Track.Id > 0 && player.CurrentQueueTrack.Track.Id == Track.Id) 
                        || TrackComparer.AreEqual(player.CurrentQueueTrack.Track, Track);
                else
                    IsPlaying = false;
            }

            player.TrackChanged -= PlayerOnTrackChanged;
            player.TrackChanged += PlayerOnTrackChanged;
        }

        private void PlayerOnTrackChanged(object sender, string s) => TrackChanged();

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            using (var lifetimeScope = App.Current.Kernel.BeginScope())
            {
                var playerService = lifetimeScope.Resolve<IPlayerService>();
                try
                {
                    var queue = playerService.ContainsTrack(Track) ?? await playerService.AddAsync(Track);
                    // player auto plays when there is only one track
                    if (playerService.PlaybackQueue.Count > 1)
                        playerService.Play(queue);
                    IsSelected = false;
                }
                catch (AppException ex)
                {
                    CurtainPrompt.ShowError(ex.Message ?? "Something happened.");
                }
            }
        }

        private async void AddCollection_Click(object sender, RoutedEventArgs e)
        {
            var button = (MenuFlyoutItem) sender;
            button.IsEnabled = false;

            using (var scope = App.Current.Kernel.BeginScope())
            {
                var trackSaveService = scope.Resolve<ITrackSaveService>();

                try
                {
                    await trackSaveService.SaveAsync(Track);
                }
                catch (AppException ex)
                {
                    Track.Status = TrackStatus.None;
                    CurtainPrompt.ShowError(ex.Message ?? "Problem saving: " + Track);
                }
                finally
                {
                    button.IsEnabled = true;
                }
            }
        }

        private async void AddQueue_Click(object sender, RoutedEventArgs e)
        {
            using (var scope = App.Current.Kernel.BeginScope())
            {
                var backgroundAudioService = scope.Resolve<IPlayerService>();
                try
                {
                    await backgroundAudioService.AddAsync(Track);
                    CurtainPrompt.Show("Added to queue");
                }
                catch (AppException ex)
                {
                    CurtainPrompt.ShowError(ex.Message ?? "Something happened.");
                }
            }
        }

        private async void AddUpNext_Click(object sender, RoutedEventArgs e)
        {
            using (var scope = App.Current.Kernel.BeginScope())
            {
                var backgroundAudioService = scope.Resolve<IPlayerService>();
                try
                {
                    await backgroundAudioService.AddUpNextAsync(Track);
                    CurtainPrompt.Show("Added up next");
                }
                catch (AppException ex)
                {
                    CurtainPrompt.ShowError(ex.Message ?? "Something happened.");
                }
            }
        }

        private void Viewer_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = (Grid) sender;
            FlyoutEx.ShowAttachedFlyoutAtPointer(grid);
        }

        private void ExploreArtist_Click(object sender, RoutedEventArgs e)
        {
            App.Current.NavigationService.Navigate(typeof (ArtistPage), Track.DisplayArtist);
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            using (var scope = App.Current.Kernel.BeginScope())
            {
                var downloadService = scope.Resolve<IDownloadService>();
                downloadService.StartDownloadAsync(Track);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (var scope = App.Current.Kernel.BeginScope())
            {
                var libraryService = scope.Resolve<ILibraryService>();
                await libraryService.DeleteTrackAsync(Track);

                // make sure to navigate away if album turns out empty
                if (!IsCatalog && App.Current.NavigationService.CurrentPageType == typeof (AlbumPage))
                {
                    var album = libraryService.Albums.FirstOrDefault(p => p.Title.EqualsIgnoreCase(Track.AlbumTitle));
                    if (album == null)
                        App.Current.NavigationService.GoBack();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ManualMatch_Click(object sender, RoutedEventArgs e)
        {
            App.Current.NavigationService.Navigate(typeof(ManualMatchPage), Track.Id);
        }
    }
}
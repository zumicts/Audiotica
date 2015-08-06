using System;
using System.Collections.Generic;
using Windows.Media.Playback;
using Audiotica.Core.Windows.Helpers;
using Audiotica.Core.Windows.Messages;
using Audiotica.Database.Models;

namespace Audiotica.Windows.Player
{
    internal class ForegroundMessenger : IDisposable
    {
        public ForegroundMessenger()
        {
            // Initialize message channel 
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
        }

        public void Dispose()
        {
            BackgroundMediaPlayer.MessageReceivedFromForeground -= BackgroundMediaPlayer_MessageReceivedFromForeground;
        }

        public event EventHandler AppSuspended;
        public event EventHandler AppResumed;
        public event EventHandler StartPlayback;
        public event EventHandler SkipToNext;
        public event EventHandler SkipToPrev;
        public event EventHandler<string> TrackChanged;
        public event EventHandler<List<QueueTrack>> UpdatePlaylist;

        /// <summary>
        ///     Raised when a message is recieved from the foreground app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender,
            MediaPlayerDataReceivedEventArgs e)
        {
            var message = MessageHelper.ParseMessage(e.Data);

            if (message is AppSuspendedMessage)
                AppSuspended?.Invoke(this, null);
            else if (message is AppResumedMessage)
                AppResumed?.Invoke(this, null);
            else if (message is StartPlaybackMessage)
                StartPlayback?.Invoke(this, null);
            else if (message is SkipNextMessage)
                SkipToNext?.Invoke(this, null);
            else if (message is SkipPreviousMessage)
                SkipToPrev?.Invoke(this, null);
            else
            {
                var changedMessage = message as TrackChangedMessage;
                if (changedMessage != null)
                    TrackChanged?.Invoke(this, changedMessage.QueueId);

                else
                {
                    var playlistMessage = message as UpdatePlaylistMessage;
                    if (playlistMessage != null)
                    {
                        UpdatePlaylist?.Invoke(this, playlistMessage.Tracks);
                    }
                }
            }
        }
    }
}
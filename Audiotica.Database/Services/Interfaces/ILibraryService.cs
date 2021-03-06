﻿using System;
using System.Threading.Tasks;
using Audiotica.Core.Common;
using Audiotica.Database.Models;

namespace Audiotica.Database.Services.Interfaces
{
    public interface ILibraryService
    {
        #region Properties

        event EventHandler Loaded;
        bool IsLoaded { get; }

        /// <summary>
        ///     Gets the tracks.
        /// </summary>
        /// <value>
        ///     The tracks.
        /// </value>
        OptimizedObservableCollection<Track> Tracks { get; }

        /// <summary>
        ///     Gets the albums.
        /// </summary>
        /// <value>
        ///     The albums.
        /// </value>
        OptimizedObservableCollection<Album> Albums { get; }

        /// <summary>
        ///     Gets the artists.
        /// </summary>
        /// <value>
        ///     The artists.
        /// </value>
        OptimizedObservableCollection<Artist> Artists { get; }

        /// <summary>
        ///     Gets the playlists.
        /// </summary>
        /// <value>
        ///     The playlists.
        /// </value>
        OptimizedObservableCollection<Playlist> Playlists { get; }

        #endregion

        #region Helpers

        Track Find(long id);
        Track Find(Track track);

        #endregion

        #region Sync

        void Load();

        /// <summary>
        ///     Adds the track to the library, saves it to the db.
        /// </summary>
        void AddTrack(Track track);

        void UpdateTrack(Track track);
        void DeleteTrack(Track track);

        #endregion

        #region Async

        Task LoadAsync();

        /// <summary>
        ///     Adds the track to the library, saves it to the db.
        /// </summary>
        Task AddTrackAsync(Track track);

        Task UpdateTrackAsync(Track track);
        Task DeleteTrackAsync(Track track);

        #endregion
    }
}
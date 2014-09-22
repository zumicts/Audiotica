﻿#region

using System;
using Audiotica.Data.Collection.SqlHelper;

#endregion

namespace Audiotica.Data.Collection.Model
{
    public class Song : BaseEntry
    {
        public string ProviderId { get; set; }

        [SqlProperty(ReferenceTo = typeof(Artist))]
        public long ArtistId { get; set; }

        [SqlProperty(ReferenceTo = typeof(Album))]
        public long AlbumId { get; set; }

        public string Name { get; set; }

        public long TrackNumber { get; set; }

        public string AudioUrl { get; set; }

        public SongState SongState { get; set; }

        public long PlayCount { get; set; }

        public HeartState HeartState { get; set; }

        
        [SqlIgnore]
        public bool IsStreaming
        {
            get { return new Uri(AudioUrl).IsAbsoluteUri; }
        }

        [SqlIgnore]
        public Artist Artist { get; set; }

        [SqlIgnore]
        public Album Album { get; set; }
    }

    public enum HeartState
    {
        None,
        Like,
        Dislike
    }

    public enum SongState
    {
        None,
        Downloading,
        Downloaded,
        Uploading
        //still playing with different states
    }
}
﻿using Audiotica.Data.Collection.SqlHelper;

namespace Audiotica.Data.Collection.Model
{
    public class QueueSong : BaseEntry
    {
        public Song Song { get; set; }

        public long SongId { get; set; }

        public long PrevId { get; set; }

        public long NextId { get; set; }

        public long ShuffleNextId { get; set; }

        public long ShufflePrevId { get; set; }
    }
}
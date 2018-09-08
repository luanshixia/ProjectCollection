using System;
using System.Collections.Generic;
using System.Text;

namespace BookKeeper
{
    public class BookInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string ISBN { get; set; }

        public string Publisher { get; set; }

        public string Author { get; set; }

        public string Series { get; set; }

        public string Language { get; set; }

        public int Pages { get; set; }

        public BookFormat Format { get; set; }

        public BookFileQuality FileQuality { get; set; }

        public long Size { get; set; }

        public DateTime PublishTime { get; set; }

        public string Tags { get; set; }

        public string Category { get; set; }

        public BookStars Stars { get; set; }

        public BookRanking[] Rankings { get; set; }
    }

    public enum BookFormat
    {
        Unknown,
        PDF,
        ePub,
        Mobi,
        Azw3,
        Djvu,
        Chm,
        Txt,
        Doc,
        Archive,
        Exe
    }

    public enum BookFileQuality
    {
        Unknown,
        Scanned,
        DIY,
        Original
    }

    public class BookStars
    {
        public double Value { get; set; }

        public int NumberOfPeople { get; set; }
    }

    public class BookRanking
    {
        public string Category { get; set; }

        public int Value { get; set; }
    }
}

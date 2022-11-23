using System;

namespace Covid.API.Models
{
    public enum ECity
    {
        Istanbul = 0,
        Ankara = 1,
        Izmir = 2,
        Konya = 3,
        Antalya = 4
    }

    public class Covid
    {
        public int Id { get; set; }
        public ECity City { get; set; }

        public int Count { get; set; }

        public DateTime CovidDate { get; set; }

    }
}

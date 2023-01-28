using System.Collections.Generic;
using System.IO;

namespace Yerevan_Housing_API.Models
{
    public class House
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string MinimalPeriod { get; set; }
        public string RoomCount { get; set; }
        public string Floor { get; set; }
        public string Animals { get; set; }
        public decimal Rent { get; set; }
        public Media Media { get; set; }
        public string Name { get; set; }
        public string Percent { get; set; }
    }
    public class Media
    {
        public List<MemoryStream> Photos { get; set; }
        public List<string> Videos { get; set; }
    }
}

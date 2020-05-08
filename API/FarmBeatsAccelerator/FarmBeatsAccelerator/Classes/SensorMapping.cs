using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmBeatsAccelerator.Classes
{
    public class SensorMapping
    {
            public Item[] items { get; set; }
        

        public class Item
        {
            public string id { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime lastModifiedAt { get; set; }
            public string hardwareId { get; set; }
            public string sensorModelId { get; set; }
            public Location location { get; set; }
            public string deviceId { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public Properties properties { get; set; }
        }

        public class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
            public float elevation { get; set; }
        }

        public class Properties
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmBeatsAccelerator.Classes
{
    public class Sensor
    {
        public int SkuId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double soilTemp { get; set; }
        public double light { get; set; }
        public double ambTemp { get; set; }
        public double soilMoisture { get; set; }
        public double windSpeed { get; set; }
        public double windDirection { get; set; }
        public double ambHumidity { get; set; }
        public double atmPressure { get; set; }
        public double CO2 { get; set; }
        public double BME280Temp { get; set; }
        public double BME280Humidity { get; set; }
        public double BME280Pressure { get; set; }
        public double Battery { get; set; }
        public DateTime Timestamp { get; set; }
        public double rain { get; set; }
        public string model { get; set; }
        public string Name { get; set; }
        public string id { get; set; }
        public Sensor() { }
    }
}

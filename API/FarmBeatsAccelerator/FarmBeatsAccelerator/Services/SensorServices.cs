using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmBeatsAccelerator.Classes;
using FarmBeatsAccelerator.Entities;
using FarmBeatsAccelerator.DBcollection;
using Microsoft.Azure.Cosmos.Table;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using RestSharp;
using System.Text.Json.Serialization;
using System.Text.Json;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace FarmBeatsAccelerator.Services
{
    public class SensorServices
    {
        static HttpClient client = new HttpClient();

        public static List<Sensor> GetSensors()
        {
            var client = new RestClient("https://ndsufarmbeats-api.azurewebsites.net/Sensor");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            List<Sensor> returnList = new List<Sensor>();
            SensorMapping SM = JsonSerializer.Deserialize<SensorMapping>(response.Content);
            foreach (SensorMapping.Item x in SM.items)
            {
                Sensor temp = new Sensor();
                temp.Name = x.id;
                temp.latitude = x.location.latitude;
                temp.longitude = x.location.longitude;
                temp.model = x.hardwareId;
                temp.id = x.id;
                returnList.Add(temp);
            }
            return returnList;

        }

        public static Sensor UpdateLocation( string SensorName, double longitude, double latitude)
        {
            List<Sensor> findSensor = GetSensors();
            Sensor updateSensor = new Sensor();
            foreach (Sensor x in findSensor)
            {
                if (x.Name.Equals(SensorName))
                {
                    updateSensor = x;
                    break;
                }
            }
            updateSensor.latitude = latitude;
            updateSensor.longitude = longitude;
            //Add a put request to Sensor to update the location of the sensor
            return updateSensor;
        }
        static List<double> getDataList(String id, string value)
        {
            var client = new RestClient("https://ndsufarmbeats-api.azurewebsites.net/Telemetry");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"sensorId\": \"" + id + "\",\r\n  \"searchSpan\": {\r\n    \"from\": \"2020-04-10T17:02:53.969Z\",\r\n    \"to\": \"2020-04-29T17:02:53.970Z\"\r\n  },\r\n  \"projectedProperties\": [\r\n    {\r\n      \"name\":\"" + value + "\",\r\n      \"type\":\"Double\"\r\n    }\r\n  ]\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            List<double> returnList = new List<double>();
            DataMapping DM = JsonSerializer.Deserialize<DataMapping>(response.Content);

            foreach (DataMapping.Property y in DM.properties)
            {
                foreach (double x in y.values)
                {
                    returnList.Add(x);
                }
            }
            return returnList;
        }
        static List<DateTime> getTimeList(String id)
        {
            var client = new RestClient("https://ndsufarmbeats-api.azurewebsites.net/Telemetry");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n  \"sensorId\": \"" + id + "\",\r\n  \"searchSpan\": {\r\n    \"from\": \"2020-04-10T17:02:53.969Z\",\r\n    \"to\": \"2020-04-29T17:02:53.970Z\"\r\n  },\r\n  \"projectedProperties\": [\r\n    {\r\n      \"name\":\"" + "soil_temperature" + "\",\r\n      \"type\":\"Double\"\r\n    }\r\n  ]\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            List<DateTime> returnList = new List<DateTime>();
            DataMapping DM = JsonSerializer.Deserialize<DataMapping>(response.Content);
            Console.WriteLine(response.Content);
            foreach (DateTime x in DM.timestamps)
            {
                returnList.Add(x);
            }
            return returnList;
        }
        public static List<Sensor> GetApiData(string AuthToken)
        {
            List<Sensor> sensorList = GetSensors(AuthToken);
            List<Sensor> returnList = new List<Sensor>();
            foreach (Sensor x in sensorList)
            {
                List<double> bme280_temperature = getDataList(AuthToken, x.id, "bme280_temperature");
                List<double> bme280_humidity = getDataList(AuthToken, x.id, "bme280_humidity");
                List<double> bme280_pressure = getDataList(AuthToken, x.id, "bme280_pressure");
                List<double> soil_moisture = getDataList(AuthToken, x.id, "soil_moisture");
                List<double> soil_temperature = getDataList(AuthToken, x.id, "soil_temperature");
                List<double> ambient_temperature = getDataList(AuthToken, x.id, "ambient_temperature");
                List<double> ambient_humidity = getDataList(AuthToken, x.id, "ambient_humidity");
                List<double> atmospheric_pressure = getDataList(AuthToken, x.id, "atmospheric_pressure");
                List<double> light = getDataList(AuthToken, x.id, "light");
                List<double> wind_speed = getDataList(AuthToken, x.id, "wind_speed");
                List<double> wind_direction = getDataList(AuthToken, x.id, "wind_direction");
                List<double> battery_level = getDataList(AuthToken, x.id, "battery_level");
                List<double> co2 = new List<double>();
                List<double> rain = new List<double>();
                List<DateTime> TimeStamps = getTimeList(AuthToken, x.id);
                if (x.model.Equals("fe0b2774-6aba-4544-9961-cd965179e6dd"))
                {
                    co2 = getDataList(AuthToken, x.id, "co2");
                }
                if (x.model.Equals("c5d9d19f-31c7-4ce6-a5ed-fbd200e100a4"))
                {
                    rain = getDataList(AuthToken, x.id, "rain");
                }
                for (int i = 0; i < soil_moisture.Count; i++)
                {
                    Sensor Temp = x;
                    Temp.BME280Temp = bme280_temperature[i];
                    Temp.BME280Humidity = bme280_humidity[i];
                    Temp.BME280Pressure = bme280_pressure[i];
                    Temp.soilMoisture = soil_moisture[i];
                    Temp.soilTemp = soil_temperature[i];
                    Temp.ambTemp = ambient_temperature[i];
                    Temp.ambHumidity = ambient_humidity[i];
                    Temp.atmPressure = atmospheric_pressure[i];
                    Temp.light = light[i];
                    Temp.windSpeed = wind_speed[i];
                    Temp.windDirection = wind_direction[i];
                    Temp.Battery = battery_level[i];
                    Temp.Timestamp = TimeStamps[i];
                    if (x.model.Equals("fe0b2774-6aba-4544-9961-cd965179e6dd"))
                    {
                        Temp.CO2 = co2[i];
                    }
                    if (x.model.Equals("c5d9d19f-31c7-4ce6-a5ed-fbd200e100a4"))
                    {
                        Temp.rain = rain[i];
                    }
                    Console.WriteLine(Temp.soilTemp);
                    returnList.Add(Temp);
                }
            }
            foreach (Sensor x in returnList)
            {
                Console.WriteLine(x.soilTemp);
            }
            return returnList;
        }
    }
}
   
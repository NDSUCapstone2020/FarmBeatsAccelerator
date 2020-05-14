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
        public static string GetAuth()
        {
            RestClient client = new RestClient("https://login.microsoftonline.com/52994fa0-e365-4bfd-9b88-a99d83a92b62/oauth2/v2.0/authorize");
            RestRequest request = new RestRequest() { Method = Method.POST };
            request.AddParameter("response_type", "code");
            request.AddParameter("redirect_uri", "https://ndsufarmbeats-api.azurewebsites.net/callback");
            request.AddParameter("client_id", "30d9bf84-bc24-4cc0-a149-3a298877703d");
            request.AddParameter("scope", "api://30d9bf84-bc24-4cc0-a149-3a298877703d/sign:in");
            request.AddParameter("state", "12345");
            request.AddParameter("response_mode", "query");
            Console.WriteLine(request.ToString());
            var response = client.Execute(request);
            //AuthCode authCode = JsonSerializer.Deserialize<AuthCode>(response.Content);
            client = new RestClient("https://login.microsoftonline.com/52994fa0-e365-4bfd-9b88-a99d83a92b62/oauth2/v2.0/token");
            request = new RestRequest() { Method = Method.POST };
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", "https://ndsufarmbeats-api.azurewebsites.net/callback");
            request.AddParameter("client_id", "30d9bf84-bc24-4cc0-a149-3a298877703d");
            // request.AddParameter("code", authCode);
            response = client.Execute(request);
            Console.WriteLine(response.Content);
            string json = response.Content.ToString();
            //Auth auth = JsonSerializer.Deserialize<Auth>(json);
            string AuthToken = "";
            //AuthToken = auth.access_token;
            return AuthToken;
        }

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

        public static Sensor UpdateLocation(string AuthToken, string SensorName, double longitude, double latitude)
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
        public static List<Sensor> GetApiData()
        {
            List<Sensor> sensorList = GetSensors();
            List<Sensor> returnList = new List<Sensor>();
            foreach (Sensor x in sensorList)
            {
                List<double> bme280_temperature = getDataList(x.id, "bme280_temperature");
                List<double> bme280_humidity = getDataList(x.id, "bme280_humidity");
                List<double> bme280_pressure = getDataList(x.id, "bme280_pressure");
                List<double> soil_moisture = getDataList(x.id, "soil_moisture");
                List<double> soil_temperature = getDataList(x.id, "soil_temperature");
                List<double> ambient_temperature = getDataList(x.id, "ambient_temperature");
                List<double> ambient_humidity = getDataList(x.id, "ambient_humidity");
                List<double> atmospheric_pressure = getDataList(x.id, "atmospheric_pressure");
                List<double> light = getDataList(x.id, "light");
                List<double> wind_speed = getDataList(x.id, "wind_speed");
                List<double> wind_direction = getDataList( x.id, "wind_direction");
                List<double> battery_level = getDataList( x.id, "battery_level");
                List<double> co2 = new List<double>();
                List<double> rain = new List<double>();
                List<DateTime> TimeStamps = getTimeList( x.id);
                if (x.model.Equals("fe0b2774-6aba-4544-9961-cd965179e6dd"))
                {
                    co2 = getDataList(x.id, "co2");
                }
                if (x.model.Equals("c5d9d19f-31c7-4ce6-a5ed-fbd200e100a4"))
                {
                    rain = getDataList(x.id, "rain");
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
    //services after this point only work with the resaerch preview
//        public static List<Sensor> getSensorData(string ID)
//        {
//            string connectionString = AppSettings.LoadAppSettings().StorageConnectionString;
//            CloudStorageAccount storageAccount;
//            try
//            {
//                storageAccount = CloudStorageAccount.Parse(connectionString);
//            }
//            catch (FormatException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
//                throw;
//            }
//            catch (ArgumentException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
//                Console.ReadLine();
//                throw;
//            }
//            string name = "AgSensorBoxData" + ID;
//            DBCollection<SensorDataEntity> DataDB = new DBCollection<SensorDataEntity>();
//            DBCollection<SensorLocationEntity> LocationDB = new DBCollection<SensorLocationEntity>();
//            CloudTable dataTable = DataDB.GetTable(name, storageAccount);
//            CloudTable locationTable = LocationDB.GetTable("SensorLocation", storageAccount);
//            List<SensorDataEntity> sensorData = DataDB.GetAll(dataTable);
//            List<SensorLocationEntity> sensorLocation = LocationDB.GetAll(locationTable);
//            List<Sensor> SD = new List<Sensor>();

//            foreach (SensorDataEntity x in sensorData)
//            {
//                Sensor temp = new Sensor();
//                temp.soilMoisture = x.CompChannel3;
//                temp.soilTemp = x.CompChannel4;
//                temp.ambTemp = x.CompChannel5;
//                temp.light = x.CompChannel9;
//                temp.windSpeed = x.CompChannel10;
//                temp.windDirection = x.CompChannel11;
//                temp.Timestamp = x.TimeStamp;
//                temp.SkuId = x.SkuId;
//                temp.ambHumidity = x.CompChannel6;
//                temp.atmPressure = x.CompChannel7;
//                temp.CO2 = x.CompChannel8;
//                //temp.Rain = x.CompChannel9;
//                temp.BME280Temp = x.CompChannel0;
//                temp.BME280Humidity = x.CompChannel1;
//                temp.BME280Pressure = x.CompChannel2;
//                //temp.digitalSensor = x.CompChannel13;
//                temp.Battery = x.CompChannel12;

//                foreach (SensorLocationEntity z in sensorLocation)
//                {
//                    Console.WriteLine(z.ToString());
//                    if (temp.SkuId.Equals(z.SkuId))
//                    {
//                        temp.longitude = z.longitude;
//                        temp.latitude = z.latitude;
//                    }
//                }

//                SD.Add(temp);

//            }
//            return SD;
//        }
//        public static List<Sensor> getFarmData(string farmName)
//        {
//            string connectionString = AppSettings.LoadAppSettings().StorageConnectionString;
//            CloudStorageAccount storageAccount;
//            try
//            {
//                storageAccount = CloudStorageAccount.Parse(connectionString);
//            }
//            catch (FormatException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
//                throw;
//            }
//            catch (ArgumentException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
//                Console.ReadLine();
//                throw;
//            }
//            //
//            DBCollection<IdFinderEntity> IdentityDB = new DBCollection<IdFinderEntity>();
//            DBCollection<SensorDataEntity> DataDB = new DBCollection<SensorDataEntity>();
//            DBCollection<SensorLocationEntity> LocationDB = new DBCollection<SensorLocationEntity>();
//            CloudTable idTable = IdentityDB.GetTable("IdFinder", storageAccount);
//            List<IdFinderEntity> IdentityData = IdentityDB.GetAll(idTable);
//            CloudTable locationTable = LocationDB.GetTable("SensorLocation", storageAccount);
//            List<SensorLocationEntity> sensorLocation = LocationDB.GetAll(locationTable);
//            List<Sensor> FD = new List<Sensor>();

//            foreach (IdFinderEntity y in IdentityData)
//            {
//                if (y.PartitionKey.Equals(farmName))
//                {
//                    string ID = y.RowKey;
//                    string name = "AgSensorBoxData" + ID;
//                    CloudTable dataTable = DataDB.GetTable(name, storageAccount);
//                    List<SensorDataEntity> sensorData = DataDB.GetAll(dataTable);



//                    foreach (SensorDataEntity x in sensorData)
//                    {
//                        Sensor temp = new Sensor();
//                        temp.soilMoisture = x.CompChannel3;
//                        temp.soilTemp = x.CompChannel4;
//                        temp.ambTemp = x.CompChannel5;
//                        temp.light = x.CompChannel9;
//                        temp.windSpeed = x.CompChannel10;
//                        temp.windDirection = x.CompChannel11;
//                        temp.Timestamp = x.TimeStamp;
//                        temp.SkuId = x.SkuId;
//                        temp.ambHumidity = x.CompChannel6;
//                        temp.atmPressure = x.CompChannel7;
//                        temp.CO2 = x.CompChannel8;
//                        //temp.Rain = x.CompChannel9;
//                        temp.BME280Temp = x.CompChannel0;
//                        temp.BME280Humidity = x.CompChannel1;
//                        temp.BME280Pressure = x.CompChannel2;
//                        //temp.digitalSensor = x.CompChannel13;
//                        temp.Battery = x.CompChannel12;

//                        foreach (SensorLocationEntity z in sensorLocation)
//                        {
//                            Console.WriteLine(z.ToString());
//                            if (temp.SkuId.Equals(z.SkuId))
//                            {
//                                temp.longitude = z.longitude;
//                                temp.latitude = z.latitude;
//                            }
//                        }

//                        FD.Add(temp);

//                    }
//                }
//            }

//            return FD;
//        }
//        public static List<Sensor> ApiCall()
//        {
//            List<Sensor> returnList = new List<Sensor>();


//            return returnList;
//        }
//        public static List<Sensor> getAllData()
//        {
//            string connectionString = AppSettings.LoadAppSettings().StorageConnectionString;
//            CloudStorageAccount storageAccount;
//            try
//            {
//                storageAccount = CloudStorageAccount.Parse(connectionString);
//            }
//            catch (FormatException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
//                throw;
//            }
//            catch (ArgumentException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
//                Console.ReadLine();
//                throw;
//            }
//            //
//            DBCollection<IdFinderEntity> IdentityDB = new DBCollection<IdFinderEntity>();
//            DBCollection<SensorDataEntity> DataDB = new DBCollection<SensorDataEntity>();
//            DBCollection<SensorLocationEntity> LocationDB = new DBCollection<SensorLocationEntity>();
//            CloudTable idTable = IdentityDB.GetTable("IdFinder", storageAccount);
//            List<IdFinderEntity> IdentityData = IdentityDB.GetAll(idTable);
//            CloudTable locationTable = LocationDB.GetTable("SensorLocation", storageAccount);
//            List<SensorLocationEntity> sensorLocation = LocationDB.GetAll(locationTable);
//            List<Sensor> FD = new List<Sensor>();

//            foreach (IdFinderEntity y in IdentityData)
//            {

//                string ID = y.RowKey;
//                string name = "AgSensorBoxData" + ID;
//                CloudTable dataTable = DataDB.GetTable(name, storageAccount);
//                List<SensorDataEntity> sensorData = DataDB.GetAll(dataTable);



//                foreach (SensorDataEntity x in sensorData)
//                {
//                    Sensor temp = new Sensor();
//                    temp.soilMoisture = x.CompChannel3;
//                    temp.soilTemp = x.CompChannel4;
//                    temp.ambTemp = x.CompChannel5;
//                    temp.light = x.CompChannel9;
//                    temp.windSpeed = x.CompChannel10;
//                    temp.windDirection = x.CompChannel11;
//                    temp.Timestamp = x.TimeStamp;
//                    temp.SkuId = x.SkuId;
//                    temp.ambHumidity = x.CompChannel6;
//                    temp.atmPressure = x.CompChannel7;
//                    temp.CO2 = x.CompChannel8;
//                    //temp.Rain = x.CompChannel9;
//                    temp.BME280Temp = x.CompChannel0;
//                    temp.BME280Humidity = x.CompChannel1;
//                    temp.BME280Pressure = x.CompChannel2;
//                    //temp.digitalSensor = x.CompChannel13;
//                    temp.Battery = x.CompChannel12;
//                    foreach (SensorLocationEntity z in sensorLocation)
//                    {
//                        Console.WriteLine(z.ToString());
//                        if (temp.SkuId.Equals(z.SkuId))
//                        {
//                            temp.longitude = z.longitude;
//                            temp.latitude = z.latitude;
//                        }
//                    }

//                    FD.Add(temp);

//                }
//            }


//            return FD;
//        }
//        public static void updateLocation(double lat, double lon, string id)
//        {
//            string connectionString = AppSettings.LoadAppSettings().StorageConnectionString;
//            CloudStorageAccount storageAccount;
//            try
//            {
//                storageAccount = CloudStorageAccount.Parse(connectionString);
//            }
//            catch (FormatException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
//                throw;
//            }
//            catch (ArgumentException)
//            {
//                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
//                Console.ReadLine();
//                throw;
//            }
//            DBCollection<SensorLocationEntity> LocationDB = new DBCollection<SensorLocationEntity>();
//            CloudTable locationTable = LocationDB.GetTable("SensorLocation", storageAccount);
//            SensorLocationEntity temp = LocationDB.GetBySkuId(id, locationTable).ElementAt(0);
//            temp.latitude = lat;
//            temp.longitude = lon;
//            LocationDB.Update(temp, locationTable);
//        }


//    }
//}


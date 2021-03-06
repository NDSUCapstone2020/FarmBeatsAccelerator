﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FarmBeatsAccelerator.Classes;
using FarmBeatsAccelerator.Services;
using FarmBeatsAccelerator.Entities;
namespace FarmBeatsAccelerator.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        // GET: Sensor
        [HttpGet]
        public List<Sensor> Get()
        {
            
            SensorServices.GetSensors("a");
            return new List<Sensor>();
        }

        // GET: Sensor/5
        [HttpGet("{ID}", Name = "Get")]
        public List<Sensor> GetSensor(string ID)
        {
            return new List<Sensor>();
        }
        // POST: Sensor/1.0,1.0,26
        [HttpPost]
        public void Post([FromBody] SensorLocationEntity location)
        {
            
        }
        [Route("Sensor/put")]
        // PUT: Sensor/1.0,1.0,27
        [HttpPut("{lat,lon,id}", Name = "PUT")]
        public void Put(double lat, double lon, string id, [FromBody] string value)
        {
            //SensorServices.updateLocation(lat,lon,id);
        }

        // DELETE: SensorController/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

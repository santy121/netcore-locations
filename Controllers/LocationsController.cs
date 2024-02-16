using LocationSearch.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace LocationSearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationsController : Controller
    {
        private readonly List<Location> locations;

        public LocationsController()
        {
            // Read locations from the JSON file
            string json = System.IO.File.ReadAllText("locations.json");
            locations = JsonConvert.DeserializeObject<List<Location>>(json);
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Location> availableLocations = new List<Location>();
            foreach (var location in locations)
            {
                foreach (var times in location.Availability)
                {
                    foreach (var timeSlot in times.Value)
                    {
                        string startTime = timeSlot.Split('-')[0].Trim();
                        string endTime = timeSlot.Split('-')[1].Trim();

                        if (IsTimeInRange(startTime, endTime))
                        {
                            availableLocations.Add(location);
                            break; // No need to check other time slots for this location
                        }
                    }
                }
            }

            if (availableLocations.Count == 0)
            {
                return NotFound("No locations are available between 10 am and 1 pm.");
            }

            return Ok(availableLocations);
        }
        private bool IsTimeInRange(string startTimeStr, string endTimeStr)
        {
            // Parsing start time and end time
            TimeSpan startTime, endTime;
            if (!TimeSpan.TryParse(startTimeStr.Replace("AM", "").Replace("PM", "").Trim(), out startTime) ||
                !TimeSpan.TryParse(endTimeStr.Replace("AM", "").Replace("PM", "").Trim(), out endTime))
            {
                return false;
            }

            // Defining time range from 10 am to 1 pm
            TimeSpan rangeStartTime = new TimeSpan(10, 0, 0);
            TimeSpan rangeEndTime = new TimeSpan(13, 0, 0);

            // Checking if the time range overlaps with the desired time range
            if ((startTime <= rangeEndTime && endTime >= rangeStartTime))
            {
                return true;
            }

            return false;
        }
    }
}

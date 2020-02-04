using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using System.Linq;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DevicesController> _logger;
        private readonly DevicesService _devService;

        public DevicesController(ILogger<DevicesController> logger, DevicesService devService)
        {
            _logger = logger;
            _devService = devService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string>() { "testdevice1", "testdevice2" };
        }

        [HttpGet]
        [Route("{id}")]
        async public Task<List<SensorValue>> GetDevice(string id)
        {
            return (await _devService.GetAllValidData()).Where(x => x.DeviceId == id).ToList();
            // return _devService.GetDeviceHistory(id);
        }
    }
}

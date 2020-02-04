using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public Task<IEnumerable<string>> Get()
        {
            return await _devService.GetDeviceHistory();
            return new List<string>() { "testdevice1", "testdevice2" };
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<List<SensorValue>> GetDevice(string id)
        {
            return await _devService.GetDeviceHistory(id);
        }
    }
}

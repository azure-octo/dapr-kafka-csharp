using System;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Dapr.Examples.Pubsub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {  
        private readonly ILogger<ConsumerController> _logger;

        public ConsumerController(
            ILogger<ConsumerController> logger)
        {
            _logger = logger;
        }
        

        [Topic("pubsub", "secondsampletopic")]
        [HttpPost("/orders")]
        public async Task<ActionResult> ReceiveMessage([FromBody]string message)
        {
            Console.WriteLine($"Message received through controller: {message}");

            return Ok();
        }
    }
}
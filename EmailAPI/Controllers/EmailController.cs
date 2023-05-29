using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EmailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private static readonly Queue<EmailRequest> RequestQueue = new Queue<EmailRequest>();
        private static readonly object QueueLock = new object();
        private static readonly TimeSpan RequestWindow = TimeSpan.FromSeconds(3);

        [HttpPost]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            lock (QueueLock)
            {
                // Remove expired requests from the queue
                while (RequestQueue.Count > 0 && RequestQueue.Peek().RequestTime < DateTime.Now - RequestWindow)
                {
                    RequestQueue.Dequeue();
                }

                // If there are too many recent requests, return the latest valid request
                if (RequestQueue.Count >= 1)
                {
                    var latestRequest = RequestQueue.Peek();
                    return StatusCode(429, new { email = latestRequest.Email, serverTime = latestRequest.RequestTime });
                }

                RequestQueue.Enqueue(new EmailRequest { Email = request.Email, RequestTime = DateTime.Now });
            }

            // Simulate processing delay
            Thread.Sleep(2000);

            return Ok(new { email = request.Email, serverTime = DateTime.Now });
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
        public DateTime RequestTime { get; set; }
    }
}

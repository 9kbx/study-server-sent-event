using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace server_sent_event.Controllers
{
    [Route("api/sse")]
    [ApiController]
    public class ServerSentEventController : ControllerBase
    {
        [HttpGet]
        public async Task Get()
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");

            for (var i = 0; true; ++i)
            {
                //await response.WriteAsync($"data: Controller {i} at {DateTime.Now}\n\n");
                await response.WriteAsync($"event:hello\ndata: Controller {i} at {DateTime.Now}\n\n");

                response.Body.Flush();
                await Task.Delay(30 * 1000);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using server_sent_event.db;
using System.Text;

namespace server_sent_event.Controllers
{
    public class ChatController : Controller
    {
        MemDbContext _db;
        public ChatController(MemDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string nick)
        {
            if (!string.IsNullOrWhiteSpace(nick))
            {
                NickName = nick;
                return RedirectToAction("Chat");
            }
            return RedirectToAction("Index");
        }

        private string _SessionKey => "nick";
        public string NickName {
            set
            {
                Request.HttpContext.Session.Set(_SessionKey, Encoding.UTF8.GetBytes(value));
            }
            get
            {
                if (Request.HttpContext.Session.Keys.Contains(_SessionKey))
                {
                    return Request.HttpContext.Session.GetString(_SessionKey);
                }
                return string.Empty;
            }
        }

        public IActionResult Chat()
        {
            if (!string.IsNullOrWhiteSpace(NickName))
            {
                ViewBag.NickName = NickName;
                return View();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SendMsg(string msg)
        {
            _db.ChatMsgs.Add(new ChatMsg()
            {
                Name = NickName,
                Msg = msg,
                CreateTime = DateTime.Now
            });
            await _db.SaveChangesAsync();
            //return Json("success");
            return Ok();
        }

        [HttpGet]
        public async Task GetMsg()
        {
            var response = Response;
            long lastTick = 0;
            var name = NickName;

            if (string.IsNullOrWhiteSpace(name))
            {
                response.Body.Flush();
                return;
            }

            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");

            //for (var i = 0; true; ++i)
            //{
            //    //await response.WriteAsync($"data: Controller {i} at {DateTime.Now}\n\n");
            //    await response.WriteAsync($"event:hello\ndata: Controller {i} at {DateTime.Now}\n\n");

            //    response.Body.Flush();
            //    await Task.Delay(30 * 1000);
            //}

            while (true)
            {
                var msgList = _db.ChatMsgs.Where(a => a.Name != name && a.CreateTime.Ticks >= lastTick).ToList();

                if (msgList != null && msgList.Count > 0) lastTick = DateTime.Now.Ticks;

                foreach (var item in msgList)
                {
                    await response.WriteAsync($"event:newmessage\ndata: {item.Name}:{item.Msg}\n\n");
                    response.Body.Flush();
                }

                await Task.Delay(3 * 1000);
            }
            
        }
    }
}

using CollegeApp.MyLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        //private readonly IMyLogger _myLogger;
        private readonly ILogger<DemoController> _myLogger;

        public DemoController(ILogger<DemoController> myLogger)
        {
            _myLogger = myLogger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            // _myLogger.Log("demo message");
            _myLogger.LogTrace("Log message from Trace");
            _myLogger.LogDebug("Log message from Debug");
            _myLogger.LogInformation("Log message from Information");
            _myLogger.LogWarning("Log message from Warning");
            _myLogger.LogError("Log message from Error");
            _myLogger.LogCritical("Log message from Critical");
            return Ok();
        }
    }
}

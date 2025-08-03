using Microsoft.AspNetCore.Mvc;
using SharpHook.Data;
using StreamController.Core;

namespace StreamOverlay.Controllers;

public class ObsController(IObsClient obs) : Controller
{
    [HttpPost]
    public IActionResult ActivateScene([FromQuery] string sceneName)
    {
        var result = obs.ActivateScene(sceneName);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

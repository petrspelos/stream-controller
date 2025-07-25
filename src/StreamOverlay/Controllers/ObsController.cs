using Microsoft.AspNetCore.Mvc;
using SharpHook.Data;

namespace StreamOverlay.Controllers;

public class ObsController(IObsService obs) : Controller
{
    [HttpPost]
    public IActionResult ActivateScene([FromQuery] string sceneName)
    {
        var result = obs.ActivateScene(sceneName);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

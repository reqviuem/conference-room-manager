using Microsoft.AspNetCore.Mvc;
using RoomManager.Dtos.Requests;

namespace RoomManager.Controllers;

[ApiController]
public class RoomController : ControllerBase
{
    
    public RoomController()
    {
        
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom(RoomCreateRequestDto roomCreateRequestDto)
    {
        return Ok();
    }
}
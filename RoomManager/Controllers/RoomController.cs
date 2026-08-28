using Microsoft.AspNetCore.Mvc;
using RoomManager.Dtos.Requests;
using RoomManager.Services;

namespace RoomManager.Controllers;

[ApiController]
public class RoomController : ControllerBase
{
    private readonly IMainService _mainService;

    public RoomController(IMainService mainService)
    {
        _mainService = mainService;
    }

    [HttpPost]
    [Route("/create")]
    public async Task<IActionResult> CreateRoom(RoomCreateRequestDto roomCreateRequestDto)
    {
        var room = await _mainService.CreateRoom(roomCreateRequestDto);
        return Ok(room);
    }
}
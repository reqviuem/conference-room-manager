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

    [HttpPatch]
    [Route("/update")]
    public async Task<IActionResult> UpdateRoom(Guid requestedRoomId, RoomUpdateRequestDto roomUpdateRequestDto)
    {
        var updatedRoom = await _mainService.UpdateRoom(requestedRoomId,roomUpdateRequestDto);

        return Ok(updatedRoom);
    }

    [HttpDelete]
    [Route("/delete")]
    public async Task<IActionResult> DeleteRoom(Guid roomId)
    {
        return Ok(await _mainService.Delete(roomId));
    }

    [HttpGet]
    [Route("/rooms")]
    public async Task<IActionResult> FindAvailableRoom([FromQuery]AvailableRoomsRequestDto request)
    {
        var availableRooms = await _mainService.FindAvailableRooms(request);

        return Ok(availableRooms);
    }

    [HttpPost]
    [Route("/book")]
    public async Task<IActionResult> BookARoom(BookingCreateRequestDto request)
    {
        var booking = await _mainService.BookRoom(request);

        return Ok(booking);
    }
}
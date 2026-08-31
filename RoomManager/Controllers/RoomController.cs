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

        if (room.ErrorCode != null)
        {
            return BadRequest(room);
        }

        return Ok(room);
    }

    [HttpPatch]
    [Route("/update")]
    public async Task<IActionResult> UpdateRoom(Guid requestedRoomId, RoomUpdateRequestDto roomUpdateRequestDto)
    {
        var updatedRoom = await _mainService.UpdateRoom(requestedRoomId,roomUpdateRequestDto);

        if (updatedRoom.ErrorCode != null)
        {
            return BadRequest(updatedRoom);
        }

        return Ok(updatedRoom);
    }

    [HttpDelete]
    [Route("/delete")]
    public async Task<IActionResult> DeleteRoom(Guid roomId)
    {
        var deletedRoom = await _mainService.Delete(roomId);
        
        if (deletedRoom.ErrorCode != null)
        {
            return BadRequest(deletedRoom);
        }
        return Ok(deletedRoom);
    }

    [HttpGet]
    [Route("/rooms")]
    public async Task<IActionResult> FindAvailableRoom([FromQuery]AvailableRoomsRequestDto request)
    {
        var availableRooms = await _mainService.FindAvailableRooms(request);

        if (availableRooms.ErrorCode != null)
        {
            return BadRequest(availableRooms);
        }
        
        return Ok(availableRooms);
    }

    [HttpPost]
    [Route("/book")]
    public async Task<IActionResult> BookARoom(BookingCreateRequestDto request)
    {
        var booking = await _mainService.BookRoom(request);

        if (booking.ErrorCode != null)
        {
            return BadRequest(booking);
        }
        
        return Ok(booking);
    }
}
using RoomManager.Data;
using RoomManager.Dtos.Requests;
using RoomManager.Models;

namespace RoomManager.Services;

public class RoomService
{
    private readonly AppDbContext _appDbContext;
    
    public RoomService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    // public async Task<string> CreateRoom(RoomCreateRequestDto roomCreateRequestDto)
    // {
    //     // var newRoom = new Room()
    //     // {
    //     //     Capacity = roomCreateRequestDto.Capacity,
    //     //     Name = roomCreateRequestDto.Name,
    //     //     PricePerHour = roomCreateRequestDto.BasePricePerHour,
    //     //     Service = roomCreateRequestDto.
    //     // }
    //     
    // }
}
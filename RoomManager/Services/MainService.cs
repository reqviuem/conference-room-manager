using Microsoft.EntityFrameworkCore;
using RoomManager.Data;
using RoomManager.Dtos.Requests;
using RoomManager.Dtos.Responses;
using RoomManager.Models;

namespace RoomManager.Services;

public class MainService : IMainService
{
    private readonly AppDbContext _appDbContext;

    public MainService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public async Task<RoomCreateResponseDto> CreateRoom(RoomCreateRequestDto roomCreateRequestDto)
    {
        
        
        var requestedServiceNames = roomCreateRequestDto.Services.Distinct().ToList();
        
        var services = await _appDbContext.Services
            .Where(service => requestedServiceNames.Contains(service.Name))
            .ToListAsync();
        
        if (services.Count != requestedServiceNames.Count)
        {
            throw new Exception("One or more services are invalid.");
        }

        var newRoom = new Room()
        {
            Capacity = roomCreateRequestDto.Capacity,
            Name = roomCreateRequestDto.Name,
            PricePerHour = roomCreateRequestDto.BasePricePerHour,
            RoomServices = services
                .Select(service => new RoomService()
                {
                    ServiceId = service.Id
                }).ToList()
        };

        await _appDbContext.Rooms.AddAsync(newRoom);

        await _appDbContext.SaveChangesAsync();

        var createResponseDto = new RoomCreateResponseDto
        {
            Id = newRoom.Id,
            BasePricePerHour = newRoom.PricePerHour,
            Services = services.Select(service => service.Name).ToList(),
            Name = newRoom.Name,
            Capacity = newRoom.Capacity
        };
        return createResponseDto;
    }
}
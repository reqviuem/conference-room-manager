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
        // if (roomCreateRequestDto.Capacity <= 0)
        //     throw new Exception("Capacity must be greater than zero.");

        // if (roomCreateRequestDto.BasePricePerHour <= 0)
        //     throw new Exception("Base price must be greater than zero.");

        var requestedServiceNames = roomCreateRequestDto.Services.Distinct().ToList();

        var services = await _appDbContext.Services
            .Where(service => requestedServiceNames.Contains(service.Name))
            .ToListAsync();

        // if (services.Count != requestedServiceNames.Count)
        // {
        //     throw new Exception("One or more services are invalid.");
        // }

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

    public async Task<RoomUpdateResponseDto> UpdateRoom(RoomUpdateRequestDto roomUpdateRequestDto)
    {
        var roomToUpdate = await _appDbContext.Rooms.FirstOrDefaultAsync(room => room.Id == roomUpdateRequestDto.Id);

        var serviceToAdd = roomUpdateRequestDto.Services.Distinct().ToList();

        var services = await _appDbContext.Services
            .Where(service => serviceToAdd.Contains(service.Name))
            .ToListAsync();

        if (roomToUpdate != null)
        {
            roomToUpdate.PricePerHour = roomUpdateRequestDto.BasePricePerHour;
            roomToUpdate.RoomServices = services
                .Select(service => new RoomService()
                {
                    ServiceId = service.Id
                }).ToList();
            
            return new RoomUpdateResponseDto()
            {
                Id = roomToUpdate.Id,
                BasePricePerHour = roomToUpdate.PricePerHour,
                Services = services.Select(service => service.Name).ToList(),
                Name = roomToUpdate.Name,
                Capacity = roomToUpdate.Capacity
            };
        }

        return null;
    }
}
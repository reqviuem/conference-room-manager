using System.Globalization;
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

    public async Task<RoomUpdateResponseDto> UpdateRoom(Guid requestedRoomId, RoomUpdateRequestDto roomUpdateRequestDto)
    {
        var roomToUpdate = await _appDbContext.Rooms.Include(room => room.RoomServices)
            .ThenInclude(roomService => roomService.Service)
            .FirstOrDefaultAsync(room => room.Id == requestedRoomId);

        if (roomToUpdate is null)
        {
            return null;
        }

        var servicesToAdd = roomUpdateRequestDto.Services.Distinct().ToList();

        var services = await _appDbContext.Services
            .Where(service => servicesToAdd.Contains(service.Name))
            .ToListAsync();

        //Using HashSet instead of list for fast lookup by value
        var existingServiceIds = roomToUpdate.RoomServices.Select(roomService => roomService.ServiceId)
            .ToHashSet();

        var roomServicesToAdd = services
            .Where(service => !existingServiceIds.Contains(service.Id))
            .Select(service => new RoomService
            {
                RoomId = roomToUpdate.Id,
                ServiceId = service.Id,
                Service = service
            }).ToList();
            
        roomToUpdate.RoomServices.AddRange(roomServicesToAdd);
        roomToUpdate.PricePerHour = roomUpdateRequestDto.BasePricePerHour;

        await _appDbContext.SaveChangesAsync();
        
        return new RoomUpdateResponseDto()
        {
            Id = roomToUpdate.Id,
            BasePricePerHour = roomToUpdate.PricePerHour,
            Capacity = roomToUpdate.Capacity,
            Name = roomToUpdate.Name,
            Services = roomToUpdate.RoomServices.Select(service => service.Service.Name).ToList()
        };

        
    }
    public async Task<string> Delete(Guid id)
    {
        var roomToDelete = await _appDbContext.Rooms.Include(room => room.RoomServices)
            .FirstOrDefaultAsync(room => id == room.Id);

        if (roomToDelete is null)
        {
            return null;
        }
        
        _appDbContext.Rooms.Remove(roomToDelete);

        await _appDbContext.SaveChangesAsync();

        return $"Room with {id} successfully deleted";

    }

    public async Task<IEnumerable<AvailableRoomsResponseDto>> FindAvailableRooms(AvailableRoomsRequestDto requestedRoom)
    {
        var startAt = StartAt(requestedRoom);
        var endAt = EndAt(requestedRoom);
        
        var rooms = await _appDbContext.Rooms
            .Where(room => room.Capacity >= requestedRoom.Capacity)
            .Where(room => !_appDbContext.Bookings
                .Any(booking => booking.RoomId == room.Id && booking.StartAt < endAt && booking.EndAt > startAt))
            .Select(room => new AvailableRoomsResponseDto()
            {
                Name = room.Name,
                BasePricePerHour = room.PricePerHour,
                Capacity = room.Capacity,
                Services = room.RoomServices.Select(service => service.Service.Name).ToList()
            }).ToListAsync();

        return rooms;
    }

    private DateTime StartAt(AvailableRoomsRequestDto requestedRoom)
    {
        var date = DateOnly.ParseExact(
            requestedRoom.Date,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture);

        var from = TimeOnly.ParseExact(
            requestedRoom.From,
            "HH:mm",
            CultureInfo.InvariantCulture);

        var local = date.ToDateTime(from);
        var startAt = local.ToUniversalTime();
        return startAt;
    } 
    
    
    private DateTime EndAt(AvailableRoomsRequestDto requestedRoom)
    {
        var date = DateOnly.ParseExact(
            requestedRoom.Date,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture);

        var until = TimeOnly.ParseExact(
            requestedRoom.Until,
            "HH:mm",
            CultureInfo.InvariantCulture);

        var  local = date.ToDateTime(until);
        var endAt = local.ToUniversalTime();
        return endAt;
    } 
}
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
        DateSerializer dateSerializer = new DateSerializer();
        
        var startAt = dateSerializer.StartAt(requestedRoom.Date,requestedRoom.From);
        var endAt = dateSerializer.EndAt(requestedRoom.Date,requestedRoom.From);

        var rooms = await _appDbContext.Rooms
            .Where(room => room.Capacity >= requestedRoom.Capacity)
            .Where(room => !_appDbContext.Bookings
                // Excluding rooms that already have a booking overlapping the requested time range.
                .Any(booking => booking.RoomId == room.Id
                                && booking.StartAt < endAt
                                && booking.EndAt > startAt))
            .Select(room => new AvailableRoomsResponseDto()
            {
                Name = room.Name,
                BasePricePerHour = room.PricePerHour,
                Capacity = room.Capacity,
                Services = room.RoomServices.Select(service => service.Service.Name).ToList()
            }).ToListAsync();

        return rooms;
    }

    public async Task<string> BookRoom(BookingCreateRequestDto request)
    {
        DateSerializer dateSerializer = new DateSerializer();

        BookingPriceCalculator bookingPriceCalculator = new BookingPriceCalculator();
        
        var startAt = dateSerializer.StartAt(request.Date,request.StartTime);
        
        var endAt = dateSerializer.BuildEndAtFromDuration(startAt,request.DurationHours);
        
        var roomToBook = await _appDbContext.Rooms
            .Include(room => room.RoomServices)
            .ThenInclude(roomService => roomService.Service)
            .Where(room => room.Id == request.RoomId)
            .Where(room => !_appDbContext.Bookings.Any(booking =>
                booking.RoomId == room.Id
                && booking.StartAt < endAt
                && booking.EndAt > startAt))
            .FirstOrDefaultAsync();
        
        if (roomToBook is null)
        {
            return null;
        }
        
        var servicesToAdd = request.Services.Distinct().ToList();
        
        var roomServiceNames = roomToBook.RoomServices
            .Select(roomService => roomService.Service.Name)
            .ToHashSet();

        var invalidServices = servicesToAdd
            .Where(serviceName => !roomServiceNames.Contains(serviceName))
            .ToList();

        if (invalidServices.Any())
        {
            return null;
        }
        
        
        var selectedService = await _appDbContext.Services.Where(service => servicesToAdd.Contains(service.Name)).ToListAsync();

        var roomPrice = bookingPriceCalculator.CalculateRoomPrice(
            roomToBook.PricePerHour,
            startAt,
            endAt);

        var servicesPrice = selectedService.Sum(service => service.Price);

        var totalPrice = roomPrice + servicesPrice;
        
        var newBooking = new Booking()
        {
            RoomId = roomToBook.Id,
            BookingServices = selectedService.Select(service => new BookingService()
            {
                ServiceId = service.Id,
                PriceAtBooking = service.Price
            }).ToList(),
            StartAt = startAt,
            EndAt = endAt,
            TotalPrice = totalPrice
        };

        await _appDbContext.AddAsync(newBooking);

        await _appDbContext.SaveChangesAsync();

        return $"Booking successfully created with the total price of {newBooking.TotalPrice}";
    }
    
}
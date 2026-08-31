using Microsoft.EntityFrameworkCore;
using RoomManager.Data;
using RoomManager.Dtos.Requests;
using RoomManager.Dtos.Responses;
using RoomManager.Models;

namespace RoomManager.Services;

public class MainService : IMainService
{
    private readonly AppDbContext _appDbContext;
    private readonly DateSerializer _dateSerializer;
    private readonly BookingPriceCalculator _bookingPriceCalculator;

    public MainService(AppDbContext appDbContext, DateSerializer dateSerializer, BookingPriceCalculator bookingPriceCalculator)
    {
        _appDbContext = appDbContext;
        _dateSerializer = dateSerializer;
        _bookingPriceCalculator = bookingPriceCalculator;
    }

    public async Task<ServiceResult<RoomCreateResponseDto>> CreateRoom(RoomCreateRequestDto roomCreateRequestDto)
    {
        if (string.IsNullOrWhiteSpace(roomCreateRequestDto.Name))
        {
            return ServiceResult<RoomCreateResponseDto>.Fail(
                "ValidationFailed",
                "Room name is required.");
        }

        if (roomCreateRequestDto.Capacity <= 0)
        {
            return ServiceResult<RoomCreateResponseDto>.Fail(
                "ValidationFailed",
                "Capacity must be greater than zero.");
        }

        if (roomCreateRequestDto.BasePricePerHour <= 0)
        {
            return ServiceResult<RoomCreateResponseDto>.Fail(
                "ValidationFailed",
                "Base price per hour must be greater than zero.");
        }

        if (roomCreateRequestDto.Services.Count == 0)
        {
            return ServiceResult<RoomCreateResponseDto>.Fail(
                "ValidationFailed",
                "At least one service is required.");
        }

        var requestedServiceNames = roomCreateRequestDto.Services
            .Where(serviceName => !string.IsNullOrWhiteSpace(serviceName))
            .Select(serviceName => serviceName.Trim())
            .Distinct()
            .ToList();

        if (requestedServiceNames.Count == 0)
        {
            return ServiceResult<RoomCreateResponseDto>.Fail(
                "ValidationFailed",
                "At least one valid service name is required.");
        }

        var services = await _appDbContext.Services
            .Where(service => requestedServiceNames.Contains(service.Name))
            .ToListAsync();

        if (services.Count != requestedServiceNames.Count)
        {
            return ServiceResult<RoomCreateResponseDto>.Fail(
                "InvalidServices",
                "One or more selected services do not exist.");
        }

        var newRoom = new Room
        {
            Capacity = roomCreateRequestDto.Capacity,
            Name = roomCreateRequestDto.Name.Trim(),
            PricePerHour = roomCreateRequestDto.BasePricePerHour,
            RoomServices = services
                .Select(service => new RoomService
                {
                    ServiceId = service.Id
                })
                .ToList()
        };

        await _appDbContext.Rooms.AddAsync(newRoom);
        await _appDbContext.SaveChangesAsync();

        var response = new RoomCreateResponseDto
        {
            Id = newRoom.Id,
            BasePricePerHour = newRoom.PricePerHour,
            Services = services.Select(service => service.Name).ToList(),
            Name = newRoom.Name,
            Capacity = newRoom.Capacity
        };

        return ServiceResult<RoomCreateResponseDto>.Ok(
            response,
            "Room was successfully created.");
    }

    public async Task<ServiceResult<RoomUpdateResponseDto>> UpdateRoom(
        Guid requestedRoomId,
        RoomUpdateRequestDto roomUpdateRequestDto)
    {
        if (requestedRoomId == Guid.Empty)
        {
            return ServiceResult<RoomUpdateResponseDto>.Fail(
                "ValidationFailed",
                "Room id is required.");
        }

        if (roomUpdateRequestDto.BasePricePerHour <= 0)
        {
            return ServiceResult<RoomUpdateResponseDto>.Fail(
                "ValidationFailed",
                "Base price per hour must be greater than zero.");
        }

        if (roomUpdateRequestDto.Services.Count == 0)
        {
            return ServiceResult<RoomUpdateResponseDto>.Fail(
                "ValidationFailed",
                "Services list is required.");
        }

        var roomToUpdate = await _appDbContext.Rooms
            .Include(room => room.RoomServices)
            .ThenInclude(roomService => roomService.Service)
            .FirstOrDefaultAsync(room => room.Id == requestedRoomId);

        if (roomToUpdate is null)
        {
            return ServiceResult<RoomUpdateResponseDto>.Fail(
                "RoomNotFound",
                "Room was not found.");
        }

        var servicesToAdd = roomUpdateRequestDto.Services
            .Where(serviceName => !string.IsNullOrWhiteSpace(serviceName))
            .Select(serviceName => serviceName.Trim())
            .Distinct()
            .ToList();

        var services = await _appDbContext.Services
            .Where(service => servicesToAdd.Contains(service.Name))
            .ToListAsync();

        if (services.Count != servicesToAdd.Count)
        {
            return ServiceResult<RoomUpdateResponseDto>.Fail(
                "InvalidServices",
                "One or more selected services do not exist.");
        }
        
        // HashSet for O(1) contains lookups when checking for existing services
        var existingServiceIds = roomToUpdate.RoomServices
            .Select(roomService => roomService.ServiceId)
            .ToHashSet();

        var roomServicesToAdd = services
            .Where(service => !existingServiceIds.Contains(service.Id))
            .Select(service => new RoomService
            {
                RoomId = roomToUpdate.Id,
                ServiceId = service.Id,
                Service = service
            })
            .ToList();

        roomToUpdate.RoomServices.AddRange(roomServicesToAdd);
        roomToUpdate.PricePerHour = roomUpdateRequestDto.BasePricePerHour;

        await _appDbContext.SaveChangesAsync();

        var response = new RoomUpdateResponseDto
        {
            Id = roomToUpdate.Id,
            BasePricePerHour = roomToUpdate.PricePerHour,
            Capacity = roomToUpdate.Capacity,
            Name = roomToUpdate.Name,
            Services = roomToUpdate.RoomServices
                .Select(roomService => roomService.Service.Name)
                .ToList()
        };

        return ServiceResult<RoomUpdateResponseDto>.Ok(
            response,
            "Room was successfully updated.");
    }

    public async Task<ServiceResult<string>> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult<string>.Fail(
                "ValidationFailed",
                "Room id is required.");
        }

        var roomToDelete = await _appDbContext.Rooms
            .Include(room => room.RoomServices)
            .FirstOrDefaultAsync(room => id == room.Id);

        if (roomToDelete is null)
        {
            return ServiceResult<string>.Fail(
                "RoomNotFound",
                "Room was not found.");
        }

        _appDbContext.Rooms.Remove(roomToDelete);
        await _appDbContext.SaveChangesAsync();

        return ServiceResult<string>.Ok(
            $"Room with {id} successfully deleted",
            "Room was successfully deleted.");
    }

    public async Task<ServiceResult<IEnumerable<AvailableRoomsResponseDto>>> FindAvailableRooms(
        AvailableRoomsRequestDto requestedRoom)
    {
        if (requestedRoom.Capacity <= 0)
        {
            return ServiceResult<IEnumerable<AvailableRoomsResponseDto>>.Fail(
                "ValidationFailed",
                "Capacity must be greater than zero.");
        }

        if (_dateSerializer.CheckPastDate(requestedRoom.Date))
        {
            return ServiceResult<IEnumerable<AvailableRoomsResponseDto>>.Fail(
                "ValidationFailed",
                "Date can not be in past.");
        }
        
        DateTime startAt;
        DateTime endAt;

        try
        {
            startAt = _dateSerializer.StartAt(requestedRoom.Date, requestedRoom.From);
            endAt = _dateSerializer.EndAt(requestedRoom.Date, requestedRoom.Until);
        }
        catch (FormatException)
        {
            return ServiceResult<IEnumerable<AvailableRoomsResponseDto>>.Fail(
                "InvalidDateTime",
                "Date must be in dd.MM.yyyy format and time must be in HH:mm format.");
        }

        if (endAt <= startAt)
        {
            return ServiceResult<IEnumerable<AvailableRoomsResponseDto>>.Fail(
                "ValidationFailed",
                "Until time must be later than from time.");
        }

        
        var rooms = await _appDbContext.Rooms
            .Where(room => room.Capacity >= requestedRoom.Capacity)
            // Exclude rooms with a booking that overlaps the requested [startAt, endAt) window
            .Where(room => !_appDbContext.Bookings
                .Any(booking => booking.RoomId == room.Id
                                && booking.StartAt < endAt
                                && booking.EndAt > startAt))
            .Select(room => new AvailableRoomsResponseDto
            {
                Name = room.Name,
                BasePricePerHour = room.PricePerHour,
                Capacity = room.Capacity,
                Services = room.RoomServices
                    .Select(roomService => roomService.Service.Name)
                    .ToList()
            })
            .ToListAsync();

        return ServiceResult<IEnumerable<AvailableRoomsResponseDto>>.Ok(
            rooms,
            "Available rooms were successfully loaded.");
    }

    public async Task<ServiceResult<BookingCreateResponseDto>> BookRoom(BookingCreateRequestDto request)
    {
        if (request.RoomId == Guid.Empty)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "ValidationFailed",
                "Room id is required.");
        }

        if (request.DurationHours <= 0)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "ValidationFailed",
                "Duration must be greater than zero.");
        }

        if (request.Services.Count == 0)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "ValidationFailed",
                "Services list is required.");
        }

        try
        {
            if (_dateSerializer.CheckPastDate(request.Date))
            {
                return ServiceResult<BookingCreateResponseDto>.Fail(
                    "ValidationFailed",
                    "Date can not be in past.");
            }
        }
        catch (FormatException)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "InvalidDateTime",
                "Date must be in dd.MM.yyyy format and start time must be in HH:mm format.");
        }
        
        

        DateTime startAt;

        try
        {
            startAt = _dateSerializer.StartAt(request.Date, request.StartTime);
        }
        catch (FormatException)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "InvalidDateTime",
                "Date must be in dd.MM.yyyy format and start time must be in HH:mm format.");
        }
        

        var endAt = _dateSerializer.BuildEndAtFromDuration(startAt, request.DurationHours);

        var roomToBook = await _appDbContext.Rooms
            .Include(room => room.RoomServices)
            .ThenInclude(roomService => roomService.Service)
            .FirstOrDefaultAsync(room => room.Id == request.RoomId);

        if (roomToBook is null)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "RoomNotFound",
                "Room was not found.");
        }

        var hasOverlappingBooking = await _appDbContext.Bookings.AnyAsync(booking =>
            booking.RoomId == roomToBook.Id
            && booking.StartAt < endAt
            && booking.EndAt > startAt);

        if (hasOverlappingBooking)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "RoomUnavailable",
                "Room is not available for the requested time range.");
        }

        var selectedServices = request.Services
            .Where(serviceName => !string.IsNullOrWhiteSpace(serviceName))
            .Select(serviceName => serviceName.Trim())
            .Distinct()
            .ToList();

        var roomServiceNames = roomToBook.RoomServices
            .Select(roomService => roomService.Service.Name)
            .ToHashSet();

        var invalidServices = selectedServices
            .Where(serviceName => !roomServiceNames.Contains(serviceName))
            .ToList();

        if (invalidServices.Any())
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "InvalidServices",
                "One or more selected services are not available in this room.");
        }

        var servicesToAdd = await _appDbContext.Services
            .Where(service => selectedServices.Contains(service.Name))
            .ToListAsync();

        if (servicesToAdd.Count != selectedServices.Count)
        {
            return ServiceResult<BookingCreateResponseDto>.Fail(
                "InvalidServices",
                "One or more selected services do not exist.");
        }

        var roomPrice = _bookingPriceCalculator.CalculateRoomPrice(
            roomToBook.PricePerHour,
            startAt,
            endAt);

        var servicesPrice = servicesToAdd.Sum(service => service.Price);
        var totalPrice = roomPrice + servicesPrice;

        var newBooking = new Booking
        {
            RoomId = roomToBook.Id,
            BookingServices = servicesToAdd
                .Select(service => new BookingService
                {
                    ServiceId = service.Id,
                    PriceAtBooking = service.Price
                })
                .ToList(),
            StartAt = startAt,
            EndAt = endAt,
            TotalPrice = totalPrice
        };

        await _appDbContext.AddAsync(newBooking);
        await _appDbContext.SaveChangesAsync();

        var response = new BookingCreateResponseDto
        {
            Id = newBooking.Id,
            RoomId = newBooking.RoomId,
            StartAt = newBooking.StartAt,
            EndAt = newBooking.EndAt,
            RoomPrice = roomPrice,
            ServicesPrice = servicesPrice,
            TotalPrice = newBooking.TotalPrice,
            Services = servicesToAdd
                .Select(service => service.Name)
                .ToList()
        };

        return ServiceResult<BookingCreateResponseDto>.Ok(
            response,
            "Booking was successfully created.");
    }
}
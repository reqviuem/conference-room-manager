using RoomManager.Dtos.Requests;
using RoomManager.Dtos.Responses;

namespace RoomManager.Services;

public interface IMainService
{
    Task<ServiceResult<RoomCreateResponseDto>> CreateRoom(RoomCreateRequestDto roomCreateRequestDto);
    Task<ServiceResult<RoomUpdateResponseDto>> UpdateRoom(Guid requestedRoomId, RoomUpdateRequestDto roomUpdateRequestDto);

    Task<ServiceResult<string>> Delete(Guid id);

    Task<ServiceResult<IEnumerable<AvailableRoomsResponseDto>>> FindAvailableRooms(AvailableRoomsRequestDto requestedRoom);

    Task<ServiceResult<BookingCreateResponseDto>> BookRoom(BookingCreateRequestDto request);
}

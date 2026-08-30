using RoomManager.Dtos.Requests;
using RoomManager.Dtos.Responses;

namespace RoomManager.Services;

public interface IMainService
{
    Task<RoomCreateResponseDto> CreateRoom(RoomCreateRequestDto roomCreateRequestDto);
    Task<RoomUpdateResponseDto> UpdateRoom(Guid requestedRoomId,RoomUpdateRequestDto roomUpdateRequestDto);

    Task<string> Delete(Guid id);

    Task<IEnumerable<AvailableRoomsResponseDto>> FindAvailableRooms(AvailableRoomsRequestDto requestedRoom);
}
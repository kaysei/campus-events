namespace CampusEvents.Backend;

public interface IEventRepository
{
    int GetMaxSeats(int eventId);
    int GetConfirmedCount(int eventId);
}

public class SeatAvailabilityService
{
    private readonly IEventRepository _repository;

    public SeatAvailabilityService(IEventRepository repository)
    {
        _repository = repository;
    }

    public bool HasSeat(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        return _repository.GetConfirmedCount(eventId) < _repository.GetMaxSeats(eventId);
    }
}

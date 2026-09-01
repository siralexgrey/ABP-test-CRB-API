namespace ConferenceRooms.Api.Exceptions;

public class OverlapException : Exception
{
    public OverlapException(string message) : base(message)
    {
    }
}
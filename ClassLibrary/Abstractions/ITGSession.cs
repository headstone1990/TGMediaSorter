using TL;

namespace ClassLibrary.Abstractions;

public interface ITGSession
{
    public Task<User> GetCurrentUserAsync();
    Task ForwardAsync(long sourcePeerId, long destinationPeerId, int offset);
}
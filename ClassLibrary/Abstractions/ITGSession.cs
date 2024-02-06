using TL;

namespace ClassLibrary.Abstractions;

public interface ITGSession
{
    public Task<User> GetCurrentUserAsync();
}
using TL;

namespace ClassLibrary.Abstractions;

public interface ITGSession
{
    public Task<User> GetCurrentUserAsync();
    Task<InputPeer> GetPeerAsync(long peerId);
    Task ForwardAsync(InputPeer fromPeer, InputPeer toPeer, int offset);
}
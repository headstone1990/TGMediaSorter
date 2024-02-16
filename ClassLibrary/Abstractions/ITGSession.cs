using TL;

namespace ClassLibrary.Abstractions;

public interface ITGSession
{
    public Task<User> GetCurrentUserAsync();
    Task<InputPeer> GetPeerAsync(long peerId);
    Task<int> GetMessagesCountAsync(InputPeer peer);
    Task<bool> ForwardAsync(InputPeer fromPeer, InputPeer toPeer, int addOffset, int limit);
}
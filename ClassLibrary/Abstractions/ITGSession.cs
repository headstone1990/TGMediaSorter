using TL;

namespace ClassLibrary.Abstractions;

public interface ITGSession
{
    public Task<User> GetCurrentUserAsync();
    Task<InputPeer> GetPeerAsync(long peerId);
    Task<int> GetMessagesCountAsync(InputPeer peer);
    Task ForwardAsync(InputPeer fromPeer, InputPeer toPeer, int offset);
    Task RegisterUpdateHandler(Func<UpdatesBase, Task> handler);
    Task UnregisterUpdateHandler(Func<UpdatesBase, Task> handler);
}
using ClassLibrary.Abstractions;
using ClassLibrary.Forward.Abstractions;

namespace ClassLibrary.Forward;

public class Forwarder : IForwarder
{
    private readonly ITGSession _tgSession;
    public Forwarder(ITGSession tgSession)
    {
        _tgSession = tgSession;
    }

    public async Task Forward(long fromPeerId, long toPeerId)
    {
        const int page = 100;
        var fromPeer = await _tgSession.GetPeerAsync(fromPeerId);
        var toPeer = await _tgSession.GetPeerAsync(toPeerId);
        var totalMessagesCount = await _tgSession.GetMessagesCountAsync(fromPeer);
        var addOffset = totalMessagesCount - page;
        var hasMessagesToForward = true;

        while (hasMessagesToForward)
        {
            hasMessagesToForward = await _tgSession.ForwardAsync(fromPeer, toPeer, addOffset, page);
            addOffset -= page;
        }

    } 
}
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

    public async Task ForwardAsync(long fromPeerId, long toPeerId)
    {
        var fromPeer = await _tgSession.GetPeerAsync(fromPeerId);
        var toPeer = await _tgSession.GetPeerAsync(toPeerId);
        
        await _tgSession.ForwardAsync(fromPeer, toPeer, 1);
    } 
}
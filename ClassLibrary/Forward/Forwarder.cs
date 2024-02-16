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

    public async Task ForwardAsync(long sourcePeerId, long destinationPeerId)
    {
        await _tgSession.ForwardAsync(sourcePeerId, destinationPeerId, 1);
    } 
}
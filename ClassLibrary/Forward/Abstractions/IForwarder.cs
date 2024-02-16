namespace ClassLibrary.Forward.Abstractions;

public interface IForwarder
{
    Task ForwardAsync(long sourcePeerId, long destinationPeerId);
}
namespace ClassLibrary.Forward.Abstractions;

public interface IForwarder
{
    Task ForwardAsync(long fromPeerId, long toPeerId);
}
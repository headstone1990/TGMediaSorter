namespace ClassLibrary.Forward.Abstractions;

public interface IForwarder
{
    Task Forward(long fromPeerId, long toPeerId);
}
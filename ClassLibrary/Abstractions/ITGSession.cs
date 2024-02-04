using TL;

namespace ClassLibrary.Abstractions;

public interface ITGSession
{
    User Myself { get; }
    void Dispose();
}
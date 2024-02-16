using TL;

namespace ClassLibrary.Abstractions;

public interface IUpdateHandler
{
    Task Client_OnUpdate(UpdatesBase updates);
}
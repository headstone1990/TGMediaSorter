using ClassLibrary.Abstractions;
using TL;

namespace ClassLibrary;

public class UpdateHandler : IUpdateHandler
{
    public async Task Client_OnUpdate(UpdatesBase updates)
    {
        Console.WriteLine("updates");

        foreach (var update in updates.UpdateList)
        {
            switch (update)
            {
                case UpdateDeleteChannelMessages:
                    Console.WriteLine("UpdateDeleteChannelMessages");
                    break;
                case UpdateDeleteMessages udm:
                    
                    Console.WriteLine("UpdateDeleteMessages");
                    break;
            }
        }
    }
}
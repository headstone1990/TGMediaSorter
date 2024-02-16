using System.Diagnostics;
using ClassLibrary.Abstractions;
using DotNext.Threading;
using TL;
using WTelegram;
using IDisposable = System.IDisposable;

namespace ClassLibrary;

public delegate string VerificationCodeCallback();

public class TGSession : ITGSession, IDisposable, IAsyncDisposable
{
    private readonly AsyncLazy<Client> _wTelegramClient;
    private readonly VerificationCodeCallback _verificationCodeCallbackCallback;
    private readonly AuthenticatorData _authData;
    private readonly CancellationToken _token;
    private readonly IUpdateHandler _updateHandler;

    public TGSession(VerificationCodeCallback verificationCodeCallbackCallback, AuthenticatorData authData, IUpdateHandler updateHandler)
    {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        _token = cancelTokenSource.Token;
        _authData = authData;
        _updateHandler = updateHandler;
        _verificationCodeCallbackCallback = verificationCodeCallbackCallback;
        _wTelegramClient = new AsyncLazy<Client>(async (token) =>
        {
            Client client = new Client(Config);
            client.OnUpdate += _updateHandler.Client_OnUpdate;
            await client.LoginUserIfNeeded();
            return client;
        });
    }


    public async Task<User> GetCurrentUserAsync()
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        return client.User;
    }

    public async Task<InputPeer> GetPeerAsync(long peerId)
    {
        Dictionary<long, User> users = new();
        Dictionary<long, ChatBase> chats = new();
        
        var client = await _wTelegramClient.WithCancellation(_token);
        var dialogs = await client.Messages_GetAllDialogs();
        dialogs.CollectUsersChats(users, chats);
        InputPeer output;
        
        
        if (peerId < 0)
        {
            var peerIdText = peerId.ToString();
            if (peerIdText.Length != 14)
            {
                throw new ArgumentException("peerId for group/channel is wrong");
            }

            peerIdText = peerIdText.Substring(4);
            peerId = long.Parse(peerIdText);
            output = chats[peerId];
        }
        else
        {
            var peerIdText = peerId.ToString();
            peerId = long.Parse(peerIdText);
            output = users[peerId];
        }

        return output;
    }

    public async Task<int> GetMessagesCountAsync(InputPeer peer)
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        var history = await client.Messages_GetHistory(peer);
        return history.Count;
    }

    public async Task<bool> ForwardAsync(InputPeer fromPeer, InputPeer toPeer, int addOffset, int limit)
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        var messageBases = await client.Messages_GetHistory(fromPeer, add_offset: addOffset, limit: limit);
        if (messageBases.Messages.Length == 0)
        {
            return false;
        }

        for (int i = messageBases.Messages.Length - 1; i >= 0; i--)
        {
            if (messageBases.Messages[i] is Message message)
            {
                await client.SendMessageAsync(toPeer, message.message, message.media?.ToInputMedia(),
                    entities: message.entities);
            }

            await Task.Delay(5000, _token);
        }

        return true;
    }

    public async Task TestMethodAsync()
    {
        const int page = 1;
        
        Dictionary<long, User> users = new();
        Dictionary<long, ChatBase> chats = new();
        var client = await _wTelegramClient.WithCancellation(_token);
        

        var dialogs = await client.Messages_GetAllDialogs();
        dialogs.CollectUsersChats(users, chats);

        InputPeer peer = users[5379626745];
        
        var initialMessagesDownload = await client.Messages_GetHistory(peer);
        var count = initialMessagesDownload.Count;
        int addOffset = count - page;
        // int addOffset = 60149;
        while (true)
        {
            Console.WriteLine();
            var messageBases = await client.Messages_GetHistory(peer, add_offset: addOffset, limit: page);
            if (messageBases.Messages.Length == 0) break;

            for (int i = messageBases.Messages.Length - 1; i >= 0; i--)
            {
                if (messageBases.Messages[i] is Message message)
                {
                    message.message = $"{message.message} \nId in source: {message.ID}";
                    await client.SendMessageAsync(InputPeer.Self, message.message, message.media?.ToInputMedia(),
                        entities: message.entities);
                }
                // await client.Messages_ForwardMessages(
                //     peer,
                //     [messageBases.Messages[i].ID],
                //     [WTelegram.Helpers.RandomLong()],
                //     InputPeer.Self,
                //     drop_author: true);
                
                await Task.Delay(5000, _token);
            }

            addOffset -= page;
            var updatedHistory = await client.Messages_GetHistory(peer);
            var updatedCount = updatedHistory.Count;
            if (count != updatedCount)
            {
                addOffset += updatedCount - count;
                count = updatedCount;
            }
        }
    }

    private string Config(string what)
    {
        switch (what)
        {
            case "api_id": return _authData.ApiId.ToString();
            case "api_hash": return _authData.ApiHash;
            case "phone_number": return _authData.PhoneNumber;
            case "verification_code": return _verificationCodeCallbackCallback();
            case "first_name": return "John"; // if sign-up is required
            case "last_name": return "Doe"; // if sign-up is required
            case "password": return _authData.Password; // if user has enabled 2FA
            case "session_pathname": return Path.Combine(Environment.CurrentDirectory, "WTelegram.session");
            default: return null!; // let WTelegramClient decide the default config
        }
    }

    public void Dispose()
    {
        var client = Task.Run(() => _wTelegramClient.WithCancellation(_token), _token).GetAwaiter().GetResult();
        client.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        client.Dispose();
    }
}
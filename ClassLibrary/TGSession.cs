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

    public TGSession(VerificationCodeCallback verificationCodeCallbackCallback, AuthenticatorData authData)
    {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        _token = cancelTokenSource.Token;
        _authData = authData;
        _verificationCodeCallbackCallback = verificationCodeCallbackCallback;
        _wTelegramClient = new AsyncLazy<Client>(async (token) =>
        {
            Client client = new Client(Config);
            await client.LoginUserIfNeeded();
            return client;
        });
    }


    public async Task RegisterUpdateHandler(Func<UpdatesBase, Task> handler)
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        client.OnUpdate += handler;
    }
    
    public async Task UnregisterUpdateHandler(Func<UpdatesBase, Task> handler)
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        client.OnUpdate -= handler;
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

        if (peerId == 1)
        {
            output = InputPeer.Self;
        }
        else if (peerId < 0)
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

    public async Task ForwardAsync(InputPeer fromPeer, InputPeer toPeer, int offset)
    {
        while (true)
        {
            var client = await _wTelegramClient.WithCancellation(_token);
            var messageBases = await client.Messages_GetHistory(fromPeer, offset_id: offset, add_offset: -100);
            if (messageBases.Messages.Length == 0)
            {
                return;
            }

            for (int i = messageBases.Messages.Length - 1; i >= 0; i--)
            {
                if (messageBases.Messages[i] is not Message message) continue;
                
                await client.SendMessageAsync(toPeer, message.message, message.media?.ToInputMedia(),
                    entities: message.entities);
                
                await Task.Delay(5000, _token);
            }

            offset = messageBases.Messages[0].ID + 1;
        }
    }
    
    public async Task<List<long>> GetMessagesPeerId(int messageId)
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        var messages = await client.Messages_GetMessages(messageId);
        var output = messages.Messages.Select(message => message.Peer.ID).ToList();
        return output;
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
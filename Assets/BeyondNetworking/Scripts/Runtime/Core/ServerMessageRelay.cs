using Riptide;

namespace Beyond.Networking
{
    public class ServerMessageRelay : CommonMessageRelay {
        public override void SendTo<T>(T message, ushort target, MessageSendMode sendMode = MessageSendMode.Unreliable) {
            Message netMessage = Message.Create(sendMode, MessageIds.CustomMessage);
            netMessage.Add(typeof(T).FullName);

            var data = BeyondConverter.ObjectToByteArray(message);
            netMessage.Add(data);

            Network.Server.Send(netMessage, target);
        }
    }
}

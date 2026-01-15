using Riptide;
namespace Beyond.Networking
{
    public class ClientMessageRelay : CommonMessageRelay
    {
        public override void SendTo<T>(T message, ushort target, MessageSendMode sendMode = MessageSendMode.Unreliable) {
            Message netMessage = Message.Create(sendMode, MessageIds.CustomMessage);
            netMessage.Add(target);
            netMessage.Add(typeof(T).FullName);

            var data = BeyondConverter.ObjectToByteArray(message);
            netMessage.Add(data);

            Network.Client.Send(netMessage);
        }
    }
}

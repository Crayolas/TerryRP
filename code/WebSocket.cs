using Sandbox;
namespace Roleplay{
    public class RPWebSocket{
        WebSocket socket;
        public RPWebSocket(){
            socket = new WebSocket();
            socket.Connect("ws://localhost:5000");
        }
    }
}
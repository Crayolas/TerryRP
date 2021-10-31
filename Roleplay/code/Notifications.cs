using Sandbox;
namespace Roleplay{
    partial class Citizen{
        [ClientRpc]
        public void Notify(string info, string type = "default", int time = 5){
            Notification noti = new Notification(info, type, time);
            Local.Hud.AddChild(noti);
            
        }
    }
}
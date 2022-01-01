using Sandbox;
namespace Roleplay{
    partial class Citizen{
        [ClientRpc]
        public static void Notify(string info, string type = "default", int time = 5, bool confirmation = false,  int confirmationid = 0, Citizen sender = null){
            Notification noti = new Notification(info, type, time, confirmation, confirmationid, sender);
            Local.Hud.AddChild(noti);
            
        }
    }
}
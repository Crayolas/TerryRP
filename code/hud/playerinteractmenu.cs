using Sandbox;
using Sandbox.UI;
namespace Roleplay{
    public class PlayerInteractMenu:Panel{
        public static PlayerInteractMenu current = null;
        public Citizen selectedcitizen;
        public Panel selectedmenu;
        Button sendmoneybutton;
        Button sendfriendrequestbutton;
        Button invitetopartybutton;
        Button invitetoorgbutton;
        
       
        SendMoneyMenu sendmoney;
        
        public PlayerInteractMenu(Citizen ci){
        //set button position and menu height here

            selectedcitizen = ci;
            
            current = this;
            sendmoneybutton = new Button("Transfer Money", " ", opensendmoney);
            sendmoneybutton.AddClass("sendmoneybutton");
            AddChild(sendmoneybutton);
            sendfriendrequestbutton = new Button("Add Friend", " ", sendfriendrequest);
            sendfriendrequestbutton.AddClass("sendfriendrequestbutton");
            AddChild(sendfriendrequestbutton);
            invitetopartybutton = new Button("Invite to Party", " ", invitetoparty);
            invitetopartybutton.AddClass("invitetopartybutton");
            AddChild(invitetopartybutton);
            invitetoorgbutton = new Button("Invite to Org", " ", invitetoorg);
            invitetoorgbutton.AddClass("invitetoorgbutton");
            AddChild(invitetoorgbutton);
            
            
        }

        public void opensendmoney(){
            if (!(selectedmenu is SendMoneyMenu)){
            Sound.FromScreen("click");
            DeleteChildren();
            SetClass("sendmoneymenuselected", true);
            selectedmenu = new SendMoneyMenu();
            AddChild(selectedmenu);
            }

        }
        public void sendfriendrequest(){
           Sound.FromScreen("click");
           Delete(); 
        }
        public void invitetoparty(){

            Sound.FromScreen("click");
            Delete();
        }
        public void invitetoorg(){
            RoleplayGame.OrgInvite();
            Sound.FromScreen("click");
            Delete();
        }



    }


    public class SendMoneyMenu:Panel{
        Label msg;
        SMTextEntry sendamount;
        Button send;
        Button cancel;
        public SendMoneyMenu(){
            sendamount = new SMTextEntry();
            sendamount.SetProperty("numeric", "true");
            sendamount.MinValue = 0;
           // sendamount.MaxValue = (Local.Pawn as Citizen).balance;
            sendamount.CaretColor = Color.White;
            sendamount.MaxLength=9;
            
               
    

            AddChild(sendamount);
            sendamount.Focus();
            msg = new Label();
            msg.Text = "Transfer amount:";
            msg.AddClass("msg");
            AddChild(msg);

            
            send = new Button("Transfer", null, sendmoney);
            send.AddClass("confirmbutton");
            AddChild(send);
            cancel = new Button("Cancel", null, exitmenu);
            cancel.AddClass("cancelbutton");
            AddChild(cancel);

            send.SetClass("empty", true);


        }
        public void onamountchanged(){
            if(int.TryParse(sendamount.Text, out var p)){
                if (p > 0) {
               send.SetClass("empty", false);
            }else{
               send.SetClass("empty", true);
           }}else{
               send.SetClass("empty", true);
           }
        }

    
        public void sendmoney(){
           
            if (int.TryParse(sendamount.Text, out var p)){
            Sound.FromScreen("click");
            RoleplayGame.SendMoney(p);
            PlayerInteractMenu.current.selectedmenu = null;
            (Local.Pawn as Citizen).ClosePInteractMenu(false);
            }
        }
        public void exitmenu(){
             Sound.FromScreen("click");
            PlayerInteractMenu.current.selectedmenu = null;
            (Local.Pawn as Citizen).ClosePInteractMenu(false);
        }
    }
    public class SMTextEntry:TextEntry{
        public override void OnValueChanged(){
            base.OnValueChanged();
            (PlayerInteractMenu.current?.selectedmenu as SendMoneyMenu).onamountchanged();
        }

    }
}
using Sandbox;
using System;
namespace Roleplay{
    public class Confirmation{
        public int id;
        public Action confirmaction;
        public Action denyaction;
        public Citizen Sender;
        public string confirmtype;
        public Confirmation(Citizen receiver, int ID, string Message, Action confirm, string Type = "default", Action deny = null, Citizen send = null){
            
            Sender = send;
            if (Type == "friendrequest"){
                confirmtype = "friendrequest";
            }else if (Type =="partyinvite"){
                confirmtype = "partyinvite";
            }else if(Type == "orginvite") {
                confirmtype = "orginvite";
            }

            Citizen.Notify(To.Single(receiver), Message, Type, 0, true, ID, Sender);
           
            id = ID;
            if(deny != null){
                denyaction = deny;

            }
            confirmaction = confirm;
          

        }
        public void doconfirm(){
            confirmaction();
           
        }
        public void dodeny(){
            if (denyaction != null){
            denyaction();
            }
        }
         
        
    }
    partial class Citizen{
        public void sendconfirmation(string Message, Action confirm, Action deny = null, string Type = "default", Citizen sender = null ){
            if(confirmations.IndexOf(null)!=-1){
           confirmations[confirmations.IndexOf(null)] =  new Confirmation(this, confirmations.IndexOf(null), Message, confirm, Type, deny, sender);
            }else{
                confirmations.Add(new Confirmation(this, confirmations.Count, Message, confirm, Type, deny, sender));
          
                
            }
      

         
        }
    }
}
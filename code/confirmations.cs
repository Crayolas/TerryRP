using Sandbox;
using System;
namespace Roleplay{
    public class Confirmation{
        public int id;
        public Action confirmaction;
        public Action denyaction;
        public Confirmation(Citizen receiver, int ID, string Message, Action confirm, string Type = "default", Action deny = null){
            Citizen.Notify(To.Single(receiver), Message, Type, 0, true, ID);
           
            id = ID;
            if(deny != null){
                denyaction = deny;

            }
            confirmaction = confirm;
          

        }
        public void doconfirm(){
            confirmaction();
           
        }

         
        
    }
    partial class Citizen{
        public void sendconfirmation(string Message, Action confirm, Action deny = null, string Type = "default" ){
            if(confirmations.IndexOf(null)!=-1){
           confirmations[confirmations.IndexOf(null)] =  new Confirmation(this, confirmations.IndexOf(null), Message, confirm, Type, deny);
            }else{
                confirmations.Add(new Confirmation(this, confirmations.Count, Message, confirm, Type, deny));
          
                
            }
      

         
        }
    }
}
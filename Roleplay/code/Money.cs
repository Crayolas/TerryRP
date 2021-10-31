using Sandbox;
using System.ComponentModel;
namespace Roleplay{
    
    public partial class Citizen
    {
        public static int paycheckinterval = 10;
        public static long circulation = 0;
        public static TimeSince timesincepay = 0;

        
         [Net, Local]   
        public int balance{get; set;} = 0;
   
    
        private int paycheck = 1000;

        public bool addMoney(int val){
            if (balance+val >= 0){
            balance += val;
            circulation +=val;
            if(!Client.IsBot)(Game.Current as RoleplayGame).UpdateInfo<Citizen, int>(this,"balance",balance);
            return true;
            }
            return false;
        }
        public int setMoney(int val){
            circulation += val-balance;
            balance = val;
            if(!Client.IsBot)(Game.Current as RoleplayGame).UpdateInfo<Citizen, int>(this,"balance",balance);
            return balance;
        }
        public int getMoney(){
            return balance;
        }
        public int sendPaycheck(){
            addMoney(paycheck);
            return paycheck;
        }
        public bool transferMoney(Citizen receiver, int transferAmount){
            if(balance-transferAmount >= 0 && transferAmount > 0){
                addMoney(-transferAmount);
                Notify(To.Single(this),"You have given $"+transferAmount+" to "+receiver.Name, "money");
                receiver.addMoney(transferAmount);
                Notify(To.Single(receiver),"You have received $"+transferAmount+" from "+Name+".", "money");

                return true;
            }
                return false;
            }
    		[Event("server.tick")]
		public static void Paycheck(){
			if (timesincepay > paycheckinterval){
					foreach(Citizen ci in AllCitizens){
						ci.sendPaycheck();
						
					}
					timesincepay = 0;
				
				}

			}
    

        public static void CreateATM(Vector3 pos){
            new ATM(){
                Position = pos
            };
        }
    }
}
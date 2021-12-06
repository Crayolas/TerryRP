using Sandbox;
//if bank drops below 0 remove mayor
namespace Roleplay{
    public class Treasury{
        public Treasury(){
            if(linkbankrewardtopaycheck) bankrewardinterval = Citizen.paycheckinterval;
        }
        private static TimeSince bankrewardtimer;
        private static bool linkbankrewardtopaycheck = true;

        private static int bankrewardinterval = 300;
        private static int mayorcutdivisor = 10;
        private static int citizenreward= 1000;
        public static int balance = 0;
        [Event("server.tick")]
		public static void CitizenReward(){

		if (bankrewardtimer > bankrewardinterval){
                balance += Citizen.AllCitizens.Count*citizenreward;
                
                bankrewardtimer = 0;
                SendMayorCut();
				}

			}

        public static void SendMayorCut(){
            
            foreach(Citizen ci in Citizen.AllCitizens){
                if(ci.IsMayor == true){
                    Log.Info("Mayor "+ci.Client.Name+" has received "+ balance/mayorcutdivisor+ " from the treasury.");
                    ci.addMoney(balance/mayorcutdivisor);
                    balance -= balance/mayorcutdivisor;
                    Log.Info("$"+balance+" remains in the treasury.");

                }

            };
        }
    }


}
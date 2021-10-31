using Sandbox;
namespace Roleplay{
    partial class Citizen{ 



        private static int manslaughter_charge = 100;
        private int manslaughters = 0;
        public int bounty = 0;



        public void SetBounty(){
            bounty = manslaughters * manslaughter_charge;
            Notify(To.Everyone,(Client.Name + " now has a $"+ bounty + " bounty. Kill them to restore justice to this world and redeem the bounty."), "blahblah", 50);

        }
        public void AddCharge(string str, int amount = 1){
            if(str == "manslaughter"){
                manslaughters += amount;
  
            }
                          SetBounty();
        }

  
        
        

    }
    
        public partial class RoleplayGame{
        public override void OnKilled(Client cl, Entity pawn){
                        base.OnKilled(cl, pawn);
            if ( pawn.LastAttacker != null )
			{
				if ( pawn.LastAttacker.Client != null )
				{ Citizen.FindByPawn(pawn.LastAttacker).AddCharge("manslaughter");
        }
    }
        }
    }

}


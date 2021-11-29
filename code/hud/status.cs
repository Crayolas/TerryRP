using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
namespace Roleplay{

    public class Status : Panel
    
    {
        public Health health = new Health();
        public Balance bal = new Balance();
        public IDicon idicon = new IDicon();
        public ID id = new ID();
        public Status(){
                   AddChild(new Healthbase());
                   AddChild(new Armorbase());
                   AddChild(health);
                   AddChild(new Armor());
                   AddChild(new Cash()); 
                   AddChild(bal);
                   AddChild(idicon);
                   AddChild(id);


        }
        
    }   public class Healthbase: Panel{

    }
        public class Health : Panel{
            public override void Tick(){

            base.Tick();
            Style.Width = 200*(Local.Pawn.Health/100);
          
            }

        }
        public class Armorbase: Panel{

        }
        public class Armor : Panel{
        
        
        }
        
        public class Cash : Panel{
        
        
        }
        public class Balance:Label{
            public override void Tick(){

            int balance = (Local.Pawn as Citizen).balance;
            base.Tick();
            Text = "$"+balance.ToString();
   
            if(balance>=1000000000){
            float bils;
            bils = (float)balance/1000000000.0f;
            Text = "$"+bils.ToString("f2")+"B";

            return;
            }
            if(balance>=1000000){
                float mils;
                mils =  (float)balance/1000000.0f; 
                Text = "$"+mils.ToString("f2")+"M";
                
            }
            }

        }
        public class IDicon:Panel{
        
        }
        public class ID:Label{
            public override void Tick(){
                            base.Tick();
                            Text = Local.Pawn.Name;
            }
        }

}

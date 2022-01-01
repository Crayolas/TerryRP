using Sandbox;
using System;
using System.Collections.Generic;
namespace Roleplay{
public enum partyicon{
    noparty,

    dog,
    horse,
    dragon,
    pig,
    ram,
    rat,
    rooster,
    crab,

    octopus,
    panda,
    dolphin,
    camel,
    snake,
    tiger




}
public partial class Citizen{
    [Net]
    public (partyicon, Color) partydata{get; set;} = (partyicon.noparty, Color.Black);
    [Net]
    public int partyind {get;set;}
}
public partial class Party:BaseNetworkable{

    [Net]
    public partyicon icon{get;set;}
    [Net]
    public Color color{get; set;}
    [Net]
    public Citizen Leader{get; set;}
    [Net]
    public List<Citizen> members {get; set;} = new List<Citizen>();
    public int index;
    public Party(){
        
    }
    public Party(Citizen leader){
        

        List<int> openindices = new();
        if(leader != null){
        AddMember(leader);
        Leader = leader;
        int ind = 0;
        foreach(Party p in (Game.Current as RoleplayGame).AllParties){
            if(p.members.Count < 1){
            openindices.Add(ind);
            
            //icon = p.icon;
            //color = p.color;
            //(Game.Current as RoleplayGame).AllParties[ind] = this;

            //break;
            }
            ind++;
        }
        
        if (openindices.Count != 0){
            int targetindex = Random.Shared.Next(0, openindices.Count);
            icon = (Game.Current as RoleplayGame).AllParties[targetindex].icon;
            color = (Game.Current as RoleplayGame).AllParties[targetindex].color;
            (Game.Current as RoleplayGame).AllParties[targetindex] = this; 

        }


        }


    }

    public static int CreateParty(Citizen Leader){
        if (Leader != null){
            List<int> openindices = new();
            int ind = 0;
        foreach(Party p in (Game.Current as RoleplayGame).AllParties){
            if(p.members.Count < 1){
            openindices.Add(ind);
            

            }
            ind++;
        }

        if (openindices.Count != 0){
            int targetindex = openindices[Random.Shared.Next(0, openindices.Count)];

            (Game.Current as RoleplayGame).AllParties[targetindex].index = targetindex;
            (Game.Current as RoleplayGame).AllParties[targetindex].AddMember(Leader);
            (Game.Current as RoleplayGame).AllParties[targetindex].Leader = Leader;
            
            return targetindex;
            

        }

        }
        return -1;
    }
    public bool AddMember(Citizen target){
        if(target.partydata.Item1 != partyicon.noparty){
            return false;
        }else{
            members.Add(target);
            target.partydata = (icon, color);
            
            target.partyind = index;

            return true;
        }
    }
        public bool RemoveMember(Citizen target){
            
        if (target.partyind == index){
            members.Remove(target);
            target.partydata = (partyicon.noparty, Color.Black);
            target.partyind = -1;
            if(Leader == target){
                foreach(Citizen ci in members){
                    Leader = ci;
                    break;
                }
            }
           

            return true;
        }else{
            return false;
        }
    }
    public bool PromoteMember(Citizen target){
        if (target.partyind == index){
            Leader = target;
            return true;
        }else{
            return false;
        }
    }

}


}
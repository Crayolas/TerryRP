using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
namespace Roleplay{
public class PartiesBase:Panel{
    public Label partylabel;
    public static int movedown = 0;
    public static PartiesBase current;
    //public static Party party;
  
    static string icon;
    static Color color;
    static Dictionary<Citizen, PartyMember> membertags = new();
    
    public PartiesBase(){

        Party p =  ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind];
        
        membertags = new();
        //party=p;
        icon = Enum.GetName(p.icon)+".png";
        color = p.color;
        Panel iconpanel = new Panel();
        iconpanel.AddClass("iconpanel");
        Panel iconbase = new Panel();
   
        iconbase.AddClass("iconbase");

        iconbase.Style.BackgroundColor = new Color(color.r, color.g, color.b, .3f);
        
        iconpanel.Style.SetBackgroundImage("/ui/"+icon);

        iconpanel.Style.BackgroundTint = color;
        partylabel = new();
        partylabel.AddClass("partylabel");
        AddChild(partylabel);
         AddChild(iconbase);
        AddChild(iconpanel);

        
        Button leavebutton = new Button(null, "", leaveparty);
       
        AddChild(leavebutton);
         leavebutton.AddClass("leavebutton");
        foreach(Citizen citizen in p.members){
            //if (citizen != Local.Pawn as Citizen){
              
            PartyMember m = new(citizen);
            //membertags.Add(citizen, m);

            //AddChild(m);
           // }
        }        
    }
    public static void leaveparty(){
        RoleplayGame.party("leave");
        Sound.FromScreen("click");
        Sound.FromScreen("menuopen");

    }
   
    public override void Tick(){
    
    base.Tick();
        Log.Info((Local.Pawn as Citizen).partyind);
        if((Local.Pawn as Citizen).partydata.Item1 == partyicon.noparty){
        current = null;
        //party = null;
        Delete();
        return;

    }
                if (Local.Pawn as Citizen ==  ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].Leader){
        SetClass("visible", true);
            
        }else{
        SetClass("visible", false);
        }
    if(movedown>0){
     SetClass("Qdown", true);   
    }else{
     SetClass("Qdown", false);
    }

    partylabel.Text = "Party ("+((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].members.Count+"/10)";
    foreach(Citizen m in ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].members){
      
        if(membertags.ContainsKey(m) == false && m != Local.Pawn as Citizen){
            PartyMember mem = new(m);
            membertags.Add(m, mem);
            AddChild(mem);
        }
    }
    int index = 0;
    foreach(KeyValuePair<Citizen, PartyMember> kvpair in membertags){
        if(! ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].members.Contains(kvpair.Key)){
            kvpair.Value.Delete();
            membertags.Remove(kvpair.Key);
        }else{
            if (kvpair.Key ==  ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].Leader){
                kvpair.Value.SetClass("isleader", true);
            }else{
                kvpair.Value.SetClass("isleader", false);
            }
            kvpair.Value.Style.Top = 40*index + 40;
            index++;
            kvpair.Value.health.Style.Width = kvpair.Value.citizen.Health/100*160;
             kvpair.Value.name.Text = kvpair.Value.citizen.Name;
        }


    }

    }

}

public class PartyMember:Panel{
    public Citizen citizen;
    public Label name;
    public Panel leadercontrols;
    public Panel leadericon;
    public Button kick;
    public Button promote;
    public Panel health;
    public PartyMember(Citizen ci){
        leadercontrols = new();
        leadercontrols.AddClass("leadercontrols");
        leadericon = new();
        leadericon.AddClass("leadericon");
        name = new Label();
        name.AddClass("name");
        kick = new("Kick", "", KickPlayer);
        kick.AddClass("kickbutton");
        promote = new("Promote", "", PromotePlayer);
        promote.AddClass("promotebutton");
        citizen = ci;
        name.Text = ci.Name;
        health = new();
        health.AddClass("healthbar");
        
        health.Style.Width = ci.Health/100*160;
        Panel healthbase = new();
        healthbase.AddClass("healthbase");
        leadercontrols.AddChild(promote);
        leadercontrols.AddChild(kick);
        AddChild(leadericon);
        AddChild(leadercontrols);
        AddChild(name);
        AddChild(health);
        AddChild(healthbase);

    }
    public void KickPlayer(){
        if( ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].Leader == Local.Pawn as Citizen){
            RoleplayGame.party("kick", citizen.characterid);
            Sound.FromScreen("click");
            Sound.FromScreen("menuopen");
        }
    }
    public void PromotePlayer(){
        if( ((RoleplayGame)Game.Current).AllParties[(Local.Pawn as Citizen).partyind].Leader == Local.Pawn as Citizen){
            Sound.FromScreen("click");
            RoleplayGame.party("promote", citizen.characterid);
        }
    }

}
}

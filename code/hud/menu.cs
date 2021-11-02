using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
namespace Roleplay{

public class MenuBase:Panel{
    public bool inmenu = false;
    HotbarButton logout;
    HotbarIcon logouticon;
    HotbarButton inventory;
    HotbarIcon inventoryicon;
    HotbarButton organizations;
    HotbarIcon orgicon;

    


    
    public MenuBase(){
        logouticon = new HotbarIcon();
        
        logout = new HotbarButton(this,logouticon,null,null,exittocharacterselect);
        logout.AddClass("logout");
        AddChild(logout);

        inventoryicon = new HotbarIcon();
        
        inventory = new HotbarButton(this, inventoryicon,null,null,gotoinventory);
        inventory.AddClass("inventory");
        AddChild(inventory);

        orgicon = new HotbarIcon();
        organizations = new HotbarButton(this, orgicon, null, null, gotoorgs);
        organizations.AddClass("organizations");
        AddChild(organizations);






    }
    public void gotoorgs(){
        Sound.FromScreen("click");
        Sound.FromScreen("menuopen");
        DeleteChildren();
    }
    public void gotoinventory(){
        Sound.FromScreen("click");
        Sound.FromScreen("menuopen");
        DeleteChildren();
    }
    public void exittocharacterselect(){
        Sound.FromScreen("click");
        Sound.FromScreen("menuopen");
        RoleplayGame.Logout();
        DeleteChildren();
    }

}
public class HotbarButton:Button{
    public MenuBase parent;
    public bool iconhovered=false;
    bool inicon = false;
   
    HotbarIcon icon;
    public HotbarButton(MenuBase par, HotbarIcon ico, string s, string e, Action x):base(s, e, x){
        parent = par;
        icon = ico;
        icon.parent = this;
                    AddEventListener("hover", mouseover);
            AddEventListener("unhover", unmouseover);
            
            AddChild(icon);

            
    }
     protected override void OnMouseOver( MousePanelEvent e ){
            
            base.OnMouseOver(e);
            
            CreateEvent("hover");

           icon.SetClass("buttonhovered", true);
            
        }
        protected override void OnMouseOut(MousePanelEvent e){
            base.OnMouseOut(e);
           CreateEvent("unhover");
           icon.SetClass("buttonhovered", false);
           
        }
         public void mouseover(){
             
            if(iconhovered ==false){
                if(inicon == true){
                    inicon = false;
                    return;
                }
            Sound.FromScreen("hoverover");
                
            icon.SetClass("buttonhovered", true);
            }else{
                inicon=true;
            }
          
          
        }
        public void unmouseover(){
           
            icon.SetClass("buttonhovered", false);
          
        }

}
public class HotbarIcon:Panel{
        public HotbarButton parent;
         protected override void OnMouseOver( MousePanelEvent e ){
            parent.iconhovered = true;

            
        }
        protected override void OnMouseOut(MousePanelEvent e){
            parent.iconhovered = false;
        }
}
public class Menu:Panel{

}






}
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
        RPMenu p = new RPMenu("organizationpanel");
        Local.Hud.AddChild(p);
        RPMenu.current = p;
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
            
        }
        protected override void OnMouseOut(MousePanelEvent e){
            base.OnMouseOut(e);
           CreateEvent("unhover");
        
           
        }
         public void mouseover(){
                         
                
           
             
            if(iconhovered ==false ){
                if(inicon == true){
                    inicon = false;
                    Sound.FromScreen("hoverover");
                    
                    return;
                }
           Sound.FromScreen("hoverover");
        
           
            }else{
                inicon=true;
            }
          
          
        }
        public void unmouseover(){
            if (iconhovered == false){
           
            }else{
           
            }
          
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
public class RPMenu:Panel{
        
        public static RPMenu current;
        public MenuConfirmation confirm;
        Panel bordertopleft;
        Panel bordertopright;
        public Panel selectedpanel;
        Button OrganizationTab;
        OrganizationPanel organizationpanel;
        Button exitmenu;



        public RPMenu(string gotopanel){
            exitmenu = new Button(null, "", CloseMenu);
            exitmenu.AddClass("exitbutton");
            AddChild(exitmenu);
            OrganizationTab = new Button("Organization", "", SelectOrgPanel);
            OrganizationTab.AddClass("orgtab");
            AddChild(OrganizationTab);
        

            bordertopleft = new Panel();
            bordertopleft.AddClass("bordertopleft");
            AddChild(bordertopleft);
            bordertopright = new Panel();
            bordertopright.AddClass("bordertopright");
            AddChild(bordertopright);
            if(gotopanel == "organizationpanel"){
            SelectOrgPanel();
            }
            
        }
         public void SelectOrgPanel(){
             if(organizationpanel == null){
             organizationpanel = new OrganizationPanel();
             AddChild(organizationpanel);
             selectedpanel = organizationpanel;
             bordertopleft.Style.Width = 0;
             bordertopright.Style.Width = 822;
             }

        }
        public void CloseMenu(){
            current = null;
            Sound.FromScreen("click");
            Sound.FromScreen("menuopen");
            Delete();
        }
        public void CreateConfirmation(string msg, Action a, Action b = null){
            if(confirm == null){
            confirm = new MenuConfirmation(msg, a, b);
            AddChild(confirm);
            }
        }
}

public class InventoryPanel:Panel{

}
public class OrganizationPanel:Panel{
       

    Panel section;

    public OrganizationPanel(){
        
        
        if((Local.Pawn as Citizen).org == null){
        //Log.Info((Local.Pawn as Citizen).org.LeaderID);
        SwitchTo("noorgpanel");
        }else{
        SwitchTo("orghomepanel");
        }


}
public void SwitchTo(string targetpanel){
    section = null;
    DeleteChildren();
    if(targetpanel == "noorgpanel"){
        section = new NoOrgPanel(this);
    }
    else if (targetpanel == "createorgpanel"){
        section = new CreateOrgPanel(this);
    }
    else if (targetpanel == "orghomepanel"){
        section = new OrgHomePanel(this);
    }
    AddChild(section);
}
}

public class OrgHomePanel:Panel{
    Org org;
    public Panel currenttab;
    OrganizationPanel parent;
    Label orgnam;
    Button members;
    Button roles;
    Button territories;
    Button wars;
    Button leave;
    public OrgHomePanel(OrganizationPanel par){
        org = (Local.Pawn as Citizen).org;
        parent = par;
        orgnam = new Label();
        orgnam.Text = (Local.Pawn as Citizen).org.Name;
        orgnam.AddClass("orgname");
        members = new Button("Members", null, gotomemberstab);
        
        
        members.AddClass("membersbutton");
        roles = new Button("Roles");
        roles.AddClass("rolesbutton");
        wars = new Button("Wars");
        wars.AddClass("warsbutton");
        territories = new Button("Turf");
        territories.AddClass("territoriesbutton");
        leave = new Button("", null, sendleaveprompt);

        if ((Local.Pawn as Citizen).characterid == org.LeaderID){
            leave.Text = "Disband";
        }else{
            leave.Text = "Leave";
        }
        leave.AddClass("leavebutton");

        
        AddChild(leave);
        AddChild(orgnam);
        AddChild(members);
        AddChild(roles);
        AddChild(wars);
        AddChild(territories);


    }
    public void sendleaveprompt(){
        Sound.FromScreen("click");
        if ((Local.Pawn as Citizen).characterid == (Local.Pawn as Citizen).org.LeaderID){
            RPMenu.current.CreateConfirmation("Are you sure you want to disband "+org.Name+"?", RoleplayGame.LeaveOrg);
        } else{
            RPMenu.current.CreateConfirmation("Are you sure you want to leave "+org.Name+"?", RoleplayGame.LeaveOrg);
        }
    }
    public void gotomemberstab(){
        //580 wide
    }

}

public class NoOrgPanel:Panel{
        OrganizationPanel parent;
        Label noorgmessage;
        Button browseorgsbutton;
         Button createorgbutton;
         public NoOrgPanel(OrganizationPanel par){
        noorgmessage = new Label();
        noorgmessage.Text = "You currently aren't part of an organization.";
        noorgmessage.AddClass("noorgmessage");
        browseorgsbutton = new Button("Browse Organizations", "", browseorgs);
        browseorgsbutton.AddClass("browseorgsbutton");
        
        createorgbutton = new Button("Create Organization", "", gotoorgcreation);
        createorgbutton.AddClass("createorgbutton");
        AddChild(browseorgsbutton);
        AddChild(noorgmessage);
        AddChild(createorgbutton);
        parent = par;
         }
    public void browseorgs(){

    }
    public void gotoorgcreation(){
        parent.SwitchTo("createorgpanel");
        Sound.FromScreen("click");

    }

    }


public class CreateOrgPanel:Panel{
    OrganizationPanel parent;
    Label prompt;
    TextEntry orgname;
    Button createorgbtn;
    Button back;
        public CreateOrgPanel(OrganizationPanel par){
            prompt = new Label();
            prompt.Text = "Organization Name";
            prompt.AddClass("prompt");
            
            AddChild(prompt);
            orgname = new TextEntry();
            orgname.CaretColor = Color.White;
            AddChild(orgname);
            
            createorgbtn = new Button("Create Organization ($"+Org.OrgCost+")", null, sendprompt);
            AddChild(createorgbtn);



        }
        public void sendprompt(){
            Sound.FromScreen("click");
            RPMenu.current.CreateConfirmation("Are you sure you would like to create "+orgname.Text+" for $"+Org.OrgCost+"?", createorg);

        }

        public void createorg(){
            //Sound.FromScreen("success");
            
            RoleplayGame.CreateOrganization(orgname.Text);
        }

}
public class MenuConfirmation:Panel{
    Panel confirmationpanel;
    Label message;
    Button confirm;
    Button deny;
    Action confirmfunc;
    Action denyfunc;
    public MenuConfirmation(string msg, Action a, Action b = null){
        confirmfunc = a;
        denyfunc = b;
        confirmationpanel = new Panel();
        confirmationpanel.AddClass("confirmpanel");
        message = new Label();
        message.Text = msg;
        message.AddClass("confirmmessage");
        confirm = new Button("Yes", null, confirmation);
        confirm.AddClass("confirmbutton");
        deny = new Button("No", null, denial);
        deny.AddClass("denybutton");
        confirmationpanel.AddChild(message);
        confirmationpanel.AddChild(confirm);
        confirmationpanel.AddChild(deny);
        AddChild(confirmationpanel);
        Sound.FromScreen("menuopen");
        
    }

    public void confirmation(){
        Sound.FromScreen("click");
        confirmfunc();
        RPMenu.current.confirm = null;
        Delete();
    }
    public void denial(){
        Sound.FromScreen("click");
        if(denyfunc != null){
        denyfunc();
        }
        RPMenu.current.confirm = null;
        Delete();
    }

}





}
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
        public Org org;
        public static RPMenu current;
        public MenuConfirmation confirm;
        Panel bordertopleft;
        Panel bordertopright;
        public Panel selectedpanel;
        Button OrganizationTab;
        OrganizationPanel organizationpanel;
        Button exitmenu;



        public RPMenu(string gotopanel){
             org = Org.GetOrg(Local.Pawn as Citizen);
            current = this;
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
             if(!(selectedpanel is OrganizationPanel)){
            if (selectedpanel != null){
                selectedpanel.Delete();
            }
            
             selectedpanel = new OrganizationPanel();
             
             AddChild(selectedpanel);
             
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

        if(RPMenu.current.org == null){
      
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
        RPMenu.current.org = Org.GetOrg(Local.Pawn as Citizen);
        section = new OrgHomePanel();

    }
    AddChild(section);
}
}

public class OrgHomePanel:Panel{
    public Org org;
    public Panel currenttab;
    OrganizationPanel parent;
    HomeTab hometab;
    Label orgnam;
    Button home;
    Button roles;
    Button territories;
    Button wars;
    Button leave;
    public OrgHomePanel(){

       org = Org.GetOrg(Local.Pawn as Citizen);
        
        orgnam = new Label();
        orgnam.Text = org.Name;
        orgnam.AddClass("orgname");
        home = new Button("Home", " ", gotohometab);
        
        
        home.AddClass("homebutton");
        roles = new Button("Roles", " ", gotorolestab);
        roles.AddClass("rolesbutton");
        wars = new Button("Wars", " ", gotowarstab);
        wars.AddClass("warsbutton");
        territories = new Button("Turf", " ", gototurftab);
        territories.AddClass("territoriesbutton");
        leave = new Button("", " ", sendleaveprompt);

        if ((Local.Pawn as Citizen).characterid == org.LeaderID){
            leave.Text = "Disband";
        }else{
            leave.Text = "Leave";
        }
        leave.AddClass("leavebutton");

        
        AddChild(leave);
        AddChild(orgnam);
        AddChild(home);
        AddChild(roles);
        AddChild(wars);
        AddChild(territories);

    selecttab("home");
    }

    public void selecttab(string tab){
        
        territories.SetClass("selected", false);
        wars.SetClass("selected", false);
        roles.SetClass("selected", false);
        home.SetClass("selected", false);
        

        if(currenttab != null){
        currenttab.Delete();
        currenttab = null;
        }
        if(tab == "home"){
            home.SetClass("selected", true);
            hometab = new HomeTab();
            currenttab = hometab;
            AddChild(currenttab);

        }
        else if(tab == "roles"){
            roles.SetClass("selected", true);
            hometab = null;
        }
        else if(tab == "wars"){
            wars.SetClass("selected", true);
            hometab = null;

        }
        else if (tab == "turf"){
            territories.SetClass("selected", true);
            hometab = null;
        }
    }
    
    public void sendleaveprompt(){
        Sound.FromScreen("click");
        if ((Local.Pawn as Citizen).characterid == org.LeaderID){
            RPMenu.current.CreateConfirmation("Are you sure you want to disband "+org.Name+"?", RoleplayGame.LeaveOrg);
        } else{
            RPMenu.current.CreateConfirmation("Are you sure you want to leave "+org.Name+"?", RoleplayGame.LeaveOrg);
        }
    }
    private void gotohometab(){
        
        if (hometab == null){
        Sound.FromScreen("click");
        selecttab("home");
        }
    }
        private void gotorolestab(){
        
        Sound.FromScreen("click");
        selecttab("roles");
    }
        private void gotowarstab(){
       
        Sound.FromScreen("click");
        selecttab("wars");
    }
        private void gototurftab(){
      
        Sound.FromScreen("click");
        selecttab("turf");
    }




}
public class HomeTab:Panel{
    MembersBase membersbase;
    public HomeTab(){
        membersbase = new();
        AddChild(membersbase);
    }
}

public class MembersBase:Panel{
    Org o;
    public MembersBase(){
       o = Org.GetOrg(Local.Pawn as Citizen);
        foreach(int i in RPMenu.current.org.Members){
           AddChild(new Member(i));
        }
    }

}
public class Member:Panel{
    public int characterid;
    public string name;
    public Label charactername;
    public Member(int cid){

        name = (Game.Current as RoleplayGame).ProfileDictionary[cid].Name;
        charactername = new();
        charactername.Text = name;
        AddChild(charactername);
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
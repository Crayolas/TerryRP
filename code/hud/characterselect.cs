using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
namespace Roleplay{
    public class CharacterSelectPage:Panel{
        
        public static CharacterSelectPage current;
        public static bool oncharacterselect = false;
        public static List<Citizen> characterslist;
        public static List<characterpanel> characterpanels;
        public static newcharacter ncpanel;
        public CharacterSelectPage(){
            current = this;
            characterpanels = new List<characterpanel>();
            characterslist = new List<Citizen>();
            SetTemplate("/hud/characterselect.html");
            AddChild(new Panel(){ElementName = "pattern"});
            AddChild(new Panel(){ElementName = "sboxlogo"});
            AddChild(new Panel(){ElementName = "terryrplogo"});
            ncpanel = new newcharacter();
            AddChild(ncpanel);
            oncharacterselect = true;
            


        }
        public static void updatecharacterlist(Citizen character){
        
            
                bool newcitizen = true;
                bool newpanel = true;
           
            foreach(Citizen ci in characterslist){

                if(ci.characterid == character.characterid){
                    newcitizen = false;
                }


            }
                if(newcitizen == true){

                    AddCharacter(character);
                    
                
            }
            

            

        }
        /////////notifications permanently increAse index by 1 if change characer?
        public static void AddCharacter(Citizen ci){
            characterpanel cpan = new characterpanel(ci.characterid, ci.Name, ci.balance);
            characterpanels.Add(cpan);
            current.AddChild(cpan);
            characterslist.Add(ci);
            
           
            
        }
        public static void mouseover(){
          
        }
        
    }
    public class characterpanel:Button{
        public int characterid;
        public string Name;
        public int balance;
        public Panel balanceicon;
        public Panel Nameicon;
        public Panel levelicon;
        public Label NameLabel;
        public Label balancelabel;
        public Label levellabel;
        public characterpanel(int cid, string n, int b){
        characterid = cid;
        Name = n;
        balance = b;
        balancelabel = new Label();
        NameLabel = new Label();
        Nameicon = new Panel();
        balanceicon = new Panel();
        levelicon = new Panel();
        levellabel = new Label();

        balanceicon.AddClass("balanceicon");
        Nameicon.AddClass("nameicon");
        balancelabel.AddClass("balancelabel");
        NameLabel.AddClass("nameLabel");
        levelicon.AddClass("levelicon");
        levellabel.AddClass("levellabel");
        AddChild(balanceicon);
        AddChild(Nameicon);
        AddChild(balancelabel);
        AddChild(NameLabel);
        AddChild(levellabel);
        AddChild(levelicon);
        levellabel.Text = "1";
        NameLabel.Text = Name;
        balancelabel.Text = balance.ToString();


        AddEventListener("onclick", selectcharacter);
        
        }
        public override void Tick(){
            Style.Left = (-125*CharacterSelectPage.characterpanels.Count+250*CharacterSelectPage.characterpanels.FindIndex(x=> x == this));

        }
        public void selectcharacter(){
            RoleplayGame.SelectCharacter(characterid);
            
            Sound.FromScreen("characterselect");
        }

    }
    public class newcharacter:Button{
        public TimeSince doubleclickprevent;
        public newcharacterform ncf;
        public bool incharacterselect = false;
        Panel addicon;
        Panel newchform;
 
        public newcharacter(){
            addicon = new Panel(){ElementName = "addicon"};
             AddChild(addicon);

            //OnClick(new MousePanelEvent("hover", this, "mouseleft"));
            AddEventListener("hover", mouseover);
            AddEventListener("unhover", unmouseover);
            AddEventListener("onclick", loadform);
          
           // OnClick(new MousePanelEvent("hover", this, "mouseleft"));
            
        }
        protected override void OnMouseOver( MousePanelEvent e ){
            if(CharacterSelectPage.characterslist.Count<=3){
            base.OnMouseOver(e);
            CreateEvent("hover");

            }
        }
        protected override void OnMouseOut(MousePanelEvent e){
            base.OnMouseOut(e);
           CreateEvent("unhover");
        }
               public void showicon(){
            addicon = new Panel(){ElementName = "addicon"};
            AddChild(addicon);
        }

        public void loadform(){
            if(CharacterSelectPage.characterslist.Count<=3){
            if(!incharacterselect && doubleclickprevent>0){
            addicon.Delete();
            Sound.FromScreen("click");
            ncf = new newcharacterform();
            AddChild(ncf);
            SetClass("focused", true);
            Switch(PseudoClass.Hover, false);
            incharacterselect = true;
            }
            }
        }
        public void mouseover(){
            if(CharacterSelectPage.characterslist.Count<=3){
            if(!incharacterselect){
                
            addicon.SetClass("buttonhovered", true);
            }else{
                Switch(PseudoClass.Hover, false);
            }
            }
        }
        public void unmouseover(){
            if(!incharacterselect){
            addicon.SetClass("buttonhovered", false);
            }
        }
                public override void Tick(){
            Style.Left = 125*CharacterSelectPage.characterpanels.Count;
            
        }
        }
        
        
        public class newcharacterform:Panel{
            TextEntry firstname;
            TextEntry lastname;
            Label fn;
            Label ln;
            bool ready = false;
            Button createcharacterbtn;
            public newcharacterform(){
                firstname = new TextEntry();
                lastname = new TextEntry();
                fn = new Label(){Text = "First name"};
                ln = new Label(){Text = "Last name"};
                
                AddChild(fn);
                AddChild(ln);
                fn.AddClass("fnlabel");
                ln.AddClass("lnlabel");

                AddChild(firstname);
                AddChild(lastname);
                firstname.CaretColor = Color.White;
                lastname.CaretColor = Color.White;

                createcharacterbtn = new Button("Create Character", null, createcharacter);
                firstname.AddClass("firstname");
                lastname.AddClass("lastname");
                AddChild(createcharacterbtn);

               
            }
             [Event.Frame]
             public void checkcompletion(){
                 
                 if(firstname.Text?.Length > 1 && lastname.Text?.Length > 1 ){
                     createcharacterbtn.SetClass("characterready", true);
                     ready = true;
                 }else{
                     createcharacterbtn.SetClass("characterready", false);
                     ready = false;
                 }
             }
            public void createcharacter(){
                if(ready == true){
                    RoleplayGame.CreateCharacter(firstname.Text, lastname.Text);
                    Sound.FromScreen("click");
                    Sound.FromScreen("notification");
                    CharacterSelectPage.ncpanel.SetClass("focused", false);
                    CharacterSelectPage.ncpanel.Switch(PseudoClass.Hover, true);
                    CharacterSelectPage.ncpanel.doubleclickprevent = 0;
                    CharacterSelectPage.ncpanel.incharacterselect = false;
                    
                    CharacterSelectPage.ncpanel.showicon();
                    
                    Delete();
                    
                }
  

                    
            }
        }
        
    }






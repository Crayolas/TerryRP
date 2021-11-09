using Sandbox;
using System.Collections.Generic;
namespace Roleplay{
    //matt is a freak
    public partial class Org : BaseNetworkable{
        
       
        public static int OrgCost {get; set;} = 5000;
        public static Org GetOrg(Citizen ci, int cid = -1){
        
        foreach(Org org in (Game.Current as RoleplayGame).AllOrgs){
            if(cid == -1){
            foreach(int MemID in org.Members){
                if(MemID == ci.characterid){
                    return org;
                }
            }}else{
            foreach(int MemID in org.Members){
                if(MemID == cid){
                    return org;
                }
            }
            {
                
            }}
            
        }
        return null;
        }
        
        public List<int> Members{get;set;} = new List<int>();
        private string _name;
        [Net]
        public string Name {get{return _name;}
        
        set{
            bool create = true; 
            foreach(Org o in (Game.Current as RoleplayGame).AllOrgs){
            if(o.Name == value.Trim()){
                create = false;
            }


        }
        if(create)_name = value.Trim();

        }}
        public int OrgID {get;set;}
        [Net]
        public int LeaderID{get;set;}
        public static bool NameTaken(string n){
            foreach(Org o in (Game.Current as RoleplayGame).AllOrgs){
            if(o.Name == n.Trim()){
                return true;
            }
                

        }
             return false;
        }

        public static Org CreateOrg(string name, Citizen creator){
           
            if(NameTaken(name)){
                return null;
            }
            if(GetOrg(creator)!=null){
                return null;
            }

            return new Org(name, creator);


        }
        public static void DisbandOrg(Org o){
            foreach(Client cl in Client.All){
                if(cl.Pawn!=null){
                    if((cl.Pawn as Citizen).org == o){
                        Citizen.Notify(To.Single(cl), o.Name+" has been disbanded.");
                        (cl.Pawn as Citizen).org = null;
                        (Game.Current as RoleplayGame).ShowNoOrg(To.Single(cl));
                    }
                }
            }

            foreach(Org or in (Game.Current as RoleplayGame).AllOrgs){
                if(or.OrgID == o.OrgID){
                    (Game.Current as RoleplayGame).AllOrgs.Remove(or);
                    SaveOrgs();
                    return;
                    
                    
                }
            }
           
           //Log.Info((Game.Current as RoleplayGame).AllOrgs.Remove(o));

        }


        public static void RemoveFromOrg(Org o, int CharacterID){
            foreach(Client cl in Client.All){
                if (cl.Pawn != null){
                    if((cl.Pawn as Citizen).characterid == CharacterID){
                        Citizen.Notify(To.Single(cl), o.Name+" you have left "+o.Name+".");
                        (cl.Pawn as Citizen).org = null;
                        (Game.Current as RoleplayGame).ShowNoOrg(To.Single(cl));
                    }
                }

            }
            foreach(Org or in (Game.Current as RoleplayGame).AllOrgs){
                if(or.OrgID == o.OrgID){
                    or.Members.Remove(CharacterID);
                    SaveOrgs();
                    return;
                    
                    
                }
            }
        }
        public static void SaveOrgs(){
                        OrgsData orgs = new OrgsData();
            foreach(Org o in (Game.Current as RoleplayGame).AllOrgs){
                orgs.AllOrgs.Add(new OrgData(){Name = o.Name, OrgID = o.OrgID, LeaderID = o.LeaderID, Members = o.Members});
            }
            
            FileSystem.Data.WriteJson("terryrpdata/serverdata/organizations.json", orgs);
        }
        public Org(){

        }
        private Org(string name,Citizen creator){


            Name = name;
            LeaderID = creator.characterid;
            Members.Add(creator.characterid);
            Serverdata sdata = FileSystem.Data.ReadJson<Serverdata>("terryrpdata/serverdata/serverdata.json");
            OrgID = sdata.currentorgid;
            (Game.Current as RoleplayGame).AllOrgs.Add(this);
            sdata.currentorgid++;
            FileSystem.Data.WriteJson("terryrpdata/serverdata/serverdata.json", sdata);
            SaveOrgs();

            Citizen.Notify(To.Single(creator), "You have created "+ name + "!", "success");
            creator.org = this;
           
            (Game.Current as RoleplayGame).sendtoorgpage(To.Single(creator));
            
        }
    }


}
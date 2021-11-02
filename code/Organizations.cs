using Sandbox;
using System.Collections.Generic;
namespace Roleplay{
    public class Org{
        public static List<Org> AllOrgs {get; set;}
        public static Org GetOrg(Citizen ci){
        foreach(Org org in AllOrgs){
            foreach(int MemID in org.Members){
                if(MemID == ci.characterid){
                    return org;
                }
            }}
            return null;
        }
        public List<int> Members;
        private string _name;
        public string Name {get{return _name;}
        
        set{
            bool create = true; 
            foreach(Org o in AllOrgs){
            if(o.Name == value.Trim()){
                create = false;
            }


        }
        if(create)_name = value.Trim();

        }}
        public int OrgID;
        public int LeaderID{get;set;}

        public Org CreateOrg(string name, Citizen creator){
            Name = name;
            if(Name == null){
                return null;
            }
            if(GetOrg(creator)!=null){
                return null;
            }

            return new Org(Name, creator);


        }
        private Org(string name,Citizen creator){


            Name = name;
            LeaderID = creator.characterid;
            Serverdata sdata = FileSystem.Data.ReadJson<Serverdata>("terryrpdata/serverdata/serverdata.json");
            OrgID = sdata.currentorgid;
            AllOrgs.Add(this);
            sdata.currentorgid++;
            FileSystem.Data.WriteJson("terryrpdata/serverdata/serverdata.json", sdata);
            OrgData orgs = new OrgData();
            orgs.AllOrgs = AllOrgs;
            FileSystem.Data.WriteJson("terryrpdata/serverdata/organizations.json", orgs);

        }
    }


}
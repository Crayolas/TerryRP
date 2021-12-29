using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System;


namespace Roleplay{


    public partial class rpnametag:WorldPanel{

        public static Dictionary<Citizen, float> cltimetillid {get; set;} = new Dictionary<Citizen, float>();
        //public static Dictionary<Citizen, TimeSince> identifiedcitizens {get; set;} = new Dictionary<Citizen, TimeSince>();
        public static List<Citizen> clidentifiedcitizens{get;set;}=new List<Citizen>();
        public static TimeSince identifyingtext = 0;
        public static Dictionary<Citizen, rpnametag> nametags = new Dictionary<Citizen, rpnametag>();

        public static List<Citizen> nearbyplayers = new List<Citizen>();
        //public rpnametag nametag;
        public rpnamebase textbase;
        public rpidentifiedpercent idpercent;
        public rpname rptext;
        public Label orgname;
        public Citizen citizen;
        public Panel partyicon;
        public rpnametag(Citizen othercitizen){
            StyleSheet.Load( "/hud/Hud.scss" );
            if (othercitizen!=null){
            orgname = new Label();
            orgname.AddClass("orgname");
            citizen = othercitizen;
            textbase = new rpnamebase();
            rptext = new rpname();
            idpercent = new rpidentifiedpercent();
            partyicon = new();
            partyicon.AddClass("partyicon");
            

            
            textbase.AddChild(partyicon);
            textbase.AddChild(idpercent);
            textbase.AddChild(rptext);
            textbase.AddChild(orgname);
            AddChild(textbase);
            //rptext.Text = othercitizen.Name;
            Transform = othercitizen.Transform;
            nametags.Add(othercitizen, this);

            nearbyplayers.Add(othercitizen);
            }
        }
        [ClientRpc]
         public static void updateidentification(Citizen ci, String type){
             
            if (type == "addtoidlist"){
                cltimetillid.Add(ci, 0);
            }
                       else if (type == "removefromidlist"){
                cltimetillid.Remove(ci);
            }
                       else if (type == "identify"){
               clidentifiedcitizens.Add(ci);
                
                           
            }
                       else if (type == "unidentify"){
                clidentifiedcitizens.Remove(ci);
                           if(cltimetillid.ContainsKey(ci)){
                                  cltimetillid[ci]=0;
                                     }


            }else{float percent = float.Parse(type);
              cltimetillid[ci]=percent;

            } 
            
            //else if (type.Substring(0,2) == "ip"){
            //    float percent = float.Parse(type.Substring(2));
            //    cltimetillid[ci]=percent;
            //
            //}else if (type.Substring(0,2)=="dp"){
            //    float percent = float.Parse(type.Substring(2));
            //    cltimetillid[ci]=percent;
            //}
            

        }
        
        public static void DeleteTag(Citizen citi){
            if(citi != null){
            if(nametags.ContainsKey(citi)){
            nametags[citi].Delete(false);
            nametags.Remove(citi);

            }
            }
        }
        public void updatenametags(Citizen citi){
            //nearbyplayers = nearbypl;
        }
        


        public override void Tick(){


            base.Tick();
                        if(!nametags.ContainsKey(citizen)){
                Delete();
                return;
            }
                if(!citizen.IsValid()){
                Delete();
                return;
             }
            var isidentified = false;
           var tagoffset = new Vector3(0f, 0,65f);

            if (clidentifiedcitizens.Contains(citizen)){
                isidentified=true;
            }
            

            if(cltimetillid.ContainsKey(citizen)){
            idpercent.Style.Width = 250*cltimetillid[citizen]/100f;
            }
            //if(previouspercent<cltimetillid[citizen]){
            //    increase = true;

                
           // }else if(previouspercent >cltimetillid[citizen]){
            //    increase = false;
           // }
            //Log.Info(increase);
           // previouspercent = cltimetillid[citizen];

           





            //make all this shit serverside and individually newnametag and add removenametag which we call from rpc with citizen object. 
            //try only calling newnametag when a new player is added to the nearbylist and handling delete clientside

            
           // foreach( KeyValuePair<Citizen, rpnametag> kv in nametags){
                //foreach(KeyValuePair<Citizen, float> keyval in cltimetillid){
                  //  if(keyval.Key == kv.Key){
                    //    kv.Value.
                   // }
               // }
               
               
               
               
               
                //bool nearby = false;
               // foreach(Citizen ci in nearbyplayers){
                  //  if(ci == kv.Key)nearby=true;

                //} 
                //if(!nearby){
                   // Log.Info("Deleted");
                   // Delete();
                 //   return;
               // };






                if(isidentified){
                rptext.Text = citizen.Name;
                
                orgname.Text = citizen.orgname;
                }else{
                if (identifyingtext>3){
                    rptext.Text = "Identifying...";
                    
                }else if(identifyingtext>2){
                    rptext.Text = "Identifying..";
                }else if (identifyingtext >1){
                    rptext.Text = "Identifying.";
                }else if (identifyingtext >0){
                    rptext.Text = "Identifying";
                }}


               // kv.Value.Transform = new Transform(kv.Key.Transform.Position);
                
               Transform = citizen.Transform;
               
               
                Vector3 rotationtoward = citizen.Position- Local.Pawn.Position;
                rotationtoward.z = 0;
                if(Enum.GetName(citizen.partydata.Item1)!="noparty"){
                partyicon.Style.SetBackgroundImage("/ui/"+Enum.GetName(citizen.partydata.Item1)+".png");
                partyicon.Style.BackgroundTint = citizen.partydata.Item2;
                }else{
                    partyicon.Style.SetBackgroundImage("");
                }
                Transform = citizen.Transform.Add(tagoffset, false);
                  Rotation = Rotation.LookAt(-rotationtoward);
                            if(identifyingtext > 4){
                identifyingtext = 0;

            }
            
            }


       // }

   }
    public class rpname : Label{
   

    }
    public class rpidentifiedpercent:Panel{
       
    }
    public class rpnamebase:Panel{

    }

    }

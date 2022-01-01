using Sandbox;
using System.Collections.Generic;
using System;
namespace Roleplay{
    

        
    
public partial class identificationlist:BaseNetworkable{
public Citizen owner;
//private bool sendmessage;
public Dictionary<Citizen, float> timetillid {get; set;} = new Dictionary<Citizen, float>();
public Dictionary<Citizen, TimeSince> identifiedcitizens {get; set;} = new Dictionary<Citizen, TimeSince>();

        public identificationlist(Citizen caller){
        owner = caller;
        
        foreach(Citizen ci in Citizen.AllCitizens){
            addtoidlist(ci);
    

        }
    }





        
       
    public void addtoidlist(Citizen ci){
        
        timetillid.Add(ci, 0);
        rpnametag.updateidentification(To.Single(owner),ci, "addtoidlist");
    }
    public void removefromidlist(Citizen ci){
        timetillid.Remove(ci);
        rpnametag.updateidentification(To.Single(owner), ci,"removefromidlist");
    }
    public void identify(Citizen ci){
        if (identifiedcitizens.ContainsKey(ci)==false){
        identifiedcitizens.Add(ci, 0);
        
       rpnametag.updateidentification(To.Single(owner),ci, "identify");
        }
    }
    public void unidentify(Citizen ci){
        if(identifiedcitizens.ContainsKey(ci)){
            identifiedcitizens.Remove(ci);
            
           rpnametag.updateidentification(To.Single(owner),ci, "unidentify");
            if(timetillid.ContainsKey(ci)){
                timetillid[ci]=0;
            }
        }
    }
    
    public void increaseidpercent(Citizen ci, float dist){

        if(timetillid[ci] >= 100){
            identifiedcitizens[ci]=0;
           return;
        }
        //if(dist < 200) timetillid[ci] += .05f;
        if(dist < 200) timetillid[ci] += 1f;
        else if (dist < 500) timetillid[ci] += .03f;
        else if (dist < 1000) timetillid[ci] += .01f;
        if(timetillid[ci] >= 100){
            timetillid[ci] = 100;
            identify(ci);

        }
            //if(sendmessage){
          // rpnametag.updateidentification(To.Single(owner),ci,"ip"+timetillid[ci]);
           // sendmessage = false;
           // }
           rpnametag.updateidentification(To.Single(owner),ci, ""+timetillid[ci]);
    }
    public void decreaseidpercent(Citizen ci){
        if(timetillid[ci]<100){
        timetillid[ci] -=.01f;
        rpnametag.updateidentification(To.Single(owner),ci, ""+timetillid[ci]);
            //
                        //if(!sendmessage){
           //rpnametag.updateidentification(To.Single(owner),ci,"dp"+timetillid[ci]);
           // sendmessage = true;
            //}
        }
        else if(identifiedcitizens[ci]>5){
            unidentify(ci);
        }

    }
}


}
//player missing disconnect
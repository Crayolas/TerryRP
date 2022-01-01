using Sandbox;
using Sandbox.UI;
using System;
namespace Roleplay{


    public class Notification : Panel{
        public static int deleteconfirmation = -1;
        public static int[] nt = new int[100];
        int index = -1;
        bool islarge = false;

        int notifypersisttime = 5;
        TimeSince elapsedtime;
        Button confirmationbutton;
        Button denybutton;
        public int confirmationid = -1;
        
        public Notification(string info, string type = "default", int time = 5, bool confirmation = false,  int conid = -1, Citizen sender = null){
            Sound.FromScreen("notification", .75f, 1);
            if (type == "success")Sound.FromScreen("success", .75f, 1);
            AddChild(new NotificationIcon(type, sender));
            AddChild(new NotificationInfo(info));
            AddChild(new NotificationTime(time));
            NotificationTime timebar = new NotificationTime(time);
            
            if (confirmation == true){
            confirmationid = conid;
            notifypersisttime=0;
            timebar.Style.BackgroundColor= new Color(0, .466f, 1f);
            confirmationbutton = new Button("Yes", "", confirm);
            denybutton = new Button("No", "", deny);
            confirmationbutton.AddClass("confirmbutton");
            denybutton.AddClass("denybutton");
            AddChild(confirmationbutton);
            AddChild(denybutton);
            }else{
            notifypersisttime = time;
            }
            if(info.Length>50){
            islarge = true;
            }
            AddChild(timebar);
            int infosize = 10;
 
            for(int i = 0; i<nt.Length; i++){
   
                if(nt[i] == 1){
                    infosize +=60;
                }
                if(nt[i]==2){
                    infosize +=120;
                    i++;
                }
                if(nt[i]==0){
                    if(islarge == false){
                        nt[i] = 1;
                        index = i;
                        Style.Bottom = infosize;

                        break;
                    }
                    if(islarge == true && i!=19){
                        if(nt[i+1]==0){
                            nt[i] = 2;
                            nt[i+1] = 2;
                            Style.Bottom = infosize;
                            Style.Height = 110;
                            index = i;
                            break;
                        }
                        infosize +=60;
                    }
                }


            }
            

            elapsedtime = 0;

        }
        public void confirm(){
            Sound.FromScreen("click");
            RoleplayGame.confirm(confirmationid);
        }

        public void deny(){
            Sound.FromScreen("click");
            RoleplayGame.deny(confirmationid);
        }

        public override void Tick(){
            base.Tick();
            if (deleteconfirmation!=-1 && confirmationid == deleteconfirmation){
                 if(islarge){
            
            nt[index] = 0;
            nt[index+1]=0;
                



            }
            else{
            nt[index]=0;
            }
                deleteconfirmation = -1;
                Delete();
                
                return;
            }
            if(elapsedtime > notifypersisttime&&notifypersisttime!=0){
            if(index == -1) Delete();
                 if(islarge){
            
            nt[index] = 0;
            nt[index+1]=0;
                



            }
            else{
            nt[index]=0;
            }

                Delete();
                return;

            };
            if (index!=0){
                if (nt[index-1]==0){
                    if(nt[index]==2){
                        nt[index+1] = 0;
                        nt[index-1] = 2;
                        Style.Bottom = Style.Bottom.Value.GetPixels(1)-60;
                        index--;
                        return;
                    }
                    if (nt[index]==1){
                        nt[index]=0;
                        nt[index-1]=1;
                        Style.Bottom = Style.Bottom.Value.GetPixels(1)-60;
                        index--;
                        return;
                    }
                }




            
        }

        }
    }
    public class NotificationIcon : Panel{
        public NotificationIcon(string type, Citizen sender = null){
            type = type.ToLower();
            if (type == "money"){
                Style.SetBackgroundImage("/ui/cash0.png");
            return;
            }
            if (type == "success"){
                Style.SetBackgroundImage("/ui/checkmark.png");
                return;
             }
             if (type == "friendrequest"){
                 Style.SetBackgroundImage("/ui/friendrequest.png");
                Style.BackgroundTint= new Color(0, .316f, 1f);
                return;
             }
             if (type == "orginvite"){
                 Style.SetBackgroundImage("/ui/orginvite.png");
                 Style.BackgroundTint= new Color(0, .316f, 1f);
                 return;
             }
             if (type == "partyinvite" && sender != null){
                Style.SetBackgroundImage("/ui/" + Enum.GetName(sender.partydata.Item1)+".png");
                Style.BackgroundTint= sender.partydata.Item2;
                return;
             }
            if (type == "default" || true){
                Style.SetBackgroundImage("/ui/infoorange.png");
            }
           
        }
    }
    public class NotificationInfo : Label{
        public NotificationInfo(string info){
            Text = info;
        }

    }
        public class NotificationTime : Panel{
        TimeSince t;
        int t2;
        public NotificationTime(int time){
            t = 0;
            t2 = time;
     
        }
    public override void Tick(){
        base.Tick();
        if(t2 != 0){
        Style.Width = t/(float)t2*300;

        if(t>t2)Delete();
        }else{
            Style.Width = 300;
        }


    }
    }
}

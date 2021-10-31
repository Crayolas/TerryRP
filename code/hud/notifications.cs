using Sandbox;
using Sandbox.UI;
namespace Roleplay{


    public class Notification : Panel{
        static int[] nt = new int[100];
        int index = -1;
        bool islarge = false;

        int notifypersisttime = 5;
        TimeSince elapsedtime;
        
        public Notification(string info, string type = "default", int time = 5){
            
            AddChild(new NotificationIcon(type));
            AddChild(new NotificationInfo(info));
            AddChild(new NotificationTime(time));
            notifypersisttime = time;
            if(info.Length>50){
            islarge = true;
            }
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
        public override void Tick(){
            base.Tick();

            
            if(elapsedtime > notifypersisttime){
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
        public NotificationIcon(string type){
            type = type.ToLower();
            if (type == "money"){
                Style.SetBackgroundImage("/ui/cash0.png");
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
        Style.Width = t/(float)t2*300;

        if(t>t2)Delete();

    }
    }
}
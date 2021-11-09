using Sandbox;
using Sandbox.UI;
namespace Roleplay{
public class LogoutMenu:Panel{
    public static LogoutMenu current;
    
    Button LogoutButton;
    Panel progressbar;
    
    int Timetologout;
    TimeSince timesincelogout;
    public LogoutMenu(int timetologout){

        Timetologout = timetologout;
        timesincelogout = 0;
        LogoutButton = new Button("", null, CancelLogout);
        AddChild(LogoutButton);
        progressbar = new Panel();
        progressbar.AddClass("progressbar");
        LogoutButton.AddChild(progressbar);

        
    }
    public override void Tick(){
        progressbar.Style.Width = 255*(timesincelogout/Timetologout);
        base.Tick();


        LogoutButton.Text =  "Cancel Logout ("+(Timetologout-(int)timesincelogout)+"s)";
    }

    public void CancelLogout(){
        Sound.FromScreen("click");
        RoleplayGame.CancelLogout();
    }
}
}
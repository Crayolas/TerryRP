using Sandbox;
using Sandbox.UI;
namespace Roleplay{
    public class RPChat:ChatBox{
        public RPChat(){
            Vector2 vec = new Vector2(300,0);
            
            Input.CaretColor = Color.White;
            Input.ScrollOffset = vec;
        }
       
    
     
    
        



    }
    public class ChatItem : Panel{

    }
    public class ChatEntry: TextEntry{
   
    }

}

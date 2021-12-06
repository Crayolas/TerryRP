using Sandbox;
namespace Roleplay{
    public struct ProfileStruct{
        public int CharacterID{get;set;}
        public string Name {get;set;} 
    }
    public partial class Profile : BaseNetworkable{
        [Net]
        public int CharacterID{get;set;}
        [Net]
        public string Name {get;set;} 
    }
}
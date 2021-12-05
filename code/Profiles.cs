using Sandbox;
namespace Roleplay{
    public partial class Profile : BaseNetworkable{
        [Net]
        public int CharacterID{get;set;}
        [Net]
        public string Name {get;set;} 
    }
}
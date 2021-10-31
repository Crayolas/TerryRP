
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;



//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Roleplay
{

	

	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// </summary>
	public class Serverdata{
		public int currentcharacterid {get; set;} = 1;

	}
	public class Characterdata{
		public int balance {get; set;}
		public String Name {get; set;}
		public int characterid {get; set;}
	}
	public class Playerdata{
		public List<Characterdata> Characters{get; set;} = new List<Characterdata>();
	}
	public partial class RoleplayGame : Sandbox.Game
	{

		
		public BaseFileSystem serverdata;

		public WebSocket socket;

		public bool usewebsocket = false;
		public bool GetConnectedViaWS(){
			if(socket.IsConnected == true)return true;
			else return false;
		}
		public RoleplayGame()
		{
			
			if ( IsServer )
			{

				Log.Info( "My Gamemode Has Created Serverside!" );

				// Create a HUD entity. This entity is globally networked
				// and when it is created clientside it creates the actual
				// UI panels. You don't have to create your HUD via an entity,
				// this just feels like a nice neat way to do it.
				RoleplayHud rphud = new RoleplayHud();
				Treasury treasury = new Treasury();
				if(usewebsocket){
				socket = new WebSocket();
				socket.Connect("ws://localhost:5000");

				}else{
					serverdata = FileSystem.Data;
					serverdata.CreateDirectory("terryrpdata/playerdata");
					serverdata.CreateDirectory("terryrpdata/serverdata");
					if(!FileSystem.Data.FileExists("terryrpdata/serverdata/serverdata.json")){
						FileSystem.Data.WriteJson<Serverdata>("terryrpdata/serverdata/serverdata.json", new Serverdata());
					}
				}

			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
			}
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );
			if(client.IsBot == true){
								var player = new Citizen();
								client.Pawn = player;
								player.Pawn = client.Pawn;
								player.Respawn();
								Citizen.NewCitizen(player);
								return;
			}
			
			if(!usewebsocket){
				if(!serverdata.FileExists("terryrpdata/playerdata/"+client.SteamId+".json")){

				serverdata.WriteJson<Playerdata>("terryrpdata/playerdata/"+client.SteamId+".json", new Playerdata());
		}
			}
			LoadPlayerData(client);

		}

		[ClientRpc]
		public void gotocharacterselect(){
			if(CharacterSelectPage.oncharacterselect == false){
			Local.Hud.AddChild<CharacterSelectPage>();
			}
		}
		[ClientRpc]
		public void updatecharacterlist(Citizen character){
			CharacterSelectPage.updatecharacterlist(character);
			
		}
		[ClientRpc]
		public void closecharacterselect(){
			CharacterSelectPage.current.Delete();
		}



				public async void UpdateInfo<T, V>(T type, string key ,V value)
				

		{	if (usewebsocket){
		if (type is Client){

			if(key == "newcharacter"){
				
				await socket.Send("[\"newcharacter\" , \"" +(type as Client).SteamId+"\", {\"Name\" : \""+value+"\"}]");
			}






		}
			if (type is Player){
				
				
				if(key == "balance"){
					//await socket.Send("{\"request\" : \"updateplayerdata\" , \"_id\" : " +(type as Player).Client.SteamId+", \"data\" : {\"balance : "+value+"\"}}");
				//await socket.Send("[\"updatecharacterdata\" , \"" +(type as Citizen).characterid+"\", {\"balance\" : "+value+"}]");
				await socket.Send("[\"updatecharacterdata\" , " +(type as Citizen).characterid+", {\"balance\" : "+value+"}]");
				}
				
				
				
				
				
				
				}

			}else{

				if (type is Client){

			if(key == "newcharacter"){
				
			
		Serverdata sdata = serverdata.ReadJson<Serverdata>("terryrpdata/serverdata/serverdata.json");

				Playerdata playerdata = serverdata.ReadJson<Playerdata>("terryrpdata/playerdata/"+(type as Client).SteamId+".json");
				

				if(playerdata.Characters.Count < 3){
			playerdata.Characters.Add(new Characterdata(){Name = ""+value, balance = 5000, characterid = sdata.currentcharacterid});
		
				sdata.currentcharacterid++;
			
			serverdata.WriteJson("terryrpdata/playerdata/"+(type as Client).SteamId+".json",playerdata);

			serverdata.WriteJson("terryrpdata/serverdata/serverdata.json", sdata);
				LoadPlayerData(type as Client);
				
			}
			}
			}
				
				
				if (type is Player){
				

				Playerdata playerdata = serverdata.ReadJson<Playerdata>("terryrpdata/playerdata/"+(type as Player).Client.SteamId+".json");
				
				if(key == "balance"){
					foreach(Characterdata ci in playerdata.Characters){
						
						if(ci.characterid == (type as Citizen).characterid){
							string convert = ""+value;
							
								ci.balance = int.Parse(convert);
								serverdata.WriteJson("terryrpdata/playerdata/"+(type as Player).Client.SteamId+".json",playerdata);				
						}
					}
				
				
				
				}
				
				
				
				}








			}}

			public async void LoadPlayerData(Client client){
				
				if(usewebsocket){
				socket.OnMessageReceived += onmessagereceive;
				await socket.Send("[\"getplayerdata\" , \""+ client.SteamId+ "\"]");
			     }else{
					 
					 gotocharacterselect(To.Single(client));





					Playerdata playerdata = serverdata.ReadJson<Playerdata>("terryrpdata/playerdata/"+client.SteamId+".json");
					if(playerdata != null){
					foreach(Characterdata ci in playerdata.Characters){
						Citizen temp = new Citizen(){balance = ci.balance, Name = ci.Name, characterid = ci.characterid};
						updatecharacterlist(To.Single(client),temp);
					
			
								if(client.GetInt("currentcharacterid") == ci.characterid){
								Citizen player = temp;
								client.Pawn = player;
								player.Pawn = client.Pawn;
								closecharacterselect(To.Single(client));
								player.Respawn();
								Citizen.NewCitizen(player);
																
								
								
								
								}		
					}
					}

					

					
				 }
						}
						public void onmessagereceive(string jsonString){
							
						if(jsonString.Substring(0,9) == "newplayer"){
						var steamid = ulong.Parse(jsonString.Substring(12));
						
						Client cl = null;
						foreach(var client in Client.All){
							if(client.SteamId == steamid)
							cl = client;
							gotocharacterselect(To.Single(cl));
						}
							}else{


							
					clientdata clientsave =  JsonSerializer.Deserialize<clientdata>(jsonString);
									ulong saveid = ulong.Parse(clientsave._id);
									foreach(var cl in Client.All){
										
					
						
						if(cl.SteamId == saveid){
								

								gotocharacterselect(To.Single(cl));
								foreach(Citizen ci in clientsave.Characters){
			
								if(cl.GetInt("currentcharacterid") == ci.characterid){
								Citizen player = ci;
								cl.Pawn = player;
								player.Pawn = cl.Pawn;
								closecharacterselect(To.Single(cl));
								player.Respawn();
								Citizen.NewCitizen(player);
																
								
								
								
								}

								updatecharacterlist(To.Single(cl),ci);
								}
					
						}
									}
						}
						socket.OnMessageReceived -= onmessagereceive;
						}
						 
		/*
				

				public void onmessagereceive(string jsonString){
					if(jsonString.Substring(0,9) == "newplayer"){
						var steamid = ulong.Parse(jsonString.Substring(12));
						Log.Info(steamid);
						Client cl = null;
						foreach(var client in Client.All){
							if(client.SteamId == steamid)
							cl = client;
						}
						if (cl != null){
						var player = new Citizen();
							cl.Pawn = player;
							player.Pawn = cl.Pawn;
							player.addMoney(5000);
							player.Respawn();
							Citizen.NewCitizen(player);
							Log.Info("New player connected.");
						}
					}else{
					Log.Info(jsonString);
					clientdata clientsave =  JsonSerializer.Deserialize<clientdata>(jsonString);
									ulong saveid = ulong.Parse(clientsave._id);
									foreach(var cl in Client.All){
										Log.Info("GAYYYYY");
						Log.Info(saveid);
						Log.Info(cl.SteamId);
						
						if(cl.SteamId == saveid){
								var player = new Citizen();
								cl.Pawn = player;
								player.Pawn = cl.Pawn;
								player.balance = clientsave.data.balance;
								player.Respawn();
								Citizen.NewCitizen(player);
					
						}
					}
					}

				

			}
			*/
							
									[ServerCmd]
						public async static void CreateCharacter(string firstname, string lastname){

							(Current as RoleplayGame).UpdateInfo<Client,String>(ConsoleSystem.Caller, "newcharacter", firstname+ " "+lastname);

							if(((RoleplayGame)Current).usewebsocket){
							((RoleplayGame)Current).socket.OnMessageReceived += ((RoleplayGame)Current).onmessagereceive;
							}
						}
						[ServerCmd]
						public async static void SelectCharacter(int characterid){
							if(ConsoleSystem.Caller.GetInt("currentcharacterid")==0){
							ConsoleSystem.Caller.SetInt("currentcharacterid", characterid);
							((RoleplayGame)Current).LoadPlayerData(ConsoleSystem.Caller);
							}
						}	
						public struct clientdata{ public string _id {get; set;}
												public List<Citizen> Characters {get; set;}
												//public Citizen data {get; set;}
										//public Citizen data;
										


					}

}
}

			
		
		

		
	



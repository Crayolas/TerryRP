
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
	public class OrgData{

		public string Name{get;set;}
		public int OrgID{get;set;}
		public int LeaderID{get;set;}
		public List<int>Members{get;set;}= new List<int>();
	}

	public class OrgsData{
		public List<OrgData> AllOrgs{get;set;} = new List<OrgData>();
	}
	public class Serverdata{
		public int currentcharacterid {get; set;} = 1;
		public int currentorgid {get; set;}=1;

	}
	public class Characterdata{
		public int balance {get; set;}
		public string Name {get; set;}
		public int characterid {get; set;}
	}
	public class Playerdata{
		public List<Characterdata> Characters{get; set;} = new List<Characterdata>();
	}
	public partial class RoleplayGame : Sandbox.Game
	{
		[Net]
		public List<Org> AllOrgs {get;set;} = new List<Org>();
		[Net]
		public List<Profile> AllProfiles{get;set;} = new List<Profile>();
		public Dictionary<int, Profile> ProfileDictionary {get; set;}= new();		
		public List<Party> StartParties = new();
		[Net, Local]
		public List<Party> AllParties {get;set;} = new();
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
					int ind = 0;
					foreach(partyicon icon in Enum.GetValues(typeof(partyicon))){
					if (icon!=partyicon.noparty){
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Cyan;
					ind++;
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Red;
					ind++;
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Magenta;
					
					ind++;
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Green;
					
					ind++;
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Yellow;
					ind++;
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Orange;
					ind++;
					StartParties.Add(new Party(null));
					StartParties[ind].icon = icon;
					StartParties[ind].color = Color.Gray;
					ind++;
					}
				}
				
				foreach(Party p in StartParties){
					//AllParties.Insert(Random.Shared.Next(0, AllParties.Count+1),p);
					AllParties.Add(p);
				}
				StartParties = null;
				RoleplayHud rphud = new RoleplayHud();
				Treasury treasury = new Treasury();
				if(usewebsocket){
				socket = new WebSocket();
				socket.Connect("ws://localhost:5000");

				}else{
					serverdata = FileSystem.Data;
					serverdata.CreateDirectory("terryrpdata/playerdata");
					serverdata.CreateDirectory("terryrpdata/serverdata");


					IEnumerable<string> playerfiles = serverdata.FindFile( "terryrpdata/playerdata");
					foreach(string filename in playerfiles){
						Playerdata pd = serverdata.ReadJson<Playerdata>("terryrpdata/playerdata/"+filename);
						foreach(Characterdata character in pd.Characters){
							Profile profile = new Profile(){CharacterID = character.characterid, Name = character.Name};
							AllProfiles.Add(profile);
							
						
						}
					}


					if(!FileSystem.Data.FileExists("terryrpdata/serverdata/serverdata.json")){
						FileSystem.Data.WriteJson<Serverdata>("terryrpdata/serverdata/serverdata.json", new Serverdata());
					}
					if(serverdata.FileExists("terryrpdata/serverdata/organizations.json")){
						OrgsData savedorgs = serverdata.ReadJson<OrgsData>("terryrpdata/serverdata/organizations.json");
						foreach(OrgData o in savedorgs.AllOrgs){
							AllOrgs.Add(new Org(){Name = o.Name, OrgID = o.OrgID, LeaderID = o.LeaderID, Members=o.Members});

						}
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
			onjoin(To.Single(client));
			if(!usewebsocket){
				if(!serverdata.FileExists("terryrpdata/playerdata/"+client.SteamId+".json")){

				serverdata.WriteJson<Playerdata>("terryrpdata/playerdata/"+client.SteamId+".json", new Playerdata());
		}
			}
			LoadPlayerData(client);

		}
		[ClientRpc]
		public void onjoin(){

							foreach(Profile p in AllProfiles){
					ProfileDictionary.Add(p.CharacterID, p);
				}
		}
		[ClientRpc]
		//make it so it just gives the index of the new profile, or find a way to make it so it adds and removes profiles from the dict automatically when the list changes
		public void newprofile(){
			ProfileDictionary.Add(AllProfiles[AllProfiles.Count-1].CharacterID, AllProfiles[AllProfiles.Count-1]);
		}

		[ClientRpc]
		public void exitlogoutscreen(){
			if(LogoutMenu.current != null){
				LogoutMenu.current.Delete();
				LogoutMenu.current = null;
				PartiesBase.movedown = 0;
			}
		}

		[ClientRpc]
		public void removeconfirmation(int id){
	
			Notification.deleteconfirmation=id;
			
		}

		[ClientRpc]
		public void gotologoutscreen(int timetologout){
			if(LogoutMenu.current == null){
			PartiesBase.movedown = 2;
			LogoutMenu p = new LogoutMenu(timetologout);
			LogoutMenu.current = p;
			
			Local.Hud.AddChild(p);
			}
		}
		[ClientRpc]
		public void gotocharacterselect(){
			//profilecache = new();
			if(CharacterSelectPage.oncharacterselect == false){
			rpnametag.cltimetillid.Clear();
			rpnametag.clidentifiedcitizens.Clear();
			rpnametag.nametags.Clear();
			rpnametag.nearbyplayers.Clear();
			LogoutMenu.current = null;
			Notification.nt = new int[100];
			Local.Hud.DeleteChildren();
			Local.Hud.AddChild<CharacterSelectPage>();
			
			}
		}
		[ClientRpc]
		public void updatecharacterlist(Citizen character){
			CharacterSelectPage.updatecharacterlist(character);
			
		}
		[ClientRpc]
		public void closecharacterselect(){
			CharacterSelectPage.oncharacterselect = false;
			
			CharacterSelectPage.current.Delete();
			
			//if (Local.Pawn != null){
			//	if ((Local.Pawn as Citizen).org != null){
			//		foreach(int p in (Local.Pawn as Citizen).org.Members){
			//			foreach(Profile profile in AllProfiles){
			//				if(profile.CharacterID == p){
			//					profilecache.Add(profile);
			//				}
			//			}
			//		}
			//	}
			//}
			
	
		}
				[ClientRpc] 
		public void ShowNoOrg(){
			if(RPMenu.current!=null && (RPMenu.current.selectedpanel is OrganizationPanel)){
				(RPMenu.current.selectedpanel as OrganizationPanel).SwitchTo("noorgpanel");
			}
		}
		[ClientRpc]
		public void sendtoorgpage(){

			if(RPMenu.current.selectedpanel is OrganizationPanel){
				
				(RPMenu.current.selectedpanel as OrganizationPanel).SwitchTo("orghomepanel");
				
			}
		}
		[ClientRpc]
		public void joinparty(){
			if (PartiesBase.current != null){
			PartiesBase.current.Delete();
			}
			PartiesBase.current = new PartiesBase();
			Local.Hud.AddChild(PartiesBase.current);
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
					Characterdata character = new Characterdata(){Name = ""+value, balance = 5000, characterid = sdata.currentcharacterid};
			playerdata.Characters.Add(character);
			//AllProfiles.Add(new Profile(){CharacterID = character.characterid, Name = character.Name});
				sdata.currentcharacterid++;
			
			serverdata.WriteJson("terryrpdata/playerdata/"+(type as Client).SteamId+".json",playerdata);

			serverdata.WriteJson("terryrpdata/serverdata/serverdata.json", sdata);
				Profile p = new Profile(){
					CharacterID = character.characterid,
					 Name = character.Name
				};
				AllProfiles.Add(p);
				newprofile(To.Everyone);
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
						Org org = Org.GetOrg(null, ci.characterid);

						string orgn;
						if(org == null){
							orgn = "";
						}else{
							orgn = org.Name;
						}
						 
						
						Citizen temp = new Citizen(){balance = ci.balance, Name = ci.Name, characterid = ci.characterid, orgname = orgn};
						updatecharacterlist(To.Single(client),temp);
					
			
								if(client.GetInt("currentcharacterid") == ci.characterid){
							
								Citizen player = temp;
								client.Pawn = player;
								player.Pawn = client.Pawn;
								closecharacterselect(To.Single(client));
								player.org = Org.GetOrg(null, ci.characterid);

								
								player.Respawn();
								
								Citizen.NewCitizen(player);
																
								
								
								}		
					}
							if (client.GetInt("currentcharacterid")!=0 && client.Pawn == null){
							client.Kick();
							
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
			*/			[ServerCmd]
						public static void SendMoney(int amount){
							if ((ConsoleSystem.Caller.Pawn as Citizen).transferMoney((ConsoleSystem.Caller.Pawn as Citizen).interactingwith,amount)){

							}else{
								Citizen.Notify("You don't have enough money for this.");
							}
						}
						[ServerCmd]
						public static void LeaveOrg(){
						
							if ((ConsoleSystem.Caller.Pawn as Citizen).org != null){
								if ((ConsoleSystem.Caller.Pawn as Citizen).org.LeaderID == (ConsoleSystem.Caller.Pawn as Citizen).characterid){
									
									Org.DisbandOrg((ConsoleSystem.Caller.Pawn as Citizen).org);
								}else{
									Org.RemoveFromOrg((ConsoleSystem.Caller.Pawn as Citizen).org, (ConsoleSystem.Caller.Pawn as Citizen).characterid);
								}
							}
						}

						[ServerCmd]
						public static void CreateOrganization(string orgname){
						if ((ConsoleSystem.Caller.Pawn as Citizen).org == null){
							
							if ((ConsoleSystem.Caller.Pawn as Citizen).balance >= Org.OrgCost){
								
								if (Org.CreateOrg(orgname, ConsoleSystem.Caller.Pawn as Citizen)!=null){
								(ConsoleSystem.Caller.Pawn as Citizen).addMoney(-Org.OrgCost);
								}
							}
						}
						}
						[ServerCmd]
						public static void CancelLogout(){
							(ConsoleSystem.Caller.Pawn as Citizen).startlogout = false;
							
							(Current as RoleplayGame).exitlogoutscreen(To.Single(ConsoleSystem.Caller));
						}

						[ServerCmd]
						public static void Logout(){
							(ConsoleSystem.Caller.Pawn as Citizen).startlogout = true;
							
							(Current as RoleplayGame).gotologoutscreen(To.Single(ConsoleSystem.Caller),Citizen.timetologout);
						}
							
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
						[ServerCmd]
						public static void OrgInvite(){
							Org org = (ConsoleSystem.Caller.Pawn as Citizen).org;
							Citizen target;
							Action functiontocall;
							if (org !=null){
							
							if ((ConsoleSystem.Caller.Pawn as Citizen).interactingwith!=null){
							if ((ConsoleSystem.Caller.Pawn as Citizen).interactingwith.characterid!=0){
							
							
								target = (ConsoleSystem.Caller.Pawn as Citizen).interactingwith;






							foreach(Confirmation c in target.confirmations){
								if (c.confirmtype == "orginvite" && c.Sender == (ConsoleSystem.Caller.Pawn as Citizen)){
									Citizen.Notify(To.Single(ConsoleSystem.Caller),"You have already invited this person.");
									return;
								}
							}






								functiontocall = () => TryJoinOrg(org, target, ConsoleSystem.Caller.Pawn as Citizen);
							if (target.org == null){
								Citizen.Notify(To.Single(ConsoleSystem.Caller), "Organization invite sent.", "success");
								//target.idlist.identify((ConsoleSystem.Caller.Pawn as Citizen));
								target.sendconfirmation("Would you like to join "+org.Name+ "?" , functiontocall, null, "orginvite", (ConsoleSystem.Caller.Pawn as Citizen));
							}else{
								Citizen.Notify(To.Single(ConsoleSystem.Caller),"This person is already in an organization.");
							}
							
							}else{
								Citizen.Notify(To.Single(ConsoleSystem.Caller),"Bots cannot join organizations.");
							}

							}
							}else{
								Citizen.Notify(To.Single(ConsoleSystem.Caller), "You're not in an organization.");
							}
							
						
								
								
							 

						}
						[ServerCmd]
						public static void PartyInvite(){
						
							Citizen ci;
							Citizen inviter;
							Action functiontocall;
							int pind = -1;
							inviter = (ConsoleSystem.Caller.Pawn as Citizen);
							
							if ((ConsoleSystem.Caller.Pawn as Citizen).partydata.Item1 == partyicon.noparty){
								pind = Party.CreateParty((ConsoleSystem.Caller.Pawn as Citizen));
							
							((RoleplayGame)Current).joinparty(To.Single(ConsoleSystem.Caller.Pawn as Citizen));
							}else if((ConsoleSystem.Caller.Pawn as Citizen).partyind != -1){
								 pind =  inviter.partyind;


							}
							if (pind != -1 && ((RoleplayGame)Game.Current).AllParties[pind].Leader == inviter){
						
							(ConsoleSystem.Caller.Pawn as Citizen).partydata = (((RoleplayGame)Game.Current).AllParties[pind].icon, ((RoleplayGame)Game.Current).AllParties[pind].color);
								ci = (ConsoleSystem.Caller.Pawn as Citizen).interactingwith;
								if (ci != null){
								foreach(Confirmation c in ci.confirmations){




								if (c.confirmtype == "partyinvite" && c.Sender == (ConsoleSystem.Caller.Pawn as Citizen)){
									Citizen.Notify(To.Single(ConsoleSystem.Caller),"You have already invited this person.");
									return;
								}







							}





								if (ci.partydata.Item1 == partyicon.noparty){

									
									
									
									
									functiontocall = () => TryJoinParty(((RoleplayGame)Game.Current).AllParties[pind], ci, inviter);
									//ci.idlist.identify(inviter);
									ci.sendconfirmation("Would you like to join "+inviter.Name+"'s party?", functiontocall, null, "partyinvite", inviter);
									Citizen.Notify(To.Single(ConsoleSystem.Caller), "Party invite sent.", "success");
								}else{
									Citizen.Notify(To.Single(ConsoleSystem.Caller), "This person is already in a party.");
								}
								}else{
									//no target found
								}

							}else{
								Citizen.Notify(To.Single(ConsoleSystem.Caller), "You aren't the leader of your party.");
							}
								
						}
						public static void TryJoinOrg(Org o, Citizen invitee, Citizen inviter){
								//PARTY BECOMES A MEM ADDRESS WHEN WOMEONE ?you invited?" LEAVES ANY PARTY?@?!? 
								//ONLY COMES BACK IF THAT PERSON MAKES A PARTY AND BREAKS IF THEY LEAVE GAME
							bool abletojoin;
							Org org = null;
							foreach(Org or in (Game.Current as RoleplayGame).AllOrgs){
								if (or.OrgID == o.OrgID){
									org = or;
								}
							}
							if (org == null){
								Citizen.Notify(To.Single(invitee), "This organization no longer exists.");
							}else{
								abletojoin = Org.AddToOrg(o,invitee.characterid);
								if(!abletojoin){
									Citizen.Notify(To.Single(invitee), "You are already in an organization.");
								}else{
									Citizen.Notify(To.Single(invitee), "You have joined "+ org.Name+"!", "success");

								}
							}


						}
						public static void TryJoinParty(Party p, Citizen invitee, Citizen inviter){

							if (invitee.partydata.Item1 == partyicon.noparty){
						
							if (p!=null && p.members.Contains(inviter)){
							p.AddMember(invitee);
							
							((RoleplayGame)Current).joinparty(To.Single(invitee));
						
							}else{
								Citizen.Notify(To.Single(invitee), "This invite is no longer valid.");
							}}else{
								Citizen.Notify(To.Single(invitee), "You are already in a party.");
							}
						}
						[ServerCmd]
						public static void confirm(int confirmationid){
							int ind = 0;
							foreach(Confirmation c in (ConsoleSystem.Caller.Pawn as Citizen).confirmations){
								
								if (confirmationid == c.id){
							
							
								c.doconfirm();
								(ConsoleSystem.Caller.Pawn as Citizen).confirmations.RemoveAt(ind);
							
								
								
							     ((RoleplayGame)Current).removeconfirmation(To.Single(ConsoleSystem.Caller),confirmationid);
								 return;
							}
							ind++;
							}
						}
						[ServerCmd]
						public static void deny(int confirmationid){
																int ind = 0;
							foreach(Confirmation c in (ConsoleSystem.Caller.Pawn as Citizen).confirmations){
								
								if (confirmationid == c.id){
							
							
								c.dodeny();
								(ConsoleSystem.Caller.Pawn as Citizen).confirmations.RemoveAt(ind);
							
								
								
							     ((RoleplayGame)Current).removeconfirmation(To.Single(ConsoleSystem.Caller),confirmationid);
								 return;
							}
							ind++;
							}
						}
						[ServerCmd]
						public static void party(string command, int characterid = -1){
										
							if ((ConsoleSystem.Caller.Pawn as Citizen).partydata.Item1 != partyicon.noparty){
							Party party = ((RoleplayGame)Game.Current).AllParties[(ConsoleSystem.Caller.Pawn as Citizen).partyind];
							if(command == "kick" && characterid != -1){
							if (party.Leader == (ConsoleSystem.Caller.Pawn as Citizen)){
								
								foreach(Citizen citi in party.members){
									if (citi.characterid == characterid){
										Citizen.Notify(To.Single(citi), "You were removed from your party.");
										party.RemoveMember(citi);
										
										return;
									}
								}

							}
							}
							if(command == "leave"){

								party.RemoveMember((ConsoleSystem.Caller.Pawn as Citizen));
							}
							if(command == "promote" && characterid != -1){
								if (party.Leader == (ConsoleSystem.Caller.Pawn as Citizen)){
									foreach(Citizen citi in party.members){
									if (citi.characterid == characterid && citi != party.Leader && ((RoleplayGame)Game.Current).AllParties[citi.partyind] == party){
										party.PromoteMember(citi);
										Citizen.Notify(To.Single(citi), "You are now party leader.");
									}
									}
								}
							}
							}

						}
						public struct clientdata{ public string _id {get; set;}
												public List<Citizen> Characters {get; set;}
												//public Citizen data {get; set;}
										//public Citizen data;
										


					}

}
}

			
		
		

		
	



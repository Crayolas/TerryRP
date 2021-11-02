using Sandbox;
using System;
using System.Linq;
using Sandbox.UI;
using System.Collections.Generic;




namespace Roleplay
{	
	
	partial class Citizen : Player, IUse
	{
		
		//rpnametags nametags = null;
		[Net, Local]
		public int characterid {get; set;}
		private TimeSince timesincelogout=0;
		public static int timetologout = 1;
		MenuBase menu;
		Status status;
		RPChat chat;
		public bool startlogout = false;
		
		//rpnametags nametags = new rpnametags();

		public Entity Pawn;
		//public List<Citizen> tagsvisible = new List<Citizen>();
		
		public identificationlist idlist;
		public Citizen(){		
			Inventory = new Inventory(this);  
			idlist = new identificationlist(this);

		}
		protected override void OnDestroy(){
			base.OnDestroy();
			AllCitizens.Remove(this);
		}

		public static List<Citizen> AllCitizens{get; set;} = new List<Citizen>();
		public List<Citizen> tagsvisible{get;set;} = new List<Citizen>();
		public static Citizen FindByPawn(Entity targetpawn){
		foreach(Citizen pl in AllCitizens){
			if(targetpawn == pl.Pawn) return pl;
		}
			return null;
		}
			public static void NewCitizen(Citizen pl){
				//bool istheremayor = false;
			foreach(Citizen ci in AllCitizens){
			//		if(ci.IsMayor == true)  istheremayor = true;
					

			//}
				//if (!istheremayor){
				//	pl.IsMayor = true;
				//	Log.Info(pl.Client.Name+" has become mayor by the rite of God.");
			ci.idlist.addtoidlist(pl);
			
				}
	
	

			AllCitizens.Add(pl);
			
			
			
			}

		public override void ClientSpawn(){

			base.ClientSpawn();
			
			if(IsLocalPawn){
				chat = Local.Hud.AddChild<RPChat>();
				
			}

		}
		public override void Respawn()
		{
			

			ShowHud(To.Single(this));
			


			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();
			

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Inventory.Add(new Fists(), true);
			
			base.Respawn();



		}
		

	
		[ClientRpc]
				public void RemoveNameTag(Citizen citi){
					rpnametag.DeleteTag(citi);
				}
		[ClientRpc]
				public void AddNameTag(Citizen citi){
	
					new rpnametag(citi);


					
					
					
		
		//public static void AddNameTag(Citizen newcitizen){
			//rpnametag temp = new rpnametag(newcitizen);
			//temp.Transform=newcitizen.Transform;
			//temp.WorldScale = .5f;
		}
		[ClientRpc]	
		public void ShowHud(){
		
			status = Local.Hud.AddChild<Status>();
		}
		[ClientRpc]
		public void HideHud(){
			if (status != null){
			status.Delete();
			status = null;
			}
		}
		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			if(IsClient){


			}
			if (IsServer){
				var temp = new List<Citizen>();
				foreach(Citizen ci in AllCitizens){
					float dist = Position.Distance(ci.Position);
					if(ci != this && dist<200){
						bool newtolist = true;
						foreach(Citizen vici in tagsvisible){
							if(vici == ci)newtolist = false;
						
						
						
						}
						temp.Add(ci);
						if(newtolist){
							AddNameTag(To.Single(this), ci);
						}
					}

					if(ci != this){
					if(dist<1000){
						idlist.increaseidpercent(ci, dist);
					}else if(dist >= 1000){
						idlist.decreaseidpercent(ci);
					
					}}
					

				}

				foreach(Citizen ci in tagsvisible){
					bool delete = true;
					foreach(Citizen newci in temp){
						if(newci == ci){
							delete = false;
						}

					}
					if(delete){
						RemoveNameTag(To.Single(this),ci);
					}
				}


				tagsvisible = temp;
				

					if( !startlogout){
						timesincelogout = 0;
					}
					
					if(timesincelogout >= timetologout){
						startlogout = false;
						HideHud(To.Single(this));
						AllCitizens.Remove(this);
						cl.Pawn.Delete();
						cl.Pawn = null;
						cl.SetInt("currentcharacterid", 0);
						(Game.Current as RoleplayGame).LoadPlayerData(cl);
						

						
					}
				
			}

	

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );
			if (IsClient && Input.Pressed(InputButton.Menu)){
				if(menu == null){
					menu = new MenuBase();
					Local.Hud.AddChild(menu);
				}
			}
			if(IsClient && Input.Released(InputButton.Menu)){
				if (menu!=null){
					if(menu.inmenu == false){
					menu.DeleteChildren();
					menu.Delete();
					menu = null;
					}
				}
			}
			//
			// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
			//
			if ( IsServer && Input.Pressed( InputButton.Use ) )
			{	
				TickPlayerUse();
				if (Using != null){
				//	if(Using.IsClient){
						//RPPlayer player = FindByPawn(Using.Client.Pawn);
	
						Citizen player = FindByPawn(Using);

						if (transferMoney(player , 100)){
				
						}
						else
						{
							Log.Info("You don't have enough money for this.");
						}
					//}

				
				}

			}

		}

	
		public override void OnKilled()
		{
			
			HideHud(To.Single(this));
 

			base.OnKilled();
			EnableDrawing = false;
		}
		public bool IsUsable(Entity user){
			return true;
		}
		public bool OnUse(Entity user){
			return true;
		}
	}
}

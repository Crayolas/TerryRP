using Sandbox.UI;


//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Roleplay
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class RoleplayHud : Sandbox.HudEntity<RootPanel>
	{



		

		public RoleplayHud()
		{
			if ( IsClient )
			{	//RootPanel.SetTemplate( "hud/Hud.html" );

				RootPanel.StyleSheet.Load("hud/Hud.scss");


			}
		}


	}
	

}


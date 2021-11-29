using Sandbox;

[Library( "weapon_smg", Title = "SMG", Spawnable = true )]
partial class SMG : BaseWeapon
{
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;


	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//

		PlaySound( "rust_smg.shoot" );

		//
		// Shoot the bullets
		//

	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}


	

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}

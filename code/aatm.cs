using Sandbox;
namespace Roleplay{
[Library("ent_atm")]
public partial class ATM : Prop, IUse
{
    public override void Spawn(){
    base.Spawn();
    SetModel("models/sbox_props/ticket_machine/ticket_machine.vmdl_c");
    SetupPhysicsFromModel(PhysicsMotionType.Static, false);

    }
    public bool IsUsable(Entity ent){
        return true;
    }
    public bool OnUse(Entity ent){
        return true;
    }
}
}
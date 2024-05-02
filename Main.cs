using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.Logging;

namespace RapidFireFix;

public class RapidFireFix : BasePlugin
{
    public override string ModuleName => "Rapid Fire Fix";

    public override string ModuleVersion => "1.0.1";

    public override string ModuleAuthor => "jon";

	[GameEventHandler]
	public HookResult OnBulletImpact(EventBulletImpact evt, GameEventInfo info)
	{
		if (evt.Userid?.Pawn?.Value?.WeaponServices?.ActiveWeapon?.Value == null)
			return HookResult.Continue;

		CBasePlayerWeapon firedWeapon = evt.Userid.Pawn.Value.WeaponServices.ActiveWeapon.Value!;

		CCSWeaponBaseVData? weaponData = firedWeapon.GetVData<CCSWeaponBaseVData>();

		if (weaponData == null)
			return HookResult.Continue;

		int tickBase = (int)evt.Userid.TickBase;

		int fixedPrimaryTick = (int)Math.Round(weaponData.CycleTime.Values[0] * 64) - 3;
		firedWeapon.NextPrimaryAttackTick = Math.Max(firedWeapon.NextPrimaryAttackTick, tickBase + fixedPrimaryTick);

		// R8 force fix
		if (firedWeapon.DesignerName == "weapon_revolver")
		{
			int fixedSecondaryTick = (int)Math.Round(weaponData.CycleTime.Values[1] * 64) - 3;
			firedWeapon.NextSecondaryAttackTick = Math.Max(firedWeapon.NextSecondaryAttackTick, tickBase + fixedSecondaryTick);
		}

		return HookResult.Continue;
	}
}
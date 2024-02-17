﻿using System.Runtime.CompilerServices;
using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;

namespace RapidFireFix;

public class RapidFireFix : BasePlugin
{
    public override string ModuleName => "Rapid Fire Fix";

    public override string ModuleVersion => "1.0.0";

    public override string ModuleAuthor => "jon";

    /*
    public override void Load(bool hotReload)
    {
        
        Server.NextWorldUpdate(() => {
            ConVar? tags = ConVar.Find("sv_tags");
            
            if (tags != null) {

                if (tags.StringValue == null) {
                    Server.ExecuteCommand("sv_tags rapidfirefix");
                } else if (!tags.StringValue.Contains("rapidfirefix")) {
                    Server.ExecuteCommand("sv_tags " + (tags.StringValue == "" ? "" : tags.StringValue + ",") + "rapidfirefix");
                }
            }
        });
        

        base.Load(hotReload);
    }
    */

    [GameEventHandler]
    public HookResult OnEventBulletFlightResolution(EventBulletFlightResolution @event, GameEventInfo info)
    {
        if (@event.Userid == null
            || !@event.Userid.IsValid
            || @event.Userid.Pawn.Value == null
            || !@event.Userid.Pawn.IsValid
            || @event.Userid.Pawn.Value.WeaponServices == null
            || @event.Userid.Pawn.Value.WeaponServices.ActiveWeapon == null
            || @event.Userid.Pawn.Value.WeaponServices.ActiveWeapon.Value == null)
            return HookResult.Continue;

        CBasePlayerWeapon firedWeapon = @event.Userid.Pawn.Value.WeaponServices.ActiveWeapon.Value!;

        CCSWeaponBaseVData? weaponData = firedWeapon.GetVData<CCSWeaponBaseVData>();

        if (weaponData == null)
            return HookResult.Continue;

        firedWeapon.NextPrimaryAttackTick   = Math.Max(firedWeapon.NextPrimaryAttackTick,   (int)@event.Userid.TickBase + (int)Math.Round(weaponData.CycleTime.Values[0] * 64) - 3);

        // R8 force fix
        if (firedWeapon.DesignerName == "weapon_revolver")
        {
            firedWeapon.NextSecondaryAttackTick = Math.Max(firedWeapon.NextSecondaryAttackTick, (int)@event.Userid.TickBase + (int)Math.Round(weaponData.CycleTime.Values[1] * 64) - 3);
        }
        
        return HookResult.Continue;
    }
}
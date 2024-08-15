using InteropDoom.Engine.Events;
using InteropDoom.Native.Definitions;
using InteropDoom.Native.Structures;
using InteropDoom.Utilities;

namespace AvaloniaPlayer.Doom.Events;

internal class OurEntityEventHandler(ILogger logger) : EntityEventHandler(logger)
{
    protected override void OnMapEntityDamaged(MapEntityDamaged data)
    {
        if (!data.Victim)
        {
            Logger?.LogError("No victim");
            return;
        }
        
        MapEntityType? dealerType = data.Dealer ? data.Dealer.Type : null;
        var dealerName = dealerType?.ToString() ?? "Something";
        if (data.Victim.IsPlayer)
        {
            Logger?.LogInfo($"Ouch! {dealerName} hurt you for {data.HealthDamage+data.ArmorDamage} damage ({data.HealthDamage} direct to HP, {data.ArmorDamage} absorbed by armor)");
        }
        else if (dealerType == MapEntityType.MT_PLAYER)
        {
            Logger?.LogInfo($"Hit {data.Victim.Type} for {data.HealthDamage} damage");
        }
    }

    protected override void OnMapEntityKilled(MapEntityKilled data)
    {
        if (!data.Victim)
        {
            Logger?.LogError("No victim");
            return;
        }
        
        MapEntityType? killerType = data.Killer ? data.Killer.Type : null;
        var killerName = killerType?.ToString() ?? "environment";
        if (data.Victim.IsPlayer)
        {
            Logger?.LogInfo($"Killed by {killerName}");
        }
        else if (killerType == MapEntityType.MT_PLAYER)
        {
            var player = data.Killer.Player;
            if (player.CurrentWeapon == WeaponType.Chainsaw)
                Logger?.LogWarning($"Fresh meat!");
            
            Logger?.LogInfo($"Killed {data.Victim.Type}");
            
            if (player.HasKeycard(KeycardType.RedSkull))
                Logger?.LogWarning($"The Red Skull absorbs the victim's soul...");
        }
    }
}

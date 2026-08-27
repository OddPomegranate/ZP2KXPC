using System.Text;

namespace ZP2K9.menu;

public class PerkDescriptions
{
	public struct PerkInfo(string name, string description)
	{
		public StringBuilder name = new StringBuilder(name);

		public StringBuilder description = new StringBuilder(description);
	}

	public PerkInfo[][] descriptions;

	public PerkDescriptions()
	{
		descriptions = new PerkInfo[3][];
		descriptions[0] = new PerkInfo[10]
		{
			new PerkInfo("Diesel Power", "Increased flight time."),
			new PerkInfo("Leadstorm", "Increased fire rate."),
			new PerkInfo("Kung Fu Hero", "Increased kick attack."),
			new PerkInfo("Samurai", "Spawn with katana."),
			new PerkInfo("Deadly Aim", "Increased bullet attack."),
			new PerkInfo("Chemist", "Increased fire/ice/poison attack."),
			new PerkInfo("Robot Hands", "Fast reload, fast blades."),
			new PerkInfo("Scavenger", "Killing enemies replenishes ammo."),
			new PerkInfo("Leech", "Hurting enemies replenishes health."),
			new PerkInfo("Shifter", "Faster dodge, kick to air recover.")
		};
		descriptions[1] = new PerkInfo[10]
		{
			new PerkInfo("Quick", "Increased run speed."),
			new PerkInfo("Ninja", "Stick to walls and ceilings."),
			new PerkInfo("Mr. Radar", "Radar sees still/stealth enemies."),
			new PerkInfo("Ammo Junkie", "Weapon pickups have double ammo."),
			new PerkInfo("Gunslinger", "Spawn with dual pistols."),
			new PerkInfo("Rocket Pants", "Increased flight thrust."),
			new PerkInfo("Mad Bomber", "Increased grenade pickups."),
			new PerkInfo("Mortar", "Increased grenade proficiency."),
			new PerkInfo("Grab Bag", "Spawn with random weapon."),
			new PerkInfo("Griefer", "Hit enemies to drain their ammo.")
		};
		descriptions[2] = new PerkInfo[10]
		{
			new PerkInfo("Hazmat Suit", "Increase fire/ice/poison defense."),
			new PerkInfo("Blast Armor", "Increased explosive defense."),
			new PerkInfo("Clumsy", "Drop grenade upon death."),
			new PerkInfo("Bulletproof", "Increased bullet defense."),
			new PerkInfo("Medic!", "Increased heal speed."),
			new PerkInfo("Tank", "Increased max health."),
			new PerkInfo("Stealth", "Radar invisibility."),
			new PerkInfo("Big Mags", "Increased magazine size."),
			new PerkInfo("Prepared", "Spawn with frag grenades."),
			new PerkInfo("Turbocharge", "Flight recharges faster.")
		};
	}
}

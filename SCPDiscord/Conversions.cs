using Exiled.API.Enums;
using System.Collections.Generic;

namespace SCPDiscord
{
	public static class Conversions
	{
		public static Dictionary<ItemType, string> items = new Dictionary<ItemType, string>()
		{
			{ ItemType.Ammo556, "556 Ammo" },
			{ ItemType.Ammo762, "762 Ammo" },
			{ ItemType.Ammo9mm, "9mm Ammo" },
			{ ItemType.GrenadeFlash, "Flashbang" },
			{ ItemType.GrenadeFrag, "Grenade" },
			{ ItemType.GunCOM15, "COM-15" },
			{ ItemType.GunE11SR, "Epsilon 11 Rifle" },
			{ ItemType.GunLogicer, "Logicer" },
			{ ItemType.GunMP7, "MP7" },
			{ ItemType.GunProject90, "P90" },
			{ ItemType.GunUSP, "USP" },
			{ ItemType.KeycardChaosInsurgency, "Chaos Insurgency Keycard" },
			{ ItemType.KeycardContainmentEngineer, "Containment Engineer Keycard" },
			{ ItemType.KeycardFacilityManager, "Facility Manager Keycard" },
			{ ItemType.KeycardGuard, "Guard Keycard" },
			{ ItemType.KeycardJanitor, "Janitor Keycard" },
			{ ItemType.KeycardNTFCommander, "Commander Keycard" },
			{ ItemType.KeycardNTFLieutenant, "Lieutenant Keycard" },
			{ ItemType.KeycardO5, "O5 Keycard" },
			{ ItemType.KeycardScientist, "Scientist Keycard" },
			{ ItemType.KeycardScientistMajor, "Major Scientist Keycard" },
			{ ItemType.KeycardSeniorGuard, "Senior Guard Keycard" },
			{ ItemType.KeycardZoneManager, "Zone Manager Keycard" },
			{ ItemType.WeaponManagerTablet, "Weapon Manager Tablet" },
			{ ItemType.SCP018, "SCP-018" },
			{ ItemType.SCP207, "SCP-207" },
			{ ItemType.SCP268, "SCP-268" },
			{ ItemType.SCP500, "SCP-500" }
		};

		public static Dictionary<RoleType, string> roles = new Dictionary<RoleType, string>()
		{
			{ RoleType.ChaosInsurgency, "Chaos Insurgency" },
			{ RoleType.ClassD, "Class-D" },
			{ RoleType.FacilityGuard, "Facility Guard" },
			{ RoleType.NtfCadet, "NTF Cadet" },
			{ RoleType.NtfCommander, "NTF Commander" },
			{ RoleType.NtfLieutenant, "NTF Lieutenant" },
			{ RoleType.NtfScientist, "NTF Scientist" },
			{ RoleType.Scp049, "SCP-049" },
			{ RoleType.Scp0492, "SCP-049-2" },
			{ RoleType.Scp079, "SCP-079" },
			{ RoleType.Scp096, "SCP-096" },
			{ RoleType.Scp106, "SCP-106" },
			{ RoleType.Scp173, "SCP-173" },
			{ RoleType.Scp93953, "SCP-939" },
			{ RoleType.Scp93989, "SCP-939" }
		};

		public static Dictionary<GrenadeType, string> grenades = new Dictionary<GrenadeType, string>()
		{
			{ GrenadeType.FragGrenade, "Granade" },
			{ GrenadeType.Flashbang, "Flashbang" },
			{ GrenadeType.Scp018, "SCP-018" }
		};

		public static Dictionary<Scp914.Scp914Knob, string> knobsettings = new Dictionary<Scp914.Scp914Knob, string>()
		{
			{ Scp914.Scp914Knob.Rough, "Rough" },
			{ Scp914.Scp914Knob.Coarse, "Course" },
			{ Scp914.Scp914Knob.OneToOne, "1:1" },
			{ Scp914.Scp914Knob.Fine, "Fine" },
			{ Scp914.Scp914Knob.VeryFine, "Very Fine" }
		};
	}
}

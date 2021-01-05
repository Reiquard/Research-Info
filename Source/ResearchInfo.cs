using HarmonyLib;
using Verse;

namespace ResearchInfo
{
	[StaticConstructorOnStartup]
	internal static class ResearchInfo
	{
		public static bool Clean { get; private set; } = true;
		public static bool ModPawnsChooseResearch { get; private set; }
		public static bool ModHumanResources { get; private set; }
		public static bool ModHospitality { get; private set; }
		static ResearchInfo()
		{
			new Harmony("reiquard.researchinfo").PatchAll();
			if (ModLister.GetActiveModWithIdentifier("Cozarkian.PawnsChooseResearch") != null)
			{
				Clean = false;
				ModPawnsChooseResearch = true;
			}
			if (ModLister.GetActiveModWithIdentifier("JPT.HumanResources") != null)
			{
				Clean = false;
				ModHumanResources = true;
			}
			if (ModLister.GetActiveModWithIdentifier("Orion.Hospitality") != null)
			{
				ModHospitality = true;
			}
		}
	}
}
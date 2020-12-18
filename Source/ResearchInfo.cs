using HarmonyLib;
using Verse;

namespace ResearchInfo
{
	[StaticConstructorOnStartup]
	internal static class ResearchInfo
	{
		public static bool clean = true;
		public static bool modPawnsChooseResearch;
		public static bool modHumanResources;
		public static bool modHospitality;
		public static Harmony Instance = new Harmony("reiquard.researchinfo");
		static ResearchInfo()
		{
			Instance.PatchAll();
			if (ModLister.GetActiveModWithIdentifier("Cozarkian.PawnsChooseResearch") != null)
			{
				clean = false;
				modPawnsChooseResearch = true;
			}
			if (ModLister.GetActiveModWithIdentifier("JPT.HumanResources") != null)
			{
				clean = false;
				modHumanResources = true;
			}
			if (ModLister.GetActiveModWithIdentifier("Orion.Hospitality") != null)
			{
				modHospitality = true;
			}
		}
	}
}
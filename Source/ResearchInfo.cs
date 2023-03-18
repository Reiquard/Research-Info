using HarmonyLib;
using Verse;

namespace ResearchInfo
{
	[StaticConstructorOnStartup]
	internal static class ResearchInfo
	{
		public static bool VanillaBehavior { get; private set; } = true;
		public static bool ModPawnsChooseResearch { get; private set; }
		public static bool ModHumanResources { get; private set; }
		public static bool ModResearchReinvented { get; private set; }
		public static bool ModHospitality { get; private set; }
		static ResearchInfo()
		{
			new Harmony("reiquard.researchinfo").PatchAll();
			bool _PCR = ModLister.GetActiveModWithIdentifier("Cozarkian.PawnsChooseResearch") != null;
			bool _PCRContinued = ModLister.GetActiveModWithIdentifier("Mlie.PawnsChooseResearch") != null;
			if (_PCR || _PCRContinued)
			{
				VanillaBehavior = false;
				ModPawnsChooseResearch = true;
			}
			if (ModLister.GetActiveModWithIdentifier("JPT.HumanResources") != null)
			{
				VanillaBehavior = false;
				ModHumanResources = true;
			}
			if (ModLister.GetActiveModWithIdentifier("PeteTimesSix.ResearchReinvented") != null)
			{
				ModResearchReinvented = true;
			}
			if (ModLister.GetActiveModWithIdentifier("Orion.Hospitality") != null)
			{
				ModHospitality = true;
			}
		}
	}
}
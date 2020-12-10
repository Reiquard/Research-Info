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
		public static Harmony Instance = new Harmony("reiquard.researchinfo");
		private static string _message;
		static ResearchInfo()
		{
			Instance.PatchAll();
			if (ModLister.GetActiveModWithIdentifier("Cozarkian.PawnsChooseResearch") != null)
			{
				clean = false;
				modPawnsChooseResearch = true;
				_message = "'Pawns Choose Research'.";
			}
			if (ModLister.GetActiveModWithIdentifier("JPT.HumanResources") != null)
			{
				clean = false;
				modHumanResources = true;
				_message = "'Human Resources'.";
			}
			Log.Message("[Research Info] Changing logic to support " + _message);
		}
	}
}
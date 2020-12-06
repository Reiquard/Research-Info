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
			if (LoadedModManager.RunningModsListForReading.Any((ModContentPack x) => x.PackageIdPlayerFacing == "Cozarkian.PawnsChooseResearch"))
			{
				clean = false;
				modPawnsChooseResearch = true;
				_message = "'Pawns Choose Research'.";
			}
			if (LoadedModManager.RunningModsListForReading.Any((ModContentPack x) => x.PackageIdPlayerFacing == "JPT.HumanResources"))
			{
				clean = false;
				modHumanResources = true;
				_message = "'Human Resources'.";
			}
			Log.Message("[Research Info] Changing logic to support " + _message);
		}
	}
}
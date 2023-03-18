using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ResearchInfo
{
    public static class Aux_PCR
    {
        private static Type _researchRecord = AccessTools.TypeByName("PawnsChooseResearch.ResearchRecord");
		private static MethodInfo _currentProject = _researchRecord.GetMethod("CurrentProject", BindingFlags.Public | BindingFlags.Static);
		public static bool PCR_VersionMismatch { get; private set; }
		public static ResearchProjectDef PCR_CurrentProject(Pawn pawn)
		{
			try
			{
				return (ResearchProjectDef)_currentProject.Invoke(_researchRecord, new Object[] { pawn, true });
			}
			catch (Exception)
			{
				PCR_VersionMismatch = true;
				Log.Error("You are using an incompatible version of the 'Pawns Choose Research' mod. You may want to look for updates!");
				return null;
			}
		}
	}
}
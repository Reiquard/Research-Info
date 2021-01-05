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
		public static bool VersionMismatch { get; private set; }
		public static ResearchProjectDef PCRCurrentProject(Pawn pawn)
		{
			try
			{
				return (ResearchProjectDef)_currentProject.Invoke(_researchRecord, new Object[] { pawn, true });
			}
			catch (Exception)
			{
				VersionMismatch = true;
				Log.Error("You are using an incompatible version of the 'Pawns Choose Research' mod. You may want to look for updates!");
				return null;
			}
		}
	}
}
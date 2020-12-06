using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ResearchInfo
{
    public static class Utilities_PCR
    {
        private static Type _researchRecord = AccessTools.TypeByName("PawnsChooseResearch.ResearchRecord");
		private static MethodInfo _currentProject = _researchRecord.GetMethod("CurrentProject", BindingFlags.Public | BindingFlags.Static);
		public static ResearchProjectDef PCRCurrentProject(Pawn pawn)
		{
			return (ResearchProjectDef)_currentProject.Invoke(_researchRecord, new Object[] { pawn });
		}
	}
}
using System;
using System.Reflection;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace ResearchInfo
{
	public static class Aux_HR
    {
		private static Type _CompKnowledge = AccessTools.TypeByName("HumanResources.CompKnowledge");
		private static FieldInfo _expertiseFI = AccessTools.Field(_CompKnowledge, "expertise");
		private static FieldInfo _techLevelFI = AccessTools.Field(_CompKnowledge, "techLevel");
		public static Dictionary<ResearchProjectDef, float> HR_Expertise(Pawn pawn)
		{
			return (Dictionary<ResearchProjectDef, float>)_expertiseFI.GetValue(pawn.AllComps.
				Where(x => _CompKnowledge.IsAssignableFrom(x.GetType())).FirstOrDefault());
		}
		public static TechLevel HR_TechLevel(Pawn pawn)
		{
			return (TechLevel)_techLevelFI.GetValue(pawn.AllComps.
				Where(x => _CompKnowledge.IsAssignableFrom(x.GetType())).FirstOrDefault());
		}
		public static float HR_PrerequisiteMultiplier(ResearchProjectDef project, Pawn pawn)
		{
			return HR_Expertise(pawn).Keys.Where(x => x.HR_IsKnownBy(pawn)).
				Where(x => !x.prerequisites.NullOrEmpty() && x.prerequisites.Contains(project)).Any() ? 2f : 1f;
		}

		private static Type _JobDriver_LearnTech = AccessTools.TypeByName("HumanResources.JobDriver_LearnTech");
		private static FieldInfo _projectFI = AccessTools.Field(_JobDriver_LearnTech, "project");
		public static ResearchProjectDef HR_CurrentProject(Pawn pawn)
		{
			return _JobDriver_LearnTech.IsAssignableFrom(pawn.jobs.curDriver.GetType()) ? (ResearchProjectDef)_projectFI.GetValue(pawn.jobs.curDriver) : null;
		}

		private static Type _Extension_Research = AccessTools.TypeByName("HumanResources.Extension_Research");
		private static PropertyInfo _ResearchPointsPerWorkTickPI = AccessTools.Property(_Extension_Research, "ResearchPointsPerWorkTick");
		private static PropertyInfo _StudyPointsPerWorkTickPI = AccessTools.Property(_Extension_Research, "StudyPointsPerWorkTick");
		private static MethodInfo _StuffCostFactorMI = _Extension_Research.GetMethod("StuffCostFactor", BindingFlags.Public | BindingFlags.Static);
		private static MethodInfo _IsKnownByMI = _Extension_Research.GetMethod("IsKnownBy", BindingFlags.Public | BindingFlags.Static);
		public static float HR_ResearchPointsPerWorkTick => (float)_ResearchPointsPerWorkTickPI.GetValue(_Extension_Research);
		public static float HR_StudyPointsPerWorkTick => (float)_StudyPointsPerWorkTickPI.GetValue(_Extension_Research);
		public static float HR_StuffCostFactor(ResearchProjectDef project)
		{
			return (float)_StuffCostFactorMI.Invoke(_Extension_Research, new Object[] { project });
		}
		public static bool HR_IsKnownBy(this ResearchProjectDef project, Pawn pawn)
		{
			return (bool)_IsKnownByMI.Invoke(_Extension_Research, new Object[] { project, pawn });
		}

		private static Type _TechJobDefOf = AccessTools.TypeByName("HumanResources.TechJobDefOf");
		private static FieldInfo _ResearchTechFI = AccessTools.Field(_TechJobDefOf, "ResearchTech");
		private static FieldInfo _LearnTechFI = AccessTools.Field(_TechJobDefOf, "LearnTech");
		public static JobDef HR_ResearchTech => (JobDef)_ResearchTechFI.GetValue(_TechJobDefOf);
		public static JobDef HR_LearnTech => (JobDef)_LearnTechFI.GetValue(_TechJobDefOf);
	}
}
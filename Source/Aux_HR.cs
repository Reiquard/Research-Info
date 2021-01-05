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
		public static Dictionary<ResearchProjectDef, float> HRExpertise(Pawn pawn)
		{
			return ((Dictionary<ResearchProjectDef, float>)_expertiseFI.GetValue(pawn.AllComps.Where(x => _CompKnowledge.IsAssignableFrom(x.GetType())).FirstOrDefault()));
		}
		public static TechLevel HRTechLevel(Pawn pawn)
		{
			return ((TechLevel)_techLevelFI.GetValue(pawn.AllComps.Where(x => _CompKnowledge.IsAssignableFrom(x.GetType())).FirstOrDefault()));
		}
		public static float HRPrerequisiteMultiplier(ResearchProjectDef project, Pawn pawn)
		{
			return HRExpertise(pawn).Keys.Where(x => x.HRIsKnownBy(pawn)).Where(x => !x.prerequisites.NullOrEmpty() && x.prerequisites.Contains(project)).Any() ? 2f : 1f;
		}

		private static Type _JobDriver_LearnTech = AccessTools.TypeByName("HumanResources.JobDriver_LearnTech");
		private static FieldInfo _projectFI = AccessTools.Field(_JobDriver_LearnTech, "project");
		public static ResearchProjectDef HRCurrentProject(Pawn pawn)
		{
			return _JobDriver_LearnTech.IsAssignableFrom(pawn.jobs.curDriver.GetType()) ? (ResearchProjectDef)_projectFI.GetValue(pawn.jobs.curDriver) : null;
		}

		private static Type _Extension_Research = AccessTools.TypeByName("HumanResources.Extension_Research");
		private static PropertyInfo _ResearchPointsPerWorkTickPI = AccessTools.Property(_Extension_Research, "ResearchPointsPerWorkTick");
		private static PropertyInfo _StudyPointsPerWorkTickPI = AccessTools.Property(_Extension_Research, "StudyPointsPerWorkTick");
		private static MethodInfo _StuffCostFactorMI = _Extension_Research.GetMethod("StuffCostFactor", BindingFlags.Public | BindingFlags.Static);
		private static MethodInfo _IsKnownByMI = _Extension_Research.GetMethod("IsKnownBy", BindingFlags.Public | BindingFlags.Static);
		public static float HRResearchPointsPerWorkTick => (float)_ResearchPointsPerWorkTickPI.GetValue(_Extension_Research);
		public static float HRStudyPointsPerWorkTick => (float)_StudyPointsPerWorkTickPI.GetValue(_Extension_Research);
		public static float HRStuffCostFactor(ResearchProjectDef project)
		{
			return (float)_StuffCostFactorMI.Invoke(_Extension_Research, new Object[] { project });
		}
		public static bool HRIsKnownBy(this ResearchProjectDef project, Pawn pawn)
		{
			return (bool)_IsKnownByMI.Invoke(_Extension_Research, new Object[] { project, pawn });
		}

		private static Type _TechJobDefOf = AccessTools.TypeByName("HumanResources.TechJobDefOf");
		private static FieldInfo _ResearchTechFI = AccessTools.Field(_TechJobDefOf, "ResearchTech");
		private static FieldInfo _LearnTechFI = AccessTools.Field(_TechJobDefOf, "LearnTech");
		public static JobDef ResearchTech => (JobDef)_ResearchTechFI.GetValue(_TechJobDefOf);
		public static JobDef LearnTech => (JobDef)_LearnTechFI.GetValue(_TechJobDefOf);
	}
}
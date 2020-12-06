using System;
using System.Reflection;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace ResearchInfo
{
	public static class Utilities_HR
    {
		private static Type _CompKnowledge = AccessTools.TypeByName("HumanResources.CompKnowledge");
		private static FieldInfo _expertiseFI = AccessTools.Field(_CompKnowledge, "expertise");
		public static Dictionary<ResearchProjectDef, float> HRExpertise(Pawn pawn)
		{
			return ((Dictionary<ResearchProjectDef, float>)_expertiseFI.GetValue(pawn.AllComps.Where(x => _CompKnowledge.IsAssignableFrom(x.GetType())).FirstOrDefault()));
		}
		public static float HRPrerequisiteMultiplier(ResearchProjectDef project, Pawn pawn)
		{
			return !project.prerequisites.NullOrEmpty() ? (HRExpertise(pawn).Keys.Where(x => project.prerequisites.Contains(x)).Any() ? 2f : 1f) : 1f;
		}

		private static Type _JobDriver_LearnTech = AccessTools.TypeByName("HumanResources.JobDriver_LearnTech");
		private static FieldInfo _projectFI = AccessTools.Field(_JobDriver_LearnTech, "project");
		public static ResearchProjectDef HRCurrentProject(Pawn pawn)
		{
			return _JobDriver_LearnTech.IsAssignableFrom(pawn.jobs.curDriver.GetType()) ? (ResearchProjectDef)_projectFI.GetValue(pawn.jobs.curDriver) : null;
		}

		private static Type _Extension_Research = AccessTools.TypeByName("HumanResources.Extension_Research");
		private static PropertyInfo _ResearchPointsPerWorkTick = AccessTools.Property(_Extension_Research, "ResearchPointsPerWorkTick");
		private static PropertyInfo _StudyPointsPerWorkTick = AccessTools.Property(_Extension_Research, "StudyPointsPerWorkTick");
		private static MethodInfo _StuffCostFactor = _Extension_Research.GetMethod("StuffCostFactor", BindingFlags.Public | BindingFlags.Static);
		public static float HRStuffCostFactor(ResearchProjectDef project)
		{
			return (float)_StuffCostFactor.Invoke(_Extension_Research, new Object[] { project });
		}
		public static float HRResearchPointsPerWorkTick => (float)_ResearchPointsPerWorkTick.GetValue(_Extension_Research);
		public static float HRStudyPointsPerWorkTick => (float)_StudyPointsPerWorkTick.GetValue(_Extension_Research);

		private static Type _TechJobDefOf = AccessTools.TypeByName("HumanResources.TechJobDefOf");
		private static FieldInfo _ResearchTech = AccessTools.Field(_TechJobDefOf, "ResearchTech");
		private static FieldInfo _LearnTech = AccessTools.Field(_TechJobDefOf, "LearnTech");
		public static JobDef ResearchTech => (JobDef)_ResearchTech.GetValue(_TechJobDefOf);
		public static JobDef LearnTech => (JobDef)_LearnTech.GetValue(_TechJobDefOf);
	}
}
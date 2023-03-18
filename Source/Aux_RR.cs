using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace ResearchInfo
{
    public static class Aux_RR
    {
        private static Type _JobDefOf_Custom = AccessTools.TypeByName("PeteTimesSix.ResearchReinvented.DefOfs.JobDefOf_Custom");
        private static FieldInfo _RR_ResearchFI = AccessTools.Field(_JobDefOf_Custom, "RR_Research");
        public static JobDef RR_Research => (JobDef)_RR_ResearchFI.GetValue(_JobDefOf_Custom);

        private static Type _ResearchOpportunityManager = AccessTools.TypeByName("PeteTimesSix.ResearchReinvented.Managers.ResearchOpportunityManager");
        private static Type _ResearchOpportunityTypeDef = AccessTools.TypeByName("PeteTimesSix.ResearchReinvented.Defs.ResearchOpportunityTypeDef");
        private static Type _ResearchOpportunityCategoryDef = AccessTools.TypeByName("PeteTimesSix.ResearchReinvented.Defs.ResearchOpportunityCategoryDef");
        private static Type _ResearchOpportunity = AccessTools.TypeByName("PeteTimesSix.ResearchReinvented.Opportunities.ResearchOpportunity");
        private static Type _CategorySettingsPreset = AccessTools.TypeByName("PeteTimesSix.ResearchReinvented.Data.CategorySettingsPreset");
        private static MethodInfo _get_instanceMI = _ResearchOpportunityManager.GetMethod("get_instance");
        private static MethodInfo _GetOpportunityForJobMI = _ResearchOpportunityManager.GetMethod("GetOpportunityForJob");
        private static MethodInfo _GetCategoryMI = _ResearchOpportunityTypeDef.GetMethod("GetCategory");
        private static MethodInfo _get_SettingsMI = _ResearchOpportunityCategoryDef.GetMethod("get_Settings");
        private static FieldInfo _defFI = AccessTools.Field(_ResearchOpportunity, "def");
        private static FieldInfo _relationFI = AccessTools.Field(_ResearchOpportunity, "relation");
        private static FieldInfo _researchSpeedMultiplierFI = AccessTools.Field(_CategorySettingsPreset, "researchSpeedMultiplier");
        public static float RR_ResearchSpeedMultiplier(Job job)
        {
            var instance = _get_instanceMI.Invoke(_ResearchOpportunityManager, new Object[] { });
            var opportunity = _GetOpportunityForJobMI.Invoke(instance, new Object[] { job });
            var def = _defFI.GetValue(opportunity);
            var relation = _relationFI.GetValue(opportunity);
            var category = _GetCategoryMI.Invoke(def, new Object[] { relation });
            var settings = _get_SettingsMI.Invoke(category, new Object[] { });
            var researchSpeedMultiplier = _researchSpeedMultiplierFI.GetValue(settings);
            return (float)researchSpeedMultiplier;
        }
    }
}
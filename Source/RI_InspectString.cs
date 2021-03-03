using HarmonyLib;
using RimWorld;
using System.Text;
using Verse;

namespace ResearchInfo
{
    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch("GetInspectString")]
    class RI_InspectString
    {
        private static Utilities _util = new Utilities();
        [HarmonyPostfix]
        public static void GetInspectString(ThingWithComps __instance, ref string __result)
        {
            if (__instance.GetType() == typeof(Building_ResearchBench))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_util.InspectStringInfo(__instance));
                sb.AppendInNewLine(__result);
                __result = sb.ToString();
            }
            if (ResearchInfo.ModHumanResources && (__instance.def.defName == "StudyDesk" || __instance.def.defName == "NetworkTerminal"))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_util.InspectStringInfo(__instance, study: true));
                sb.AppendInNewLine(__result);
                __result = sb.ToString();
            }
        }
    }
}
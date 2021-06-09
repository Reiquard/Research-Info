using UnityEngine;
using Verse;

namespace ResearchInfo
{
    public class RI_Settings : ModSettings
    {
        public bool showCurrentProject = true;
        public bool showResearchProgress = true;
        public bool showTimeToComplete = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.showCurrentProject, "showCurrentProject", true);
            Scribe_Values.Look(ref this.showResearchProgress, "showResearchProgress", true);
            Scribe_Values.Look(ref this.showTimeToComplete, "showTimeToComplete", true);
        }
    }

    class RI_Mod : Mod
    {
        public static RI_Settings settings;

        public RI_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RI_Settings>();
        }

        public override string SettingsCategory() => "Research Info";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(inRect);
            ls.CheckboxLabeled("RqRI_Settings_ShowCurrentProject".Translate() + ": ", ref settings.showCurrentProject);
            ls.CheckboxLabeled("RqRI_Settings_ShowResearchProgress".Translate() + ": ", ref settings.showResearchProgress);
            ls.CheckboxLabeled("RqRI_Settings_ShowTimeToComplete".Translate() + ": ", ref settings.showTimeToComplete);
            ls.End();
            settings.Write();
        }
    }
}

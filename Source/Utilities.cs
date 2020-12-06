using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ResearchInfo
{
    public class Utilities
    {
        public Dictionary<Pawn, float> dictCurrentResearchers = new Dictionary<Pawn, float>();
        public string InspectStringInfo(Thing thing, bool study = false)
        {
            StringBuilder sb = new StringBuilder();
            ResearchProjectDef curProj;
            if (study)
            {
                string text = string.Empty;
                Pawn pawn = ListOfPawnsCurrentlyPerformingResearch(study: true).Where(x => x.CurJob.targetA.Thing == thing).FirstOrDefault();
                if (pawn != null)
                {
                    curProj = Utilities_HR.HRCurrentProject(pawn);
                    text = "RqRI_CurrentProject".Translate() + curProj.LabelCap;
                    text += "\n";
                    text += "RqRI_LearningProgress".Translate() + Utilities_HR.HRExpertise(pawn)[curProj].ToStringPercent("F0");
                    text += "\n";
                    text += "RqRI_TimeToComplete".Translate() + TimeToCompleteLearning(curProj, pawn);
                }
                sb.Append(text);
                return sb.ToString();
            }
            if (ResearchInfo.clean)
            {
                curProj = Find.ResearchManager.currentProj;
                sb.Append("RqRI_CurrentProject".Translate());
                if (curProj != null)
                {
                    sb.Append(curProj.LabelCap);
                    sb.AppendInNewLine("RqRI_ResearchProgress".Translate() + curProj.ProgressApparent.ToString("F0") + " / " + curProj.CostApparent.ToString("F0") + $" ({curProj.ProgressPercent.ToStringPercent("F1")})");
                    if (TimeToCompleteResearch(curProj) != "-")
                    {
                        sb.AppendInNewLine("RqRI_TimeToComplete".Translate() + TimeToCompleteResearch(curProj));
                    }
                }
                else
                {
                    sb.Append("None".Translate());
                }
                return sb.ToString();
            }
            if (ResearchInfo.modHumanResources)
            {
                string text = "RqRI_NotCurrentlyInUse".Translate();
                Pawn pawn = ListOfPawnsCurrentlyPerformingResearch().Where(x => x.CurJob.targetA.Thing == thing).FirstOrDefault();
                if (pawn != null)
                {
                    curProj = Utilities_HR.HRCurrentProject(pawn);
                    text = "RqRI_CurrentProject".Translate() + curProj.LabelCap;
                    text += "\n";
                    text += "RqRI_ResearchProgress".Translate() + Utilities_HR.HRExpertise(pawn)[curProj].ToStringPercent("F1");
                    text += "\n";
                    text += "RqRI_TimeToComplete".Translate() + TimeToCompleteResearch(curProj, pawn);
                }
                sb.Append(text);
                return sb.ToString();
            }
            if (ResearchInfo.modPawnsChooseResearch && !ResearchInfo.modHumanResources)
            {
                string text = "RqRI_NotCurrentlyInUse".Translate();
                Pawn pawn = ListOfPawnsCurrentlyPerformingResearch().Where(x => x.CurJob.targetA.Thing == thing).FirstOrDefault();
                if (pawn != null)
                {
                    curProj = Utilities_PCR.PCRCurrentProject(pawn);
                    text = "RqRI_CurrentProject".Translate() + curProj.LabelCap;
                    text += "\n";
                    text += "RqRI_ResearchProgress".Translate() + curProj.ProgressApparent.ToString("F0") + " / " + curProj.CostApparent.ToString("F0") + $" ({curProj.ProgressPercent.ToStringPercent("F1")})";
                    text += "\n";
                    text += "RqRI_TimeToComplete".Translate() + TimeToCompleteResearch(curProj);
                }
                sb.Append(text);
                return sb.ToString();
            }
            return string.Empty;
        }
        public List<Pawn> ListOfPawnsCurrentlyPerformingResearch(bool study = false)
        {
            if (ResearchInfo.modHumanResources)
            {
                if (study)
                    return PawnsFinder.AllMaps_FreeColonistsSpawned.Where(x => x.CurJobDef == Utilities_HR.LearnTech && x.Position == x.CurJob.targetA.Thing.InteractionCell).ToList();
                else
                    return PawnsFinder.AllMaps_FreeColonistsSpawned.Where(x => x.CurJobDef == Utilities_HR.ResearchTech && x.Position == x.CurJob.targetA.Thing.InteractionCell).ToList();
            }
            else
                return PawnsFinder.AllMaps_FreeColonistsSpawned.Where(x => x.CurJobDef == JobDefOf.Research && x.Position == x.CurJob.targetA.Thing.InteractionCell).ToList();
        }
        public float GetResearchSpeedPerProject(ResearchProjectDef curProj, Pawn pawn)
        {
            float researchSpeedPerProject = 0f;
            if (ResearchInfo.clean)
            {
                dictCurrentResearchers.Clear();
                if (ListOfPawnsCurrentlyPerformingResearch().Count <= 0)
                {
                    return 0f;
                }
                for (int i = 0; i < ListOfPawnsCurrentlyPerformingResearch().Count; i++)
                {
                    dictCurrentResearchers.Add(ListOfPawnsCurrentlyPerformingResearch().ElementAt(i), ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).GetStatValue(StatDefOf.ResearchSpeed) * ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor));
                    researchSpeedPerProject += ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).GetStatValue(StatDefOf.ResearchSpeed) * ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor);
                }
            }
            if (ResearchInfo.modHumanResources)
            {
                researchSpeedPerProject = pawn.GetStatValue(StatDefOf.ResearchSpeed)
                    * pawn.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)
                    * Utilities_HR.HRPrerequisiteMultiplier(curProj, pawn);
            }
            if (ResearchInfo.modPawnsChooseResearch && !ResearchInfo.modHumanResources)
            {
                for (int i = 0; i < ListOfPawnsCurrentlyPerformingResearch().Count; i++)
                {
                    if (Utilities_PCR.PCRCurrentProject(ListOfPawnsCurrentlyPerformingResearch().ElementAt(i)) == curProj)
                    {
                        researchSpeedPerProject += ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).GetStatValue(StatDefOf.ResearchSpeed) * ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor);
                    }
                }
            }
            return researchSpeedPerProject;
        }
        public int CurrentResearchersOfProject(ResearchProjectDef curProj)
        {
            if (ResearchInfo.clean)
            {
                return dictCurrentResearchers.Count;
            }
            if (ResearchInfo.modHumanResources)
            {
                dictCurrentResearchers.Clear();
                int counter = 0;
                for (int i = 0; i < ListOfPawnsCurrentlyPerformingResearch().Count; i++)
                {
                    if (Utilities_HR.HRCurrentProject(ListOfPawnsCurrentlyPerformingResearch().ElementAt(i)) == curProj)
                    {
                        counter++;
                        dictCurrentResearchers.Add(ListOfPawnsCurrentlyPerformingResearch().ElementAt(i), ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).GetStatValue(StatDefOf.ResearchSpeed) * ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor));
                    }
                }
                return counter;
            }
            if (ResearchInfo.modPawnsChooseResearch && !ResearchInfo.modHumanResources)
            {
                dictCurrentResearchers.Clear();
                int counter = 0;
                for (int i = 0; i < ListOfPawnsCurrentlyPerformingResearch().Count; i++)
                {
                    if (Utilities_PCR.PCRCurrentProject(ListOfPawnsCurrentlyPerformingResearch().ElementAt(i)) == curProj)
                    {
                        counter++;
                        dictCurrentResearchers.Add(ListOfPawnsCurrentlyPerformingResearch().ElementAt(i), ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).GetStatValue(StatDefOf.ResearchSpeed) * ListOfPawnsCurrentlyPerformingResearch().ElementAt(i).CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor));
                    }
                }
                return counter;
            }
            return 0;
        }
        public string TimeToCompleteResearch(ResearchProjectDef curProj, Pawn pawn = null)
        {
            if (GetResearchSpeedPerProject(curProj, pawn) <= 0)
                return "-";
            float hoursToComplete;
            if (ResearchInfo.modHumanResources)
            {
                hoursToComplete = ((1 - Utilities_HR.HRExpertise(pawn)[curProj]) * curProj.CostApparent / (GetResearchSpeedPerProject(curProj, pawn) * Utilities_HR.HRResearchPointsPerWorkTick * 2500 * (DebugSettings.fastResearch ? 500f : 1f)));
            }
            else
            {
                hoursToComplete = (curProj.CostApparent - curProj.ProgressApparent) / (GetResearchSpeedPerProject(curProj, pawn) * 0.00825f * 2500 * Find.Storyteller.difficultyValues.researchSpeedFactor * (DebugSettings.fastResearch ? 500f : 1f));
            }
            TimeSpan time = TimeSpan.FromHours(hoursToComplete);
            return (time.Days > 0 ? $"{time.Days.ToString() + "LetterDay".Translate()} " : "") + (time.Hours > 0 ? $"{time.Hours.ToString() + "LetterHour".Translate()} " : "") + ($"{time.Minutes.ToString() + "LetterMinute".Translate()}");
        }
        public string TimeToCompleteLearning(ResearchProjectDef curProj, Pawn pawn = null)
        {
            float hoursToComplete;
            hoursToComplete = ((1 - Utilities_HR.HRExpertise(pawn)[curProj]) * pawn.CurJob.bill.recipe.workAmount * Utilities_HR.HRStuffCostFactor(curProj) / (GetResearchSpeedPerProject(curProj, pawn) * Utilities_HR.HRStudyPointsPerWorkTick * 2500 * (DebugSettings.fastResearch ? 500f : 1f)));
            TimeSpan time = TimeSpan.FromHours(hoursToComplete);
            return (time.Days > 0 ? $"{time.Days.ToString() + "LetterDay".Translate()} " : "") + (time.Hours > 0 ? $"{time.Hours.ToString() + "LetterHour".Translate()} " : "") + ($"{time.Minutes.ToString() + "LetterMinute".Translate()}");
        }
        public string ToolTipNamesList()
        {
            string names = string.Empty;
            dictCurrentResearchers = dictCurrentResearchers.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            for (int i = 0; i < dictCurrentResearchers.Count; i++)
            {
                names += $"\n{dictCurrentResearchers.Keys.ElementAt(i).NameShortColored}: {dictCurrentResearchers.Values.ElementAt(i).ToStringPercent("F1")}";
            }
            return names;
        }
        public string ToolTipResearchSpeedDetails(ResearchProjectDef project, Pawn pawn)
        {
            string details = string.Empty;
            details += $"\n{(pawn.GetStatValue(StatDefOf.ResearchSpeed).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_Researcher".Translate()).Truncate(260f)}";
            details += $"\n{(pawn.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_ResearchBench".Translate()).Truncate(260f)}";
            if (ResearchInfo.modHumanResources && Utilities_HR.HRPrerequisiteMultiplier(project, pawn) != 1f)
            {
                details += $"\n{(Utilities_HR.HRPrerequisiteMultiplier(project, pawn).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_PrerequisiteMultiplier".Translate()).Truncate(260f)}";
            }
            return details;
        }
    }
}
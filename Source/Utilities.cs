﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ResearchInfo
{
    public class Utilities
    {
        public string InspectStringInfo(Thing thing, bool study = false)
        {
            StringBuilder sb = new StringBuilder();
            ResearchProjectDef curProj;
            if (study)
            {
                Pawn pawn = ListOfCurrentResearchers(study: true).Where(x => x.CurJob.targetA.Thing == thing).FirstOrDefault();
                if (pawn != null)
                {
                    curProj = Aux_HR.HR_CurrentProject(pawn);
                    sb.AppendInNewLine(RI_Mod.settings.showCurrentProject ? $"{"RqRI_CurrentProject".Translate() + curProj.LabelCap}" : "");
                    sb.AppendInNewLine(RI_Mod.settings.showResearchProgress ? $"{"RqRI_LearningProgress".Translate() + Aux_HR.HR_Expertise(pawn)[curProj].ToStringPercent("F0")}" : "");
                    sb.AppendInNewLine(RI_Mod.settings.showTimeToComplete ? $"{"RqRI_TimeToComplete".Translate() + TimeToCompleteLearning(curProj, pawn)}" : "");
                }
                return sb.ToString();
            }
            if (ResearchInfo.VanillaBehavior)
            {
                curProj = Find.ResearchManager.currentProj;
                sb.AppendInNewLine(RI_Mod.settings.showCurrentProject ? $"{"RqRI_CurrentProject".Translate() + (curProj != null ? curProj.LabelCap : "None".Translate())}" : "");
                if (curProj != null)
                {
                    sb.AppendInNewLine(RI_Mod.settings.showResearchProgress ? $"{"RqRI_ResearchProgress".Translate() + curProj.ProgressApparent.ToString("F0") + " / " + curProj.CostApparent.ToString("F0") + $" ({curProj.ProgressPercent.ToStringPercent("F1")})"}" : "");
                    sb.AppendInNewLine(RI_Mod.settings.showTimeToComplete && TimeToCompleteResearch(curProj) != "-" ? $"{"RqRI_TimeToComplete".Translate() + TimeToCompleteResearch(curProj)}" : "");
                }
                return sb.ToString();
            }
            if (ResearchInfo.ModHumanResources)
            {
                Pawn pawn = ListOfCurrentResearchers().Where(x => x.CurJob.targetA.Thing == thing).FirstOrDefault();
                if (pawn != null)
                {
                    curProj = Aux_HR.HR_CurrentProject(pawn);
                    sb.AppendInNewLine(RI_Mod.settings.showCurrentProject ? $"{"RqRI_CurrentProject".Translate() + curProj.LabelCap}" : "");
                    sb.AppendInNewLine(RI_Mod.settings.showResearchProgress ? $"{"RqRI_ResearchProgress".Translate() + Aux_HR.HR_Expertise(pawn)[curProj].ToStringPercent("F1")}" : "");
                    sb.AppendInNewLine(RI_Mod.settings.showTimeToComplete ? $"{"RqRI_TimeToComplete".Translate() + TimeToCompleteResearch(curProj, pawn)}" : "");
                }
                else
                {
                    sb.Append("RqRI_NotCurrentlyInUse".Translate());
                }
                return sb.ToString();
            }
            if (ResearchInfo.ModPawnsChooseResearch && !ResearchInfo.ModHumanResources)
            {
                if (Aux_PCR.PCR_VersionMismatch)
                {
                    sb.Append("You are using an incompatible version of the 'Pawns Choose Research' mod.");
                    return sb.ToString();
                }
                Pawn pawn = ListOfCurrentResearchers().Where(x => x.CurJob.targetA.Thing == thing).FirstOrDefault();
                if (pawn != null)
                {
                    curProj = Aux_PCR.PCR_CurrentProject(pawn);
                    if (curProj != null)
                    {
                        sb.AppendInNewLine(RI_Mod.settings.showCurrentProject ? $"{"RqRI_CurrentProject".Translate() + curProj.LabelCap}" : "");
                        sb.AppendInNewLine(RI_Mod.settings.showResearchProgress ? $"{"RqRI_ResearchProgress".Translate() + $"{curProj.ProgressApparent.ToString("F0")} / {curProj.CostApparent.ToString("F0")} ({curProj.ProgressPercent.ToStringPercent("F1")})"}" : "");
                        sb.AppendInNewLine(RI_Mod.settings.showTimeToComplete ? $"{"RqRI_TimeToComplete".Translate() + TimeToCompleteResearch(curProj)}" : "");
                    }
                }
                else
                {
                    sb.Append("RqRI_NotCurrentlyInUse".Translate());
                }
                return sb.ToString();
            }
            return string.Empty;
        }
        public List<Pawn> ListOfCurrentResearchers(bool study = false)
        {
            if (ResearchInfo.ModHumanResources)
            {
                JobDef jobDef_HR = study ? Aux_HR.HR_LearnTech : Aux_HR.HR_ResearchTech;
                    return PawnsFinder.AllMaps_FreeColonistsSpawned.
                        Where(p => p.CurJobDef == jobDef_HR && p.Position == p.CurJob.targetA.Thing.InteractionCell).ToList();
            }
            List<Pawn> pawns = ResearchInfo.ModHospitality ? PawnsFinder.AllMaps_Spawned : PawnsFinder.AllMaps_FreeColonistsSpawned;
            JobDef jobDef = ResearchInfo.ModResearchReinvented ? Aux_RR.RR_Research : JobDefOf.Research;
            return pawns.Where(p => p.CurJobDef == jobDef && p.Position == p.CurJob.targetA.Thing.InteractionCell).ToList();
        }
        public float ResearchSpeedForGivenProject(ResearchProjectDef curProj, Pawn pawn)
        {
            if (ResearchInfo.ModResearchReinvented)
            {
                return ListOfCurrentResearchers().
                    Select(p => p.GetStatValue(StatDefOf.ResearchSpeed)
                    * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)
                    * Aux_RR.RR_ResearchSpeedMultiplier()).Sum();
            }
            if (ResearchInfo.VanillaBehavior)
                return ListOfCurrentResearchers().
                    Select(p => p.GetStatValue(StatDefOf.ResearchSpeed) * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)).
                    Sum();
            if (ResearchInfo.ModHumanResources)
            {
                return pawn.GetStatValue(StatDefOf.ResearchSpeed)
                    * pawn.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)
                    * Aux_HR.HR_PrerequisiteMultiplier(curProj, pawn)
                    * pawn.GetStatValue(StatDefOf.GlobalLearningFactor)
                    / curProj.CostFactor(Aux_HR.HR_TechLevel(pawn));
            }
            if (ResearchInfo.ModPawnsChooseResearch && !ResearchInfo.ModHumanResources)
                return ListOfCurrentResearchers().
                    Where(p => Aux_PCR.PCR_CurrentProject(p) == curProj).
                    Select(p => p.GetStatValue(StatDefOf.ResearchSpeed) * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)).
                    Sum();
            return 0f;
        }
        public byte NumberOfCurrentResearchersOfGivenProject(ResearchProjectDef curProj)
        {
            if (ResearchInfo.VanillaBehavior)
                return (byte)ListOfCurrentResearchers().Count;
            if (ResearchInfo.ModPawnsChooseResearch && !ResearchInfo.ModHumanResources)
                return (byte)ListOfCurrentResearchers().Where(p => Aux_PCR.PCR_CurrentProject(p) == curProj).Count();
            return 0;
        }
        public string TimeToCompleteResearch(ResearchProjectDef curProj, Pawn pawn = null)
        {
            if (ResearchSpeedForGivenProject(curProj, pawn) <= 0)
                return "-";
            float hoursToComplete;
            if (ResearchInfo.ModHumanResources)
            {
                hoursToComplete = ((1 - Aux_HR.HR_Expertise(pawn)[curProj]) * curProj.CostApparent /
                    (ResearchSpeedForGivenProject(curProj, pawn) * curProj.CostFactor(pawn.Faction.def.techLevel) * Aux_HR.HR_ResearchPointsPerWorkTick * 2500f * (DebugSettings.fastResearch ? 500f : 1f)));
            }
            else
            {
                hoursToComplete = (curProj.CostApparent - curProj.ProgressApparent) /
                    (ResearchSpeedForGivenProject(curProj, pawn) * 0.00825f * 2500f * Find.Storyteller.difficulty.researchSpeedFactor * (DebugSettings.fastResearch ? 500f : 1f));
            }
            TimeSpan time = TimeSpan.FromHours(hoursToComplete);
            return TimeToString(time);
        }
        public string TimeToCompleteLearning(ResearchProjectDef curProj, Pawn pawn = null)
        {
            float hoursToComplete = ((1 - Aux_HR.HR_Expertise(pawn)[curProj]) * pawn.CurJob.bill.recipe.workAmount * Aux_HR.HR_StuffCostFactor(curProj) /
                (ResearchSpeedForGivenProject(curProj, pawn) * Aux_HR.HR_StudyPointsPerWorkTick * 2500f * (DebugSettings.fastResearch ? 500f : 1f)));
            TimeSpan time = TimeSpan.FromHours(hoursToComplete);
            return TimeToString(time);
        }
        private string TimeToString(TimeSpan time)
        {
            string s = "";
            if (time.Days > 0)
            {
                s += $"{time.Days.ToString() + "LetterDay".Translate()} ";
                s += $"{time.Hours.ToString() + "LetterHour".Translate()} ";
                s += $"{time.Minutes.ToString() + "LetterMinute".Translate()}";
            }
            else
            {
                if (time.Hours > 0)
                {
                    s += $"{time.Hours.ToString() + "LetterHour".Translate()} ";
                }
                s += $"{time.Minutes.ToString() + "LetterMinute".Translate()}";
            }
            return s;
        }
        public string ToolTipNamesList(ResearchProjectDef curProj)
        {
            Dictionary<Pawn, float> d = new Dictionary<Pawn, float>();
            if (ResearchInfo.VanillaBehavior)
                d = ListOfCurrentResearchers().
                    OrderByDescending(p => p.GetStatValue(StatDefOf.ResearchSpeed) * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)).
                    ToDictionary(p => p, p => p.GetStatValue(StatDefOf.ResearchSpeed) * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor));
            if (ResearchInfo.ModPawnsChooseResearch && !ResearchInfo.ModHumanResources)
                d = ListOfCurrentResearchers().
                    Where(p => Aux_PCR.PCR_CurrentProject(p) == curProj).
                    OrderByDescending(p => p.GetStatValue(StatDefOf.ResearchSpeed) * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor)).
                    ToDictionary(p => p, p => p.GetStatValue(StatDefOf.ResearchSpeed) * p.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor));
            string names = string.Empty;
            for (int i = 0; i < d.Count; i++)
            {
                names += $"\n{d.Keys.ElementAt(i).NameShortColored}: {d.Values.ElementAt(i).ToStringPercent("F1")}";
            }
            return names;
        }
        public string ToolTipResearchSpeedDetails(ResearchProjectDef project, Pawn pawn)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\n{(pawn.GetStatValue(StatDefOf.ResearchSpeed).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_Researcher".Translate()).Truncate(260f)}");
            if (ResearchInfo.ModHumanResources)
            {
                sb.Append(pawn.GetStatValue(StatDefOf.GlobalLearningFactor) != 1f ? $"\n{(pawn.GetStatValue(StatDefOf.GlobalLearningFactor).ToStringPercent("F0") + " - " + StatDefOf.GlobalLearningFactor.LabelCap).Truncate(260f)}" : "");
                sb.Append(Aux_HR.HR_PrerequisiteMultiplier(project, pawn) != 1f ? $"\n{(Aux_HR.HR_PrerequisiteMultiplier(project, pawn).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_PrerequisiteMultiplier".Translate()).Truncate(260f)}" : "");
                sb.Append(project.CostFactor(Aux_HR.HR_TechLevel(pawn)) != 1f ? $"\n{((1 / project.CostFactor(Aux_HR.HR_TechLevel(pawn))).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_TechLevelMultiplier".Translate()).Truncate(260f)}" : "");
            }
            sb.Append($"\n{(pawn.CurJob.targetA.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor).ToStringPercent("F0") + " - " + "RqRI_ToolTipDetails_ResearchBench".Translate()).Truncate(260f)}");
            return sb.ToString();
        }
    }
}
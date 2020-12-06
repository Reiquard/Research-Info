using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace ResearchInfo
{
	public class RI_ITab : ITab
	{
		private Utilities _util = new Utilities();
		private Vector2 _scrollPosition = Vector2.zero;
		private Pawn _userpawn => _util.ListOfPawnsCurrentlyPerformingResearch().Where(x => x.CurJob.targetA.Thing == SelThing).FirstOrDefault();
		private ResearchProjectDef _curProj => GetCurProj();
		public override bool IsVisible => Visible();

		private ResearchProjectDef GetCurProj()
		{
			if (ResearchInfo.clean)
				return Find.ResearchManager.currentProj;
			if (_userpawn != null && ResearchInfo.modHumanResources)
				return Utilities_HR.HRCurrentProject(_userpawn);
			if (_userpawn != null && ResearchInfo.modPawnsChooseResearch && !ResearchInfo.modHumanResources)
				return Utilities_PCR.PCRCurrentProject(_userpawn);
			return null;
		}
		private bool Visible()
		{
			if (_curProj != null && ResearchInfo.clean)
				return true;
			if (_curProj != null && (ResearchInfo.modHumanResources || ResearchInfo.modPawnsChooseResearch))
				return _userpawn != null;
			return false;
		}
		public RI_ITab()
		{
			size = new Vector2(432f, 290f);
			labelKey = "RqRI_ITab_Label".Translate();
		}
		protected override void FillTab()
		{
			float curY = 5f;
			GUI.color = Color.white;

			/* Begin of Label */
			float labelY = 30f;
			Text.Font = GameFont.Medium;
			Rect rectProjectLabel = new Rect(7f, curY, size.x - 30f, labelY);
			GUI.BeginGroup(rectProjectLabel);
			Widgets.Label(new Rect(0f, 0f, rectProjectLabel.width, rectProjectLabel.height), _curProj.LabelCap.Truncate(rectProjectLabel.width));
			if (Mouse.IsOver(rectProjectLabel) && (_curProj.LabelCap.Truncate(rectProjectLabel.width) != _curProj.LabelCap))
			{
				GUI.DrawTexture(rectProjectLabel, TexUI.HighlightTex);
				TooltipHandler.TipRegion(rectProjectLabel, _curProj.LabelCap);
			}
			GUI.EndGroup();
			Text.Font = GameFont.Small;
			curY += labelY;
			/* End of Label */

			/* Begin of Desc */
			float descY = 120f;
			Rect rectProjectDesc = new Rect(10f, curY, size.x - 20f, descY);
			GUI.BeginGroup(rectProjectDesc);
			Widgets.LabelScrollable(new Rect(0f, 0f, rectProjectDesc.width, rectProjectDesc.height), _curProj.description, ref _scrollPosition);
			GUI.EndGroup();
			curY += descY;
			/* End of Desc */

			Widgets.DrawLine(new Vector2(10f, curY + 5f), new Vector2(size.x - 20f, curY + 5f), Color.grey, 1f);
			curY += 10f;

			/* Begin of Details */
			float detailsY = 150f;
			float stringPos = 0f;
			float stringHeight = 21f;
			Rect rectProjectDetails = new Rect(10f, curY, size.x - 30f, detailsY);
			GUI.BeginGroup(rectProjectDetails);

			Rect rectProjectTechLevel = new Rect(0f, stringPos, rectProjectDetails.width, stringHeight);
			Text.Anchor = TextAnchor.MiddleLeft;
			if (_curProj.techLevel == Faction.OfPlayer.def.techLevel)
			{
				Widgets.Label(rectProjectTechLevel, "RqRI_ProjectTechLevel".Translate());
			}
			else
			{
				Text.CurFontStyle.fontStyle = FontStyle.Italic;
				Widgets.Label(rectProjectTechLevel, "RqRI_ProjectTechLevel".Translate());
				Text.CurFontStyle.fontStyle = FontStyle.Normal;
				if (Mouse.IsOver(rectProjectTechLevel))
				{
					GUI.DrawTexture(rectProjectTechLevel, TexUI.HighlightTex);
					TooltipHandler.TipRegion(rectProjectTechLevel, "RqRI_FactionTechLevel".Translate() + Faction.OfPlayer.def.techLevel.ToStringHuman().CapitalizeFirst());
				}
			}
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rectProjectTechLevel, _curProj.techLevel.ToStringHuman().CapitalizeFirst());
			stringPos += stringHeight;

			Rect rectProjectCost = new Rect(0f, stringPos, rectProjectDetails.width, stringHeight);
			Text.Anchor = TextAnchor.MiddleLeft;
			if (_curProj.CostFactor(Faction.OfPlayer.def.techLevel) == 1)
			{
				Widgets.Label(rectProjectCost, "RqRI_ProjectCost".Translate());
			}
			else
			{
				Text.CurFontStyle.fontStyle = FontStyle.Italic;
				Widgets.Label(rectProjectCost, "RqRI_ProjectCostMultiplied".Translate((_curProj.CostFactor(Faction.OfPlayer.def.techLevel)).ToStringPercent()));
				Text.CurFontStyle.fontStyle = FontStyle.Normal;
				if (Mouse.IsOver(rectProjectCost))
				{
					GUI.DrawTexture(rectProjectCost, TexUI.HighlightTex);
					TooltipHandler.TipRegion(rectProjectCost, "ResearchCostComparison".Translate(_curProj.baseCost.ToString("F0"), _curProj.CostApparent.ToString("F0")));
				}
			}
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rectProjectCost, _curProj.CostApparent.ToString());
			stringPos += stringHeight + 10f;

			Rect rectResearchProgress = new Rect(0f, stringPos, rectProjectDetails.width, stringHeight);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rectResearchProgress, "RqRI_ResearchProgress".Translate());
			Text.Anchor = TextAnchor.MiddleRight;
			if (ResearchInfo.modHumanResources)
			{
				Widgets.Label(rectResearchProgress, Utilities_HR.HRExpertise(_userpawn)[_curProj].ToStringPercent("F1"));
			}
			else
			{
				Widgets.Label(rectResearchProgress, _curProj.ProgressApparent.ToString("F0") + " / " + _curProj.CostApparent.ToString("F0") + $" ({_curProj.ProgressPercent.ToStringPercent("F1")})");
			}
			stringPos += stringHeight;

			Rect rectResearchSpeed = new Rect(0f, stringPos, rectProjectDetails.width, stringHeight);
			Text.Anchor = TextAnchor.MiddleLeft;
			if (_util.CurrentResearchersOfProject(_curProj) > 0)
			{
				Text.CurFontStyle.fontStyle = FontStyle.Italic;
				Widgets.Label(rectResearchSpeed, "RqRI_ResearchSpeed".Translate());
				Text.CurFontStyle.fontStyle = FontStyle.Normal;
				if (_util.CurrentResearchersOfProject(_curProj) == 1 && Mouse.IsOver(rectResearchSpeed))
				{
					GUI.DrawTexture(rectResearchSpeed, TexUI.HighlightTex);
					TooltipHandler.TipRegion(rectResearchSpeed, "RqRI_ResearchSpeedSoloDesc".Translate(_util.dictCurrentResearchers.Keys.First().NameShortColored)
						+ _util.ToolTipResearchSpeedDetails(_curProj, _userpawn));
				}
				if (_util.CurrentResearchersOfProject(_curProj) > 1 && Mouse.IsOver(rectResearchSpeed))
				{
					GUI.DrawTexture(rectResearchSpeed, TexUI.HighlightTex);
					TooltipHandler.TipRegion(rectResearchSpeed, "RqRI_ResearchSpeedMultipleDesc".Translate() + _util.ToolTipNamesList());
				}
			}
			else
			{
				Widgets.Label(rectResearchSpeed, "RqRI_ResearchSpeed".Translate());
			}
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rectResearchSpeed, _util.GetResearchSpeedPerProject(_curProj, _userpawn).ToStringPercent("F1"));
			stringPos += stringHeight;

			Rect rectTimeToComplete = new Rect(0f, stringPos, rectProjectDetails.width, stringHeight);
			Text.Anchor = TextAnchor.MiddleLeft;
			if (_util.GetResearchSpeedPerProject(_curProj, _userpawn) > 0)
			{
				Text.CurFontStyle.fontStyle = FontStyle.Italic;
				Widgets.Label(rectTimeToComplete, "RqRI_TimeToComplete".Translate());
				Text.CurFontStyle.fontStyle = FontStyle.Normal;
				if (Mouse.IsOver(rectTimeToComplete))
				{
					GUI.DrawTexture(rectTimeToComplete, TexUI.HighlightTex);
					TooltipHandler.TipRegion(rectTimeToComplete, "RqRI_TimeToCompleteDesc".Translate());
				}
			}
			else
			{
				Widgets.Label(rectTimeToComplete, "RqRI_TimeToComplete".Translate());
			}
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rectTimeToComplete, _util.TimeToCompleteResearch(_curProj, _userpawn));
			GUI.EndGroup();
			Text.Anchor = TextAnchor.UpperLeft;
			/* End of Details */
		}
	}
}
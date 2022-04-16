using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;

namespace MemoryImplants
{
	[HarmonyPriority(Priority.HigherThanNormal)]
	[HarmonyPatch(typeof(SkillRecord))]
	[HarmonyPatch(nameof(SkillRecord.Interval))]
	class Patch_SkillRecordInterval
    {
		static FieldInfo pawnField = AccessTools.Field(typeof(SkillRecord), "pawn");

		static bool Prefix(SkillRecord __instance)
		{
			if (__instance.levelInt >= 10)
			{
				Pawn pawn = pawnField.GetValue(__instance) as Pawn;
				float memoryTraitModifier = pawn.story.traits.HasTrait(TraitDefOf.GreatMemory) ? 0.5f : 1f;
				float memoryImplantModifier = pawn.health.hediffSet.HasHediff(HediffDef.Named("MemoryAssistant")) ? 0.2f : 1f;
				__instance.Learn(LevelMultiplier(__instance.levelInt) * memoryTraitModifier * memoryImplantModifier);
			}
			return false;
		}

		public static float LevelMultiplier(int levelInt)
		{
			switch (levelInt)
			{
				case 10: return -0.1f;
				case 11: return -0.2f;
				case 12: return -0.4f;
				case 13: return -0.6f;
				case 14: return -1f;
				case 15: return -1.8f;
				case 16: return -2.8f;
				case 17: return -4f;
				case 18: return -6f;
				case 19: return -8f;
				case 20: return -12f;
				default: return 0f;
			}
		}

		/* Original code
		public void Interval()
		{
			float num = this.pawn.story.traits.HasTrait(TraitDefOf.GreatMemory) ? 0.5f : 1f;
			switch (this.levelInt)
			{
				case 10:
					this.Learn(-0.1f * num, false);
					return;
				case 11:
					this.Learn(-0.2f * num, false);
					return;
				case 12:
					this.Learn(-0.4f * num, false);
					return;
				case 13:
					this.Learn(-0.6f * num, false);
					return;
				case 14:
					this.Learn(-1f * num, false);
					return;
				case 15:
					this.Learn(-1.8f * num, false);
					return;
				case 16:
					this.Learn(-2.8f * num, false);
					return;
				case 17:
					this.Learn(-4f * num, false);
					return;
				case 18:
					this.Learn(-6f * num, false);
					return;
				case 19:
					this.Learn(-8f * num, false);
					return;
				case 20:
					this.Learn(-12f * num, false);
					return;
				default:
					return;
			}
		}
		*/
	}
}

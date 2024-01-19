using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MemoryImplants {
	
	[HarmonyPatch(typeof(SkillRecord))]
	[HarmonyPatch(nameof(SkillRecord.Interval))]
	static class Patch_SkillRecordInterval {

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();

			int index = instructionList.FindLastIndex(instruction => instruction.opcode == OpCodes.Stloc_0);

			if (index == -1) {
				ReportFailure();
				return instructions;
			}

			List<CodeInstruction> collection = new List<CodeInstruction>() {
				new CodeInstruction(OpCodes.Ldloc_0),
				new CodeInstruction(OpCodes.Ldarg_0),
				CodeInstruction.Call(typeof(Patch_SkillRecordInterval), nameof(Patch_SkillRecordInterval.ReturnMemoryImplantMultiplier)),
				new CodeInstruction(OpCodes.Mul),
				new CodeInstruction(OpCodes.Stloc_0),
			};

			instructionList.InsertRange(index + 1, collection);

			return instructionList.AsEnumerable();
		}

		public static void ReportFailure() {
			var patches = Harmony.GetPatchInfo(AccessTools.Constructor(typeof(RimWorld.SkillRecord)));
			Log.Error($"[MEMORY IMPLANTS] Failed to patch RimWorld.SkillRecord.Interval. This generally means the mod isn't working due to some incompatibility or whatever. RimWorld.SkillRecord now has the following patches: prefixes:{patches?.Prefixes?.ToStringSafeEnumerable()}, postfixes: {patches?.Postfixes?.ToStringSafeEnumerable()}, transpilers: {patches?.Transpilers?.ToStringSafeEnumerable()}, finalizers: {patches?.Finalizers?.ToStringSafeEnumerable()}");
		}

		public static float ReturnMemoryImplantMultiplier(this SkillRecord skillRecord) {
			return skillRecord.Pawn.health.hediffSet.HasHediff(HediffDef.Named("MemoryAssistant")) ? 0.2f : 1f;
		}
	}
}
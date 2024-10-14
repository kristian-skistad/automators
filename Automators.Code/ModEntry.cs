using HarmonyLib;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.GameData.BigCraftables;
using System;

namespace KristianSkistad.Automator
{
    public class ModEntry : Mod
    {
        private static readonly string ModDataId = "KristianSkistad.Automator";

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(Type.GetType("Pathoschild.Stardew.Automate.Framework.AutomationFactory,Automate"), "GetFor", parameters: new Type[] { typeof(StardewValley.Object), typeof(GameLocation), typeof(Vector2).MakeByRefType() }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Automate_AutomationFactory_GetFor_SObject__Postfix)));
        }

        // Removes automation from every chest that does not have the automator mod data.
        private static void Automate_AutomationFactory_GetFor_SObject__Postfix(ref IAutomatable __result)
        {
            if (__result is IContainer container)
            {
                Chest chest = container.Location.getObjectAtTile(__result.TileArea.X, __result.TileArea.Y) as Chest;
                if (chest != null)
                {
                    Game1.bigCraftableData.TryGetValue(chest.itemId.Value, out BigCraftableData bigCraftableData);
                    if (bigCraftableData != null && !bigCraftableData.CustomFields.ContainsKey(ModDataId)) __result = null;
                }
            }
        }
    }
}
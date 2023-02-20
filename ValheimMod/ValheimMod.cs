// ValheimMod
// a Valheim mod skeleton using Jötunn
// 
// File:    ValheimMod.cs
// Project: ValheimMod

using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine;
using static Humanoid;
using static ItemDrop;
using static ItemSets;

namespace ValheimMod
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ValheimMod : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.ValheimMod";
        public const string PluginName = "ValheimMod";
        public const string PluginVersion = "0.0.1";
        private readonly Harmony harmony = new Harmony("test.ValheimMod");
        public static List<string> elderTeleport = new List<string>();
        public static List<string> bonemassTeleport = new List<string>();
        public static List<string> moderTeleport = new List<string>();
        public static List<string> yagluthTeleport = new List<string>();
        public static List<string> allTeleport = new List<string> {
            "$item_iron",
        }
        ;
        

        //private CustomStatusEffect EvilSwordEffect;

        public static List<string> notele = new List<string>();
        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html

        private void Awake()
        {
            
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("ValheimMod has landed");
            harmony.PatchAll();

            // To learn more about Jotunn's features, go to
            // https://valheim-modding.github.io/Jotunn/tutorials/overview.html
            PrefabManager.OnVanillaPrefabsAvailable += AddClonedItems;
            PrefabManager.OnVanillaPrefabsAvailable += Test;

            PrefabManager.OnPrefabsRegistered += Test;

        }
        
        private void Test()
        {
            var Moder = PrefabManager.Cache.GetPrefab<StatusEffect>("GP_Moder");
            var prefab = PrefabManager.Cache.GetPrefab<ItemDrop>("PrefabName");
            prefab.m_itemData.m_shared.m_equipStatusEffect = Moder;
            var x = PrefabManager.Instance.GetPrefab("GP_Moder");
            var y = PrefabManager.Instance.GetPrefab("$se_moder_name");
            Jotunn.Logger.LogFatal("x:" + x);
            Jotunn.Logger.LogFatal("y:" + y);
        }
        public static bool HasEffect(string effect)
        {
            return Player.m_localPlayer.GetSEMan().HaveStatusEffect(effect);
        }
        private void AddEvilSword()
        {
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_evilsword";
            IC.Description = "$item_evilsword_desc";
            CustomItem customItem = new CustomItem("EvilSword", "SwordBlackmetal", IC);
            ItemManager.Instance.AddItem(customItem);


            // Add our custom status effect to it
            var itemDrop = customItem.ItemDrop;
            //itemDrop.m_itemData.m_shared.m_damages.m_fire = 100;
            itemDrop.m_itemData.m_shared.m_damages.Add(new HitData.DamageTypes
            {
                m_fire = 20,
                m_blunt = 20,
                m_lightning = 20,
                m_chop = 4000,
            });
            itemDrop.m_itemData.m_shared.m_teleportable = true;
            itemDrop.m_itemData.m_shared.m_useDurabilityDrain = 0;
            itemDrop.m_itemData.m_shared.m_timedBlockBonus = 10;
            itemDrop.m_itemData.m_shared.m_movementModifier = 1;

            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = "EvilStatusEffect";
            effect.m_name = "$evilsword_effectname";
            effect.m_icon = AssetUtils.LoadSpriteFromFile("ValheimMod/Assets/reee.png");

            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_startMessage = "$evilsword_effectstart";
            effect.m_addMaxCarryWeight = 304;
            effect.m_speedModifier = 4;
            effect.m_tooltip = "$evilsword_tooltip";
            effect.m_staminaRegenMultiplier = 10;


            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessage = "$evilsword_effectstop";

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = CE.StatusEffect;
        }
        private void AddEikthyrRing()
        {
            string _name = "EikthyrRing";
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_" + _name.ToLower();
            IC.Description = "$item_" + _name.ToLower() + "_desc";
            Sprite icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            IC.Icons = new Sprite[] { icon };
            /*  Sprite var1 = AssetUtils.LoadSpriteFromFile("ValheimMod/Assets/test_var1.png");
              Sprite var2 = AssetUtils.LoadSpriteFromFile("ValheimMod/Assets/test_var2.png");
              Sprite var3 = AssetUtils.LoadSpriteFromFile("ValheimMod/Assets/test_var3.png");
              Sprite var4 = AssetUtils.LoadSpriteFromFile("ValheimMod/Assets/test_var4.png");
              Texture2D styleTex = AssetUtils.LoadTexture("ValheimMod/Assets/test_varpaint.png");

            IC.Icons = new Sprite[] { var1, var2, var3, var4 };
            IC.StyleTex = styleTex;*/

            IC.AddRequirement(new RequirementConfig("BeltStrength", 1));
            IC.AddRequirement(new RequirementConfig("TrophyEikthyr", 1));

            CustomItem customItem = new CustomItem(_name, "BeltStrength", IC);
            ItemManager.Instance.AddItem(customItem);


            // Add our custom status effect to it
            ItemDrop itemDrop = customItem.ItemDrop;

            itemDrop.m_itemData.m_shared.m_movementModifier = 1;
            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = _name + "Effect";
            effect.m_name = "$" + _name.ToLower() + "_effectname";
            effect.m_tooltip = "$" + _name.ToLower() + "_tooltip";
            effect.m_startMessage = "$" + _name.ToLower() + "_effectstart";
            effect.m_stopMessage = "$" + _name.ToLower() + "_effectstop";
            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");

            effect.m_addMaxCarryWeight = 175;
            effect.m_staminaRegenMultiplier = 2;
            effect.m_jumpStaminaUseModifier = -0.3f;
            effect.m_runStaminaDrainModifier = -0.3f;
            effect.m_fallDamageModifier = -1f;
            

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = CE.StatusEffect;


        }
        private void AddElderRing()
        {
            string _name = "ElderRing";
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_" + _name.ToLower();
            IC.Description = "$item_" + _name.ToLower() + "_desc";
            Sprite icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            IC.Icons = new Sprite[] { icon };
            CustomItem customItem = new CustomItem(_name, "BeltStrength", IC);
            ItemManager.Instance.AddItem(customItem);

            var incineratorConfig = new IncineratorConversionConfig();

            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("EikthyrRing", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("TrophyTheElder", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("Thunderstone", 1));
            incineratorConfig.ProducedItems = 1;
            incineratorConfig.RequireOnlyOneIngredient = false;  // true = only one of the requirements is needed to produce the output
            incineratorConfig.Priority = 10;                      // Higher priorities get preferred when multiple requirements are met
            incineratorConfig.ToItem = _name;
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(incineratorConfig));

            // Add our custom status effect to it
            ItemDrop itemDrop = customItem.ItemDrop;

            itemDrop.m_itemData.m_shared.m_movementModifier = 1;

            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = _name + "Effect";
            effect.m_name = "$" + _name.ToLower() + "_effectname";
            effect.m_tooltip = "$" + _name.ToLower() + "_tooltip";
            effect.m_startMessage = "$" + _name.ToLower() + "_effectstart";
            effect.m_stopMessage = "$" + _name.ToLower() + "_effectstop";
            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");

            effect.m_addMaxCarryWeight = 200;
            effect.m_staminaRegenMultiplier = 2;
            effect.m_jumpStaminaUseModifier = -0.3f;
            effect.m_runStaminaDrainModifier = -0.3f;
            effect.m_fallDamageModifier = -0.1f;

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = CE.StatusEffect;
            elderTeleport.Add("$item_copper");
            elderTeleport.Add("$item_copperore");
            elderTeleport.Add("$item_copperscrap");
            elderTeleport.Add("$item_tin");
            elderTeleport.Add("$item_tinore");
        }
        private void AddBonemassRing()
        {
            string _name = "BonemassRing";
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_" + _name.ToLower();
            IC.Description = "$item_" + _name.ToLower() + "_desc";
            Sprite icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            IC.Icons = new Sprite[] { icon };

            CustomItem customItem = new CustomItem(_name, "BeltStrength", IC);
            ItemManager.Instance.AddItem(customItem);

            var incineratorConfig = new IncineratorConversionConfig();
            
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("ElderRing", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("TrophyBonemass", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("Thunderstone", 1));
            incineratorConfig.ProducedItems = 1;
            incineratorConfig.RequireOnlyOneIngredient = false;  // true = only one of the requirements is needed to produce the output
            incineratorConfig.Priority = 10;                      // Higher priorities get preferred when multiple requirements are met
            incineratorConfig.ToItem = _name;
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(incineratorConfig));

            // Add our custom status effect to it
            ItemDrop itemDrop = customItem.ItemDrop;

            itemDrop.m_itemData.m_shared.m_movementModifier = 1;

            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = _name + "Effect";
            effect.m_name = "$" + _name.ToLower() + "_effectname";
            effect.m_tooltip = "$" + _name.ToLower() + "_tooltip";
            effect.m_startMessage = "$" + _name.ToLower() + "_effectstart";
            effect.m_stopMessage = "$" + _name.ToLower() + "_effectstop";
            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");

            effect.m_addMaxCarryWeight = 225;
            effect.m_staminaRegenMultiplier = 2;
            effect.m_jumpStaminaUseModifier = -0.3f;
            effect.m_runStaminaDrainModifier = -0.3f;
            effect.m_fallDamageModifier = -0.2f;

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = CE.StatusEffect;
            bonemassTeleport.Add("$item_iron");
            bonemassTeleport.Add("$item_ironscraps");
        }
        private void AddModerRing()
        {
            string _name = "ModerRing";
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_" + _name.ToLower();
            IC.Description = "$item_" + _name.ToLower() + "_desc";
            Sprite icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            IC.Icons = new Sprite[] { icon };

            CustomItem customItem = new CustomItem(_name, "BeltStrength", IC);
            ItemManager.Instance.AddItem(customItem);

            var incineratorConfig = new IncineratorConversionConfig();

            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("BonemassRing", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("TrophyDragonQueen", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("Thunderstone", 1));
            incineratorConfig.ProducedItems = 1;
            incineratorConfig.RequireOnlyOneIngredient = false;  // true = only one of the requirements is needed to produce the output
            incineratorConfig.Priority = 10;                      // Higher priorities get preferred when multiple requirements are met
            incineratorConfig.ToItem = _name;
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(incineratorConfig));

            // Add our custom status effect to it
            ItemDrop itemDrop = customItem.ItemDrop;

            itemDrop.m_itemData.m_shared.m_movementModifier = 1;

            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = _name + "Effect";
            effect.m_name = "$" + _name.ToLower() + "_effectname";
            effect.m_tooltip = "$" + _name.ToLower() + "_tooltip";
            effect.m_startMessage = "$" + _name.ToLower() + "_effectstart";
            effect.m_stopMessage = "$" + _name.ToLower() + "_effectstop";
            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            effect.m_attributes = StatusEffect.StatusAttribute.SailingPower;


            effect.m_addMaxCarryWeight = 250;
            effect.m_staminaRegenMultiplier = 2;
            effect.m_jumpStaminaUseModifier = -0.3f;
            effect.m_runStaminaDrainModifier = -0.3f;
            effect.m_fallDamageModifier = -0.3f;


            var moder = PrefabManager.Cache.GetPrefab<StatusEffect>("GP_Moder");

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = moder;

            moderTeleport.Add("$item_silver");
            moderTeleport.Add("$item_silverore");
        }
        private void AddYagluthRing()
        {
            string _name = "YagluthRing";
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_" + _name.ToLower();
            IC.Description = "$item_" + _name.ToLower() + "_desc";
            Sprite icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            IC.Icons = new Sprite[] { icon };

            CustomItem customItem = new CustomItem(_name, "BeltStrength", IC);
            ItemManager.Instance.AddItem(customItem);

            var incineratorConfig = new IncineratorConversionConfig();

            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("ModerRing", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("TrophyGoblinKing", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("Thunderstone", 1));
            incineratorConfig.ProducedItems = 1;
            incineratorConfig.RequireOnlyOneIngredient = false;  // true = only one of the requirements is needed to produce the output
            incineratorConfig.Priority = 10;                      // Higher priorities get preferred when multiple requirements are met
            incineratorConfig.ToItem = _name;
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(incineratorConfig));

            // Add our custom status effect to it
            ItemDrop itemDrop = customItem.ItemDrop;

            itemDrop.m_itemData.m_shared.m_movementModifier = 1;

            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = _name + "Effect";
            effect.m_name = "$" + _name.ToLower() + "_effectname";
            effect.m_tooltip = "$" + _name.ToLower() + "_tooltip";
            effect.m_startMessage = "$" + _name.ToLower() + "_effectstart";
            effect.m_stopMessage = "$" + _name.ToLower() + "_effectstop";
            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");

            effect.m_addMaxCarryWeight = 275;
            effect.m_staminaRegenMultiplier = 2;
            effect.m_jumpStaminaUseModifier = -0.3f;
            effect.m_runStaminaDrainModifier = -0.3f;
            effect.m_fallDamageModifier = -0.4f;

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = CE.StatusEffect;
            yagluthTeleport.Add("$item_blackmetal");
            yagluthTeleport.Add("$item_blackmetalscrap");
        }
        private void AddQueenRing()
        {
            string _name = "QueenRing";
            // Create and add a custom item based on SwordBlackmetal
            ItemConfig IC = new ItemConfig();
            IC.Name = "$item_" + _name.ToLower();
            IC.Description = "$item_" + _name.ToLower() + "_desc";
            Sprite icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");
            IC.Icons = new Sprite[] { icon };

            CustomItem customItem = new CustomItem(_name, "BeltStrength", IC);
            ItemManager.Instance.AddItem(customItem);

            var incineratorConfig = new IncineratorConversionConfig();

            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("YagluthRing", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("TrophySeekerQueen", 1));
            incineratorConfig.Requirements.Add(new IncineratorRequirementConfig("Thunderstone", 1));
            incineratorConfig.ProducedItems = 1;
            incineratorConfig.RequireOnlyOneIngredient = false;  // true = only one of the requirements is needed to produce the output
            incineratorConfig.Priority = 10;                      // Higher priorities get preferred when multiple requirements are met
            incineratorConfig.ToItem = _name;
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(incineratorConfig));

            // Add our custom status effect to it
            ItemDrop itemDrop = customItem.ItemDrop;

            itemDrop.m_itemData.m_shared.m_movementModifier = 1;

            // add effect
            SE_Stats effect = ScriptableObject.CreateInstance<SE_Stats>();
            effect.name = _name + "Effect";
            effect.m_name = "$" + _name.ToLower() + "_effectname";
            effect.m_tooltip = "$" + _name.ToLower() + "_tooltip";
            effect.m_startMessage = "$" + _name.ToLower() + "_effectstart";
            effect.m_stopMessage = "$" + _name.ToLower() + "_effectstop";
            effect.m_startMessageType = MessageHud.MessageType.Center;
            effect.m_stopMessageType = MessageHud.MessageType.Center;
            effect.m_icon = AssetUtils.LoadSpriteFromFile($"ValheimMod/Assets/{_name}.png");

            effect.m_addMaxCarryWeight = 300;
            effect.m_staminaRegenMultiplier = 2;
            effect.m_jumpStaminaUseModifier = -0.3f;
            effect.m_runStaminaDrainModifier = -0.3f;
            effect.m_fallDamageModifier = -0.5f;

            CustomStatusEffect CE = new CustomStatusEffect(effect, fixReference: false);  // We dont need to fix refs here, because no mocks were used
            ItemManager.Instance.AddStatusEffect(CE);

            itemDrop.m_itemData.m_shared.m_equipStatusEffect = CE.StatusEffect;

        }
        private void AddClonedItems()
        {
            try
            {
                AddEikthyrRing();
                AddElderRing();
                AddBonemassRing();
                AddModerRing();
                AddYagluthRing();
                AddQueenRing();


            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Error while adding cloned item: {ex}");
            }
            finally
            {
                // You want that to run only once, Jotunn has the item cached for the game session
                PrefabManager.OnVanillaPrefabsAvailable -= AddClonedItems;
            }
        }
       
        //---------------------------------------------------------------
        /*        
                public static void UnlockInventory(List<ItemDrop.ItemData> items)
                    {
                        foreach (ItemDrop.ItemData itemData in items)
                        {
                            if (!itemData.m_shared.m_teleportable)
                            {
                                if (!notele.Contains(itemData.m_shared.m_name))
                                {
                                    notele.Add(itemData.m_shared.m_name);
                                }
                                itemData.m_shared.m_teleportable = true;
                            }
                        }
                    }
                public static void LockInventory(List<ItemDrop.ItemData> items)
                {
                    foreach (ItemDrop.ItemData itemData in items)
                    {
                        if (notele.Contains(itemData.m_shared.m_name))
                        {
                            itemData.m_shared.m_teleportable = false;
                        }
                    }
                }
        */
        public static void SyncTeleportability(string effectName, List<string> toTeleport)
        {

            //if (HasEffect(effectName))
            //{
                foreach (ItemDrop.ItemData item in Player.m_localPlayer.GetInventory().GetAllItems()) {
                    if (toTeleport.Contains(item.m_shared.m_name)) {
                        item.m_shared.m_teleportable = true;
                    }
                }

            //}
        }
        public static void ResetTeleportability(string effectName,List<string> toTeleport)
        {
            //if (HasEffect(effectName))
            //{
                foreach (ItemDrop.ItemData item in Player.m_localPlayer.GetInventory().GetAllItems())
                {
                    if (toTeleport.Contains(item.m_shared.m_name)) {
                        item.m_shared.m_teleportable = false;
                    }
                }

            //}
        }
        
        /*

        public static void SyncTeleportability(List<string> item)
        {
            if (IsTeleportable("elderringeffect"))
            {
                foreach (ItemDrop.ItemData itemData in Player.m_localPlayer.GetInventory().GetAllItems())
                {
                    if (!itemData.m_shared.m_teleportable)
                    {
                        if (!notele.Contains(itemData.m_shared.m_name))
                        {
                            notele.Add(itemData.m_shared.m_name);
                        }
                        itemData.m_shared.m_teleportable = true;
                    }
                }
            }
        }
        public static void ResetTeleportability(List<string> item)
        {
            foreach (ItemDrop.ItemData itemData in Player.m_localPlayer.GetInventory().GetAllItems())
            {
                if (notele.Contains(itemData.m_shared.m_name))
                {
                    itemData.m_shared.m_teleportable = false;
                }
            }
        }
*/
    }
    //[HarmonyPatch(typeof(Inventory), "IsTeleportable")]
    [HarmonyPatch(typeof(Player), "UpdateTeleport")] 
        public static class Teleport
        {
        private static bool effectAdded = false;
        public static void Prefix()
        {



            if (ValheimMod.HasEffect("ElderRingEffect"))
            {
                if (!effectAdded) { 
                ValheimMod.SyncTeleportability("ElderRingEffect", ValheimMod.elderTeleport);
                Jotunn.Logger.LogInfo($"HasEffect:{ValheimMod.HasEffect("ElderRingEffect")} effectAdded: {effectAdded}");
                }
                effectAdded = true;
            }
            else
            {
                if (effectAdded)
                {
                   ValheimMod.ResetTeleportability("ElderRingEffect", ValheimMod.elderTeleport);
                   Jotunn.Logger.LogInfo($"HasEffect:{ValheimMod.HasEffect("ElderRingEffect")} effectAdded: {effectAdded}");
                }
                effectAdded = false;
            }

            }
        /*if (ValheimMod.HasEffect("BonemassRingEffect"))
{
    ValheimMod.SyncTeleportability("BonemassRingEffect", ValheimMod.bonemassTeleport);
}
if (ValheimMod.HasEffect("ModerRingEffect"))
{
    ValheimMod.SyncTeleportability("ModerRingEffect", ValheimMod.moderTeleport);
}
if (ValheimMod.HasEffect("YagluthRingEffect"))
{
    ValheimMod.SyncTeleportability("YagluthRingEffect", ValheimMod.yagluthTeleport);
}
if (ValheimMod.HasEffect("QueenRingEffect"))
{
    ValheimMod.SyncTeleportability("QueenRingEffect", ValheimMod.allTeleport);
}*/
        public static void Postfix()
            {
/*            if (ValheimMod.HasEffect("ElderRingEffect"))
            {
                ValheimMod.ResetTeleportability("ElderRingEffect", ValheimMod.elderTeleport);
            }
            if (ValheimMod.HasEffect("BonemassRingEffect"))
            {
                ValheimMod.ResetTeleportability("BonemassRingEffect", ValheimMod.bonemassTeleport);
            }
            if (ValheimMod.HasEffect("ModerRingEffect"))
            {
                ValheimMod.ResetTeleportability("ModerRingEffect", ValheimMod.moderTeleport);
            }
            if (ValheimMod.HasEffect("YagluthRingEffect"))
            {
                ValheimMod.ResetTeleportability("YagluthRingEffect", ValheimMod.yagluthTeleport);
            }
            if (ValheimMod.HasEffect("QueenRingEffect"))
            {
                ValheimMod.ResetTeleportability("QueenRingEffect", ValheimMod.allTeleport);
            }
            Jotunn.Logger.LogInfo("After teleport");*/
            //ValheimMod.ResetTeleportability();

        }
    }
    
}


//$item_trophy_eikthyr
//$item_trophy_elder
//$item_trophy_bonemass
//$item_trophy_dragonqueen
//$item_trophy_goblinking
//$item_trophy_seekerqueen


//$item_beltstrength -- add effect status when equipped
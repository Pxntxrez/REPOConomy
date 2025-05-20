using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REPOConomyMod
{
    [BepInPlugin("PxntxrezStudio.REPOConomy", "REPOConomy", "1.0.0")]
    public class REPOConomyPlugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static bool HasEconomy { get; set; } = false;
        public static EconomyType CurrentEconomy { get; set; }
        public static float MinPercent { get; set; }
        public static float MaxPercent { get; set; }
        private static readonly List<ValuableObject> pendingValuables = new();
        public static bool EconomyInitialized { get; private set; } = false;
        public static string CurrentEconomyName = "Stability";
        public static string CurrentEconomyFlavor = "Economy by default";
        public static float ClientDeltaMin { get; set; }
        public static float ClientDeltaMax { get; set; }

        private void Awake()
        {
            Logger = base.Logger;

            Harmony harmony = new Harmony("PxntxrezStudio.REPOConomy");
            harmony.PatchAll();

            REPOConfig.Init(Config);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SemiFunc.RunIsLobby() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
                return;

            if (EconomyController.Instance == null)
            {
                GameObject go = new GameObject("EconomyUI");
                EconomyController.Instance = go.AddComponent<EconomyController>();
                go.AddComponent<REPOConomyNetworkHandler>();
                DontDestroyOnLoad(go);
            }

            if (SemiFunc.RunIsLevel())
            {
                REPOConomyPlugin.InitializeEconomy();
            }
        }

        public static void InitializeEconomy()
        {
            if (EconomyInitialized)
            {
                Logger.LogInfo("[Economy] Already initialized. Skipping.");
                return;
            }

            if (SemiFunc.RunIsLobby() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
            {
                Logger.LogInfo("[Economy] Initialization canceled: Lobby, store, or arena.");
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                Logger.LogInfo("[Economy] Initialization skipped: not the MasterClient.");
                return;
            }

            EconomyInitialized = true;

            int level = GetCurrentLevel();
            float deltaMin = (level - 1) * REPOConfig.LevelScaleMin.Value;
            float deltaMax = (level - 1) * REPOConfig.LevelScaleMax.Value;

            ClientDeltaMin = deltaMin;
            ClientDeltaMax = deltaMax;

            float levelScaleMin = deltaMin;
            float levelScaleMax = deltaMax;

            float baseMin = -REPOConfig.DefaultBaseMin.Value - levelScaleMin;
            float baseMax = REPOConfig.DefaultBaseMax.Value + levelScaleMax;

            bool forceStable = REPOConfig.ForceStableEconomy.Value;

            bool allowEvents = level >= REPOConfig.EventStartLevel.Value;
            bool rollPassed =
                !REPOConfig.UseEventChance.Value
                || UnityEngine.Random.Range(1, 101) <= REPOConfig.EventChance.Value;

            if (forceStable || !allowEvents || !rollPassed)
            {
                CurrentEconomy = EconomyType.Stable;
            }
            else
            {
                int totalTypes = System.Enum.GetNames(typeof(EconomyType)).Length;
                int random = UnityEngine.Random.Range(1, totalTypes);
                CurrentEconomy = (EconomyType)random;
            }

            switch (CurrentEconomy)
            {
                case EconomyType.FreezeMarket:
                    baseMin = 0f;
                    baseMax = 0f;
                    break;
                case EconomyType.Deflation:
                    baseMin -= 10f;
                    baseMax -= 5f;
                    break;
                case EconomyType.Inflation:
                    baseMin += 0f;
                    baseMax += 10f;
                    break;
                case EconomyType.Chaos:
                    baseMin -= 10f;
                    baseMax += 10f;
                    break;
                case EconomyType.BlackMarketSurge:
                    baseMin -= 10f;
                    baseMax += 20f;
                    break;
                case EconomyType.Overload:
                    baseMin = -100f;
                    baseMax += +100f;
                    break;
                case EconomyType.RareBoom:
                    baseMin += 5f;
                    baseMax += 30f;
                    break;
                case EconomyType.CommonCrash:
                    baseMin -= 30f;
                    baseMax -= 10f;
                    break;
                case EconomyType.LegendaryOnlyMatters:
                    baseMin = -20f;
                    baseMax = 5f;
                    break;
                case EconomyType.EchoMarket:
                    baseMin += UnityEngine.Random.Range(-25f, 0f);
                    baseMax += UnityEngine.Random.Range(0f, 25f);
                    break;
                case EconomyType.ReverseInflation:
                    baseMin -= 15f;
                    baseMax -= 5f;
                    break;
                case EconomyType.LuxuryHunt:
                    baseMin += 10f;
                    baseMax += 40f;
                    break;
                case EconomyType.DumpsterDive:
                    baseMin += 5f;
                    baseMax += 60f;
                    break;
                case EconomyType.ExtraProfit:
                    baseMax += 50f;
                    break;
                case EconomyType.ScamSeason:
                    baseMin -= 30f;
                    baseMax -= 10f;
                    break;
                case EconomyType.SuddenDrop:
                    baseMin = -50f;
                    baseMax = -20f;
                    break;
                case EconomyType.TreasureRush:
                    baseMin += 100f;
                    baseMax += 200f;
                    break;
                case EconomyType.ZeroGravity:
                    baseMin = -50f;
                    baseMax = +150f;
                    break;
                case EconomyType.Turbulence:
                    float random = UnityEngine.Random.Range(-30f, 30f);
                    baseMin += random;
                    baseMax += random;
                    break;
            }

            baseMin = Mathf.Clamp(
                baseMin,
                -REPOConfig.MinClampLimit.Value,
                REPOConfig.MaxClampLimit.Value
            );
            baseMax = Mathf.Clamp(
                baseMax,
                -REPOConfig.MinClampLimit.Value,
                REPOConfig.MaxClampLimit.Value
            );

            MinPercent = baseMin;
            MaxPercent = baseMax;
            HasEconomy = true;

            Logger.LogInfo(
                $"[Economy] Level: {level}, Economy: {CurrentEconomy}, Range: {MinPercent}% - {MaxPercent}%"
            );

            CurrentEconomyName = GetEconomyName();
            CurrentEconomyFlavor = GetEconomyFlavor();

            if (PhotonNetwork.IsMasterClient)
            {
                var payload = new object[]
                {
                    (int)CurrentEconomy,
                    GetEconomyName(),
                    GetEconomyFlavor(),
                    MinPercent,
                    MaxPercent,
                    deltaMin,
                    deltaMax,
                };
                var opts = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                PhotonNetwork.RaiseEvent(179, payload, opts, SendOptions.SendReliable);
            }

            foreach (var vo in pendingValuables)
            {
                ValuablePatch.ApplyRandomValue(vo);
            }

            pendingValuables.Clear();
        }

        private static void LogTotalMapValue()
        {
            if (REPOConfig.ShowTotalMapValue.Value)
            {
                float total = 0f;
                foreach (var valuable in UnityEngine.Object.FindObjectsOfType<ValuableObject>())
                {
                    total += valuable.dollarValueCurrent;
                }

                Logger.LogInfo($"[REPOConomy] 💰 Total value on map: ${Mathf.RoundToInt(total)}");
            }
        }

        public static int GetCurrentLevel()
        {
            if (
                StatsManager.instance?.runStats != null
                && StatsManager.instance.runStats.ContainsKey("level")
            )
            {
                return StatsManager.instance.runStats["level"] + 1;
            }
            return 1;
        }

        public static string GetEconomyName()
        {
            if (!HasEconomy)
                return "Stability";

            return CurrentEconomy switch
            {
                EconomyType.Stable => "Stability",
                EconomyType.Deflation => "Deflation",
                EconomyType.Inflation => "Inflation",
                EconomyType.Chaos => "Chaos",
                EconomyType.FreezeMarket => "Freeze",
                EconomyType.BlackMarketSurge => "Black Market",
                EconomyType.Overload => "Overload",
                EconomyType.RareBoom => "Rare Boom",
                EconomyType.CommonCrash => "Ordinary Disaster",
                EconomyType.LegendaryOnlyMatters => "Legends Are Valued!",
                EconomyType.EchoMarket => "Echo Market",
                EconomyType.ReverseInflation => "Reverse Inflation",
                EconomyType.LuxuryHunt => "Hunting For Luxury",
                EconomyType.DumpsterDive => "Gold In The Dump",
                EconomyType.ExtraProfit => "Additional Profit",
                EconomyType.ScamSeason => "Season Of Scam",
                EconomyType.SuddenDrop => "Sharp Fall",
                EconomyType.TreasureRush => "Treasure Hunt",
                EconomyType.ZeroGravity => "Weightlessness Of Prices",
                EconomyType.Turbulence => "Turbulence",
                _ => "???",
            };
        }

        public static string GetEconomyFlavor()
        {
            return CurrentEconomy switch
            {
                EconomyType.Stable => "Economy by default",
                EconomyType.Deflation => "Market Crash",
                EconomyType.Inflation => "Moment to Get Rich!",
                EconomyType.Chaos => "Financial chaos...",
                EconomyType.FreezeMarket => "Economy frozen :(",
                EconomyType.BlackMarketSurge => "Values have become more expensive...",
                EconomyType.Overload => "50/50",
                EconomyType.RareBoom => "Values are suddenly in price!",
                EconomyType.CommonCrash => "Values are devalued!",
                EconomyType.LegendaryOnlyMatters => "Only the best makes sense...",
                EconomyType.EchoMarket => "Prices are jumping like crazy...",
                EconomyType.ReverseInflation => "Budget is better than wealth...",
                EconomyType.LuxuryHunt => "Values are much more expensive!",
                EconomyType.DumpsterDive => "Everything has become expensive!",
                EconomyType.ExtraProfit => "Today you can sell more profitably!",
                EconomyType.ScamSeason => "Prices are falling... :(",
                EconomyType.SuddenDrop => "Almost everything has depreciated!",
                EconomyType.TreasureRush => "Every value is - treasure!",
                EconomyType.ZeroGravity => "Everything is unstable...",
                EconomyType.Turbulence => "Prices behave unpredictably...",
                _ => "???",
            };
        }

        public static Color GetEconomyColor()
        {
            return CurrentEconomy switch
            {
                EconomyType.Stable => Color.white,
                EconomyType.Deflation => Color.red,
                EconomyType.Inflation => Color.green,
                EconomyType.Chaos => Color.cyan,
                EconomyType.FreezeMarket => Color.gray,
                EconomyType.BlackMarketSurge => new Color(1f, 0.84f, 0f),
                EconomyType.Overload => Color.magenta,
                EconomyType.RareBoom => new Color(0.7f, 0.2f, 1f),
                EconomyType.CommonCrash => Color.gray,
                EconomyType.LegendaryOnlyMatters => new Color(1f, 0.9f, 0.1f),
                EconomyType.EchoMarket => Color.yellow,
                EconomyType.ReverseInflation => Color.blue,
                EconomyType.LuxuryHunt => new Color(0.2f, 1f, 0.4f),
                EconomyType.DumpsterDive => new Color(0.6f, 0.3f, 0f),
                EconomyType.ExtraProfit => new Color(0.1f, 0.8f, 1f),
                EconomyType.ScamSeason => new Color(0.5f, 0f, 0.5f),
                EconomyType.SuddenDrop => new Color(1f, 0.4f, 0f),
                EconomyType.TreasureRush => new Color(1f, 0.65f, 0f),
                EconomyType.ZeroGravity => new Color(0.8f, 0.8f, 0.8f),
                EconomyType.Turbulence => new Color(0.4f, 0.6f, 0.8f),
                _ => Color.gray,
            };
        }

        public enum EconomyType
        {
            Stable,
            Deflation,
            Inflation,
            Chaos,
            FreezeMarket,
            BlackMarketSurge,
            Overload,
            RareBoom,
            CommonCrash,
            LegendaryOnlyMatters,
            EchoMarket,
            ReverseInflation,
            LuxuryHunt,
            DumpsterDive,
            ExtraProfit,
            ScamSeason,
            SuddenDrop,
            TreasureRush,
            ZeroGravity,
            Turbulence,
        }

        public static void ResetEconomy()
        {
            HasEconomy = false;
            MinPercent = 0;
            MaxPercent = 0;
            CurrentEconomy = EconomyType.Stable;
            Logger.LogInfo("[Economy] The economy has been reset. Ready for a new race.");
        }

        [HarmonyPatch(typeof(TruckScreenText), "ArrowPointAtGoalLogic")]
        public static class EconomyUITriggerPatch
        {
            [HarmonyPostfix]
            public static void ShowEconomyUI()
            {
                if (
                    !EconomyController.UIShownOnce
                    && SemiFunc.RunIsLevel()
                    && EconomyController.Instance != null
                )
                {
                    EconomyController.Instance.ShowGUI();
                    EconomyController.UIShownOnce = true;
                    LogTotalMapValue();
                }
            }
        }

        [HarmonyPatch(typeof(TruckScreenText), "Start")]
        public static class TruckTextStartReset
        {
            [HarmonyPrefix]
            public static void ResetFlag()
            {
                EconomyController.UIShownOnce = false;
                EconomyInitialized = false;
            }
        }

        [HarmonyPatch(typeof(ValuableObject), "DollarValueSetLogic")]
        public class ValuablePatch
        {
            [HarmonyPostfix]
            public static void ApplyRandomValue(ValuableObject __instance)
            {
                if (__instance == null)
                    return;

                if (SemiFunc.RunIsLobby() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
                    return;

                if (!REPOConomyPlugin.HasEconomy)
                {
                    if (!REPOConomyPlugin.pendingValuables.Contains(__instance))
                        REPOConomyPlugin.pendingValuables.Add(__instance);
                    return;
                }

                if (
                    AssetManager.instance == null
                    || AssetManager.instance.surplusValuableSmall == null
                    || AssetManager.instance.surplusValuableMedium == null
                    || AssetManager.instance.surplusValuableBig == null
                )
                {
                    REPOConomyPlugin.Logger.LogWarning(
                        "[REPOConomy] AssetManager or its values are missing, skipping…"
                    );
                    return;
                }

                if (
                    __instance.gameObject.name.Contains(
                        AssetManager.instance.surplusValuableSmall.name
                    )
                    || __instance.gameObject.name.Contains(
                        AssetManager.instance.surplusValuableMedium.name
                    )
                    || __instance.gameObject.name.Contains(
                        AssetManager.instance.surplusValuableBig.name
                    )
                )
                {
                    Logger.LogInfo($"[REPOConomy] Skipped (Surplus): {__instance.gameObject.name}");
                    return;
                }
                float baseValue = __instance.dollarValueOriginal;
                float percent;
                float newValue;

                if (REPOConfig.UseSeparateModifiers.Value)
                {
                    float neg =
                        REPOConomyPlugin.MinPercent < 0f
                            ? UnityEngine.Random.Range(0f, Mathf.Abs(REPOConomyPlugin.MinPercent))
                            : 0f;

                    float pos =
                        REPOConomyPlugin.MaxPercent > 0f
                            ? UnityEngine.Random.Range(0f, REPOConomyPlugin.MaxPercent)
                            : 0f;

                    percent = -neg + pos;
                    newValue = Mathf.Round(baseValue * (1f + percent / 100f));

                    Logger.LogInfo(
                        $"[REPOConomy] {__instance.gameObject.name} | {(neg > 0 ? "-" + neg.ToString("0.##") + "%" : "")}{(neg > 0 && pos > 0 ? " + " : "")}{(pos > 0 ? "+" + pos.ToString("0.##") + "%" : "")} = {percent:+0.##;-0.##}% | {baseValue} → {newValue} |"
                    );
                }
                else
                {
                    percent = UnityEngine.Random.Range(
                        REPOConomyPlugin.MinPercent,
                        REPOConomyPlugin.MaxPercent
                    );
                    newValue = Mathf.Round(baseValue * (1f + percent / 100f));

                    Logger.LogInfo(
                        $"[REPOConomy] {__instance.gameObject.name} | {baseValue} → {newValue} ({percent:+0.##;-0.##}%)"
                    );
                }

                __instance.dollarValueCurrent = newValue;
            }
        }
    }
}

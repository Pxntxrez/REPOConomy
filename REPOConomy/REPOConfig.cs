using BepInEx.Configuration;

namespace REPOConomyMod
{
    public static class REPOConfig
    {
        public static ConfigEntry<float> DefaultBaseMin;
        public static ConfigEntry<float> DefaultBaseMax;
        public static ConfigEntry<float> LevelScaleMin;
        public static ConfigEntry<float> LevelScaleMax;
        public static ConfigEntry<float> MinClampLimit;
        public static ConfigEntry<float> MaxClampLimit;
        public static ConfigEntry<bool> ForceStableEconomy;
        public static ConfigEntry<int> EventStartLevel;
        public static ConfigEntry<bool> UseEventChance;
        public static ConfigEntry<int> EventChance;
        public static ConfigEntry<bool> ShowTotalMapValue;
        public static ConfigEntry<bool> UseSeparateModifiers;

        public static void Init(ConfigFile config)
        {
            DefaultBaseMin = config.Bind(
                "Economy",
                "DefaultBaseMin",
                5f,
                new ConfigDescription(
                    "Base minimum (-%) value without modifiers.",
                    new AcceptableValueRange<float>(0.1f, 500f)
                )
            );

            DefaultBaseMax = config.Bind(
                "Economy",
                "DefaultBaseMax",
                10f,
                new ConfigDescription(
                    "Base maximum (+%) value without modifiers.",
                    new AcceptableValueRange<float>(0.1f, 500f)
                )
            );

            UseSeparateModifiers = config.Bind(
                "Economy",
                "UseTwoStepPercentRoll",
                false,
                "Use separate positive and negative percent rolls. Result = (base - rollMin) + rollMax."
            );

            LevelScaleMin = config.Bind(
                "LevelScale",
                "LevelScaleMin",
                0.5f,
                new ConfigDescription(
                    "How much to add to the minimum (-%) for each level.",
                    new AcceptableValueRange<float>(0.1f, 500f)
                )
            );

            LevelScaleMax = config.Bind(
                "LevelScale",
                "LevelScaleMax",
                0.5f,
                new ConfigDescription(
                    "How much to add to the maximum (+%) for each level.",
                    new AcceptableValueRange<float>(0.1f, 500f)
                )
            );

            MinClampLimit = config.Bind(
                "Limit",
                "ClampMinLimit",
                100f,
                new ConfigDescription(
                    "Maximum reduction limit (for example: -100%).",
                    new AcceptableValueRange<float>(0.1f, 500f)
                )
            );

            MaxClampLimit = config.Bind(
                "Limit",
                "ClampMaxLimit",
                1000f,
                new ConfigDescription(
                    "Maximum increase limit (for example: +1000%).",
                    new AcceptableValueRange<float>(0.1f, 10000f)
                )
            );

            EventStartLevel = config.Bind(
                "Events",
                "EventsStartLevel",
                2,
                new ConfigDescription(
                    "At what level will events start (1–12). 1 = immediately, 2 = from the second level, etc.",
                    new AcceptableValueRange<int>(1, 12)
                )
            );

            ForceStableEconomy = config.Bind(
                "Events",
                "NoEvents",
                false,
                "Force enable a stable economy (without events)."
            );

            UseEventChance = config.Bind(
                "EventsChance",
                "UseEventsChance",
                true,
                "Enable the event drop system based on chance."
            );

            EventChance = config.Bind(
                "EventsChance",
                "EventChance",
                50,
                new ConfigDescription(
                    "Chance (1–100) for an event to occur instead of a stable economy.",
                    new AcceptableValueRange<int>(1, 100)
                )
            );

            ShowTotalMapValue = config.Bind(
                "Debug",
                "ShowTotalMapValue",
                false,
                "Show total value of all valuables on the map in logs."
            );
        }
    }
}

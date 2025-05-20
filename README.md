## ⚠️ IMPORTANT NOTICE: HOST-ONLY MOD ⚠️  
**This mod is ONLY required for the host in multiplayer games.** Other players do not need to install this mod for it to work properly.  

**Clients can install it to see the same synced economy UI as the host**

---

[![Thunderstore Profile](https://img.shields.io/badge/Thunderstore-Profile-4065F2?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/PxntxrezStudio/)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/PxntxrezStudio/REPOConomy?style=for-the-badge&logo=thunderstore&logoColor=white&color=40c4ff)](https://thunderstore.io/c/repo/p/PxntxrezStudio/REPOConomy/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/PxntxrezStudio/REPOConomy?style=for-the-badge&logo=thunderstore&logoColor=white&color=00e111)](https://thunderstore.io/c/repo/p/PxntxrezStudio/REPOConomy/)

---
> **If you like my work, you can:** [![DonationAlerts](https://i.imgur.com/OMyWf9T.png)](https://www.donationalerts.com/r/pxntxrez)

# REPOConomy  
A full-featured dynamic economy mod for R.E.P.O.

Every level, the game has a 50% chance (configurable) to roll one of 20+ unique economy types — like Inflation, Deflation, or Chaos — each affecting how much valuables are worth. Events start from level 2 and up (also configurable).

Valuable prices are no longer fixed. Instead, they dynamically change based on random economic conditions, level scaling, and custom config settings.

The result? A fresh, unpredictable economy every run!

---

## Features

- **Dynamic Economy Types**: A dozen+ market conditions (Stable, Inflation, Chaos, etc.) each with unique value modifiers  
- **Automatic Daily Events**: New “economy events” unlock as you progress or on a schedule  
- **Value Randomizer**: Adjusts the dollar‐value of valuables on spawn within configured bounds  
- **Real-Time Sync**: MasterClient rolls the economy and shares **UI** with all players via Photon (if they also use this mod)
- **On-Screen HUD Overlay**: Shows economy type, level scaling, and value range (toggle with **G**) 
or automatically hides after 6 seconds 
- **Fully Configurable**: Tweak every parameter via BepInEx config or in-game GUI with **REPOConfig**

## Compatibility

- Supports valuables from **Other Mods**
- Supports the latest **Beta Version** of **R.E.P.O.**

## Installation

1. Install **[BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) for R.E.P.O.**  
2. (Optional) Install [**REPOConfig**](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/) for in-game GUI:   
3. Copy `REPOConomy.dll` into your `BepInEx/plugins` folder  
4. Launch R.E.P.O.

---

## Configuration
 💡 **Recommended:** Install the [**REPOConfig**](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/) mod to change these settings directly in-game via a GUI.

**Located in:**  
`BepInEx/config/Pxntxrez.REPOConomy.cfg`

Below are all available configuration options.

---

### 🔹 Economy

| Setting                | Description                                             | Default |
|------------------------|---------------------------------------------------------|---------|
| DefaultBaseMin         | Base minimum multiplier applied before level scaling    | 5.0     |
| DefaultBaseMax         | Base maximum multiplier applied before level scaling    | 10.0    |
| UseTwoStepPercentRoll  | Separate negative and positive percent rolls           | false   |

---

### 🔹 LevelScale

| Setting            | Description                                           | Default |
|--------------------|-------------------------------------------------------|---------|
| LevelScaleMin      | Percent increase added to minimum per level           | 0.5     |
| LevelScaleMax      | Percent increase added to maximum per level           | 0.5     |

---

### 🔹 Limits

| Setting           | Description                                       | Default |
|-------------------|---------------------------------------------------|---------|
| ClampMinLimit     | Lowest allowed percent (e.g. –100% reduction)     | 100.0   |
| ClampMaxLimit     | Highest allowed percent (e.g. +1000% increase)    | 1000.0  |

---

### 🔹 Events

| Setting               | Description                                                          | Default |
|-----------------------|----------------------------------------------------------------------|---------|
| EventsStartLevel      | Level at which economy events can begin                             | 2       |
| NoEvents              | Force all runs to use Stable economy (disable events)                | false   |

---

### 🔹 EventsChance

| Setting             | Description                                                       | Default |
|---------------------|-------------------------------------------------------------------|---------|
| UseEventsChance     | Enable random event roll per level                                | true    |
| EventChance         | Chance (%) to trigger an economy event when permitted             | 50      |

---

### 🔹 Debug

| Setting             | Description                                               | Default |
|---------------------|-----------------------------------------------------------|---------|
| ShowTotalMapValue   | Log (In console) total dollar value of all valuables on the map       | false   |

---

## 🎲 Economy Events & Effects

Below are all available **Events** and their effects.  
***More events will be added with every updates!***

| Event (EconomyType)       | Effect on `baseMin` / `baseMax`                     | In-Game Description                       |
|---------------------------|----------------------------------------|-------------------------------------------|
| **Stable**                | No change (Uses the values from the config)        | Economy by default                        |
| **Deflation**             | –10% min, –5% max                       | Market Crash                              |
| **Inflation**             | +0% min, +10% max                       | Moment to Get Rich!                       |
| **Chaos**                 | –10% min, +10% max                      | Financial chaos...                        |
| **FreezeMarket**          | 0% min, 0% max                          | Economy frozen :(                         |
| **BlackMarketSurge**      | –10% min, +20% max                      | Values have become more expensive...      |
| **Overload**              | –100% min, +100% max                    | 50/50                                     |
| **RareBoom**              | +5% min, +30% max                       | Values are suddenly in price!             |
| **CommonCrash**           | –30% min, –10% max                      | Values are devalued!                      |
| **LegendaryOnlyMatters**  | –20% min, +5% max                       | Only the best makes sense...              |
| **EchoMarket**            | –25%→0% min, 0%→+25% max (random)      | Prices are jumping like crazy...          |
| **ReverseInflation**      | –15% min, –5% max                       | Budget is better than wealth...           |
| **LuxuryHunt**            | +10% min, +40% max                      | Values are much more expensive!           |
| **DumpsterDive**          | +5% min, +60% max                       | Everything has become expensive!          |
| **ExtraProfit**           | +0% min, +50% max                       | Today you can sell more profitably!       |
| **ScamSeason**            | –30% min, –10% max                      | Prices are falling... :(                  |
| **SuddenDrop**            | –50% min, –20% max                      | Almost everything has depreciated!        |
| **TreasureRush**          | +100% min, +200% max                    | Every value is — treasure!                |
| **ZeroGravity**           | –50% min, +150% max                     | Everything is unstable...                 |
| **Turbulence**            | ±30% equally to min & max (random)     | Prices behave unpredictably...            |


_All events roll each level (if level ≥ EventsStartLevel and chance roll succeeds) and are clamped between ClampMinLimit and ClampMaxLimit._

## Developer Contact
**Report bugs, suggest features, or provide feedback:**

| **Discord Server** | **Channel** | **Post** |  
|--------------------|-----------|----------|  
| [R.E.P.O. Modding Server](https://discord.com/invite/vPJtKhYAFe) | `#released-mods` | [REPOConomy](https://discord.com/channels/1344557689979670578/1374483473041068032) |

using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace HearthSimulator
{
    public static class PluginConfig
    {
        public static ConfigEntry<bool> isPluginEnable;
        public static ConfigEntry<int> fakePackCount;
        public static ConfigEntry<BoosterDbId> fakeBoosterDbId;

        public static ConfigEntry<bool> isRandomResult;
        public static ConfigEntry<bool> isRandomRarity;
        public static ConfigEntry<bool> isRandomPremium;
        public static ConfigEntry<bool> isRandomDiamond;

        public static ConfigEntry<TAG_PREMIUM> randomPremium;
        public static ConfigEntry<Utils.CardRarity> randomRarity;

        public static ConfigEntry<int> cardID1;
        public static ConfigEntry<TAG_PREMIUM> cardPremium1;
        public static ConfigEntry<int> cardID2;
        public static ConfigEntry<TAG_PREMIUM> cardPremium2;
        public static ConfigEntry<int> cardID3;
        public static ConfigEntry<TAG_PREMIUM> cardPremium3;
        public static ConfigEntry<int> cardID4;
        public static ConfigEntry<TAG_PREMIUM> cardPremium4;
        public static ConfigEntry<int> cardID5;
        public static ConfigEntry<TAG_PREMIUM> cardPremium5;

        
        public static void ConfigBind(ConfigFile config)
        {
            config.Clear();
            isPluginEnable = config.Bind("Global", "HearthSimulatorStatus", false, "Enable HearthSimulator? (Maybe need to restart Hearthstone)");
            fakePackCount = config.Bind("Simulator", "Count", 233, "Number of Card Packs.");
            fakeBoosterDbId = config.Bind("Simulator", "Type", BoosterDbId.GOLDEN_CLASSIC_PACK, "Card Pack Type.(Just replace Packs icon)");
            isRandomResult = config.Bind("Simulator", "RandomResult", false, "Random Result?(Highest Priority)");
            isRandomRarity = config.Bind("Simulator", "RandomRarity", false, "Random Rarity?(Based on RandomResult)");
            isRandomPremium = config.Bind("Simulator", "RandomPremium", false, "Random Premium?(Based on RandomResult)");
            isRandomDiamond = config.Bind("Simulator", "RandomDiamond", false, "Random Premium include Diamond?(Based on RandomPremium)");
            randomRarity = config.Bind("Simulator", "RandomRarityType", Utils.CardRarity.LEGENDARY, "Random Rarity Type(Based on RandomRarity)");
            randomPremium = config.Bind("Simulator", "RandomPremiumType", TAG_PREMIUM.GOLDEN, "Random Premium Type(Based on RandomPremium)");

            cardID1 = config.Bind("Simulator", "CardID1", 71984, "Card 1 DbID.");
            cardPremium1 = config.Bind("Simulator", "CardPremium1", TAG_PREMIUM.GOLDEN, "Card 1 Premium.");
            cardID2 = config.Bind("Simulator", "CardID2", 71945, "Card 2 DbID.");
            cardPremium2 = config.Bind("Simulator", "CardPremium2", TAG_PREMIUM.GOLDEN, "Card 2 Premium.");
            cardID3 = config.Bind("Simulator", "CardID3", 73446, "Card 3 DbID.");
            cardPremium3 = config.Bind("Simulator", "CardPremium3", TAG_PREMIUM.GOLDEN, "Card 3 Premium.");
            cardID4 = config.Bind("Simulator", "CardID4", 71781, "Card 4 DbID.");
            cardPremium4 = config.Bind("Simulator", "CardPremium4", TAG_PREMIUM.GOLDEN, "Card 4 Premium.");
            cardID5 = config.Bind("Simulator", "CardID5", 67040, "Card 5 DbID.");
            cardPremium5 = config.Bind("Simulator", "CardPremium5", TAG_PREMIUM.GOLDEN, "Card 5 Premium.");
        }

        public static ConfigValue configValue = new ConfigValue();
        public static List<int> GetCardsDbId()
        {
            List<int> cardsDbId = new List<int>();
            foreach(int dbid in GameDbf.GetIndex().GetCollectibleCardDbIds())
            {
                var entitydef = DefLoader.Get().GetEntityDef(dbid, false);
                if (entitydef != null)
                {
                    if (entitydef.GetRarity() != TAG_RARITY.FREE
                        && entitydef.GetRarity() != TAG_RARITY.INVALID)
                    {
                        if (entitydef.GetCardType() != TAG_CARDTYPE.HERO)
                            cardsDbId.Add(dbid);
                        else if (entitydef.GetCost() != 0)    // ignore hero skin
                            cardsDbId.Add(dbid);
                    }
                }
            }
            return cardsDbId;
        }

        public static int GetRandomCardID(TAG_RARITY rarity)
        {
            int dbid;
            List<int> dbids = GetCardsDbId();
            while (true)
            {
                dbid = dbids[UnityEngine.Random.Range(0, dbids.Count)];
                if (DefLoader.Get().GetEntityDef(dbid, false).GetRarity() == rarity)
                {
                    break;
                }
            }
            return dbid;
        }

        public static void GenerateRandomCard(bool rarityRandom = false, bool premiumRandom = false, TAG_RARITY rarity = TAG_RARITY.LEGENDARY, TAG_PREMIUM premium = TAG_PREMIUM.GOLDEN)
        {
            if (!rarityRandom) rarity = (TAG_RARITY)randomRarity.Value;
            if (!premiumRandom) premium = randomPremium.Value;
            if (fakeBoosterDbId.Value.ToString().Substring(0, 7) == "GOLDEN_")
            {
                premiumRandom = false;
                premium = TAG_PREMIUM.GOLDEN;
            }
            List<int> dbids = GetCardsDbId();
            for (int i = 1; i <= 5; i++)
            {
                if (premiumRandom)
                {
                    if (!isRandomDiamond.Value)
                    {
                        premium = (TAG_PREMIUM)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TAG_PREMIUM)).Length - 1);
                    }
                    else premium = (TAG_PREMIUM)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TAG_PREMIUM)).Length);
                }
                switch (i)
                {
                    case 1:
                        cardID1.Value = rarityRandom ? dbids[UnityEngine.Random.Range(0, dbids.Count)] : GetRandomCardID(rarity);
                        cardPremium1.Value = premium;
                        break;
                    case 2:
                        cardID2.Value = rarityRandom ? dbids[UnityEngine.Random.Range(0, dbids.Count)] : GetRandomCardID(rarity);
                        cardPremium2.Value = premium;
                        break;
                    case 3:
                        cardID3.Value = rarityRandom ? dbids[UnityEngine.Random.Range(0, dbids.Count)] : GetRandomCardID(rarity);
                        cardPremium3.Value = premium;
                        break;
                    case 4:
                        cardID4.Value = rarityRandom ? dbids[UnityEngine.Random.Range(0, dbids.Count)] : GetRandomCardID(rarity);
                        cardPremium4.Value = premium;
                        break;
                    case 5:
                        cardID5.Value = rarityRandom ? dbids[UnityEngine.Random.Range(0, dbids.Count)] : GetRandomCardID(rarity);
                        cardPremium5.Value = premium;
                        break;
                }
            }
        }


    }
    public class ConfigValue
    {
        public bool IsPluginEnableValue
        {
            get { return PluginConfig.isPluginEnable.Value; }
            set { PluginConfig.isPluginEnable.Value = value; }
        }
        public static ConfigValue Get()
        {
            return PluginConfig.configValue;
        }
    }
}

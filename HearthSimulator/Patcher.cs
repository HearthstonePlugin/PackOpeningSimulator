using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using static HearthSimulator.PluginConfig;

namespace HearthSimulator
{
    //public struct AllPatch
    //{
    //    Harmony mHarmony;
    //    Type mType;
    //}
    public static class PatchManager
    {
        public static List<Harmony> AllHarmony = new List<Harmony>();    // Cache Patch Info
        public static List<string> AllHarmonyName = new List<string>();

        public static void LoadPatch(Type loadType)
        {
            Harmony harmony;
            int harmonyCount;
            harmony = Harmony.CreateAndPatchAll(loadType);
            harmonyCount = harmony.GetPatchedMethods().Count();
            Utils.MyLogger(BepInEx.Logging.LogLevel.Warning, $"{loadType.Name} => Patched {harmonyCount} methods");
            AllHarmony.Add(harmony);
            AllHarmonyName.Add(loadType.Name);
        }
        public static bool UnPatch(string name)
        {

            for (int i = 0; i < AllHarmonyName.Count; i++)
            {
                if (AllHarmonyName[i] == name)
                {
                    AllHarmony[i].UnpatchSelf();
                    AllHarmonyName.Remove(AllHarmonyName[i]);
                    AllHarmony.Remove(AllHarmony[i]);
                    Utils.MyLogger(BepInEx.Logging.LogLevel.Warning, $"Unpatched {name}!");
                    return true;
                }
            }
            return false;
        }

        public static void PatchSettingDelegate()
        {
            isPluginEnable.SettingChanged += delegate
            {
                if (isPluginEnable.Value)
                {
                    PatchAll();
                }
                else UnPatchAll();
                PackOpening.Get().UpdatePacks();
            };
        }
        public static void PatchAll()
        {
            LoadPatch(typeof(Patcher));

        }
        public static void UnPatchAll()
        {
            for (int i = 0; i < AllHarmony.Count; i++)
            {
                AllHarmony[i].UnpatchSelf();
                Utils.MyLogger(BepInEx.Logging.LogLevel.Warning, $"Unpatched {AllHarmonyName[i]}!");
            }
            AllHarmony.Clear();
            AllHarmonyName.Clear();
        }
        public static void RePatchAll()
        {
            UnPatchAll();
            PatchAll();
        }
    }

    public class Patcher
    {
        //activate fake pack opening
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameUtils), "IsFakePackOpeningEnabled")]
        public static bool PatchIsFakePackOpeningEnabled(ref bool __result)
        {
            __result = true;
            return false;
        }
        //setting fake pack count
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameUtils), "GetFakePackCount")]
        public static bool PatchGetFakePackCount(ref int __result)
        {
            __result = fakePackCount.Value >= 0 ? fakePackCount.Value : 0;
            return false;
        }
        //setting pack type
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetCache), "GetTestData")]
        public static bool PatchGetTestData(ref Type type, ref object __result)
        {
            if (type == typeof(NetCache.NetCacheBoosters) && GameUtils.IsFakePackOpeningEnabled())
            {
                NetCache.NetCacheBoosters netCacheBoosters = new NetCache.NetCacheBoosters();
                int fakePackCount = GameUtils.GetFakePackCount();
                NetCache.BoosterStack boosterStack = new NetCache.BoosterStack
                {
                    Id = (int)fakeBoosterDbId.Value,
                    Count = fakePackCount
                };
                netCacheBoosters.BoosterStacks.Add(boosterStack);
                __result = netCacheBoosters;
                return false;
            }
            __result = null;
            return false;
        }

        //Simulator
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(MonoBehaviour), "StopCoroutine", new Type[] { typeof(Coroutine) })]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void PackOpeningBaseStopCoroutine(PackOpening instance, Coroutine routine) {; }
        private static readonly MethodInfo onDirectorFinished = typeof(PackOpening).GetMethod("OnDirectorFinished",
            BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo onBoosterOpened = typeof(PackOpening).GetMethod("OnBoosterOpened",
            BindingFlags.Instance | BindingFlags.NonPublic);
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PackOpening), "OpenBooster")]
        public static bool PatchOpenBooster(ref UnopenedPack pack,
                                            ref PackOpening __instance,
                                            ref float ___m_packOpeningStartTime,
                                            ref int ___m_packOpeningId,
                                            ref GameObject ___m_InputBlocker,
                                            ref Coroutine ___m_autoOpenPackCoroutine,
                                            ref PackOpeningDirector ___m_director,
                                            ref int ___m_lastOpenedBoosterId,
                                            ref UIBScrollable ___m_UnopenedPackScroller
            )
        {
            Hearthstone.Progression.AchievementManager.Get().PauseToastNotifications();
            int num = (int)fakeBoosterDbId.Value;
            if (!GameUtils.IsFakePackOpeningEnabled())
            {
                num = pack.GetBoosterId();
                ___m_packOpeningStartTime = Time.realtimeSinceStartup;
                ___m_packOpeningId = num;
                BoosterPackUtils.OpenBooster(num);
            }
            ___m_InputBlocker.SetActive(true);
            if (___m_autoOpenPackCoroutine != null)
            {
                PackOpeningBaseStopCoroutine(__instance, ___m_autoOpenPackCoroutine);
                ___m_autoOpenPackCoroutine = null;
            }

            object target = __instance;
            Delegate myDelegate = Delegate.CreateDelegate(typeof(EventHandler), target, onDirectorFinished);
            EventHandler myMethod = myDelegate as EventHandler;
            ___m_director.OnFinishedEvent += myMethod;
            ___m_lastOpenedBoosterId = num;
            BnetBar.Get().HideCurrencyTemporarily();
            if (GameUtils.IsFakePackOpeningEnabled())
            {
                onBoosterOpened?.Invoke(__instance, null);
            }
            ___m_UnopenedPackScroller.Pause(true);
            return false;
        }
        // result replace
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PackOpening), "OnBoosterOpened")]
        public static bool PatchOnBoosterOpened(ref float ___m_packOpeningStartTime,
                                           ref PackOpeningDirector ___m_director,
                                           ref int ___m_lastOpenedBoosterId,
                                           ref int ___m_packOpeningId,
                                           ref bool ___m_autoOpenPending)
        {
            float timeToRegisterPackOpening = Time.realtimeSinceStartup - ___m_packOpeningStartTime;
            ___m_director.Play(___m_lastOpenedBoosterId, timeToRegisterPackOpening, ___m_packOpeningId);
            ___m_autoOpenPending = false;
            //List<NetCache.BoosterCard> list = Network.Get().OpenedBooster();
            if (isRandomResult.Value)
            {
                GenerateRandomCard(isRandomRarity.Value, isRandomPremium.Value);
            }
            List<NetCache.BoosterCard> cards = new List<NetCache.BoosterCard>
            {
                new NetCache.BoosterCard
                {
                    Def = {
                            Name = GameUtils.TranslateDbIdToCardId(cardID1.Value),
                            Premium = cardPremium1.Value
                        }
                },
                new NetCache.BoosterCard
                {
                    Def = {
                            Name = GameUtils.TranslateDbIdToCardId(cardID2.Value),
                            Premium = cardPremium2.Value
                        }
                },
                new NetCache.BoosterCard
                {
                    Def = {
                            Name = GameUtils.TranslateDbIdToCardId(cardID3.Value),
                            Premium = cardPremium3.Value
                        }
                },
                new NetCache.BoosterCard
                {
                    Def = {
                            Name = GameUtils.TranslateDbIdToCardId(cardID4.Value),
                            Premium = cardPremium4.Value
                        }
                },
                new NetCache.BoosterCard
                {
                    Def = {
                            Name = GameUtils.TranslateDbIdToCardId(cardID5.Value),
                            Premium = cardPremium5.Value
                        }
                }
            };
            ___m_director.OnBoosterOpened(cards);
            return false;
        }
    }

    public static class PackOpeningPatch
    {
        private static readonly MethodInfo onReloginComplete = typeof(PackOpening).GetMethod("OnReloginComplete", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo updatePacks = typeof(PackOpening).GetMethod("UpdatePacks", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void OnReloginComplete(this PackOpening __instance)
        {
            onReloginComplete?.Invoke(__instance, null);
        }

        public static void UpdatePacks(this PackOpening __instance)
        {
            updatePacks?.Invoke(__instance, null);
        }
    }

}


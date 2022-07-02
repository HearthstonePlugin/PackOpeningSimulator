using BepInEx;
using System.Linq;
using UnityEngine;
using static HearthSimulator.PluginConfig;


namespace HearthSimulator
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            ConfigBind(Config);
            PatchManager.PatchSettingDelegate();
            if (isPluginEnable.Value)
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
                PatchManager.PatchAll();
            }
            else
            {
                OnDestroy();
                return;
            }
        }

        private void Start()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is started!");
            if (isPluginEnable.Value)
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is started!");
            }
            else
            {
                OnDestroy();
                return;
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F7))
            {
                int allPatchNum = 0;
                foreach (var tempatch in PatchManager.AllHarmony)
                {
                    allPatchNum += tempatch.GetPatchedMethods().Count();
                }
                UIStatus.Get().AddInfo($"[{allPatchNum}] Status：" + (isPluginEnable.Value ? "Running" : "Stopped"));
            }
        }

        private void OnDestroy()
        {
            PatchManager.UnPatchAll();
        }

    }

}
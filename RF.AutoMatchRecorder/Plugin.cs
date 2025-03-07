using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using RF.AutoMatchRecorder.Plugins;
using UnityEngine;
using System.Collections;

namespace RF.AutoMatchRecorder
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public const string ModName = "RF.AutoMatchRecorder";

        public static Plugin Instance;
        private Harmony _harmony = null;
        public new static ManualLogSource Log;


        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<string> ConfigObsHost;
        public ConfigEntry<int> ConfigObsPort;
        public ConfigEntry<string> ConfigObsPassword;


        public override void Load()
        {
            Instance = this;

            Log = base.Log;

            SetupConfig();
            SetupHarmony();
        }

        private void SetupConfig()
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");

            ConfigObsHost = Config.Bind("OBS",
                "Host",
                "127.0.0.1",
                "Host of OBS web socket"
            );

            ConfigObsPort = Config.Bind("OBS",
                "Port",
                4455,
                "Port of OBS web socket"
            );

            ConfigObsPassword = Config.Bind("OBS",
                "Password",
                "",
                "Password for OBS web socket"
            );
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            LoadPlugin();
        }

        public static void LoadPlugin()
        {
            if (Instance.ConfigEnabled.Value)
            {
                bool result = true;
                // If any PatchFile fails, result will become false
                //result &= Instance.PatchFile(typeof(SwapJpEngTitlesPatch));
                //result &= Instance.PatchFile(typeof(AdjustUraFlipTimePatch));
                //SwapJpEngTitlesPatch.SetOverrideLanguages();
                if (result)
                {
                    Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
                }
                else
                {
                    Log.LogError($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.");
                    // Unload this instance of Harmony
                    // I hope this works the way I think it does
                    Instance._harmony.UnpatchSelf();
                }
            }
            else
            {
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
#if DEBUG
                Log.LogInfo("File patched: " + type.FullName);
#endif
                return true;
            }
            catch (Exception e)
            {
                Log.LogInfo("Failed to patch file: " + type.FullName);
                Log.LogInfo(e.Message);
                return false;
            }
        }

        public static void UnloadPlugin()
        {
            Instance._harmony.UnpatchSelf();
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} has been unpatched.");
        }

        public static void ReloadPlugin()
        {
            // Reloading will always be completely different per mod
            // You'll want to reload any config file or save data that may be specific per profile
            // If there's nothing to reload, don't put anything here, and keep it commented in AddToSaveManager
            //SwapSongLanguagesPatch.InitializeOverrideLanguages();
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.Reload();
        }

        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return GetMonoBehaviour().StartCoroutine(enumerator);
        }
    }
}

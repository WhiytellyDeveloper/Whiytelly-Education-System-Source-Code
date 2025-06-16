using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System.Collections;
using TeacherAPI;
using UnityEngine;
using WhiytellyEducationSystem.GameLoaders;

namespace WhiytellyEducationSystem
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", MTM101BaldiDevAPI.VersionNumber)]
    [BepInProcess("BALDI.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin _instance { get; private set; }

        private void Awake()
        {
            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            _instance = this;
            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, PreLoadingAssets(), false);
            LoadingEvents.RegisterOnAssetsLoaded(Info, PostLoadingAssets(), true);

            ModdedSaveGame.AddSaveHandler(Info);
        }

        private IEnumerator PreLoadingAssets()
        {
            yield return 1;
            yield return "W.E.S.P Pre Loading";
            CharacterLoader.Intialize(this);

            GeneratorManagement.Register(this, GenerationModType.Override, ChangeFloorTest);
        }

        private IEnumerator PostLoadingAssets()
        {
            yield return 1;
            yield return "W.E.S.P Post Loading";
        }

        public void ChangeFloorTest(string floorName, int floorNum, SceneObject scene)
        {
            foreach (var level in scene.GetCustomLevelObjects()) level.AddPotentialTeacher(CharacterLoader.whiytelly, 99999999);
            
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "whiytellydeveloper.plugin.plusmod.wesp";
        public const string PLUGIN_NAME = "Whiytelly Education System Plus";
        public const string PLUGIN_VERSION = "1.0";
    }
}

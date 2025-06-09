using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using WhiytellyEducationSystem.Characters;

namespace WhiytellyEducationSystem.GameLoaders
{
    public static class CharacterLoader 
    {
        public static void Intialize(Plugin plugin)
        {
            var whiyPosterTexture = AssetLoader.TextureFromMod(plugin, ["Characters", "Whiytelly", "Textures", "pri_whiltelly.png"]);

             whiytelly = new NPCBuilder<Whiytelly>(plugin.Info)
            .SetName("Whiytelly").SetEnum("Whiytelly").SetMetaName("Whiytelly")
            .AddLooker().SetMaxSightDistance(250).SetFOV(180)
            .AddHeatmap().AddTrigger()
            .SetForcedSubtitleColor(new(70f/255f, 21f/255f, 142f/255f, 1f))
            .SetMinMaxAudioDistance(50, 150)
            .SetWanderEnterRooms()
            .SetPoster(whiyPosterTexture, "Whiytelly", "Test")
            .Build();

            whiytelly.gameObject.ConvertToPrefab(false);
            whiytelly.PreInitialize();
            whiytelly.gameObject.ConvertToPrefab(true);
        }


        public static Whiytelly whiytelly;
    }
}

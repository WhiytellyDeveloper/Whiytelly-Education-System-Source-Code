using HarmonyLib;
using MTM101BaldAPI;
using UnityEngine;

namespace WhiytellyEducationSystem.Helpers
{
    public static class AudioExtensions
    {
        public static SoundObject ConvertToSoundObject(this AudioClip clip, string subKey, SoundType type, Color color) => ObjectCreators.CreateSoundObject(clip, subKey, type, color);
        public static SoundObject ConvertToSoundObject(this AudioClip clip, string subKey, SoundType type) => ObjectCreators.CreateSoundObject(clip, subKey, type, Color.white);
        public static SoundObject AddTimedSubKey(this SoundObject sound, string key, float time)
        {
            sound.additionalKeys = sound.additionalKeys.AddRangeToArray([new SubtitleTimedKey { key = key, time = time }]);
            return sound;
        }
    }
}

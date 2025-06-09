using PixelInternalAPI.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhiytellyEducationSystem.Helpers
{
    public class KapoAnimator : MonoBehaviour
    {
        public void Initialize(SpriteRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void AddAnimation(string name, Sprite[] sprites, int[] indexs, float time, bool asr = false)
        {
            var animation = new KapoStandardAnimation();
            animation.name = name;
            animation.time = time;
            animation.indexs = indexs;
            animation.sprites = sprites;
            animation.arsAnim = asr;
            animations.Add(name, animation);
        }

        public void PlayAnimation(string name, bool __override, bool loop)
        {
            if (PlayingAnimation != null && __override)
            {
                StopCoroutine(PlayingAnimation);
                PlayingAnimation = null;
            };

            PlayingAnimation = AnimationPlaying(animations[name]);
            animations[name].isLoop = loop;

            if (animations[name] is KapoStandardAnimation && asr != null)
            {
                var __anim = animations[name] as KapoStandardAnimation;
                asr.enabled = __anim.arsAnim;
                asr.BypassRotation(!__anim.arsAnim);
            }


            StartCoroutine(PlayingAnimation);
        }

        public IEnumerator AnimationPlaying(KapoBasicAnimation anim)
        {
            if (anim is KapoStandardAnimation standardAnim)
            {
                if (playingAnim != null) playingAnim.isPlaying = false;
                

                anim.isPlaying = true;
                playingAnim = anim;

                do {
                    foreach (int index in standardAnim.indexs)
                    {
                        float time = standardAnim.time;

                        if (!standardAnim.arsAnim)
                            renderer.sprite = standardAnim.sprites[index];
                        else 
                            asr.targetSprite = standardAnim.sprites[index];

                        while (time > 0f)
                        {
                            time -= Time.deltaTime;
                            yield return null;
                        }
                    }

                    yield return null;
                }
                while (anim.isLoop);
            }

            anim.isPlaying = false;
            PlayingAnimation = null;
        }


        public SpriteRenderer renderer;
        public AnimatedSpriteRotator asr;
        public Dictionary<string, KapoBasicAnimation> animations = [];

        private IEnumerator PlayingAnimation;
        public KapoBasicAnimation playingAnim;
    }

    public class KapoBasicAnimation
    {
        public string name = "Anim_";
        public bool isLoop = false;
        public bool isPlaying;
    }

    public class KapoStandardAnimation : KapoBasicAnimation
    {
        public Sprite[] sprites = [];
        public int[] indexs = [];
        public float time = 1f;
        public bool arsAnim;
    }
}

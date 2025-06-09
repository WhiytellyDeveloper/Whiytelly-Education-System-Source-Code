using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Reflection;
using PixelInternalAPI.Extensions;
using System.Collections;
using System.IO;
using System.Linq;
using TeacherAPI;
using UnityEngine;
using WhiytellyEducationSystem.Helpers;

namespace WhiytellyEducationSystem.Characters
{
    public class Whiytelly : Teacher
    {
        public void PreInitialize()
        {
            var helloSprites = AssetLoader.SpritesFromSpritesheet(4, 3, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "Whiytelly_HelloSpriteSheet.png"]));
            var countdownSprites = AssetLoader.SpritesFromSpritesheet(2, 1, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "Whiytelly_CoutingSpriteSheet.png"]));
            var walking1Sprites = AssetLoader.SpritesFromSpritesheet(4, 2, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "Whiytelly_WalkingSpriteSheet0.png"]));
            var walking2Sprites = AssetLoader.SpritesFromSpritesheet(4, 2, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "Whiytelly_WalkingSpriteSheet1.png"]));
            var walking3Sprites = AssetLoader.SpritesFromSpritesheet(4, 2, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "Whiytelly_WalkingSpriteSheet2.png"]));
            var knockSprites = AssetLoader.SpritesFromSpritesheet(4, 2, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "Whiytelly_KnockSpriteSheet.png"]));
            whiytellIndicatorSprites = AssetLoader.SpritesFromSpritesheet(3, 3, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", "WhiytellyIndicator.png"]));

            spriteRenderer[0].sprite = helloSprites[0];
            spriteRenderer[0].transform.localPosition = Vector3.zero;
            spriteBase.transform.localPosition = new(0f, -1.25f, 0f);

            assetMan.AddRange(helloSprites);
            assetMan.AddRange(countdownSprites);

            SpriteRotationMap walkMap1 = GenericExtensions.CreateRotationMap(8, walking1Sprites);
            SpriteRotationMap walkMap2 = GenericExtensions.CreateRotationMap(8, walking2Sprites);
            SpriteRotationMap walkMap3 = GenericExtensions.CreateRotationMap(8, walking3Sprites);
            SpriteRotationMap knock = GenericExtensions.CreateRotationMap(8, knockSprites);

            animator = gameObject.AddComponent<KapoAnimator>();
            animator.asr = spriteRenderer[0].CreateAnimatedSpriteRotator([walkMap1, walkMap2, walkMap3]);
            animator.asr.targetSprite = walking1Sprites[0];

            placeholder = walking1Sprites[0];
            placeholder2 = walking2Sprites[0];
            placeholder3 = walking3Sprites[0];

            audMan = GetComponent<AudioManager>();

            AssetLoader.LocalizationFromFile(Path.Combine([AssetLoader.GetModPath(Plugin._instance), "Characters", "Whiytelly", "Language", "Whiy_English.json"]), Language.English);

            AudioClip LoadSound(string name) => AssetLoader.AudioClipFromMod(Plugin._instance, ["Characters", "Whiytelly", "Audio", name]);

            audHello = LoadSound("Whiytelly_HellowF1.wav").ConvertToSoundObject("Whiytelly_HellowF1_0", SoundType.Voice).AddTimedSubKey("Whiytelly_HellowF1_1", 3.594f);
            audStartCountdown = LoadSound("Whiytelly_StartCountdown.wav").ConvertToSoundObject("Whiytelly_CountdownStart", SoundType.Voice);
            audCountdown[0] = LoadSound("Whiytelly_10.wav").ConvertToSoundObject("Whiytelly_10", SoundType.Voice);
            audCountdown[1] = LoadSound("Whiytelly_9.wav").ConvertToSoundObject("Whiytelly_9", SoundType.Voice);
            audCountdown[2] = LoadSound("Whiytelly_8.wav").ConvertToSoundObject("Whiytelly_8", SoundType.Voice);
            audCountdown[3] = LoadSound("Whiytelly_7.wav").ConvertToSoundObject("Whiytelly_7", SoundType.Voice);
            audCountdown[4] = LoadSound("Whiytelly_6.wav").ConvertToSoundObject("Whiytelly_6", SoundType.Voice);
            audCountdown[5] = LoadSound("Whiytelly_5.wav").ConvertToSoundObject("Whiytelly_5", SoundType.Voice);
            audCountdown[6] = LoadSound("Whiytelly_4.wav").ConvertToSoundObject("Whiytelly_4", SoundType.Voice);
            audCountdown[7] = LoadSound("Whiytelly_3.wav").ConvertToSoundObject("Whiytelly_3", SoundType.Voice);
            audCountdown[8] = LoadSound("Whiytelly_2.wav").ConvertToSoundObject("Whiytelly_2", SoundType.Voice);
            audCountdown[9] = LoadSound("Whiytelly_1.wav").ConvertToSoundObject("Whiytelly_1", SoundType.Voice);
            audEndCountdown = LoadSound("Whiytelly_EndCountdown.wav").ConvertToSoundObject("Whiytelly_CountdownEnd_0", SoundType.Voice).AddTimedSubKey("Whiytelly_CountdownEnd_1", 2.580f);
        }

        public override void Initialize()
        {
            base.Initialize();
            baseSpeed = 22;
            animator.Initialize(spriteRenderer[0]);
            animator.AddAnimation("Anim_WhiyHello", assetMan.GetAll<Sprite>().Take(11).ToArray(), [0, 1, 2, 3, 4, 5, 4, 5, 4, 5, 4, 3, 2, 1, 0, 6, 7, 8, 9, 10, 9, 10, 9, 10, 9, 10, 9, 8, 7, 6, 0], 0.15f);
            animator.AddAnimation("Anim_CountingStart", assetMan.GetAll<Sprite>().Skip(11).Take(2).ToArray(), [1], 0f);
            animator.AddAnimation("Anim_Counting", assetMan.GetAll<Sprite>().Skip(11).Take(2).ToArray(), [0], 0f);
            animator.AddAnimation("Anim_Angry", [placeholder], [0], 0.15f, true);
            animator.AddAnimation("Anim_WalkAngry", [placeholder, placeholder2, placeholder3], [0, 1, 0, 2], 0.15f, true);
            animator.AddAnimation("Anim_Knock", [placeholder4], [0], 0f, true);

            whiytellyIndicator = CustomBaldicator.CreateBaldicator();
            whiytellyIndicator.transform.localPosition = new(320f, -320f, 0f);
            whiytellyIndicator.ReflectionSetVariable("StartingPosition", new Vector2(0f, -125f));
            whiytellyIndicator.SetHearingAnimation(new CustomAnimation<Sprite>(Enumerable.Repeat(whiytellIndicatorSprites.Take(4), 5).SelectMany(x => x).Take(19).Append(whiytellIndicatorSprites[0]).ToArray(), 0.65f));
            whiytellyIndicator.AddAnimation("Happy", new CustomAnimation<Sprite>([whiytellIndicatorSprites[5]], 1));
            whiytellyIndicator.AddAnimation("Sad", new CustomAnimation<Sprite>([whiytellIndicatorSprites[6]], 1));
            whiytellyIndicator.AddAnimation("Distracted", new CustomAnimation<Sprite>([whiytellIndicatorSprites[7]], 1));
        }

        public new void Hear(GameObject source, Vector3 position, int value, bool appear)
        {
            var currentSoundVal = (int)AccessTools.Field(typeof(Baldi), "currentSoundVal").GetValue(this);

            if (appear)
            {
                if (value >= currentSoundVal) whiytellyIndicator.ActivateBaldicator("Happy");
                else if (value <= currentSoundVal) whiytellyIndicator.ActivateBaldicator("Sad");
            }

            UpdateSoundTarget();
            base.Hear(source, position, value, false);
        }

        public void FacultyDoorHit(StandardDoor door, Cell otherSide)
        {
            if (lastKnockedDoor != door) KnockOnDoor(door, otherSide);
            else door.OpenTimedWithKey(door.DefaultTime, false);
            
            lastKnockedDoor = door;
        }

        public void KnockOnDoor(StandardDoor door, Cell otherSide)
        {
            door.Knock();
            navigator.Entity.ExternalActivity.moveMods.Add(movMod);
            StopAllCoroutines();
            StartCoroutine(UnpauseAfterKnock(door, 4, otherSide));
        }

        public IEnumerator UnpauseAfterKnock(StandardDoor door, float time, Cell otherSide)
        {
            while (time > 0f)
            {
                time -= Time.deltaTime * TimeScale;
                yield return null;
            }
            navigationStateMachine.ChangeState(new NavigationState_TargetPosition(this, -1, otherSide.FloorWorldPosition));
            navigator.Entity.ExternalActivity.moveMods.Remove(movMod);

            if (Vector3.Distance(transform.position, door.CenteredPosition) <= 5f)
                door.OpenTimedWithKey(door.DefaultTime, false);
            
            yield break;
        }

        public override TeacherState GetHappyState() => new Whiytelly_Initial(this);
        public override TeacherState GetAngryState() => new Whiytelly_Walking(this);

        public static AssetManager assetMan = new();

        public AudioManager audMan;
        public KapoAnimator animator;
        public Sprite placeholder, placeholder2, placeholder3, placeholder4;
        public SoundObject audHello, audStartCountdown, audEndCountdown;
        public SoundObject[] audCountdown = new SoundObject[10];
        public Sprite[] whiytellIndicatorSprites;
        public CustomBaldicator whiytellyIndicator;
        public StandardDoor lastKnockedDoor;
        public MovementModifier movMod = new(Vector3.zero, 0f);
    }

    public class Whiytelly_StateBase : TeacherState
    {
        public Whiytelly whiy;

        public Whiytelly_StateBase(Whiytelly whiy) : base(whiy)
        {
            this.whiy = whiy;
            teacher = whiy;
            npc = whiy;
        }

        public override void DestinationEmpty()
        {
            base.DestinationEmpty();
            whiy.UpdateSoundTarget();
        }

        public override void Hear(GameObject source, Vector3 position, int value)
        {
            base.Hear(source, position, value);
            whiy.Hear(source, position, value, true);
        }
    }

    public class Whiytelly_Initial : Whiytelly_StateBase
    {
        public Whiytelly_Initial(Whiytelly whiy) : base(whiy) { }

        public override void Initialize()
        {
            base.Initialize();
            whiy.Navigator.Entity.SetFrozen(true);
            whiy.audMan.PlaySingle(whiy.audHello);
            whiy.animator.PlayAnimation("Anim_WhiyHello", true, false);
        }

        public override void Update()
        {
            base.Update();

            var player = Singleton<CoreGameManager>.Instance.GetPlayer(0);

            if (Vector3.Distance(player.transform.position, whiy.transform.position) >= 45)
            {
                whiy.behaviorStateMachine.ChangeState(new Whiytelly_Counting(whiy));
            }
        }

        public override void Hear(GameObject source, Vector3 position, int value)
        {
        }
    }


    public class Whiytelly_Counting : Whiytelly_StateBase
    {
        public Whiytelly_Counting(Whiytelly whiy) : base(whiy) { }

        public override void Initialize()
        {
            base.Initialize();
            whiy.audMan.FlushQueue(true);
            whiy.audMan.PlaySingle(whiy.audStartCountdown);
            whiy.animator.PlayAnimation("Anim_CountingStart", true, false);
            whiy.StartCoroutine(Countdown(10));
        }

        public IEnumerator Countdown(float totalDuration)
        {
            while (whiy.audMan.AnyAudioIsPlaying || Singleton<CoreGameManager>.Instance.Paused)
                yield return null;

            int count = 20;
            if (count == 0)
                yield break;

            float interval = totalDuration / count;

            foreach (SoundObject voiceline in whiy.audCountdown)
            {
                whiy.audMan.PlaySingle(voiceline);

                float elapsed = 0f;
                while (elapsed < interval)
                {
                    if (!Singleton<CoreGameManager>.Instance.Paused)
                        elapsed += Time.deltaTime;

                    yield return null;
                }
            }

            whiy.audMan.PlaySingle(whiy.audEndCountdown);

            while (whiy.audMan.AnyAudioIsPlaying || Singleton<CoreGameManager>.Instance.Paused)
                yield return null;

            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<BaseGameManager>.Instance.BeginSpoopMode();
            whiy.ec.SpawnNPCs();
            whiy.ec.StartEventTimers();

            if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
                whiy.behaviorStateMachine.ChangeState(whiy.GetAngryState());
        }

        public override void Hear(GameObject source, Vector3 position, int value)
        {
        }

    }


    public class Whiytelly_Walking : Whiytelly_StateBase
    {
        public Whiytelly_Walking(Whiytelly whiy) : base(whiy) { }

        public override void Initialize()
        {
            base.Initialize();
            whiy.Navigator.Entity.SetFrozen(false);
            whiy.animator.PlayAnimation("Anim_WalkAngry", true, true);
        }

        public override void Update()
        {
            base.Update();

            whiy.UpdateSlapDistance();
            delayTimer -= Time.deltaTime * this.npc.TimeScale;
            if (delayTimer <= 0f)
            {
                whiy.Slap();
                delayTimer = 0.05f;
            }

        }

        public override void DoorHit(StandardDoor door)
        {
            if (!door.IsOpen)
            {
                if (this.npc.ec.CellFromPosition(this.npc.transform.position) == door.aTile)
                {
                    if (door.bTile.room.category != RoomCategory.Class)
                    {
                        whiy.FacultyDoorHit(door, door.bTile);
                        return;
                    }
                }
                else if (door.aTile.room.category != RoomCategory.Class)
                {
                    whiy.FacultyDoorHit(door, door.aTile);
                    return;
                }
            }
            door.OpenTimedWithKey(door.DefaultTime, false);
        }


        public float delayTimer;
    }

    public class Whiytelly_OnKnock : Whiytelly_StateBase
    {
        public Whiytelly_OnKnock(Whiytelly whiy) : base(whiy) { }

        public override void Initialize()
        {
            base.Initialize();
            //whiy.audMan.PlaySingle(whiy.audHello);
            whiy.animator.PlayAnimation("Anim_WhiyHello", true, false);
        }
    }
}


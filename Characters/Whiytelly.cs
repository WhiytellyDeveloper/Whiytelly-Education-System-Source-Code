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
        public void PreInitialize(Whiytelly whiytelly)
        {
            AudioClip LoadSound(string name) => AssetLoader.AudioClipFromMod(Plugin._instance, ["Characters", "Whiytelly", "Audio", name]);
            Sprite[] LoadSpritesheet(string file, int cols, int rows) => AssetLoader.SpritesFromSpritesheet(cols, rows, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Characters", "Whiytelly", "Sprites", file]));

            helloAnim = LoadSpritesheet("Whiytelly_HelloSpriteSheet.png", 4, 3);
            countdownAnim = LoadSpritesheet("Whiytelly_CoutingSpriteSheet.png", 2, 1);
            clappingAnim = LoadSpritesheet("Whiytelly_PraiseSpriteSheet.png", 2, 1);
            whiytellIndicatorSprites = LoadSpritesheet("WhiytellyIndicator.png", 3, 3);

            var walkingSpritesheets = new[] { LoadSpritesheet("Whiytelly_WalkingSpriteSheet0.png", 4, 2), LoadSpritesheet("Whiytelly_WalkingSpriteSheet1.png", 4, 2), LoadSpritesheet("Whiytelly_WalkingSpriteSheet2.png", 4, 2) };
            var knockSprites = LoadSpritesheet("Whiytelly_KnockSpriteSheet.png", 4, 2);

            spriteRenderer[0].sprite = helloAnim[0];
            spriteRenderer[0].transform.localPosition = new Vector3(0f, -1.25f, 0f);

            var rotationMaps = walkingSpritesheets.Select(s => GenericExtensions.CreateRotationMap(8, s)).ToArray();
            var knockMap = GenericExtensions.CreateRotationMap(8, knockSprites);

            animator = gameObject.AddComponent<KapoAnimator>();
            animator.asr = spriteRenderer[0].CreateAnimatedSpriteRotator(rotationMaps);
            animator.asr.targetSprite = walkingSpritesheets[0][0];

            placeholder = walkingSpritesheets[0][0];
            placeholder2 = walkingSpritesheets[1][0];
            placeholder3 = walkingSpritesheets[2][0];

            AssetLoader.LocalizationFromFile(Path.Combine([AssetLoader.GetModPath(Plugin._instance), "Characters", "Whiytelly", "Language", "Whiy_English.json"]), Language.English);

            audMan = GetComponent<AudioManager>();

            audHello = LoadSound("Whiytelly_HellowF1.wav").ConvertToSoundObject("Whiytelly_HellowF1_0", SoundType.Voice).AddTimedSubKey("Whiytelly_HellowF1_1", 3.594f);
            audStartCountdown = LoadSound("Whiytelly_StartCountdown.wav").ConvertToSoundObject("Whiytelly_CountdownStart", SoundType.Voice);
            audEndCountdown = LoadSound("Whiytelly_EndCountdown.wav").ConvertToSoundObject("Whiytelly_CountdownEnd_0", SoundType.Voice).AddTimedSubKey("Whiytelly_CountdownEnd_1", 2.580f);

            for (int i = 0; i < 10; i++) audCountdown[i] = LoadSound($"Whiytelly_{10 - i}.wav").ConvertToSoundObject($"Whiytelly_{10 - i}", SoundType.Voice);
            for (int i = 0; i < 4; i++) footsteps[i] = LoadSound($"Whiytelly_Footstep{i}.wav").ConvertToSoundObject("Whiytelly_Steps", SoundType.Effect);

            loseSound = AssetLoader.AudioClipFromMod(Plugin._instance, ["Misc", "HellMensage.wav"]).ConvertToSoundObject("", SoundType.Effect);
            loseSound.subtitle = false;

            claps = LoadSound("Whiytelly_Aplause.wav").ConvertToSoundObject("Whiytelly_Applauding", SoundType.Effect);
        }


        public override void Initialize()
        {
            base.Initialize();
            baseSpeed = 13;
            animator.Initialize(spriteRenderer[0]);
            animator.AddAnimation("Anim_WhiyHello", helloAnim, [0, 1, 2, 3, 4, 5, 4, 5, 4, 5, 4, 3, 2, 1, 0, 6, 7, 8, 9, 10, 9, 10, 9, 10, 9, 10, 9, 8, 7, 6, 0], 0.15f);
            animator.AddAnimation("Anim_CountingStart", countdownAnim, [0], 0f);
            animator.AddAnimation("Anim_Counting", countdownAnim, [1], 0f);
            animator.AddAnimation("Anim_Angry", [placeholder], [0], 0.15f, true);
            animator.AddAnimation("Anim_WalkAngry", [placeholder, placeholder2, placeholder3], [0, 1, 0, 2], 0.15f, true);
            animator.AddAnimation("Anim_Clapping", clappingAnim, [0, 1], 0.05f);

            whiytellyIndicator = CustomBaldicator.CreateBaldicator();
            whiytellyIndicator.transform.localPosition = new(320f, -320f, 0f);
            whiytellyIndicator.ReflectionSetVariable("StartingPosition", new Vector2(0f, -130f));
            whiytellyIndicator.SetHearingAnimation(new CustomAnimation<Sprite>(Enumerable.Repeat(whiytellIndicatorSprites.Take(4), 5).SelectMany(x => x).Take(19).Append(whiytellIndicatorSprites[0]).ToArray(), 0.55f));
            whiytellyIndicator.AddAnimation("Happy", new CustomAnimation<Sprite>([whiytellIndicatorSprites[5]], 1));
            whiytellyIndicator.AddAnimation("Sad", new CustomAnimation<Sprite>([whiytellIndicatorSprites[6]], 1));
            whiytellyIndicator.AddAnimation("Neutral", new CustomAnimation<Sprite>([whiytellIndicatorSprites[8]], 1));
            whiytellyIndicator.AddAnimation("Distracted", new CustomAnimation<Sprite>([whiytellIndicatorSprites[7]], 1));

            caughtOffset = -Vector3.one * 7.5f;
            navigator.passableObstacles.Clear();
            AddLoseSound(loseSound, 100);
        }

        public override WeightedTeacherNotebook GetTeacherNotebookWeight()
        {
            WeightedTeacherNotebook notebook = new WeightedTeacherNotebook(this);
            notebook.Sprite(AssetLoader.SpritesFromSpritesheet(3, 4, 40, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin._instance, ["Pickups", "WhiyNotebooks.png"])));
            notebook.Weight(100);
            return notebook;
        }

        public new void Hear(GameObject source, Vector3 position, int value, bool appear)
        {
            var currentSoundVal = (int)AccessTools.Field(typeof(Baldi), "currentSoundVal").GetValue(this);

            if (appear)
            {
                if (value > currentSoundVal) whiytellyIndicator.ActivateBaldicator("Happy");
                else if (value < currentSoundVal) whiytellyIndicator.ActivateBaldicator("Sad");
                else if (value == currentSoundVal) whiytellyIndicator.ActivateBaldicator("Neutral");
            }

            UpdateSoundTarget();
            base.Hear(source, position, value, false);
        }

        public override void Slap()
        {
            base.Slap();

            if (!audMan.AnyAudioIsPlaying)
            audMan.PlayRandomAudio(footsteps);
            
        }

        public override void CaughtPlayer(PlayerManager player)
        {
            navigator.SetSpeed(0);

            if (Singleton<BaseGameManager>.Instance != null) Singleton<BaseGameManager>.Instance.EndGame(player.transform, this);
            else Singleton<CoreGameManager>.Instance.EndGame(player.transform, this);
        }

        public bool FacingPlayer(PlayerManager player) => (double)Vector3.Angle(transform.forward, player.transform.position - transform.position) <= 22.5;

        public override TeacherState GetHappyState() => new Whiytelly_Initial(this);
        public override TeacherState GetAngryState() => new Whiytelly_Walking(this);

        public static AssetManager assetMan = new();

        public AudioManager audMan;
        public KapoAnimator animator;
        public Sprite placeholder, placeholder2, placeholder3, placeholder4;
        public Sprite[] helloAnim, countdownAnim, clappingAnim;
        public SoundObject audHello, audStartCountdown, audEndCountdown;
        public SoundObject[] audCountdown = new SoundObject[10];
        public SoundObject[] footsteps = new SoundObject[4];
        public SoundObject loseSound, claps;
        public Sprite[] whiytellIndicatorSprites;
        public CustomBaldicator whiytellyIndicator;
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

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);

            if (whiy.FacingPlayer(player))
                whiy.Hear(player.gameObject, player.transform.position, 127, false);
        }
    }

    public class Whiytelly_SubState : Whiytelly_StateBase
    {
        public Whiytelly_StateBase state;

        public Whiytelly_SubState(Whiytelly whiy, Whiytelly_StateBase state) : base(whiy)
        {
            this.state = state;
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
            whiy.animator.PlayAnimation("Anim_Counting", true, false);
            whiy.StartCoroutine(Countdown(10));
        }

        public IEnumerator Countdown(float totalDuration)
        {
            while (whiy.audMan.AnyAudioIsPlaying || Singleton<CoreGameManager>.Instance.Paused)
                yield return null;

            whiy.animator.PlayAnimation("Anim_CountingStart", true, false);

            float interval = 1.5f;

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

            whiy.animator.PlayAnimation("Anim_Counting", true, false);
            whiy.audMan.PlaySingle(whiy.audEndCountdown);

            while (whiy.audMan.AnyAudioIsPlaying || Singleton<CoreGameManager>.Instance.Paused)
                yield return null;

            whiy.behaviorStateMachine.ChangeState(whiy.GetAngryState());
            whiy.ActivateSpoopMode();
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
            whiy.UpdateSoundTarget();
        }

        public override void Update()
        {
            base.Update();

            whiy.UpdateSlapDistance();
            delayTimer -= Time.deltaTime * npc.TimeScale;
            if (delayTimer <= 0f)
            {
                whiy.Slap();
                delayTimer = 0.1f;
            }
        }

        public override void OnStateTriggerStay(Collider other)
        {
            base.OnStateTriggerStay(other);


            if (whiy.IsTouchingPlayer(other))
            {
                PlayerManager component = other.GetComponent<PlayerManager>();

                if (whiy.FacingPlayer(component))
                    whiy.CaughtPlayer(component);
            }
        }

        public override void GoodMathMachineAnswer()
        {
            base.GoodMathMachineAnswer();
            whiy.behaviorStateMachine.ChangeState(new Whiytelly_Clapping(whiy, whiy.GetAngryState() as Whiytelly_StateBase));
        }

        public float delayTimer;
    }

    public class Whiytelly_Clapping : Whiytelly_SubState
    {
        public float time = 5;

        public  Whiytelly_Clapping(Whiytelly whiytelly, Whiytelly_StateBase stateBase) : base(whiytelly, stateBase)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            whiy.animator.PlayAnimation("Anim_Clapping", true, true);
            whiy.audMan.PlaySingle(whiy.claps);
        }

        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * whiy.TimeScale;

            if (time <= 0f)
            {
                whiy.animator.StopAllCoroutines();
                whiy.behaviorStateMachine.ChangeState(state);
            }
            
        }
    }
}


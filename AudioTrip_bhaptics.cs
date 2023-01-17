using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using UnityEngine;

[assembly: MelonInfo(typeof(AudioTrip_bhaptics.AudioTrip_bhaptics), "AudioTrip_bhaptics", "1.1.2", "Florian Fahrenberger")]
[assembly: MelonGame("Kinemotik Studios", "Audio Trip")]


namespace AudioTrip_bhaptics
{
    public class AudioTrip_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(PlayerHand), "HitNote", new Type[] { typeof(AudioTrip.PhysicsBody), typeof(bool) })]
        public class bhaptics_NoteHit
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerHand __instance, AudioTrip.PhysicsBody ev)
            {
                bool isRightHand = (ev.ChoreoEventInstance.eventData.handedness == AudioTrip.ChoreoEvent.Handedness.Right);
                bool isFoot = false;
                string hitPattern;
                if (ev.ChoreoEventInstance is DrumInstance) hitPattern = "Smash";
                switch (ev.ChoreoEventInstance.eventData.type)
                {
                    case AudioTrip.ChoreoEvent.EventType.Gem_Left:
                    case AudioTrip.ChoreoEvent.EventType.Gem_Right:
                        hitPattern = "Touch";
                        break;
                    case AudioTrip.ChoreoEvent.EventType.DirGem_Left:
                    case AudioTrip.ChoreoEvent.EventType.DirGem_Right:
                        hitPattern = "Strike";
                        break;
                    case AudioTrip.ChoreoEvent.EventType.Drum_Left:
                    case AudioTrip.ChoreoEvent.EventType.Drum_Right:
                        hitPattern = "Smash";
                        break;
                    case AudioTrip.ChoreoEvent.EventType.FootGem_Left:
                    case AudioTrip.ChoreoEvent.EventType.FootGem_Right:
                        hitPattern = "FootTouch";
                        isFoot = true;
                        break;
                    case AudioTrip.ChoreoEvent.EventType.FootDirGem_Left:
                    case AudioTrip.ChoreoEvent.EventType.FootDirGem_Right:
                        hitPattern = "FootStrike";
                        isFoot = true;
                        break;
                    case AudioTrip.ChoreoEvent.EventType.FootDrum_Left:
                    case AudioTrip.ChoreoEvent.EventType.FootDrum_Right:
                        hitPattern = "FootSmash";
                        isFoot = true;
                        break;
                    default:
                        hitPattern = "Touch";
                        break;
                }
                tactsuitVr.HitNote(hitPattern, isRightHand, isFoot);
            }
        }

        [HarmonyPatch(typeof(PlayerHand), "OnBarrierHit", new Type[] { typeof(AudioTrip.Collision) })]
        public class bhaptics_BarrierHit
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("HitByWall");
                tactsuitVr.PlaybackHaptics("HitByWallHead");
            }
        }

        [HarmonyPatch(typeof(PlayerHand), "OnMissedGem", new Type[] { typeof(AudioTrip.PhysicsBody) })]
        public class bhaptics_MissedGem
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("MissedNote");
            }
        }

        
        [HarmonyPatch(typeof(PlayerHand), "ScoreRibbon", new Type[] { typeof(bool) })]
        public class bhaptics_ScoreRibbon
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerHand __instance, RibbonInstance ___lastRibbon)
            {
                bool isRightHand = ( ___lastRibbon.eventData.type == AudioTrip.ChoreoEvent.EventType.Ribbon_Right );
                if (isRightHand) tactsuitVr.PlaybackHaptics("RibbonBuzz_R");
                else tactsuitVr.PlaybackHaptics("RibbonBuzz_L");
            }
        }
        
        [HarmonyPatch(typeof(TrackSparksController), "Play", new Type[] { typeof(int) })]
        public class bhaptics_PlaySparks
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("BellyBoom");
            }
        }

    }
}

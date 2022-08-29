using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using UnityEngine;

namespace AudioTrip_bhaptics
{
    public class AudioTrip_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(PlayerHand), "HitNote", new Type[] { typeof(AudioTrip.ChoreoEventInstance), typeof(bool) })]
        public class bhaptics_NoteHit
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerHand __instance, AudioTrip.ChoreoEventInstance ev)
            {
                bool isRightHand = (ev.eventData.handedness == AudioTrip.ChoreoEvent.Handedness.Right);
                string hitPattern;
                if (ev is DrumInstance) hitPattern = "Smash";
                switch (ev.eventData.type)
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
                    default:
                        hitPattern = "Touch";
                        break;
                }
                tactsuitVr.HitNote(hitPattern, isRightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerHand), "OnBarrierHit", new Type[] { typeof(GameObject) })]
        public class bhaptics_BarrierHit
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("HitByWall");
                tactsuitVr.PlaybackHaptics("HitByWallHead");
            }
        }

        [HarmonyPatch(typeof(PlayerHand), "OnMissedGem", new Type[] { typeof(GameObject) })]
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
            public static void Postfix(PlayerHand __instance)
            {
                bool isRightHand = ( (__instance.name.Contains("right")) | (__instance.name.Contains("Right")) );
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

using System;
using MelonLoader;
using Bhaptics.SDK2;


namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        /* A class that contains the basic functions for the bhaptics Tactsuit, like:
         * - A Heartbeat function that can be turned on/off
         * - A function to read in and register all .tact patterns in the bHaptics subfolder
         * - A logging hook to output to the Melonloader log
         * - 
         * */
        public bool suitDisabled = true;
        public bool systemInitialized = false;

        public TactsuitVR()
        {
            LOG("Initializing suit");
            // Default configuration exported in the portal, in case the PC is not online
            var config = System.Text.Encoding.UTF8.GetString(AudioTrip_bhaptics.Resource1.config);
            // Initialize with appID, apiKey, and default value in case it is unreachable
            var res = BhapticsSDK2.Initialize("h9209ftduZgWtMDPNX3t", "lzwIKi5SvVJoEihFMw9h", config);
            // if it worked, enable the suit
            suitDisabled = res != 0;
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }



        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f)
        {
            if (suitDisabled) return;
            BhapticsSDK2.Play(key.ToLower(), intensity, duration, 0f, 0f);
        }

        public void HitNote(string patternName, bool isRightHand, bool isFoot, float intensity = 1.0f)
        {
            // weaponName is a parameter that will go into the vest feedback pattern name
            // isRightHand is just which side the feedback is on
            // intensity should usually be between 0 and 1

            // make postfix according to parameter
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }

            // stitch together pattern names for Arm and Hand recoil
            string keyArm = patternName + "Arm" + postfix;
            string keyFoot = patternName + postfix;
            // vest pattern name contains the weapon name. This way, you can quickly switch
            // between swords, pistols, shotguns, ... by just changing the shoulder feedback
            // and scaling via the intensity for arms and hands
            string keyVest = patternName + "Vest" + postfix;
            if (isFoot) PlaybackHaptics(keyFoot, intensity);
            else PlaybackHaptics(keyArm, intensity);
            PlaybackHaptics(keyVest, intensity);
        }


    }
}

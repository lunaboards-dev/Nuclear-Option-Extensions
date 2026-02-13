using System;
using System.Linq;
using HarmonyLib;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Synthesis;
using NOX;
using NOX.Manager;

[HarmonyPatch(typeof(WindowsTTS), nameof(WindowsTTS.Speak))]
class LinuxTTS
{
    static SpeechSynthesizer synth = new();
    static bool has_setup = false;
    public static void SetupTTS()
    {
        if (has_setup) return;
        //Plugin.Logger.LogInfo(synth.GetInstalledVoices());
        //synth.SelectVoice("Microsoft Sam");

        synth.SetOutputToDefaultAudioDevice();
        has_setup = true;
    }
    public static bool Prefix(WindowsTTS __instance, int speed, int volume, string text, bool asSsml)
    {
        if (WineDetect.OnWine)
        {
            SetupTTS();
            // actually do our thing
            synth.Rate = speed;
            synth.Volume = volume;
            if (asSsml)
            {
                var p = new Prompt("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">" + text + "</speak>", SynthesisTextFormat.Ssml);
                synth.Speak(p);
            } else
            {
                synth.Speak(text);
            }
            return true;
        }
        return false;
    }
}
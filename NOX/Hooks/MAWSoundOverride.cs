// this is the worst thing ever
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NOX.RWRs;
using UnityEngine;

namespace NOX.Hooks;

[HarmonyPatch]
public class MAWSoundOverride
{
    static Type ThreatListClass = typeof(ThreatList);
    static Type MissileAlarmClass = AccessTools.Inner(ThreatListClass, "MissileAlarm");
    static FieldInfo SeekerField = AccessTools.Field(MissileAlarmClass, "seekerType");
    static FieldInfo AudioSourceField = AccessTools.Field(MissileAlarmClass, "alarmSource");
    static FieldInfo AudioClipField = AccessTools.Field(MissileAlarmClass, "alertClip");
    static FieldInfo MissileListField = AccessTools.Field(MissileAlarmClass, "missiles");
    static MethodBase TargetMethod()
    {
        return AccessTools.Method(MissileAlarmClass, "ManageAlarmSound");
    }

    static bool Prefix(object __instance)
    {
        var seker_type = (string) SeekerField.GetValue(__instance);
        var alarm_source = (AudioSource) AudioSourceField.GetValue(__instance);
        var alarm_clip = (AudioClip) AudioClipField.GetValue(__instance);
        var missiles = (List<Missile>) MissileListField.GetValue(__instance);
        
        if (missiles.Count > 0 && alarm_source != null)
        {
            int num = 1;
            foreach (Missile missile in missiles)
            {
                num = Mathf.Min(num, (int)(missile.seekerMode - 1));
            }
            var clip = Resources.Crazy[num];
            if (alarm_clip != clip)
            {
                AudioClipField.SetValue(__instance, clip);
                alarm_source.clip = clip;
                alarm_source.time = 0;
                alarm_source.Play();
            }
        }
        return true;
    }
}
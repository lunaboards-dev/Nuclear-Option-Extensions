using NuclearOption.Networking.Lobbies;
using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace NOX.UI.Lobby;

class Hooks
{
    [HarmonyPatch(typeof(LobbyList), "Start")]
    class MoveLobbyUI
    {
        static float pos_x = 952.5f;
        static void Prefix(LobbyList __instance)
        {
            /*var go = GameObject.FindObjectsOfType<GameObject>().Where<GameObject>(o => o.name == "Lobby Panel").First<GameObject>();
            Plugin.Logger.LogInfo($"Found object: {go}");
            var rt = go.GetComponent<RectTransform>();
            var pos = rt.position;
            pos.x = pos_x;
            rt.position = pos;*/
            var rt = __instance.gameObject.GetComponent<RectTransform>();
            //rt.position = new Vector3(0, 100, 0);
            rt.pivot = new Vector2(0, 0.5f);
            rt.anchoredPosition = new Vector2(0, 0);
            //rt.anchorMax = new Vector2(0, 1);
            //rt.anchorMin = new Vector2(0, 0);
        }
    }
}
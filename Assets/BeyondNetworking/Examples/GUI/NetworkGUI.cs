using Beyond.Networking;
using UnityEngine;
using Network = Beyond.Networking.Network;

public class NetworkGUI : MonoBehaviour
{
    Rect GUIWindowRect = new(0, 0, 200, 200);
    void OnGUI() {
        if (!enabled)
            return;
        GUIWindowRect = GUILayout.Window(0, GUIWindowRect, GUIWindow, "NetworkGUI");
    }
    string address;
    bool showData;
    bool showSpawnMenu;
    string selectedSpawnable;
    private void GUIWindow(int id) {
        GUILayout.Box("Server");
        showData = GUILayout.Toggle(showData, "Show Data");
        if (showData) {
            GUILayout.Box("DATA");
            if (GUILayout.Button("Update Server Data to Clients")) {
                Beyond.Networking.Network.UpdateServerData();
            }
            GUILayout.Box(JsonUtility.ToJson(Beyond.Networking.Network.CurrentServer, true));
        }
        showSpawnMenu = GUILayout.Toggle(showSpawnMenu, "Show Spawn Menu");
        if (showSpawnMenu) {
            GUILayout.Box("SPAWN MENU");
            GUILayout.BeginVertical();
            foreach (var spawnable in Network.Prefabs.Keys) {
                if(GUILayout.Button(spawnable)) selectedSpawnable = spawnable;
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("SPAWN")) {
                Network.Instantiate(selectedSpawnable);
            }
        }
        if (GUILayout.Button("Create Server")) {
            var properties = new string[3];
            properties[0] = "Balls";
            properties[1] = "Squares";
            properties[2] = "CUSTOM";
            Beyond.Networking.Network.CreateServer("Server", properties);
        }
        
        GUILayout.Box("Client");
        Beyond.Networking.Network.NickName = GUILayout.TextField(Beyond.Networking.Network.NickName);
        Beyond.Networking.Network.UserId = GUILayout.TextField(Beyond.Networking.Network.UserId);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Join Server")) {
            Beyond.Networking.Network.JoinServer(address);
        }
        address = GUILayout.TextField(address);
        GUILayout.EndHorizontal();
    }
}

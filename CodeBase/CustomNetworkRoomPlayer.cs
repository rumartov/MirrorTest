using System;
using Mirror;
using UnityEngine;

namespace CodeBase.Scripts
{
    [AddComponentMenu("")]
    public class CustomNetworkRoomPlayer : NetworkRoomPlayer
    {
        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string PlayerName;

        public event Action<string> OnPlayerNameChange;
     
        private string userInput = "";

        public override void OnGUI()
        {
            if (!showRoomGUI)
                return;
            
            DrawPlayerReadyState();
            DrawPlayerReadyButton();
        }

        private void DrawPlayerReadyState()
        {
            GUILayout.BeginArea(new Rect(20f + (index * 100), 200f, 90f, 130f));

            GUILayout.Label($"{PlayerName} [{index + 1}]");

            GUILayout.Label(readyToBegin ? "Ready" : "Not Ready");

            if ((isServer && index > 0 || isServerOnly) && GUILayout.Button("REMOVE"))
            {
                GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            }

            if (isLocalPlayer)
            {
                userInput = GUILayout.TextField(userInput, 25);

                if (GUILayout.Button("SET NAME"))
                {
                    CmdSetPlayerName(userInput);
                }
            }

            GUILayout.EndArea();
        }

        private void DrawPlayerReadyButton()
        {
            if (NetworkClient.active && isLocalPlayer)
            {
                GUILayout.BeginArea(new Rect(20f, 300f, 120f, 20f));

                if (readyToBegin)
                {
                    if (GUILayout.Button("Cancel"))
                        CmdChangeReadyState(false);
                }
                else
                {
                    if (GUILayout.Button("Ready"))
                        CmdChangeReadyState(true);
                }

                GUILayout.EndArea();
            }
        }

        [Command]
        private void CmdSetPlayerName(string newName)
        {
            PlayerName = newName;
        }

        [ClientRpc]
        private void OnPlayerNameChanged(string oldName, string newName)
        {
            OnPlayerNameChange?.Invoke(newName);
        }
    }
}

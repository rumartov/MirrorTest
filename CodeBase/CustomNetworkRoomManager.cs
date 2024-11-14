using Mirror;
using UnityEngine;
using Zenject;

namespace CodeBase.Scripts
{
    [AddComponentMenu("")]
    public class CustomNetworkRoomManager : NetworkRoomManager
    {
        [Header("Spawner Setup")]
        public GameObject medkitPrefab;

        private Factory _factory;
        private StaticData _staticData;
        private bool showStartButton;

        [Inject]
        public void Construct(Factory factory, StaticData staticData)
        {
            _factory = factory;
            _staticData = staticData;
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName == GameplayScene)
            {
                _factory.InitialSpawn();
            }
        }

        public override void OnRoomServerPlayersReady()
        {
            if (Utils.IsHeadless())
            {
                base.OnRoomServerPlayersReady();
            }
            else
            {
                showStartButton = true;
            }
        }

        public override void SceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            if (Utils.IsSceneActive(RoomScene))
            {
                PendingPlayer pending;
                pending.conn = conn;
                pending.roomPlayer = roomPlayer;
                pendingPlayers.Add(pending);
                return;
            }

            GameObject gamePlayer = OnRoomServerCreateGamePlayer(conn, roomPlayer);
            if (gamePlayer == null)
            {
                Transform startPos = GetStartPosition();
                gamePlayer = _factory.SpawnGamePlayer(startPos, playerPrefab, roomPlayer);
            }

            if (!OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer))
                return;

            NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, ReplacePlayerOptions.KeepAuthority);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            return true;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                showStartButton = false;
                ServerChangeScene(GameplayScene);
            }
        }
    }
}

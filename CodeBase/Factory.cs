using Mirror;
using UnityEngine;
using Zenject;

namespace CodeBase.Scripts
{
    public class Factory
    {
        private readonly CustomNetworkRoomManager _networkRoomManager;
        private readonly StaticData _staticData;
        private readonly DiContainer _container;

        public Factory(CustomNetworkRoomManager networkRoomManager, StaticData staticData, DiContainer container)
        {
            _networkRoomManager = networkRoomManager;
            _staticData = staticData;
            _container = container;
        }

        [ServerCallback]
        internal void InitialSpawn()
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnMedkit();
            }
        }

        public GameObject SpawnGamePlayer(Transform startPos, GameObject playerPrefab, GameObject roomPlayer)
        {
            Vector3 spawnPosition = startPos != null ? startPos.position : Vector3.zero;
            Quaternion spawnRotation = startPos != null ? startPos.rotation : Quaternion.identity;

            GameObject gamePlayer = Object.Instantiate(playerPrefab, spawnPosition, spawnRotation);

            PlayerHealth playerHealth = gamePlayer.GetComponent<PlayerHealth>();
            playerHealth.Construct(_staticData.DefaultGamePlayerHelath);
    
            CustomNetworkRoomPlayer customNetworkRoomPlayer = roomPlayer.GetComponentInChildren<CustomNetworkRoomPlayer>();
            string playerName = customNetworkRoomPlayer.PlayerName;
            PlayerUI playerUI = gamePlayer.GetComponentInChildren<PlayerUI>();
            playerUI.Construct(playerName, customNetworkRoomPlayer); 

            return gamePlayer;
        }


        [ServerCallback]
        public void SpawnMedkit()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-19, 20), 1, Random.Range(-19, 20));

            Object medkitPrefab = _networkRoomManager.medkitPrefab;
            GameObject medkitInstance = _container.InstantiatePrefab(medkitPrefab, spawnPosition, Quaternion.identity, null);

            Medkit medkitComponent = medkitInstance.GetComponent<Medkit>();
            medkitComponent.Construct(this);

            NetworkServer.Spawn(medkitInstance);
        }
    }
}

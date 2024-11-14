using Mirror;
using UnityEngine;
using Zenject;

namespace CodeBase.Scripts
{
    public class Medkit : MonoBehaviour
    {
        private bool available;
        private Factory _factory;
        public int healAmount = 10;
        public void Construct(Factory factory)
        {
            _factory = factory;
            available = true;
        }
        
        [ServerCallback]
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                ClaimMedkit(other.gameObject);
        }

        [ServerCallback]
        public void ClaimMedkit(GameObject player)
        {
            if (available)
            {
                available = false;

                player.GetComponent<PlayerHealth>().Heal(healAmount);
                
                _factory.SpawnMedkit();
               
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
using CodeBase.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CodeBase
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _customNetworkRoomManagerPrefab;
        public override void InstallBindings()
        {
            Debug.Log("Start bind");
            
            Container.Bind<StaticData>().FromNew().AsSingle().NonLazy();
            Container.Bind<CustomNetworkRoomManager>()
                .FromComponentInNewPrefab(_customNetworkRoomManagerPrefab)
                .AsSingle()
                .NonLazy();
            
            Container.Bind<Factory>().FromNew().AsSingle().NonLazy();
            
            SceneManager.LoadScene("Art/Scenes/MirrorRoomOffline");
        }
    }
}
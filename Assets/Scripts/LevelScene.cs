using AxieMixer.Unity;
using Data;
using Gui;
using UnityEngine;

public class LevelScene : MonoBehaviour
{
    [SerializeField] private GameData GameData;
    private HomeScreenExtension _homeScreenExtension;

    private void Start()
    {
        Mixer.Init();
        _homeScreenExtension = new HomeScreenExtension();
        _homeScreenExtension.Init(GameData);
    }

    private void Update()
    {
    }
}
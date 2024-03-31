using AxieMixer.Unity;
using Data;
using Gui;
using UnityEngine;
using Utility;

public class LevelScene : MonoBehaviour
{
    [SerializeField] private GameResource gameResource;
    private IHomeScreenExtension _homeScreenEx;

    private void Start()
    {
        Mixer.Init();
        _homeScreenEx = new HomeScreenExtension();
        _homeScreenEx.Init(gameResource);
        var mapData = MapDataUtil.GenExMap();
        _homeScreenEx.LoadMap(mapData);
        var gameStage = new GameStage();
        gameStage.Init(mapData, _homeScreenEx, gameResource);
        // Set Action
        _homeScreenEx.HomeScreen.BtPreview.setOnClick(() =>
        {
            gameStage.DoPreviewStep(); //
        });
        _homeScreenEx.HomeScreen.BtNext.setOnClick(() =>
        {
            gameStage.DoNextStep(); //
        });
    }

    private void Update()
    {
    }
}
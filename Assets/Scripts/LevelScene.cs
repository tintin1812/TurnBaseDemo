using AxieMixer.Unity;
using Data;
using FairyGUI;
using Gui;
using UnityEngine;
using Utility;

public class LevelScene : MonoBehaviour
{
    [SerializeField] private GameResource resource;
    private IHomeScreenExtension _homeScreenEx;

    private void Start()
    {
        Mixer.Init();
        _homeScreenEx = new HomeScreenExtension();
        _homeScreenEx.Init(resource);
        Timers.inst.CallLater(LoadGame);
    }

    private void LoadGame(object param)
    {
        var mapData = MapDataUtil.GenExMap();
        _homeScreenEx.LoadMap(mapData);

        var gameStage = new GameStage();
        gameStage.Init(mapData, _homeScreenEx, resource);

        GTweenCallback refreshGui = () =>
        {
            _homeScreenEx.HomeScreen.BtPreview.enabled = gameStage.CanRevertAble;
            _homeScreenEx.HomeScreen.BtNext.enabled = true;
        };
        refreshGui();
        _homeScreenEx.HomeScreen.BtPreview.setOnClick(() =>
        {
            DisableBt();
            gameStage.DoPreviewStep(refreshGui);
        });
        _homeScreenEx.HomeScreen.BtNext.setOnClick(() =>
        {
            DisableBt();
            gameStage.DoNextStep(refreshGui); //
        });
        return;

        void DisableBt()
        {
            _homeScreenEx.HomeScreen.BtPreview.enabled = false;
            _homeScreenEx.HomeScreen.BtNext.enabled = false;
        }
    }
}
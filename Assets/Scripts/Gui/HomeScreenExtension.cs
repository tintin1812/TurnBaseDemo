using Data;
using FairyGUI;
using Plugins.PathFinding;
using UnityEngine;

namespace Gui
{
    public class HomeScreenExtension
    {
        private const string PathPackAge = "Gui/Gui";
        private HomeScreen _homeScreen;

        public void Init(GameData gameData)
        {
            UIPackage.AddPackage(PathPackAge);
            GuiBinder.BindAll();
            // Map Sound
            UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("Gui", "sfx_pop");
            _homeScreen = HomeScreen.CreateInstance();
            _homeScreen.MakeFullScreen();
            GRoot.inst.AddChild(_homeScreen);


            InitZoom();

            var tileGrid = new TileGrid(15, 15);

            var start = tileGrid.GetTile(9, 2);
            var end = tileGrid.GetTile(7, 14);
            var pathRoutine = tileGrid.FindPath(start, end, PathFinder.FindPath_AStar);
            Debug.Log($"pathRoutine Count {pathRoutine.Count}");

            var listSlot = _homeScreen.Map.Content.ListSlot;
            listSlot.SetVirtual();
            listSlot.itemRenderer = (idx, item) =>
            {
                var slot = (SlotAxie)item;
                // slot.ReloadData(idx % 2 == 0 ? gameData.MatchData.Defender : gameData.MatchData.Attacker);
                // slot.setOnClick(() =>
                // {
                //     Debug.Log($"OnClick at {idx}"); //
                // });
            };
            listSlot.columnCount = tileGrid.Cols;
            listSlot.numItems = tileGrid.Tiles.Length;
        }

        private void InitZoom()
        {
            // _homeScreen.Map.Content.draggable = true;
            _homeScreen.SliderZoom.value = 50;
            SetZoom(_homeScreen.Map, (float)_homeScreen.SliderZoom.value); //
            _homeScreen.SliderZoom.onChanged.Add(() =>
            {
                SetZoom(_homeScreen.Map, (float)_homeScreen.SliderZoom.value); //   
            });

            var gesture = new PinchGesture(_homeScreen.Map.Content);
            gesture.onAction.Add(() =>
            {
                var vTo = (float)_homeScreen.SliderZoom.value + gesture.delta;
                _homeScreen.SliderZoom.value = Mathf.Clamp(vTo, 0.0f, 1.0f);
                SetZoom(_homeScreen.Map, (float)_homeScreen.SliderZoom.value); //
            });
        }

        private static void SetZoom(Map map, float percent)
        {
            const float min = 0.7f;
            const float max = 2.0f;
            var scaleTo = min + percent * (max - min) * 0.01f;
            map.scale = new Vector2(scaleTo, scaleTo);
        }
    }
}
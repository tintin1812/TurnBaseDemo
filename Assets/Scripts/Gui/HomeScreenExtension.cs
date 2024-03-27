using Data;
using FairyGUI;
using UnityEngine;
using Utility;

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
            var listSlot = _homeScreen.ListSlot;
            for (var idx = 0; idx < listSlot.numChildren; idx++)
            {
                var slot = (SlotAxie)listSlot.GetChildAt(idx);
                slot.ReloadData(idx % 2 == 0 ? gameData.MatchData.Defender : gameData.MatchData.Attacker);
                var idxC = idx;
                slot.setOnClick(() =>
                {
                    Debug.Log($"OnClick at {idxC}"); //
                });
            }
        }
    }
}
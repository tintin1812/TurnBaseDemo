using AxieMixer.Unity;
using FairyGUI;
using Gui;
using Spine.Unity;
using UnityEngine;

public class LevelScene : MonoBehaviour
{
    private const string PathPackAge = "Gui/Gui";

    private void Start()
    {
        UIPackage.AddPackage(PathPackAge);
        GuiBinder.BindAll();
        // Map Sound
        UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("Gui", "sfx_pop");
        var homeScreen = HomeScreen.CreateInstance();
        homeScreen.MakeFullScreen();
        GRoot.inst.AddChild(homeScreen);
        //
        Mixer.Init();

        // homeScreen.Holder
        var go = CreatePetAni("2724598");
        GoWrapper wrapper = new GoWrapper(go);
        homeScreen.Holder.SetNativeObject(wrapper);
    }

    private void Update()
    {
    }

    private static string GenesStr = "0x2000000000000300008100e08308000000010010088081040001000010a043020000009008004106000100100860c40200010000084081060001001410a04406";

    /// <summary>
    /// Follow Link: https://www.fairygui.com/docs/unity/insert3d
    /// </summary>
    /// <param name="providerId">Is Axie id</param>
    /// <returns></returns>
    private static GameObject CreatePetAni(string providerId)
    {
        const float scale = 0.2f;
        var go = new GameObject($"pet_{providerId}");
        // var meta = new Dictionary<string, string>();
        // var builderResult = Mixer.Builder.BuildSpineFromGene(providerId, GenesStr, meta, scale, false);
        // SkeletonAnimation runtimeSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(builderResult.skeletonDataAsset);
        var runtimeSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(null);
        Mixer.SpawnSkeletonAnimation(runtimeSkeletonAnimation, providerId, GenesStr, scale);
        runtimeSkeletonAnimation.gameObject.layer = LayerMask.NameToLayer("Player");
        runtimeSkeletonAnimation.transform.SetParent(go.transform, false);
        runtimeSkeletonAnimation.GetComponent<MeshRenderer>();
        runtimeSkeletonAnimation.gameObject.AddComponent<AutoBlendAnimController>();
        runtimeSkeletonAnimation.state.SetAnimation(0, "action/idle/normal", true);
        runtimeSkeletonAnimation.state.TimeScale = 0.5f;
        return go;
    }
}
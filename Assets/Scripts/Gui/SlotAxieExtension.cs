using AxieMixer.Unity;
using Data;
using FairyGUI;
using Spine.Unity;
using UnityEngine;

namespace Gui
{
    public static class SlotAxieExtension
    {
        public static void ReloadData(this GGraph image, AxieData axieData)
        {
            const float scale = 0.08f;
            var go = new GameObject($"pet_{axieData.AxieId}");
            var runtimeSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(null);
            Mixer.SpawnSkeletonAnimation(runtimeSkeletonAnimation, axieData.AxieId, axieData.Genes, scale);
            runtimeSkeletonAnimation.gameObject.layer = LayerMask.NameToLayer("Player");
            runtimeSkeletonAnimation.transform.SetParent(go.transform, false);
            runtimeSkeletonAnimation.GetComponent<MeshRenderer>();
            runtimeSkeletonAnimation.gameObject.AddComponent<AutoBlendAnimController>();
            runtimeSkeletonAnimation.state.SetAnimation(0, "action/idle/normal", true);
            runtimeSkeletonAnimation.state.TimeScale = 0.5f;
            GoWrapper wrapper = new GoWrapper(go);
            image.SetNativeObject(wrapper);
        }
    }

    public class AxieAni
    {
        public static AxieAni Create(GComponent parent, Vector2 pos, AxieData axieData)
        {
            var com = AxieCom.CreateInstance();
            com.Image.ReloadData(axieData);
            parent.AddChild(com);
            com.xy = pos;
            return new AxieAni()
            {
                AxieCom = com, //
            };
        }

        public AxieCom AxieCom { get; set; }
    }
}
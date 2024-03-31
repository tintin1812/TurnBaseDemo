using AxieMixer.Unity;
using Data;
using FairyGUI;
using Spine.Unity;
using UnityEngine;

namespace Gui
{
    public static class SlotAxieExtension
    {
        public static SkeletonAnimation ReloadData(this GGraph image, AxieResource axieResource)
        {
            const float scale = 0.08f;
            var go = new GameObject($"pet_{axieResource.AxieId}");
            var animation = SkeletonAnimation.NewSkeletonAnimationGameObject(null);
            Mixer.SpawnSkeletonAnimation(animation, axieResource.AxieId, axieResource.Genes, scale);
            animation.gameObject.layer = LayerMask.NameToLayer("Player");
            animation.transform.SetParent(go.transform, false);
            animation.GetComponent<MeshRenderer>();
            animation.gameObject.AddComponent<AutoBlendAnimController>();
            animation.state.SetAnimation(0, "action/idle/normal", true);
            animation.state.TimeScale = 0.5f;
            GoWrapper wrapper = new GoWrapper(go);
            image.SetNativeObject(wrapper);
            return animation;
        }
    }

    public class AxieAni
    {
        public static AxieAni Create(GComponent parent, Vector2 pos, AxieResource axieResource)
        {
            var com = AxieCom.CreateInstance();
            var ani = com.Image.ReloadData(axieResource);
            parent.AddChild(com);
            com.xy = pos;
            return new AxieAni()
            {
                AxieCom = com, //
                Ani = ani, //
            };
        }

        public AxieCom AxieCom { get; private set; }
        private SkeletonAnimation Ani { get; set; }

        public void FaceTo(bool isFaceToRight)
        {
            Ani.skeleton.ScaleX = isFaceToRight ? -1.0f : 1.0f;
        }
    }
}
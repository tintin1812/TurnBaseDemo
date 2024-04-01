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
        public enum AxieTeam
        {
            Non,
            Attack,
            Defend
        }

        public enum AxieStatus
        {
            Non,
            MarkFind,
            MarkMove,
            MarkAttack,
            MarkIdle,
        }

        public static AxieAni Create(GComponent parent, Vector2 pos, AxieResource axieResource)
        {
            var com = AxieCom.CreateInstance();
            var ani = com.Image.ReloadData(axieResource);
            parent.AddChild(com);
            com.xy = pos;
            return new AxieAni()
            {
                Ani = ani, //
                AxieCom = com, //
            };
        }

        public void Init(AxieTeam team, int hp)
        {
            Status = AxieStatus.Non;
            Team = team;
            HpMax = hp;
            Hp = hp;
            UpdateHpBar();
        }

        private void UpdateHpBar()
        {
            AxieCom.BarHp.value = 100.0f * Hp / HpMax;
            AxieCom.BarHp.TitleSub.text = $"{Hp}/{HpMax}";
        }

        private SkeletonAnimation Ani { get; set; }
        public AxieCom AxieCom { get; private set; }
        public AxieTeam Team { get; set; }
        public int HpMax { get; set; }
        public int Hp { get; set; }
        public bool IsAlive => Hp > 0;
        public AxieStatus Status { get; set; }
        public Vector2Int TilePos { get; set; }

        public void FaceTo(bool isFaceToRight)
        {
            Ani.skeleton.ScaleX = isFaceToRight ? -1.0f : 1.0f;
        }

        public void BeAttack(int hpLost)
        {
            Hp -= hpLost;
            if (Hp < 0) Hp = 0;
            UpdateHpBar();
        }
    }
}
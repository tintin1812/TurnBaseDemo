using System;
using AxieMixer.Unity;
using Data;
using FairyGUI;
using Spine.Unity;
using UnityEngine;
using Utility;

namespace Gui
{
    public static class SlotAxieExtension
    {
        public static SkeletonAnimation ReloadData(this GGraph image, AxieResource axieResource)
        {
            const float scale = 0.12f;
            var go = new GameObject($"pet_{axieResource.AxieId}");
            var animation = SkeletonAnimation.NewSkeletonAnimationGameObject(null);
            Mixer.SpawnSkeletonAnimation(animation, axieResource.AxieId, axieResource.Genes, scale);
            animation.gameObject.layer = LayerMask.NameToLayer("Player");
            animation.transform.SetParent(go.transform, false);
            animation.GetComponent<MeshRenderer>();
            animation.gameObject.AddComponent<AutoBlendAnimController>();
            GoWrapper wrapper = new GoWrapper(go);
            image.SetNativeObject(wrapper);
            return animation;
        }
    }

    public class AxieHolder
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

        private const string AniNameIdle = "action/idle/normal";
        private const string AniNameMove = "action/move-forward";
        private const string AniNameAttack = "attack/melee/normal-attack";
        private const string AniNameHit = "defense/hit-by-normal";

        public static AxieHolder Create(GComponent parent, Vector2 pos, AxieResource axieResource)
        {
            var com = AxieCom.CreateInstance();
            var ani = com.Image.ReloadData(axieResource);
            parent.AddChild(com);
            com.xy = pos;
            return new AxieHolder()
            {
                Ani = ani, //
                AxieCom = com, //
            };
        }

        private SkeletonAnimation Ani { get; set; }
        public AxieCom AxieCom { get; private set; }
        public AxieTeam Team { get; set; }
        public int HpMax { get; set; }
        public int Hp { get; set; }
        public bool IsAlive => Hp > 0;
        public AxieStatus Status { get; set; }
        public Vector2Int TilePos { get; set; }
        public int AttackerNumber { get; private set; }

        public void Init(AxieTeam team, int hp)
        {
            Status = AxieStatus.Non;
            Team = team;
            HpMax = hp;
            Hp = hp;
            AttackerNumber = Ran.RandomRanger(0, 2);
            UpdateHpBar();
            ReloadAni(AniNameMove);
            ReloadAni(AniNameAttack);
            ReloadAni(AniNameHit);
            Ani.state.ClearTracks();
            Ani.state.SetAnimation(0, AniNameIdle, true);
            AxieCom.setOnClick(() =>
            {
                switch (Team)
                {
                    case AxieTeam.Attack:
                        Util.ShowNotiText($"Attacker Number is {AttackerNumber}");
                        break;
                    case AxieTeam.Defend:
                        Util.ShowNotiText($"Target Number is {AttackerNumber}");
                        break;
                    case AxieTeam.Non:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void UpdateHpBar()
        {
            if (Hp == HpMax)
            {
                AxieCom.BarHp.visible = false;
                return;
            }

            AxieCom.BarHp.visible = true;
            AxieCom.BarHp.value = 100.0f * Hp / HpMax;
            AxieCom.BarHp.TitleSub.text = $"{Hp}/{HpMax}";
        }

        public void FaceToRight(bool isFaceToRight)
        {
            Ani.skeleton.ScaleX = isFaceToRight ? -1.0f : 1.0f;
        }

        public void FaceToPos(int tileX)
        {
            if (tileX == TilePos.x) return;
            Ani.skeleton.ScaleX = tileX > TilePos.x ? -1.0f : 1.0f;
        }

        public void BeAttack(int hpLost)
        {
            Hp -= hpLost;
            if (Hp < 0) Hp = 0;
            UpdateHpBar();
            // Effect Damage
            var textDamage = TextDamage.CreateInstance();
            AxieCom.parent.AddChild(textDamage);
            textDamage.xy = AxieCom.xy + AxieCom.size * 0.5f;
            textDamage.y -= 30;
            textDamage.alpha = 1;
            textDamage.Label.text = $"-{hpLost}";
            textDamage.TweenMoveY(textDamage.y - 14, 0.5f).OnComplete(() => { textDamage.Dispose(); });
        }

        public void DoRevertHp(int hp)
        {
            Hp = hp;
            UpdateHpBar();
        }

        public float DoAniMove()
        {
            return DoAni(AniNameMove);
        }

        public float DoAniAttack()
        {
            return DoAni(AniNameAttack);
        }

        public float DoAniDie()
        {
            return DoAni(AniNameHit);
        }

        public void HideIndicator()
        {
            AxieCom.IndicatorMove.visible = false;
            AxieCom.IconAttack.visible = false;
        }

        public void ShowIndicatorMove(int x, int y)
        {
            AxieCom.IndicatorMove.visible = true;
            AxieCom.IndicatorMove.rotation = Util.VToAngle(x, y);
        }

        public void ShowIndicatorAttack(int x, int y)
        {
            AxieCom.IconAttack.visible = true;
            // AxieCom.IconAttack.Center();
            // AxieCom.IconAttack.xy += new Vector2(x, y) * 10;
        }

        private float DoAni(string animationName)
        {
            var trackEntry = Ani.state.SetAnimation(1, animationName, false);
            var duration = trackEntry.Animation.Duration;
            return duration;
        }

        private void ReloadAni(string animationName)
        {
            Ani.state.AddAnimation(1, animationName, false, 0.0f);
            Ani.state.Update(0);
        }
    }
}
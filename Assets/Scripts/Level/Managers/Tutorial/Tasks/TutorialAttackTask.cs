using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Player;

namespace Level.Tutorial
{
    public class TutorialAttackTask : TutorialButtonPressTask
    {
        Coroutine attackCooldown;

        new void Start()
        {
            base.Start();
            attackCooldown = null;
            ButtonPressed += HandleAttack;
        }

        new void Update()
        {
            if (attackCooldown != null) return;
            base.Update();
        }

        public override bool CheckTaskCompletion()
        {
            return !EventSystem.current.IsPointerOverGameObject() && base.CheckTaskCompletion();
        }

        void HandleAttack(int count)
        {
            if (attackCooldown != null) StopCoroutine(attackCooldown);
            attackCooldown = StartCoroutine(CountAttackCooldown());
        }

        IEnumerator CountAttackCooldown()
        {
            yield return new WaitForSeconds(PlayerController.Instance.Data.netAttackDuration);
            attackCooldown = null;
        }
    }
}

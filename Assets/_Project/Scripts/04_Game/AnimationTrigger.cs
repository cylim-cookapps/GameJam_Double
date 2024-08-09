using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class AnimationTrigger : MonoBehaviour
    {
        [SerializeField, GetComponentInParent]
        private HeroUnit _heroUnit;

        public void AttackEvent()
        {
            _heroUnit.TryAttack();
        }
    }
}

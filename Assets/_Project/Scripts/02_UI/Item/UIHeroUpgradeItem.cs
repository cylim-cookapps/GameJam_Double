using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class UIHeroUpgradeItem : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textPrice;

        [SerializeField]
        private List<GameObject> _goTier;

        [SerializeField]
        private List<GameObject> _goTierBottom;

        private InGameHeroData _heroData;

        public void Init(InGameHeroData heroData)
        {

        }

        #region Event

        private void OnClickUpgrade()
        {

        }


        #endregion

    }
}

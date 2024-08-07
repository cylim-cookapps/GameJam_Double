using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class OutGameUI : UIBoard
    {
        [SerializeField, GetComponentInChildrenOnly]
        private OutGameUI_Top _top;

        public override void OnInitialize()
        {
            _top.OnInitialize();
        }

        #region Event


        #endregion
    }
}

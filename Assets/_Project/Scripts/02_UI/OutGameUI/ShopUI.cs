using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class ShopUI : BoardBase<OutGameUI>
    {
        public override OutGameUI Parent { get; protected set; }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

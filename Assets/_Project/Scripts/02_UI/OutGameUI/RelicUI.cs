using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class Relic : BoardBase<MainUI>
    {
        public override MainUI Parent { get; protected set; }

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

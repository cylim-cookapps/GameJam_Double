using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class Popup_Wait : Popup<Popup_Wait>
    {
        public override void Initialize()
        {
            base.Initialize();
            EventManager.Inst.EventMatch.AddListener(OnEventMatch);
        }

        public override void Hide()
        {
            base.Hide();
            LobbyManager.Inst.CancelMatch();
        }

        #region EventHandler

        private void OnEventMatch()
        {
            base.Hide();
        }

        #endregion
    }
}

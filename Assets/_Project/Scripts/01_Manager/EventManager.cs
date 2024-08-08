using System.Collections;
using System.Collections.Generic;
using Sigtrap.Relays;
using UnityEngine;

namespace Pxp
{
    public class EventManager : Singleton<EventManager>
    {
        /// <summary>
        /// 동료 장착 업데이트
        /// </summary>
        public void OnEventEquippedHero() => EventEquippedHero.Dispatch();

        public readonly Relay EventEquippedHero = new();

        /// <summary>
        /// 동료 잠금 해제
        /// </summary>
        public void OnEventUnlockHero() => EventUnlockHero.Dispatch();

        public readonly Relay EventUnlockHero = new();

        public void OnEventToast(string value) => EventToast.Dispatch(value);
        public readonly Relay<string> EventToast = new();

    }
}

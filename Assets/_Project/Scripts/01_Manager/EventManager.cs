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


        public void OnEventWave(int wave) => EventWave.Dispatch(wave);
        public readonly Relay<int> EventWave = new();

        public void OnEventGameTimer(int sec) => EventGameTimer.Dispatch(sec);
        public readonly Relay<int> EventGameTimer = new();

        public void OnEventMonsterCount(int count) => EventMonsterCount.Dispatch(count);
        public readonly Relay<int> EventMonsterCount = new();

        public void OnEventGameCoin(int coin) => EventGameCoin.Dispatch(coin);
        public readonly Relay<int> EventGameCoin = new();

        public void OnEventGameChip(int chip) => EventGameChip.Dispatch(chip);
        public readonly Relay<int> EventGameChip = new();


    }
}

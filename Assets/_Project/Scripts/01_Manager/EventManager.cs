using System.Collections;
using System.Collections.Generic;
using Sigtrap.Relays;
using UnityEngine;

namespace Pxp
{
    public class EventManager : Singleton<EventManager>
    {
        /// <summary>
        /// 영웅 장착 업데이트
        /// </summary>
        public void OnEventEquippedHero() => EventEquippedHero.Dispatch();

        public readonly Relay EventEquippedHero = new();

        /// <summary>
        /// 영웅 잠금 해제
        /// </summary>
        public void OnEventUnlockHero() => EventUnlockHero.Dispatch();

        public readonly Relay EventUnlockHero = new();

        /// <summary>
        /// 영웅 레벨업
        /// </summary>
        public void OnEventHeroLevelUp(int heroId) => EventHeroLevelUp.Dispatch(heroId);
        public readonly Relay<int> EventHeroLevelUp = new();

        public void OnEventToast(string value) => EventToast.Dispatch(value);
        public readonly Relay<string> EventToast = new();

        public void OnEventNickname(string value) => EventNickname.Dispatch(value);
        public readonly Relay<string> EventNickname = new();





        public void OnEventMatch() => EventMatch.Dispatch();
        public readonly Relay EventMatch = new();

        public void OnEventGameState(Enum_GameState state) => EventGameState.Dispatch(state);
        public readonly Relay<Enum_GameState> EventGameState = new();

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

        public void OnEventGameHeroUpgrade(int actor, int heroId) => EventGameHeroUpgrade.Dispatch(actor, heroId);
        public readonly Relay<int, int> EventGameHeroUpgrade = new();
    }
}

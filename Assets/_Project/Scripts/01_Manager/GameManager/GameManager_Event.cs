using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    public partial class GameManager
    {
        private const byte PLAYER_LOADED_LEVEL = 0;
        private const byte PLAYER_DATA_EVENT = 1;
        private const byte EVENT_START = 2;
        private const byte EVENT_END = 3;
        private const byte HERO_LEVEL_UP = 100;

        private void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == PLAYER_LOADED_LEVEL)
            {
                SendPlayerData();
            }
            else if (photonEvent.Code == PLAYER_DATA_EVENT)
            {
                Hashtable playerDataHash = (Hashtable) photonEvent.CustomData;
                InGameUserData userData = InGameUserData.FromHashtable(playerDataHash);
                playerDataDict[userData.Index] = userData;

                if (playerDataDict.Count == LobbyManager.Inst.MaxPlayersPerRoom)
                {
                    if (CurrGameState == Enum_GameState.Start)
                        return;

                    if (LobbyManager.Inst.IsSingleMode)
                    {
                        InGameUserData ai = new InGameUserData();
                        ai.InitAI();
                        playerDataDict[ai.Index] = ai;
                        StartCoroutine(GameAI());
                    }

                    if (PhotonNetwork.IsMasterClient)
                    {
                        StartCoroutine("GameLoop");
                    }

                    CurrGameState = Enum_GameState.Start;
                    EventManager.Inst.OnEventGameState(Enum_GameState.Start);
                }
            }
            else if (photonEvent.Code == EVENT_START)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine("GameLoop");
                }
            }
            else if (photonEvent.Code == EVENT_END)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                CurrGameState = Enum_GameState.End;
                GameEnd();
            }
            else if (photonEvent.Code == HERO_LEVEL_UP)
            {
                Hashtable playerDataHash = (Hashtable) photonEvent.CustomData;
                HeroLevelData userData = HeroLevelData.FromHashtable(playerDataHash);
                playerDataDict[userData.Owner].Coin = userData.Coin;
                playerDataDict[userData.Owner].Heroes.Find(x => x.HeroId == userData.HeroId).InGameLevel = userData.Upgrade;
                EventManager.Inst.OnEventGameHeroUpgrade(userData.Owner, userData.HeroId);
            }
        }

        private void SendPlayerData()
        {
            List<InGameHeroData> myHeroes = new List<InGameHeroData>();

            for (int i = 0; i < 5; i++)
            {
                var heroData = UserManager.Inst.Hero.GetEquippedHero(i);
                myHeroes.Add(new InGameHeroData(heroData.Id, heroData.Level, heroData.Star, 0));
            }

            List<InGameUnitData> myUnits = new List<InGameUnitData>();
            for (int i = 0; i < 15; i++)
            {
                myUnits.Add(new InGameUnitData(0, 0));
            }

            var startCoin = (int) SpecDataManager.Inst.Option.Get("StartCoin").value;

            InGameUserData myData = new InGameUserData(
                PhotonNetwork.LocalPlayer.ActorNumber,
                UserManager.Inst.PlayerId,
                UserManager.Inst.NickName,
                UserManager.Inst.Info.Level,
                startCoin,
                0,
                0,
                0,
                myHeroes, myUnits
            );

            PhotonNetwork.RaiseEvent(PLAYER_DATA_EVENT, myData.ToHashtable(), new RaiseEventOptions {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }

        private void SendGameLoop()
        {
        }

        private void SendGameEnd()
        {
            PhotonNetwork.RaiseEvent(EVENT_END, null, new RaiseEventOptions {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }

        public void SendHeroLevelUp(int heroId, int upgrade, int coin)
        {
            HeroLevelData data = new HeroLevelData(PhotonNetwork.LocalPlayer.ActorNumber, heroId, upgrade, coin);

            PhotonNetwork.RaiseEvent(HERO_LEVEL_UP, data.ToHashtable(), new RaiseEventOptions {Receivers = ReceiverGroup.Others}, SendOptions.SendReliable);
        }
    }

    public enum Enum_GameState
    {
        Ready,
        Start,
        End
    }
}

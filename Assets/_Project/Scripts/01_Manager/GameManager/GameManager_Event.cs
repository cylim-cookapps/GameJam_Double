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
                    if (PhotonNetwork.IsMasterClient && _isGameStarted == false)
                    {
                        Debug.Log("GameLoop");
                        StartCoroutine(GameLoop());
                    }

                    _isGameStarted = true;
                }
            }
            else if (photonEvent.Code == EVENT_START)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log(PhotonNetwork.IsMasterClient);
                    StartCoroutine(GameLoop());
                }
            }
            else if (photonEvent.Code == EVENT_END)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                    _isGameStarted = false;
                }

                GameEnd();
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
                UserManager.Inst.NickName,
                UserManager.Inst.PlayerId,
                UserManager.Inst.Info.Level,
                startCoin,
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
    }
}

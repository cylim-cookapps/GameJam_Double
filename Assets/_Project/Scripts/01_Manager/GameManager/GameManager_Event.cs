using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Pxp
{
    public partial class GameManager
    {
        private const byte PLAYER_LOADED_LEVEL = 0;
        private const byte PLAYER_DATA_EVENT = 1;
        private const byte START_WAVE = 2;

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
                playerDataDict[userData.Name] = userData;
                Debug.Log(userData);
                if (PhotonNetwork.IsMasterClient && playerDataDict.Count == 2)
                {
                    SendGameLoop();
                }
            }
            else if (photonEvent.Code == START_WAVE)
            {
                if (PhotonNetwork.IsMasterClient)
                    StartCoroutine(GameLoop());
            }
        }

        private void SendPlayerData()
        {
            List<InGameHeroData> myHeroes = new List<InGameHeroData>();

            for (int i = 0; i < 5; i++)
            {
                var heroData = UserManager.Inst.Hero.GetEquippedHero(i);
                myHeroes.Add(new InGameHeroData(heroData.Id, heroData.Level, heroData.Star));
            }

            InGameUserData myData = new InGameUserData(
                PhotonNetwork.LocalPlayer.ActorNumber,
                UserManager.Inst.NickName,
                UserManager.Inst.PlayerId,
                UserManager.Inst.Info.Level,
                myHeroes
            );

            playerDataDict[myData.Name] = myData;
            PhotonNetwork.RaiseEvent(PLAYER_DATA_EVENT, myData.ToHashtable(), new RaiseEventOptions {Receivers = ReceiverGroup.Others}, SendOptions.SendReliable);
            Debug.Log(nameof(SendPlayerData));
            _isGameStarted = true;
        }

        private void SendGameLoop()
        {
            _isGameStarted = true;
            PhotonNetwork.RaiseEvent(START_WAVE, null, new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient}, SendOptions.SendReliable);
        }
    }
}

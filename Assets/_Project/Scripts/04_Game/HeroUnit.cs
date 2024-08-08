using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    public class HeroUnit : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        private int _grade = 0;

        public int Grade
        {
            get => Grade;
            set
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetGradeRpc), RpcTarget.All, value);
            }
        }

        [PunRPC]
        private void SetGradeRpc(int grade)
        {
            _grade = grade;
        }

        [SerializeField]
        private GameObject _projectilePrefab;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            var heroId = (int) instantiationData[0];
            var index =  (int) instantiationData[1];

            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }
        }
    }
}

using System;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Pxp;
using Pxp.Data;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if __DEV || UNITY_EDITOR
/// <summary>
/// 게임 치트
/// </summary>
public partial class SROptions
{
    [Category("[치트] 영웅"), DisplayName("영웅 전체 언락")]
    public void 영웅_전체_언락()
    {
        UserManager.Inst.Hero?.Editor_AllUnlockHero();
        UserManager.Inst.Save(true).Forget();
    }

    [Category("[치트] 계정"), DisplayName("계정 삭제")]
    public void 계정_삭제()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignOut(true);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }

    [Category("[치트] 아웃게임"), DisplayName("골드 추가")]
    public void 골드_추가()
    {
        if (UserManager.Inst.Currency != null)
            UserManager.Inst.Currency.AddCurrency(Enum_ItemType.Gold, 10000);
    }

    [Category("[치트] 아웃게임"), DisplayName("젬 추가")]
    public void 젬_추가()
    {
        if (UserManager.Inst.Currency != null)
            UserManager.Inst.Currency.AddCurrency(Enum_ItemType.Gem, 10000);
    }

    [Category("[치트] 아웃게임"), DisplayName("상자 추가")]
    public void 상자_추가()
    {
        if (UserManager.Inst.Currency != null)
            UserManager.Inst.Currency.AddCurrency(Enum_ItemType.Ticket_NormalChest, 100);
    }

    [Category("[치트] 배경음"), DisplayName("배경음 On")]
    public void 배경음_On()
    {
        AudioController.SetCategoryVolume("BGM", 1f);
    }

    [Category("[치트] 배경음"), DisplayName("배경음 Off")]
    public void 배경음_Off()
    {
        AudioController.SetCategoryVolume("BGM", 0f);
    }

    [Category("[치트] 인게임"), DisplayName("칩 추가")]
    public void 칩_추가()
    {
        if (GameManager.IsCreated)
        {
            GameManager.Inst.photonView.RPC("AddChip", RpcTarget.All, 10);
        }
    }

    [Category("[치트] 인게임"), DisplayName("다음 웨이브")]
    public void 다음_웨이브()
    {
        if (GameManager.IsCreated)
        {
            GameManager.Inst.Editor_NextWave();
        }
    }
}
#endif

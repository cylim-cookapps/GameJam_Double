using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

namespace Pxp
{
    public class GameUI : MonoSingleton<GameUI>
    {
        [SerializeField, GetComponentInChildrenOnly]
        private List<UIProfileInGameItem> _uiProfileInGameItem;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textWave, _textMonster, _textTimer, _textCoin, _textChip, _textSummonPrice;

        [SerializeField, GetComponentInChildrenName]
        private Slider _sliderMonster;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnSummon;

        [SerializeField, GetComponentInChildrenOnly]
        private List<UIHeroUpgradeItem> _uiHeroUpgradeItems;

        [SerializeField]
        private GameObject hpBarPrefab;

        [SerializeField]
        private Transform hpBarParent;

        [SerializeField]
        private GameObject damageTextPrefab;

        private ObjectPool<HPBar> hpBarPool;
        private ObjectPool<DamageText> damageTextPool;
        private int _index = 0;

        private int Summon_Default = 50;
        private int Summon_Increase = 10;

        private void Awake()
        {
            Summon_Default = (int) SpecDataManager.Inst.Option.Get("Summon_Default")!.value;
            Summon_Increase = (int) SpecDataManager.Inst.Option.Get("Summon_Increase")!.value;

            OnEventGameTimer(0);
            OnEventGameCoin((int) SpecDataManager.Inst.Option.Get("StartCoin").value);
            OnEventGameChip(0);
            _textSummonPrice.SetText(Summon_Default);
            _sliderMonster.maxValue = 100;
            _btnSummon.AddListener(OnClickSummon);

            EventManager.Inst.EventGameState.AddListener(OnEventGameState);
            EventManager.Inst.EventGameTimer.AddListener(OnEventGameTimer);
            EventManager.Inst.EventWave.AddListener(OnEventWave);
            EventManager.Inst.EventMonsterCount.AddListener(OnEventMonsterCount);
            EventManager.Inst.EventGameCoin.AddListener(OnEventGameCoin);
            EventManager.Inst.EventGameChip.AddListener(OnEventGameChip);

            InitializePool();
        }

        private void Start()
        {
            AudioController.PlayMusic("BGM_Game");

            for (int i = 0; i < UserManager.Inst.Hero.EquipHeroes.Count; i++)
            {
                _uiHeroUpgradeItems[i].Init(UserManager.Inst.Hero.EquipHeroes[i]);
            }
        }

        private void OnDestroy()
        {
            EventManager.Inst.EventGameState.RemoveListener(OnEventGameState);
            EventManager.Inst.EventGameTimer.RemoveListener(OnEventGameTimer);
            EventManager.Inst.EventWave.RemoveListener(OnEventWave);
            EventManager.Inst.EventMonsterCount.RemoveListener(OnEventMonsterCount);
            EventManager.Inst.EventGameCoin.RemoveListener(OnEventGameCoin);
            EventManager.Inst.EventGameChip.RemoveListener(OnEventGameChip);

            hpBarPool.Dispose();
            damageTextPool.Dispose();
        }

        #region Pool

        private void InitializePool()
        {
            hpBarPool = new ObjectPool<HPBar>(
                createFunc: CreateHPBar,
                actionOnGet: OnGetHPBar,
                actionOnRelease: OnReleaseHPBar,
                actionOnDestroy: OnDestroyHPBar,
                defaultCapacity: 10,
                maxSize: 200
            );

            damageTextPool = new ObjectPool<DamageText>(
                createFunc: CreateDamageText,
                actionOnGet: OnGetDamageText,
                actionOnRelease: OnReleaseDamageText,
                actionOnDestroy: OnDestroyDamageText,
                defaultCapacity: 20,
                maxSize: 100
            );
        }

        private HPBar CreateHPBar()
        {
            GameObject hpBarObj = Instantiate(hpBarPrefab, hpBarParent);
            HPBar hpBar = hpBarObj.GetComponent<HPBar>();
            _index++;
            hpBar.name = $"HPBar_{_index}";
            return hpBar;
        }

        private void OnGetHPBar(HPBar hpBar)
        {
            hpBar.gameObject.SetActive(true);
        }

        private void OnReleaseHPBar(HPBar hpBar)
        {
            hpBar.ReturnToPool();
        }

        private void OnDestroyHPBar(HPBar hpBar)
        {
            Destroy(hpBar.gameObject);
        }

        private DamageText CreateDamageText()
        {
            GameObject damageTextObj = Instantiate(damageTextPrefab, hpBarParent);
            return damageTextObj.GetComponent<DamageText>();
        }

        private void OnGetDamageText(DamageText damageText)
        {
            damageText.gameObject.SetActive(true);
        }

        private void OnReleaseDamageText(DamageText damageText)
        {
            damageText.gameObject.SetActive(false);
        }

        private void OnDestroyDamageText(DamageText damageText)
        {
            Destroy(damageText.gameObject);
        }

        public void ShowDamageText(int damage, bool isCri, Transform target)
        {
            DamageText damageText = damageTextPool.Get();
            damageText.Initialize(damage, isCri, target);
        }

        public void ReturnDamageText(DamageText damageText)
        {
            damageTextPool.Release(damageText);
        }

        #endregion

        public HPBar GetHPBar()
        {
            return hpBarPool.Get();
        }

        public void ReturnHPBar(HPBar hpBar)
        {
            hpBarPool.Release(hpBar);
        }

        #region Event

        private void OnClickSummon()
        {
            GameManager.Inst.SpawnHero();
        }

        #endregion

        #region EventHandler

        private void OnEventGameState(Enum_GameState state)
        {
            if (state == Enum_GameState.Start)
            {
                _uiProfileInGameItem[0].SetProfile(GameManager.Inst.MyInGameUserData.Name, GameManager.Inst.MyInGameUserData.Level);
                _uiProfileInGameItem[1].SetProfile(GameManager.Inst.OtherInGameUserData.Name, GameManager.Inst.OtherInGameUserData.Level);
            }
        }

        private void OnEventGameTimer(int sec)
        {
            int minutes = Mathf.FloorToInt(sec / 60f);
            int seconds = Mathf.FloorToInt(sec % 60f);
            _textTimer.SetTextFormat("{0:00}:{1:00}", minutes, seconds);
        }

        private void OnEventWave(int wave)
        {
            _textWave.SetTextFormat("WAVE {0}", wave + 1);
        }

        private void OnEventMonsterCount(int count)
        {
            _sliderMonster.value = count;
            _textMonster.SetTextFormat("{0}/{1}", count, _sliderMonster.maxValue);
        }

        private void OnEventGameChip(int chip)
        {
            _textChip.SetText(chip);
        }

        private void OnEventGameCoin(int coin)
        {
            if (GameManager.Inst.CurrGameState == Enum_GameState.Start)
                _textSummonPrice.SetText(Summon_Default + Summon_Increase * GameManager.Inst.MyInGameUserData.Summon);
            _textCoin.SetText(coin);
        }

        #endregion
    }
}

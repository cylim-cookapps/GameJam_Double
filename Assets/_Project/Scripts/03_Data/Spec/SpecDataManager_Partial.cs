using System;
using System.Collections;
using System.Collections.Generic;
using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pxp.Data
{
    public partial class SpecDataManager : Singleton<SpecDataManager>
    {
        /// <summary>
        /// 스펙 로드
        /// </summary>
        internal async UniTask LoadSpecData()
        {
            Load(SpecDataResourceLoader.LoadSpecData());
        }

        public List<ItemInfo> GetRandomItemInfoList(int count)
        {
            List<ItemInfo> itemInfoList = new();
            List<Gacha> ga = new();
            foreach (var gacha in Gacha.All)
            {
                if (gacha.gachaType == Enum_GachaType.Gacha_Normal)
                {
                    ga.Add(gacha);
                }
            }

            WeightedList<Gacha> list = new();
            foreach (var gacha in ga)
            {
                list.Add(new WeightedListItem<Gacha>(gacha, gacha.weight));
            }

            for (int i = 0; i < count; i++)
            {
                Gacha gacha = list.RandomElement();
                itemInfoList.Add(new ItemInfo(gacha.itemID, gacha.value));
            }

            return itemInfoList;
        }
    }
}

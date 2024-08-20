using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp.Data
{
    public partial class Shop
    {
        private List<ItemInfo> _list;

        public List<ItemInfo> GetProductList()
        {
            if (_list != null)
                return _list;

            _list = new List<ItemInfo>();
            if (item_1 != Enum_ItemType.None) _list.Add(new ItemInfo(item_1, itemAmount_1));
            if (item_2 != Enum_ItemType.None) _list.Add(new ItemInfo(item_2, itemAmount_2));
            if (item_3 != Enum_ItemType.None) _list.Add(new ItemInfo(item_3, itemAmount_3));
            if (item_4 != Enum_ItemType.None) _list.Add(new ItemInfo(item_4, itemAmount_4));
            if (item_5 != Enum_ItemType.None) _list.Add(new ItemInfo(item_5, itemAmount_5));
            return _list;
        }
    }

    public partial class Hero
    {
        public string TierName
        {
            get
            {
                switch (tier)
                {
                    default:
                    case Enum_TierType.Normal:
                        return "노멀";
                    case Enum_TierType.Rare:
                        return "레어";
                    case Enum_TierType.Epic:
                        return "에픽";
                    case Enum_TierType.Legend:
                        return "레전드";
                }
            }
        }

    }
}

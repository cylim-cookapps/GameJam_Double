using System.Collections;
using System.Collections.Generic;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    public struct ItemInfo
    {
        public Enum_ItemType ItemType { get; private set; }
        public int Count { get; private set; }

        public ItemInfo(Enum_ItemType type, int count)
        {
            ItemType = type;
            Count = count;
        }


    }
}

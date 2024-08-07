using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}

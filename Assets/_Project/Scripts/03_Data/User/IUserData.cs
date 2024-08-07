using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Pxp
{
    public abstract class UserData
    {
        /// <summary>
        /// 카테고리
        /// </summary>
        public abstract Enum_UserData Category { get; }

        /// <summary>
        /// 데이터 체크
        /// </summary>
        public abstract void CheckAndCreate();

        /// <summary>
        /// 다음날 체크
        /// </summary>
        public virtual void NextDay(bool isNextWeek)
        {
        }
    }

    public interface IData<out TSpec>
    {
        TSpec Spec { get; }
    }
}

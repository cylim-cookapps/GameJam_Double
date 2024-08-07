using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public interface IUserData
    {
        /// <summary>
        /// 카테고리
        /// </summary>
        Enum_UserData Category { get; }

        /// <summary>
        /// 데이터 체크
        /// </summary>
        void CheckAndCreate();

        /// <summary>
        /// 다음날 체크
        /// </summary>
        void NextDay(bool isNextWeek);
    }

    public interface IData<out TSpec>
    {
        TSpec Spec { get; }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CookApps.BadWordFilter;
using Cysharp.Threading.Tasks;
using Pxp;
using UnityEngine;

namespace Pxp
{
    public class LanguageManager : Singleton<LanguageManager>
    {
        private bool _isInit = false;

        public static readonly int MinWordAmount = 2;

        public static readonly int MaxWordAmount = 15;

        /// <summary>
        /// 추가 정규식 패턴
        /// </summary>
        public static readonly string AdditionalPatternsRegex = "운영자|고객센터|고객문의|고객지원|고객상담|일대일문의|문의담당|" +
                                                                "문의지원|운영팀|상담원|상담자|문의센터|helpservice|헬프서비스|마스터|" +
                                                                "고객만족|admin|담당자|관리자|지엠|지앰|GM|쥐앰|쥐엠|해킹|핵|" +
                                                                "사이버수사대|해커|hacking|hack|라운지|운지|네이버|네이버라운지|GOD";

        /// <summary>
        /// 클라사이드 -닉네임 무결성 체크
        /// !!!중복 체크는 서버에서 에러 코드 받아서 처리 해야함!!!
        /// </summary>
        /// <param name="nickname">확인할 닉네임</param>
        /// <returns>eNickNameValid 닉네임 무결성 체크 결과</returns>
        public Enum_NickNameValid IsValidNickName(string nickname)
        {
            if (_isInit == false)
            {
                CookAppsBadWordFilter.SetLanguage(SystemLanguage.Korean);
                _isInit = true;
            }

            // NOTE : 닉네임 길이 체크
            if (!NickNameLength(ref nickname, out var result))
            {
                switch (result)
                {
                    case 1: return Enum_NickNameValid.MaxLength;
                    case 2: return Enum_NickNameValid.MinLength;
                }
            }

            // NOTE : 특수 문자 체크
            if (IsMatchSpecialCharacter(ref nickname, "_"))
            {
                return Enum_NickNameValid.ContainSpecialWord;
            }

            // NOTE : 비속어 체크
            if (CookAppsBadWordFilter.Contains(nickname, AdditionalPatternsRegex))
            {
                return Enum_NickNameValid.ContainBadWord;
            }

            // NOTE : 닉네임 무결
            return Enum_NickNameValid.Valid;
        }

        /// <summary>
        /// 특수문자 체크
        /// </summary>
        /// <param name="text">체크할 문자열</param>
        /// <param name="exceptString">추가로 매칭 체크할 문자. 없으면 string.empty</param>
        /// <returns></returns>
        private static bool IsMatchSpecialCharacter(ref string text, string exceptString = "")
        {
            // 특수문자, 자음, 모음, 공백 등 Regex 매칭 체크
            if (!string.IsNullOrEmpty(exceptString))
            {
                return Regex.IsMatch(text, @"[" + $"^a-zA-Z0-9가-힇ぁ-ゔァ-ヴー々〆〤一-龥{exceptString}" + "]", RegexOptions.Singleline);
            }

            return Regex.IsMatch(text, @"[^a-zA-Z0-9가-힇ぁ-ゔァ-ヴー々〆〤一-龥]", RegexOptions.Singleline);
        }

        /// <summary>
        /// 닉네임 문자열 길이 체크
        /// </summary>
        /// <param name="name">체크할 문자열</param>
        /// /// <param name="result">0 : 가능, 1 : 최대길이제한초과, 2 : 최소 길이 제한 미달</param>
        /// <returns></returns>
        private bool NickNameLength(ref string name, out int result)
        {
            var nameByte = Encoding.UTF8.GetByteCount(name);

            // 12글자 (알파벳, 숫자) 이상 이거나 24바이트(알파벳 숫자 제외)
            if (MaxWordAmount < name.Length || MaxWordAmount * 3 < nameByte)
            {
                result = 1;
                return false;
            }

            // 영어 2글자(3byte) 미만 이거나 한글 2글자(6byte) 미만
            if ((name.Length < MinWordAmount && nameByte < MinWordAmount) ||
                (name.Length < MinWordAmount && nameByte < MinWordAmount * 3))
            {
                result = 2;
                return false;
            }

            result = 0;
            return true;
        }
    }
}

public enum Enum_NickNameValid
{
    Valid = 0,
    ContainBadWord = 1,
    ContainSpecialWord = 2,
    MinLength = 3,
    MaxLength = 4,
    Duplication = 5,
    NotChanged = 6,
}

using System.Collections;
using System.Collections.Generic;
using Pxp.Data;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class UIHeroSkillInfo : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textStar, _textDesc;

        [SerializeField]
        private GameObject _goLock;

        private UserHeroItem _hero;
        private Hero_Skill _skillData;

        public void SetHeroSkill(UserHeroItem hero, int star, Hero_Skill data)
        {
            _hero = hero;
            _skillData = data;
            _textStar.SetText(star + 1);
            _textDesc.SetText(_skillData.heroSkillDescription);
            _goLock.SetActive(_hero.Star < star + 1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class StarUI : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _stars;

        public void SetGrade(int grade)
        {
            _stars.ForEach(star => star.SetActive(false));
            _stars[grade].SetActive(true);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class Board : MonoBehaviour
    {
        public int BoardIndex { get; private set; }
        public int Actor;

        public void Awake()
        {
            BoardIndex = transform.GetSiblingIndex();
        }
    }
}

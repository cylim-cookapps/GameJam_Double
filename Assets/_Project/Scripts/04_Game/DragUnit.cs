using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Pxp
{
    public class DragUnit : MonoBehaviour
    {
        private HeroUnit _heroUnit;

        private GameObject _prefab;

        public void SetUnit(HeroUnit heroUnit, Vector2 pos)
        {
            _heroUnit = heroUnit;

            var resource = Resources.Load<GameObject>($"Model/Hero/{_heroUnit.HeroData.prefab_key}");
            _prefab = Instantiate(resource, transform);
            transform.position = pos;
            AdjustSpritesAlpha();

            ChangeLayerRecursively(_prefab, LayerMask.NameToLayer("HeroDrag"));
        }

        public void Dispose()
        {
            if (_prefab != null)
            {
                Destroy(_prefab);
                _prefab = null;
            }
        }

        private void AdjustSpritesAlpha()
        {
            // 해당 게임오브젝트와 모든 자식 오브젝트의 SpriteRenderer를 찾습니다.
            SpriteRenderer[] spriteRenderers = _prefab.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // 현재 색상을 가져옵니다.
                Color color = renderer.color;

                // 알파값만 변경합니다.
                color.a = 0.5f;

                // 변경된 색상을 적용합니다.
                renderer.color = color;
            }
        }

        private void ChangeLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (child == null) continue;
                ChangeLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}

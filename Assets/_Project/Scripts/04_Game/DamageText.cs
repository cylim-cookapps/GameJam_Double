using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;

namespace Pxp
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]
        private GameObject critical;

        [SerializeField]
        private TextMeshProUGUI textMesh;

        [SerializeField]
        private float lifeTime = 1f;

        [SerializeField]
        private float moveSpeed = 1f;

        [SerializeField]
        private float fadeSpeed = 1f;

        private Vector3 offset = new Vector3(0, 0.5f, 0);

        public void Initialize(int damage, bool isCri, Transform target)
        {
            critical.SetActive(isCri);
            textMesh.SetText(damage.ToString("N0"));
            Vector2 screenPoint;
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position + offset);
            else
                screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position - offset);
            transform.position = screenPoint;
            StartCoroutine(FadeAndMove());
        }

        private IEnumerator FadeAndMove()
        {
            float elapsedTime = 0f;
            Color originalColor = textMesh.color;

            while (elapsedTime < lifeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifeTime * fadeSpeed);
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

                yield return null;
            }

            GameUI.Inst.ReturnDamageText(this);
        }
    }
}

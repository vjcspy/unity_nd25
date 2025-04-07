using System;
using UnityEngine;
namespace ND25.Component.Sprite
{
    /// <summary>
    ///     Với những sprite cần có config phải manually create thì dùng SpriteSimpleRepeater
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSimpleRepeater : MonoBehaviour
    {
        Transform camTransform;
        Vector3 lastCamPosition;
        float spriteWidth;
        SpriteRenderer sr;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite)
            {
                Debug.LogError("[SimpleRepeater] SpriteRenderer or sprite missing.");
                enabled = false;
                return;
            }

            // Tính chiều rộng thực tế theo scale
            UnityEngine.Sprite sprite = sr.sprite;
            float unitWidth = sprite.rect.width / sprite.pixelsPerUnit;
            spriteWidth = unitWidth * transform.lossyScale.x;
            Debug.Log("Sprite Width: " + spriteWidth);
        }
        void Start()
        {
            if (Camera.main != null)
            {
                camTransform = Camera.main.transform;
                lastCamPosition = camTransform.position;
            }
            else
            {
                Debug.LogWarning("[SimpleRepeater] No main camera found.");
                enabled = false;
            }
        }

        void Update()
        {
            CheckAndRepeat();
        }

        void CheckAndRepeat()
        {
            if (!camTransform) return;

            Vector3 deltaMovement = camTransform.position - lastCamPosition;

            if (!(Math.Abs(deltaMovement.x) >= spriteWidth)) return;

            int direction = deltaMovement.x > 0 ? 1 : -1;
            transform.position += new Vector3(spriteWidth * direction, 0f, 0f);
            lastCamPosition = camTransform.position;
        }
    }
}

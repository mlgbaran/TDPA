using UnityEngine;

public class ShadowSpriteUpdater : MonoBehaviour
{
    public SpriteRenderer mainSpriteRenderer;    // Assign your player's SR here
    public SpriteRenderer shadowSpriteRenderer;  // Assign your shadow SR here

    [Tooltip("Optional offset for the shadow in local space.")]
    public Vector3 shadowOffset = new Vector3(0.05f, -0.05f, 0);

    void LateUpdate()
    {
        if (mainSpriteRenderer != null && shadowSpriteRenderer != null)
        {
            // 1) Copy the current sprite
            shadowSpriteRenderer.sprite = mainSpriteRenderer.sprite;

            // 2) Copy flip settings if you flip left/right
            shadowSpriteRenderer.flipX = mainSpriteRenderer.flipX;
            shadowSpriteRenderer.flipY = true;

            // 3) Optionally offset the shadow object
            shadowSpriteRenderer.transform.localPosition = shadowOffset;
        }
    }
}

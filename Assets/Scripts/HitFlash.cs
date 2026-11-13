
using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    public float flashDuration = 0.4f;

    private Renderer targetRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine flashCoroutine;

    private static readonly int FlashAmountID =
        Shader.PropertyToID("_FlashAmount");

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void TriggerFlash()
    {
        if (targetRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;

            float flashAmount =
                1f - Mathf.Clamp01(elapsed / flashDuration);

            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(FlashAmountID, flashAmount);
            targetRenderer.SetPropertyBlock(propertyBlock);

            yield return null;
        }

        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FlashAmountID, 0f);
        targetRenderer.SetPropertyBlock(propertyBlock);

        flashCoroutine = null;
    }
}

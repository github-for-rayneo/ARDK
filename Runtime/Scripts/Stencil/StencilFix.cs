using UnityEngine;

public class StencilFix : MonoBehaviour
{
    /// <summary>
    /// �޸�cardboard StencilʧЧBUG����
    /// </summary>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source,destination);
    }
}

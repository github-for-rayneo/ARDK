using UnityEngine;

public class StencilFix : MonoBehaviour
{
    /// <summary>
    /// ÐÞ¸´cardboard StencilÊ§Ð§BUG·½·¨
    /// </summary>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source,destination);
    }
}

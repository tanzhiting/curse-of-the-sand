using UnityEngine;
using System.Collections.Generic;

public class ObstacleFader : MonoBehaviour
{
    public Transform target;           // 角色对象
    public LayerMask obstacleMask;     // 遮挡物层（建议设定成 Obstacles）

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    void Update()
    {
        FadeObstaclesBetweenCameraAndTarget();
    }

    void FadeObstaclesBetweenCameraAndTarget()
    {
        // 1. 清除上一次的遮挡物
        foreach (Renderer r in currentObstacles)
        {
            if (r != null && originalMaterials.ContainsKey(r))
                r.materials = originalMaterials[r];
        }
        currentObstacles.Clear();

        // 2. 发射射线
        Vector3 direction = target.position - transform.position;
        float distance = Vector3.Distance(target.position, transform.position);
        Ray ray = new Ray(transform.position, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, distance, obstacleMask);
        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend && !currentObstacles.Contains(rend))
            {
                currentObstacles.Add(rend);

                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.materials;

                SetTransparent(rend);
            }
        }
    }

    void SetTransparent(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = 0.3f;
                mat.color = color;

                mat.SetFloat("_Mode", 2); // Fade 模式
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}
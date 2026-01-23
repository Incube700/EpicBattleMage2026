#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BigCityMaterialUpgrader
{
    private static readonly string[] SearchFolders =
    {
        "Assets/Big City",
        "Assets/BigCity"
    };

    [MenuItem("Tools/Render Pipeline/Upgrade Big City Materials to URP")]
    public static void Upgrade()
    {
        var materialGuids = new List<string>();
        foreach (var folder in SearchFolders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                continue;
            }

            materialGuids.AddRange(AssetDatabase.FindAssets("t:Material", new[] { folder }));
        }

        if (materialGuids.Count == 0)
        {
            Debug.LogWarning("Big City material upgrade skipped: no materials found in Assets/Big City or Assets/BigCity.");
            return;
        }

        var urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null)
        {
            Debug.LogError("URP Lit shader not found. Ensure the Universal Render Pipeline package is installed.");
            return;
        }

        var changed = 0;
        foreach (var guid in materialGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                continue;
            }

            if (material.shader != null && material.shader.name.StartsWith("Universal Render Pipeline/"))
            {
                continue;
            }

            UpgradeMaterial(material, urpLit);
            EditorUtility.SetDirty(material);
            changed++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Big City material upgrade complete. Updated {changed} materials.");
    }

    private static void UpgradeMaterial(Material material, Shader urpLit)
    {
        var baseColor = material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
        var baseMap = material.HasProperty("_MainTex") ? material.GetTexture("_MainTex") : null;
        var baseMapScale = material.HasProperty("_MainTex") ? material.GetTextureScale("_MainTex") : Vector2.one;
        var baseMapOffset = material.HasProperty("_MainTex") ? material.GetTextureOffset("_MainTex") : Vector2.zero;
        var metallic = material.HasProperty("_Metallic") ? material.GetFloat("_Metallic") : 0f;
        var smoothness = material.HasProperty("_Glossiness") ? material.GetFloat("_Glossiness") : 0.5f;
        var metalMap = material.HasProperty("_MetallicGlossMap") ? material.GetTexture("_MetallicGlossMap") : null;
        var bumpMap = material.HasProperty("_BumpMap") ? material.GetTexture("_BumpMap") : null;
        var bumpScale = material.HasProperty("_BumpScale") ? material.GetFloat("_BumpScale") : 1f;
        var occlusionMap = material.HasProperty("_OcclusionMap") ? material.GetTexture("_OcclusionMap") : null;
        var occlusionStrength = material.HasProperty("_OcclusionStrength") ? material.GetFloat("_OcclusionStrength") : 1f;
        var emissionMap = material.HasProperty("_EmissionMap") ? material.GetTexture("_EmissionMap") : null;
        var emissionColor = material.HasProperty("_EmissionColor") ? material.GetColor("_EmissionColor") : Color.black;
        var cutoff = material.HasProperty("_Cutoff") ? material.GetFloat("_Cutoff") : 0.5f;
        var hasAlphaClip = material.HasProperty("_Mode") && Mathf.Approximately(material.GetFloat("_Mode"), 1f);
        var isTransparent = material.HasProperty("_Mode") && Mathf.Approximately(material.GetFloat("_Mode"), 3f);

        material.shader = urpLit;

        material.SetColor("_BaseColor", baseColor);
        if (baseMap != null)
        {
            material.SetTexture("_BaseMap", baseMap);
            material.SetTextureScale("_BaseMap", baseMapScale);
            material.SetTextureOffset("_BaseMap", baseMapOffset);
        }

        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Smoothness", smoothness);
        if (metalMap != null)
        {
            material.SetTexture("_MetallicGlossMap", metalMap);
            material.EnableKeyword("_METALLICSPECGLOSSMAP");
        }

        if (bumpMap != null)
        {
            material.SetTexture("_BumpMap", bumpMap);
            material.SetFloat("_BumpScale", bumpScale);
            material.EnableKeyword("_NORMALMAP");
        }

        if (occlusionMap != null)
        {
            material.SetTexture("_OcclusionMap", occlusionMap);
            material.SetFloat("_OcclusionStrength", occlusionStrength);
        }

        if (emissionMap != null || emissionColor.maxColorComponent > 0f)
        {
            if (emissionMap != null)
            {
                material.SetTexture("_EmissionMap", emissionMap);
            }

            material.SetColor("_EmissionColor", emissionColor);
            material.EnableKeyword("_EMISSION");
        }

        if (hasAlphaClip)
        {
            material.SetFloat("_AlphaClip", 1f);
            material.SetFloat("_Cutoff", cutoff);
        }
        else
        {
            material.SetFloat("_AlphaClip", 0f);
        }

        if (isTransparent)
        {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_ZWrite", 0f);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else
        {
            material.SetFloat("_Surface", 0f);
            material.SetFloat("_ZWrite", 1f);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }
    }
}
#endif

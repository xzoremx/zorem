using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace ZoremGame.EditorTools
{
    /// <summary>
    /// Reemplaza el shader de los materiales rotos (Built-in Standard/Legacy/Mobile,
    /// o shader nulo) por "Universal Render Pipeline/Lit", conservando color y textura
    /// principal. Soluciona el típico magenta tras migrar a URP.
    ///
    /// Uso: menú "ZoremGame > Fix Materials (Built-in -> URP)".
    /// </summary>
    public static class UrpMaterialFixer
    {
        [MenuItem("ZoremGame/Fix URP Materials")]
        public static void Fix()
        {
            var urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null)
            {
                Debug.LogError("[UrpMaterialFixer] No se encontró el shader 'Universal Render Pipeline/Lit'. ¿URP está activo?");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Material");
            int converted = 0;
            var sample = new List<string>();

            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;

                var sh = mat.shader;
                string sname = sh == null ? "" : sh.name;
                bool broken = sh == null
                    || sname == "Standard"
                    || sname == "Standard (Specular setup)"
                    || sname == "Hidden/InternalErrorShader"
                    || sname.StartsWith("Legacy Shaders/")
                    || sname.StartsWith("Mobile/");
                if (!broken) continue;

                Color col = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
                Texture tex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;

                mat.shader = urpLit;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", col);
                if (tex != null && mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);

                EditorUtility.SetDirty(mat);
                converted++;
                if (sample.Count < 40) sample.Add(Path.GetFileName(path));
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[UrpMaterialFixer] Convertidos {converted} materiales a URP/Lit. Ejemplos: {string.Join(", ", sample)}");
        }
    }
}

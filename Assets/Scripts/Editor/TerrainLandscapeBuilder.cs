using System.IO;
using UnityEngine;
using UnityEditor;

namespace ZoremGame.EditorTools
{
    /// <summary>
    /// Genera un Terrain natural (colinas + valles) por ruido fractal, con una
    /// zona plana central para spawn, y lo pinta con dos capas simples
    /// (pasto en zonas planas, tierra en pendientes). Estilo "textura no compleja"
    /// tipo GTA San Andreas: geometría de terreno + texturas tileables de baja res.
    ///
    /// Uso: menú "ZoremGame > Build Terrain Landscape".
    /// Los assets generados se guardan en Assets/Environment/Generated/ y son
    /// 100% editables. Para el look final, reemplaza las texturas de cada
    /// TerrainLayer por las tileables de Kenney (kenney.nl).
    /// </summary>
    public static class TerrainLandscapeBuilder
    {
        // --- Parámetros del terreno (ajustables aquí) ---
        private const int   HeightmapRes = 513;   // potencia de 2 + 1
        private const float WidthMeters  = 500f;
        private const float LengthMeters = 500f;
        private const float MaxHeight    = 45f;    // altura máx de colinas
        private const float NoiseScale   = 2.5f;   // más alto = colinas más pequeñas/frecuentes
        private const int   Octaves      = 5;
        private const float Persistence  = 0.45f;
        private const float Lacunarity   = 2.0f;
        private const float FlatRadius   = 45f;    // radio de la zona plana central (m)
        private const float FlatBlend    = 35f;    // transición suave alrededor de la zona plana

        private const int   AlphamapRes  = 512;
        private const string GenFolder   = "Assets/Environment/Generated";

        [MenuItem("ZoremGame/Build Terrain Landscape")]
        public static void Build()
        {
            EnsureFolder("Assets/Environment");
            EnsureFolder(GenFolder);

            EditorUtility.DisplayProgressBar("Terrain Landscape", "Generando relieve...", 0.1f);

            // 1) Heightmap fractal con zona plana central.
            var terrainData = new TerrainData
            {
                heightmapResolution = HeightmapRes,
                size = new Vector3(WidthMeters, MaxHeight, LengthMeters),
                alphamapResolution = AlphamapRes
            };

            float[,] heights = GenerateHeights(HeightmapRes);
            terrainData.SetHeights(0, 0, heights);

            EditorUtility.DisplayProgressBar("Terrain Landscape", "Creando texturas...", 0.4f);

            // 2) Texturas simples (ruido suave) para pasto y tierra.
            var grassTex = CreateNoiseTexture("grass_simple", new Color(0.34f, 0.50f, 0.22f), 0.06f);
            var dirtTex  = CreateNoiseTexture("dirt_simple",  new Color(0.42f, 0.32f, 0.20f), 0.07f);

            var grassLayer = CreateTerrainLayer("Layer_Grass", grassTex, 15f);
            var dirtLayer  = CreateTerrainLayer("Layer_Dirt",  dirtTex, 12f);
            terrainData.terrainLayers = new[] { grassLayer, dirtLayer };

            EditorUtility.DisplayProgressBar("Terrain Landscape", "Pintando pendientes...", 0.7f);

            // 3) Splatmap: tierra donde hay pendiente fuerte, pasto en lo plano.
            PaintBySlope(terrainData);

            // 4) Guardar TerrainData y crear el GameObject del terreno centrado en el origen.
            string tdPath = $"{GenFolder}/TerrainData_Landscape.asset";
            AssetDatabase.CreateAsset(terrainData, AssetDatabase.GenerateUniqueAssetPath(tdPath));

            var existing = GameObject.Find("Landscape");
            if (existing != null) Object.DestroyImmediate(existing);

            var terrainGO = Terrain.CreateTerrainGameObject(terrainData);
            terrainGO.name = "Landscape";
            terrainGO.transform.position = new Vector3(-WidthMeters / 2f, 0f, -LengthMeters / 2f);
            terrainGO.isStatic = true;
            Undo.RegisterCreatedObjectUndo(terrainGO, "Build Terrain Landscape");

            // 5) Marcador de spawn en el centro (zona plana), sobre la superficie.
            var spawn = new GameObject("SpawnMarker");
            float centerY = terrainData.GetInterpolatedHeight(0.5f, 0.5f);
            spawn.transform.position = new Vector3(0f, centerY + 1f, 0f);
            Undo.RegisterCreatedObjectUndo(spawn, "Build Terrain Landscape");

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();

            Selection.activeGameObject = terrainGO;
            SceneView.lastActiveSceneView?.FrameSelected();
            Debug.Log("[TerrainLandscapeBuilder] Terrain creado en Assets/Environment/Generated. " +
                      "Coloca al Player cerca del 'SpawnMarker' (centro plano).");
        }

        // --- Generación de alturas ------------------------------------------

        private static float[,] GenerateHeights(int res)
        {
            var heights = new float[res, res];
            float seedX = Random.Range(0f, 1000f);
            float seedY = Random.Range(0f, 1000f);

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float nx = (float)x / (res - 1);
                    float ny = (float)y / (res - 1);

                    float h = Fractal(nx, ny, seedX, seedY);

                    // Aplanar el centro para el spawn (en metros del mundo).
                    float worldX = (nx - 0.5f) * WidthMeters;
                    float worldZ = (ny - 0.5f) * LengthMeters;
                    float distToCenter = Mathf.Sqrt(worldX * worldX + worldZ * worldZ);
                    float flat = Mathf.InverseLerp(FlatRadius, FlatRadius + FlatBlend, distToCenter);
                    float baseFlat = 0.12f; // altura normalizada de la planicie
                    h = Mathf.Lerp(baseFlat, h, flat);

                    heights[y, x] = Mathf.Clamp01(h);
                }
            }
            return heights;
        }

        private static float Fractal(float x, float y, float seedX, float seedY)
        {
            float total = 0f, freq = 1f, amp = 1f, max = 0f;
            for (int o = 0; o < Octaves; o++)
            {
                float sx = x * NoiseScale * freq + seedX;
                float sy = y * NoiseScale * freq + seedY;
                total += Mathf.PerlinNoise(sx, sy) * amp;
                max += amp;
                amp *= Persistence;
                freq *= Lacunarity;
            }
            return total / max;
        }

        // --- Pintado por pendiente ------------------------------------------

        private static void PaintBySlope(TerrainData data)
        {
            int res = data.alphamapResolution;
            var map = new float[res, res, 2];

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float nx = (float)x / (res - 1);
                    float ny = (float)y / (res - 1);
                    // GetSteepness usa coords normalizadas (x = ancho, y = largo).
                    float steep = data.GetSteepness(nx, ny);

                    // 0° = todo pasto, 35°+ = todo tierra, con transición.
                    float dirt = Mathf.InverseLerp(18f, 35f, steep);
                    map[y, x, 0] = 1f - dirt; // grass
                    map[y, x, 1] = dirt;      // dirt
                }
            }
            data.SetAlphamaps(0, 0, map);
        }

        // --- Helpers de assets ----------------------------------------------

        private static Texture2D CreateNoiseTexture(string name, Color baseColor, float variation)
        {
            const int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGB24, true);
            float seed = Random.Range(0f, 100f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float n = Mathf.PerlinNoise(x * 0.15f + seed, y * 0.15f + seed) - 0.5f;
                    var c = baseColor + new Color(n, n, n) * variation;
                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();

            string rel = $"{GenFolder}/{name}.png";
            string abs = Path.Combine(Directory.GetCurrentDirectory(), rel);
            File.WriteAllBytes(abs, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(rel, ImportAssetOptions.ForceUpdate);

            var importer = AssetImporter.GetAtPath(rel) as TextureImporter;
            if (importer != null)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(rel);
        }

        private static TerrainLayer CreateTerrainLayer(string name, Texture2D diffuse, float tileSize)
        {
            var layer = new TerrainLayer
            {
                diffuseTexture = diffuse,
                tileSize = new Vector2(tileSize, tileSize)
            };
            string path = AssetDatabase.GenerateUniqueAssetPath($"{GenFolder}/{name}.terrainlayer");
            AssetDatabase.CreateAsset(layer, path);
            return layer;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string leaf = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}

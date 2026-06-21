using UnityEngine;
using UnityEditor;

namespace ZoremGame.EditorTools
{
    /// <summary>
    /// Genera un greybox de pruebas con geometría a medida para validar
    /// el sistema de locomoción (Documentation/GDD/Etapa_1/locomotion.md):
    /// step-up, vault, crouch, slide, coyote time, landing recovery y slope limit.
    ///
    /// Uso: menú "ZoremGame > Build Test Playground".
    /// Todo se crea bajo un único GameObject "TestPlayground" para editar/borrar fácil.
    /// </summary>
    public static class TestPlaygroundBuilder
    {
        private const string RootName = "TestPlayground";

        [MenuItem("ZoremGame/Build Test Playground")]
        public static void Build()
        {
            // Si ya existe, lo reemplazamos para poder regenerar sin duplicar.
            var existing = GameObject.Find(RootName);
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("Test Playground",
                        "Ya existe un 'TestPlayground' en la escena. ¿Reemplazarlo?",
                        "Reemplazar", "Cancelar"))
                    return;
                Object.DestroyImmediate(existing);
            }

            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Build Test Playground");

            // Materiales por color (URP/Lit por defecto en los primitivos).
            var matGround   = MakeMaterial("PG_Ground",   new Color(0.55f, 0.55f, 0.58f));
            var matStep      = MakeMaterial("PG_Step",     new Color(0.30f, 0.65f, 0.90f)); // azul: step-up
            var matVault     = MakeMaterial("PG_Vault",    new Color(0.95f, 0.75f, 0.20f)); // amarillo: vault
            var matSlope     = MakeMaterial("PG_Slope",    new Color(0.60f, 0.85f, 0.40f)); // verde: pendientes
            var matPlatform = MakeMaterial("PG_Platform", new Color(0.85f, 0.40f, 0.40f)); // rojo: caídas
            var matWall      = MakeMaterial("PG_Wall",     new Color(0.45f, 0.35f, 0.55f)); // morado: paredes
            var matLane      = MakeMaterial("PG_Lane",     new Color(0.80f, 0.80f, 0.85f)); // claro: carril slide

            BuildGround(root, matGround);
            BuildSpawnMarker(root);
            BuildStepUpStairs(root, matStep);     // escalones para step-up suave
            BuildVaultObstacles(root, matVault);  // obstáculos bajos para vault
            BuildSlopes(root, matSlope);          // pendientes 30/45/60/75°
            BuildDropPlatforms(root, matPlatform);// plataformas 2/5/10m para landing recovery
            BuildSlideLane(root, matLane);        // carril plano para slide
            BuildCrouchTunnel(root, matWall);     // túnel bajo para crouch
            BuildWallRunWalls(root, matWall);     // paredes para wall-run (futuro)
            BuildLabels(root);

            Selection.activeGameObject = root;
            SceneView.lastActiveSceneView?.FrameSelected();
            Debug.Log("[TestPlaygroundBuilder] Playground creado. Coloca al Player cerca del 'SpawnMarker'.");
        }

        // --- Secciones -------------------------------------------------------

        private static void BuildGround(GameObject root, Material mat)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(root.transform);
            ground.transform.localScale = new Vector3(8f, 1f, 8f); // 80x80 m
            ground.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void BuildSpawnMarker(GameObject root)
        {
            var marker = new GameObject("SpawnMarker");
            marker.transform.SetParent(root.transform);
            marker.transform.localPosition = new Vector3(0f, 1f, 0f);
        }

        // Escalera de escalones de altura creciente: 0.20 / 0.35 / 0.50 / 0.70 m.
        // 0.35 es el límite de step-up; por encima debería requerir vault/salto.
        private static void BuildStepUpStairs(GameObject root, Material mat)
        {
            var section = new GameObject("01_StepUp_Stairs");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(-12f, 0f, 6f);

            float[] heights = { 0.20f, 0.35f, 0.50f, 0.70f };
            float z = 0f;
            float cumulative = 0f;
            foreach (var h in heights)
            {
                var step = GameObject.CreatePrimitive(PrimitiveType.Cube);
                step.name = $"Step_{h:0.00}m";
                step.transform.SetParent(section.transform);
                step.transform.localScale = new Vector3(3f, h, 1.5f);
                step.transform.localPosition = new Vector3(0f, cumulative + h / 2f, z);
                step.GetComponent<Renderer>().sharedMaterial = mat;
                cumulative += h;
                z += 1.5f;
            }
        }

        // Obstáculos bajos para vault: 0.3 / 0.6 / 0.9 / 1.2 m de alto.
        private static void BuildVaultObstacles(GameObject root, Material mat)
        {
            var section = new GameObject("02_Vault_Obstacles");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(-4f, 0f, 6f);

            float[] heights = { 0.3f, 0.6f, 0.9f, 1.2f };
            float z = 0f;
            foreach (var h in heights)
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Vault_{h:0.0}m";
                wall.transform.SetParent(section.transform);
                wall.transform.localScale = new Vector3(4f, h, 0.4f); // delgado = saltable por encima
                wall.transform.localPosition = new Vector3(0f, h / 2f, z);
                wall.GetComponent<Renderer>().sharedMaterial = mat;
                z += 3f;
            }
        }

        // Rampas a 30 / 45 / 60 / 75°. El slopeLimit por defecto es 75°.
        private static void BuildSlopes(GameObject root, Material mat)
        {
            var section = new GameObject("03_Slopes");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(6f, 0f, 6f);

            float[] angles = { 30f, 45f, 60f, 75f };
            float x = 0f;
            foreach (var a in angles)
            {
                var ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ramp.name = $"Slope_{a:0}deg";
                ramp.transform.SetParent(section.transform);
                ramp.transform.localScale = new Vector3(3f, 0.5f, 6f);
                ramp.transform.localPosition = new Vector3(x, 1.5f, 0f);
                ramp.transform.localRotation = Quaternion.Euler(a, 0f, 0f);
                ramp.GetComponent<Renderer>().sharedMaterial = mat;
                x += 4f;
            }
        }

        // Plataformas elevadas a 2 / 5 / 10 m para probar Landing Recovery + Coyote Time.
        private static void BuildDropPlatforms(GameObject root, Material mat)
        {
            var section = new GameObject("04_Drop_Platforms");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(0f, 0f, -8f);

            float[] heights = { 2f, 5f, 10f };
            float x = -6f;
            foreach (var h in heights)
            {
                var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                platform.name = $"DropPlatform_{h:0}m";
                platform.transform.SetParent(section.transform);
                platform.transform.localScale = new Vector3(4f, 0.5f, 4f);
                platform.transform.localPosition = new Vector3(x, h, 0f);
                platform.GetComponent<Renderer>().sharedMaterial = mat;

                // Rampa de acceso para subir sin saltar.
                var access = GameObject.CreatePrimitive(PrimitiveType.Cube);
                access.name = $"Ramp_to_{h:0}m";
                access.transform.SetParent(section.transform);
                float rampLen = h * 2.2f;
                access.transform.localScale = new Vector3(2f, 0.3f, rampLen);
                access.transform.localPosition = new Vector3(x, h / 2f, -2f - rampLen / 2.4f);
                access.transform.localRotation = Quaternion.Euler(Mathf.Atan2(h, rampLen) * Mathf.Rad2Deg, 0f, 0f);
                access.GetComponent<Renderer>().sharedMaterial = mat;

                x += 6f;
            }
        }

        // Carril largo y plano para medir distancia/duración de slide.
        private static void BuildSlideLane(GameObject root, Material mat)
        {
            var section = new GameObject("05_Slide_Lane");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(14f, 0f, -6f);

            var lane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lane.name = "SlideLane";
            lane.transform.SetParent(section.transform);
            lane.transform.localScale = new Vector3(2.5f, 0.1f, 20f);
            lane.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            lane.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Túnel bajo (1.0 m) que obliga a hacer crouch para pasar.
        private static void BuildCrouchTunnel(GameObject root, Material mat)
        {
            var section = new GameObject("06_Crouch_Tunnel");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(-14f, 0f, -6f);

            var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "TunnelRoof_1.0m";
            roof.transform.SetParent(section.transform);
            roof.transform.localScale = new Vector3(4f, 0.3f, 4f);
            roof.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            roof.GetComponent<Renderer>().sharedMaterial = mat;

            CreatePillar(section, mat, new Vector3(-1.85f, 0.5f, 0f));
            CreatePillar(section, mat, new Vector3(1.85f, 0.5f, 0f));
        }

        private static void CreatePillar(GameObject parent, Material mat, Vector3 pos)
        {
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = "Pillar";
            pillar.transform.SetParent(parent.transform);
            pillar.transform.localScale = new Vector3(0.3f, 1.0f, 4f);
            pillar.transform.localPosition = pos;
            pillar.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Dos paredes verticales paralelas para futuro wall-run / parkour.
        private static void BuildWallRunWalls(GameObject root, Material mat)
        {
            var section = new GameObject("07_WallRun_Walls");
            section.transform.SetParent(root.transform);
            section.transform.localPosition = new Vector3(20f, 0f, 6f);

            var wallA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallA.name = "Wall_A";
            wallA.transform.SetParent(section.transform);
            wallA.transform.localScale = new Vector3(0.5f, 4f, 12f);
            wallA.transform.localPosition = new Vector3(-2f, 2f, 0f);
            wallA.GetComponent<Renderer>().sharedMaterial = mat;

            var wallB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallB.name = "Wall_B";
            wallB.transform.SetParent(section.transform);
            wallB.transform.localScale = new Vector3(0.5f, 4f, 12f);
            wallB.transform.localPosition = new Vector3(2f, 2f, 0f);
            wallB.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void BuildLabels(GameObject root)
        {
            // Etiquetas de texto sobre cada sección para orientarse en la escena.
            AddLabel(root, "STEP-UP (0.20/0.35/0.50/0.70m)", new Vector3(-12f, 2.5f, 6f));
            AddLabel(root, "VAULT (0.3/0.6/0.9/1.2m)",        new Vector3(-4f, 2.5f, 6f));
            AddLabel(root, "SLOPES (30/45/60/75)",            new Vector3(9f, 4f, 6f));
            AddLabel(root, "DROPS (2/5/10m)",                 new Vector3(0f, 11f, -8f));
            AddLabel(root, "SLIDE LANE",                      new Vector3(14f, 2f, -6f));
            AddLabel(root, "CROUCH TUNNEL (1.0m)",            new Vector3(-14f, 2.5f, -6f));
            AddLabel(root, "WALL-RUN",                        new Vector3(20f, 5f, 6f));
        }

        private static void AddLabel(GameObject root, string text, Vector3 pos)
        {
            var go = new GameObject($"Label_{text}");
            go.transform.SetParent(root.transform);
            go.transform.localPosition = pos;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.fontSize = 48;
            tm.characterSize = 0.08f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
        }

        // --- Util ------------------------------------------------------------

        private static Material MakeMaterial(string name, Color color)
        {
            // Usa el shader del pipeline activo (URP/Lit en este proyecto).
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard"); // fallback Built-in
            var mat = new Material(shader) { name = name };
            mat.color = color;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            return mat;
        }
    }
}

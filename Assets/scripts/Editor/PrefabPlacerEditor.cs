using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PrefabPlacer))]
[CanEditMultipleObjects]
public class PrefabPlacerEditor : Editor
{


    #region Vars
    #region InspectorVars

    #endregion

    /// <summary>
    /// Main object
    /// </summary>
    private PrefabPlacer placer;

    /// <summary>
    /// ? Allowing drawind/deleting spawned GO
    /// </summary>
    bool allowToDraw = true;

    /// <summary>
    /// We drawing(true) or deleting(false) objects
    /// </summary>
    bool isDrawing = true;



    #endregion

    #region Props

    /// <summary>
    /// Mouse position
    /// </summary>
    /// <value>Vector3 of mouse position on scene</value>
    private Vector3 MousePosition
    {
        get
        {
            RaycastHit hit;
            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit))
            {
                return new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            return placer.transform.position;
        }
    }

    private GameObject Prefab
    {
        get => placer.prefab;
        set
        {
            placer.prefab = value;
        }
    }

    private float YOffset
    {
        get => placer.yOffset;
        set
        {
            placer.yOffset = value;
        }
    }
    private float Radius
    {
        get => placer.radius;
        set
        {
            if (!(value < 0))
            {
                placer.radius = value;
            }
        }
    }
    private bool RandomiseYRotation
    {
        get => placer.randomiseYRotation;
        set
        {
            placer.randomiseYRotation = value;
        }
    }
    private bool UseGeometryInteadOfPivot
    {
        get => placer.useGeometryInsteadOfPivot;
        set
        {
            placer.useGeometryInsteadOfPivot = value;
        }
    }
    private int AmountOfGO
    {
        get => placer.amountOfGO;
        set
        {
            if (!(value < 0))
            {
                placer.amountOfGO = value;
            }
        }
    }
    private List<GameObject> SpawnedObjects
    {
        get => placer.spawnedObjects;
        set
        {
            if (value != null)
            {
                placer.spawnedObjects = value;
            }
        }
    }

    #endregion

    #region BuildInMethods
    private void OnEnable()
    {
        placer = target as PrefabPlacer;
    }
    public override void OnInspectorGUI()
    {
        DrawRandomizeYRotationToogle();
        DrawUseGeomInstPivot();
        DrawYOffsetFiled();
        DrawRadiusSlider();
        DrawAmountOfGO();
        DrawDeleteAllBut();
        DrawPrefabsPreviews();
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        DeactivateSelectionOnDrawing();
        Drawing();
    }

    // Creating custom GO menu options
    [MenuItem("GameObject/PrefabPlacer", false, 10)]
    private static void CreateCustomGameObject(MenuCommand mCommand)
    {
        GameObject go = new GameObject("SpawnedObject");
        GameObjectUtility.SetParentAndAlign(go, mCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        go.AddComponent<PrefabPlacer>();
        Selection.activeObject = go;
    }
    #endregion




    #region customMethods
    /// <summary>
    /// Drawing sequinces
    /// </summary>
    private void Drawing()
    {
        CheckForOtherKeys();
        DrawOrDelete();
        DrawCircleUnderMouse();
        OnMouseDragActions();
        RedrawOnAction();
    }



    #region InputHandle
    /// <summary>
    /// Deactivate selectiong while drawing
    /// </summary>
    private void DeactivateSelectionOnDrawing()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }


    /// <summary>
    /// Check for keys that's not involved in drawing
    /// </summary>
    private void CheckForOtherKeys()
    {
        Event e = Event.current;

        //lock drawing if nonfunctional key was pressed
        if (e.type == EventType.KeyDown && e.keyCode != KeyCode.LeftShift)
        {
            allowToDraw = false;
        }
        //Allow drawing if key was released
        if (e.type == EventType.KeyUp)
        {
            allowToDraw = true;
        }
    }

    private void OnMouseDragActions()
    {
        Event e = Event.current;
        //Check if ,LMB was dragged or pressed and we are allowed to draw
        if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0 && allowToDraw && (Tools.current != Tool.View))
        {
            //Check if we drawing not deleting objects
            if (isDrawing)
            {

                RaycastHit hit;
                if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), out hit))
                {
                    //Check if not hitted already spawned object
                    string hittedObject = hit.transform.name.Split('_')[0];
                    if (Prefab.name == hittedObject)
                    {
                        return;
                    }

                    PlacePrefabs();
                    SceneView.RepaintAll();
                }
            }
            //Delete already spawned objects
            else
            {
                DeleteSpawnedPrefabs();
                SceneView.RepaintAll();
            }
        }

    }


    /// <summary>
    /// Repaint scene on mouse actions
    /// </summary>
    /// <param name="e">Current event</param>
    private void RedrawOnAction()
    {
        Event e = Event.current;
        if (e.isMouse || e.isKey)
        {
            SceneView.RepaintAll();
        }
    }

    /// <summary>
    /// Check if drwaing or deleting objects
    /// </summary>
    /// <param name="isDrawing">?able to draw</param>
    private void DrawOrDelete()
    {
        Event e = Event.current;

        if (e.keyCode == KeyCode.LeftShift)
        {
            if (e.type == EventType.KeyUp)
            {
                isDrawing = true;
            }
            else
            {
                isDrawing = false;
            }
        }
    }
    #endregion



    #region InspectorGUI
    private void DrawRandomizeYRotationToogle()
    {
        EditorGUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Randomise Y rotation");
        RandomiseYRotation = EditorGUILayout.Toggle(RandomiseYRotation);
        GUILayout.EndHorizontal();
    }

    private void DrawUseGeomInstPivot()
    {
        EditorGUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Use geometry instead of pivot");
        UseGeometryInteadOfPivot = EditorGUILayout.Toggle(UseGeometryInteadOfPivot);
        GUILayout.EndHorizontal();
    }

    private void DrawYOffsetFiled()
    {
        EditorGUILayout.Space(10f);
        GUILayout.Label("Y offset");
        YOffset = EditorGUILayout.FloatField(YOffset);
    }


    private void DrawRadiusSlider()
    {
        EditorGUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Radius");
        Radius = EditorGUILayout.Slider(Radius, 0f, 100f);
        GUILayout.EndHorizontal();
    }

    private void DrawAmountOfGO()
    {
        EditorGUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Amount of GO to spawn per radius");
        AmountOfGO = EditorGUILayout.IntField(AmountOfGO);
        GUILayout.EndHorizontal();
    }



    private void DrawDeleteAllBut()
    {
        EditorGUILayout.Space(10f);
        if (GUILayout.Button("Destroy spawned", GUILayout.Height(20f)))
        {
            DestroySpawned();
        }
    }

    private void DrawPrefabsPreviews()
    {
        EditorGUILayout.Space(10f);
        GUILayout.Label(AssetPreview.GetAssetPreview(Prefab), new GUIStyle() { alignment = TextAnchor.MiddleCenter });
    }



    #endregion


    #region SceneGUI
    /// <summary>
    /// Drawing circle under mouse if mouse above GO
    /// </summary>
    /// <param name="isDrawing">if we able to draw</param>
    private void DrawCircleUnderMouse()
    {
        //If mouse collide with smth we can draw disc
        if (MousePosition != placer.transform.position)
        {
            //If we are ready to draw set color to green
            if (isDrawing)
            {
                Handles.color = new Color(0f, 1f, 0f, 0.1f);
            }
            //If we are deleting set color to red
            else
            {
                Handles.color = new Color(1f, 0f, 0f, 0.1f);
            }
            Handles.DrawSolidDisc(MousePosition, Vector3.up, Radius);
        }
    }
    #endregion



    #region SupsMethods
    #endregion



    #region SpawnControll
    /// <summary>
    /// Place prefab on scene
    /// </summary>
    private void PlacePrefabs()
    {
        foreach (Vector3 i in RandomiseSpawnPositions())
        {
            if (AllowToSpawn())
            {
                GameObject go = Instantiate(Prefab, new Vector3(i.x, i.y + YOffset, i.z), CalculateRotation());
                Undo.RegisterCreatedObjectUndo(go, "New object created " + Prefab.name);
                go.transform.SetParent(placer.transform);
                SpawnedObjects.Add(go);

            }
        }
    }

    private Quaternion CalculateRotation()
    {
        Quaternion rot = Prefab.transform.rotation;
        if (RandomiseYRotation)
        {
            rot.y = Random.Range(0, 359);
        }
        return rot;
    }

    private float GetBottomVert()
    {
        float lowest = Prefab.transform.position.y;
        foreach (MeshFilter i in Prefab.GetComponentsInChildren<MeshFilter>())
        {
            if (i != null)
            {
                lowest = i.sharedMesh.vertices[0].y;
                foreach (Vector3 j in i.sharedMesh.vertices)
                {
                    if (lowest > j.y)
                    {
                        lowest = j.y;
                    }
                }
            }
        }
        return lowest;
    }

    private Vector3 FindGround(Vector3 position)
    {
        Vector3 spawnPos = position;
        RaycastHit hit;
        if (!Physics.Raycast(position, Vector3.up, out hit))
        {
            if (Physics.Raycast(new Vector3(position.x, position.y + 1000f, position.z), -Vector3.up, out hit))
            {
                spawnPos = hit.point;
            }
        }
        else
        {
            if (Physics.Raycast(hit.point, -Vector3.up, out hit))
            {
                spawnPos = hit.point;
            }
        }

        return spawnPos;
    }

    private Vector3[] RandomiseSpawnPositions()
    {
        Vector3[] positions = new Vector3[AmountOfGO];
        for (int i = 0; i < positions.Length; i++)
        {
            float x = MousePosition.x + Random.Range(-Radius, Radius);
            float z = MousePosition.z + Random.Range(-Radius, Radius);
            float y = MousePosition.y;
            positions[i] = new Vector3(x, y, z);

            positions[i].y = FindGround(positions[i]).y;
            if (UseGeometryInteadOfPivot)
            {
                positions[i].y -= GetBottomVert();
            }

        }
        return positions;
    }

    private bool AllowToSpawn()
    {
        int amount = 0;

        if (SpawnedObjects != null)
        {
            foreach (GameObject i in SpawnedObjects)
            {
                if (i != null)
                {
                    if (InCircle(i.transform.position) && i.transform.parent == placer.transform)
                    {
                        amount++;
                    }
                }


            }

        }
        return amount < AmountOfGO;
    }

    private bool InCircle(Vector3 target) => Mathf.Sqrt(Mathf.Pow((target.x - MousePosition.x), 2) +
                                                Mathf.Pow((target.z - MousePosition.z), 2)) < Radius;

    /// <summary>
    /// Delete prefab from scene
    /// </summary>
    private void DeleteSpawnedPrefabs()
    {
        foreach (GameObject i in SpawnedObjects)
        {
            if (i != null)
            {
                if (InCircle(i.transform.position))
                {
                    DestroyImmediate(i);
                }
            }
        }
    }

    private void DestroySpawned()
    {
        foreach (Transform i in placer.GetComponentsInChildren<Transform>())
        {
            if (i != null)
            {
                if (i.name != placer.name)
                {
                    DestroyImmediate(i.gameObject);
                }
            }
        }
        SpawnedObjects = new List<GameObject>();
    }
    #endregion

    #endregion























    /*
        private void CreateSmth()
        {
            if (placer.prefabsToPlace != null || placer.prefabsToPlace.Length != 0)
            {


                bool[] avalibleToPlace = new bool[placer.prefabsToPlace.Length];


                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.margin = new RectOffset(5, 5, 5, 5);

                for (int i = 0; i < placer.prefabsToPlace.Length; i++)
                {

                    avalibleToPlace[i] = GUILayout.Toggle(avalibleToPlace[i], AssetPreview.GetAssetPreview(placer.prefabsToPlace[i]), style, GUILayout.Width(50), GUILayout.Height(50));
                }

            }
        }
        */

}


using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlatformBuilder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject previewPrefab;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private List<GameObject> previewInstances = new List<GameObject>();
    [SerializeField] private Vector2 unitSize = Vector2.one;
    [SerializeField] private LayerMask platformLayer;

    [Header("Editor Tools")]
    [SerializeField] public bool isDrawingMode;
    [SerializeField] private Color gridColor = Color.cyan;

    public List<Vector2Int> coordinates = new List<Vector2Int>();
    private PlatformController currentPlatform;

    private void Update()
    {
        if (!isDrawingMode) return;

        if (Input.GetMouseButton(0))
        {
            Debug.Log("�ڻ���");
            Vector2 mousePos = GetMouseWorldPosition();
            Vector2Int gridPos = WorldToGrid(mousePos);

            if (!coordinates.Contains(gridPos))
            {
                coordinates.Add(gridPos);
                UpdatePreviewVisual();
            }
        }
        //Debug.Log("����ģʽ");
    }

    public void GeneratePlatform()
    {
        //currentPlatform = new GameObject("PlatformController");
        //var controller = currentPlatform.AddComponent<PlatformController>();


        currentPlatform = Instantiate(platformPrefab).GetComponent<PlatformController>();
        var units = new GameObject("PlatformUnits");
        units.transform.parent = currentPlatform.transform;
        foreach (var pos in coordinates)
        {
            Vector3 spawnPos = GridToWorld(pos);
            PlatformUnit unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity, units.transform).GetComponent<PlatformUnit>();
            //unit.Init(currentPlatform.GetComponentInParent<PlatformController>());
            unit.GetComponent<BoxCollider2D>().size = Vector2.one;//�༭ģʽ�¸���û��Awake ���Բ���ֱ����thisBoxCol
            unit.GetComponent<BoxCollider2D>().usedByComposite = true;
        }
        currentPlatform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        currentPlatform.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
        currentPlatform.GetComponent<CompositeCollider2D>(). GenerateGeometry();
        //AutoGenerateCompositeCollider();
        coordinates.Clear();
        UpdatePreviewVisual();
        isDrawingMode = false;
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt((worldPos.x - unitSize.x/2) / unitSize.x),
            Mathf.RoundToInt((worldPos.y - unitSize.y/2) / unitSize.y)
        );
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(
            unitSize.x / 2 + gridPos.x * unitSize.x,
            unitSize.y / 2 + gridPos.y * unitSize.y,
            0
        );
    }

    private void OnDrawGizmos()
    {
        if (!isDrawingMode) return;

        Gizmos.color = gridColor;
        foreach (var pos in coordinates)
        {
            Vector3 center = GridToWorld(pos);
            Gizmos.DrawWireCube(center, unitSize * 0.9f);
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        // ����ʹ�ó�����ͼ�����
        if (SceneView.currentDrawingSceneView != null && !Application.isPlaying)
        {
            sceneCamera = SceneView.currentDrawingSceneView.camera;
            return sceneCamera.ScreenToWorldPoint(Event.current.mousePosition);
        }

        // ����ʱʹ��Cinemachine�������
        if (Camera.main != null)
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // ���÷���������Cinemachine�Բ������
        var brain = GetActiveCamera();
        return brain != null ?
            brain.ScreenToWorldPoint(Input.mousePosition) :
            Vector2.zero;
    }
    private Camera GetActiveCamera()
    {
        // �����༭ģʽ�µ����⴦��
        if (!Application.isPlaying)
        {
            return SceneView.currentDrawingSceneView?.camera;
        }

        // ����ʱ��ȡCinemachine������
        var brains = FindObjectsOfType<CinemachineBrain>();
        foreach (var brain in brains)
        {
            if (brain.IsLive(brain.ActiveVirtualCamera))
            {
                return brain.OutputCamera;
            }
        }

        // ���÷����������������
        return Camera.main;
    }


    public void UpdatePreviewVisual()
    {
        // �����Ԥ��
        foreach (var obj in previewInstances)
        {
            DestroyImmediate(obj);
        }
        previewInstances.Clear();

        // ������Ԥ��
        foreach (var pos in coordinates)
        {
            Vector3 spawnPos = GridToWorld(pos);
            GameObject preview = Instantiate(
                previewPrefab,
                spawnPos,
                Quaternion.identity
            );
            previewInstances.Add(preview);
        }

    }
    private void AutoGenerateCompositeCollider()
    {
        if (currentPlatform == null) return;
        Rigidbody2D rb = currentPlatform.GetComponent<Rigidbody2D>();
        CompositeCollider2D composite = currentPlatform.GetComponent<CompositeCollider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;

        // Ϊ�����Ӷ��������ײ��
        foreach (Transform child in currentPlatform.transform)
        {
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.usedByComposite = true; // �ؼ����ã�
        }

        // ǿ�Ƹ�����ײ��
        composite.GenerateGeometry();
    }
    public void ClearPreview()
    {
        coordinates.Clear();
        UpdatePreviewVisual();
    }

}

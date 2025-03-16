using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlatformBuilder))]
public class PlatformBuilderEditor : Editor
{
    public PlatformBuilder builder;

    private void OnEnable()
    {
        builder = (PlatformBuilder)target;
    }

    public override void OnInspectorGUI()
    {
        // ����Ĭ��Inspector
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        // ����ģʽ����
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("�������ģʽ"))
            {
                builder.isDrawingMode = true;
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("�˳�����ģʽ"))
            {
                builder.isDrawingMode = false;
                builder.ClearPreview();
            }
        }
        EditorGUILayout.EndHorizontal();

        // ������ť
        if (builder.isDrawingMode)
        {
            EditorGUILayout.BeginVertical("Box");
            {
                if (GUILayout.Button("����ƽ̨ (B)"))
                {
                    builder.GeneratePlatform();
                }

                if (GUILayout.Button("��յ�ǰ���� (C)"))
                {
                    builder.ClearPreview();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    // ������ͼ����
    private void OnSceneGUI()
    {
        if (!builder.isDrawingMode) return;

        Event e = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

        // �ֶ�������������
        Vector2Int gridPos = builder.WorldToGrid(mousePos);

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (!builder.coordinates.Contains(gridPos))
            {
                builder.coordinates.Add(gridPos);
                builder.UpdatePreviewVisual();
                e.Use();
            }
        }

        // ǿ���ػ泡����ͼ
        SceneView.RepaintAll();
    }
}
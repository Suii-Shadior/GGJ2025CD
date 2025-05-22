// SceneConnectionEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SceneConnectionEditor : EditorWindow
{
    private SceneConnectionManager manager;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Scene Connection Manager")]
    public static void ShowWindow()
    {
        GetWindow<SceneConnectionEditor>("Scene Connections");
    }

    void OnGUI()
    {
        #region Window��һ��
        EditorGUILayout.BeginHorizontal();//WindowUI��һ�е���ʼ��־
        manager = (SceneConnectionManager)EditorGUILayout.ObjectField(
            "Connection Manager",
            manager,
            typeof(SceneConnectionManager),
            false
        );

        if (GUILayout.Button("Create New", GUILayout.Width(100)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Scene Connection Manager",
                "SceneConnectionManager",
                "asset",
                "Select save location"
            );
            if (!string.IsNullOrEmpty(path))
            {
                manager = CreateInstance<SceneConnectionManager>();
                AssetDatabase.CreateAsset(manager, path);
                AssetDatabase.SaveAssets();
            }
        }
        EditorGUILayout.EndHorizontal();//WindowUI��һ�еĽ�����־
        #endregion

        if (manager == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);//��������λ�ã������������ڶ���

        for (int i = 0; i < manager.connections.Count; i++)//������������
        {
            EditorGUILayout.BeginVertical("Box");

            // �������ֶ�
            manager.connections[i].mainScene = EditorGUILayout.TextField(
                "Main Scene",
                manager.connections[i].mainScene
            );

            // �ڽ������б�
            EditorGUILayout.LabelField("Neighbor Scenes");
            for (int j = 0; j < manager.connections[i].neighborScenes.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                manager.connections[i].neighborScenes[j] = EditorGUILayout.TextField(
                    manager.connections[i].neighborScenes[j]
                );
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    manager.connections[i].neighborScenes.RemoveAt(j);
                    j--;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Neighbor"))
            {
                manager.connections[i].neighborScenes.Add("");
            }

            // ɾ����ť
            if (GUILayout.Button("Remove This Connection"))
            {
                manager.connections.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (GUILayout.Button("Add New Connection"))
        {
            manager.connections.Add(new SceneConnectionManager.SceneConnection());
        }

        EditorGUILayout.EndScrollView();//�����ײ�λ��

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }
}
#endif

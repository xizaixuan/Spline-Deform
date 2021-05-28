using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    private Spline Spline => (Spline)serializedObject.targetObject;

    private SplineNode m_Selection = null;

    public GUIStyle NormalNodeStyle = new GUIStyle();
    public GUIStyle VisualNodeStyle = new GUIStyle();
    public GUIStyle SelectedNodeStyle = new GUIStyle();

    public void OnSceneGUI()
    {
        var oldMat = Handles.matrix;
        Handles.matrix = Spline.transform.localToWorldMatrix;

        foreach (var curve in Spline.Curves)
        {
            Handles.DrawAAPolyLine(4, curve.Samples.ConvertAll(sample => sample.Position).ToArray());
        }

        if (m_Selection != null)
        {
            Vector3 newPosition = Handles.PositionHandle(m_Selection.Position, Spline.transform.rotation);
            if (newPosition != m_Selection.Position)
            {
                Spline.MoveNode(m_Selection, newPosition - m_Selection.Position);
            }
        }

        Handles.BeginGUI();
        foreach (var node in Spline.Nodes)
        {
            if (NodeControlPoint(HandleUtility.WorldToGUIPoint(node.Position), new GUIContent(" " + Spline.Nodes.IndexOf(node).ToString()), m_Selection == node ? SelectedNodeStyle : NormalNodeStyle))
            {
                m_Selection = node;
            }
        }

        foreach (var curve in Spline.Curves)
        {
            var node = curve.Samples[curve.Samples.Count >> 1];
            if (NodeControlPoint(HandleUtility.WorldToGUIPoint(node.Position), new GUIContent(), VisualNodeStyle))
            {
                var scale = Vector3.Lerp(curve.node0.Scale, curve.node1.Scale, 0.5f);
                var newNode = new SplineNode() {  Position = node.Position, Scale = scale };
                Spline.InsertNode(Spline.Nodes.IndexOf(curve.node1), newNode);

                m_Selection = newNode;
            }
        }
        Handles.EndGUI();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        Handles.matrix = oldMat;
    }

    private void OnEnable()
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f, 1.0f));
        t.Apply();
        NormalNodeStyle = new GUIStyle();
        NormalNodeStyle.normal.background = t;

        var t1 = new Texture2D(1, 1);
        t1.SetPixel(0, 0, new Color(0.0f, 1.0f, 0.0f, 0.3f));
        t1.Apply();
        VisualNodeStyle = new GUIStyle();
        VisualNodeStyle.normal.background = t1;

        var t2 = new Texture2D(1, 1);
        t2.SetPixel(0, 0, new Color(1.0f, 0.0f, 0.0f, 1.0f));
        t2.Apply();
        SelectedNodeStyle = new GUIStyle();
        SelectedNodeStyle.normal.background = t2;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Delete Node"))
        {
            if (m_Selection != null)
            {
                Spline.RemoveNode(m_Selection);
                m_Selection = null;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (m_Selection != null)
        {
            EditorGUILayout.LabelField("Selected node");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector3Field("Position", m_Selection.Position);
            EditorGUI.EndDisabledGroup();
            m_Selection.Scale = EditorGUILayout.Vector3Field("Scale", m_Selection.Scale);
            m_Selection.Roll = EditorGUILayout.Slider("Role", m_Selection.Roll, 0.0f, 359.0f);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    private bool NodeControlPoint(Vector2 position, GUIContent content,GUIStyle style)
    {
        var buttonSize = 16;
        return GUI.Button(new Rect(position - new Vector2(buttonSize / 2, buttonSize / 2), new Vector2(buttonSize, buttonSize)), content, style);
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    private Spline Spline => (Spline)serializedObject.targetObject;

    private SplineNode m_Selection = null;

    public void OnSceneGUI()
    {
        var oldMat = Handles.matrix;
        Handles.matrix = Spline.transform.localToWorldMatrix;

        foreach (var curve in Spline.Curves)
        {
            Handles.DrawAAPolyLine(4, curve.Samples.ConvertAll(sample => sample.position).ToArray());
        }

        if (m_Selection != null)
        {
            Vector3 newPosition = Handles.PositionHandle(m_Selection.Position, Spline.transform.rotation);
            if (newPosition != m_Selection.Position)
            {
                m_Selection.InVec += newPosition - m_Selection.Position;
                m_Selection.OutVec += newPosition - m_Selection.Position;
                m_Selection.Position = newPosition;
            }
        }

        Handles.BeginGUI();
        foreach (var node in Spline.Nodes)
        {
            if (NodeButton(HandleUtility.WorldToGUIPoint(node.Position), new GUIContent(" " + Spline.Nodes.IndexOf(node).ToString()), NodeButtonStyle))
            {
                m_Selection = node;
            }
        }
        Handles.EndGUI();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        Handles.matrix = oldMat;
    }

    public GUIStyle NodeButtonStyle = new GUIStyle();

    private void OnEnable()
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f, 1.0f));
        t.Apply();
        NodeButtonStyle = new GUIStyle();
        NodeButtonStyle.normal.background = t;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private bool NodeButton(Vector2 position, GUIContent content,GUIStyle style)
    {
        var buttonSize = 16;
        return GUI.Button(new Rect(position - new Vector2(buttonSize / 2, buttonSize / 2), new Vector2(buttonSize, buttonSize)), content, style);
    }
}

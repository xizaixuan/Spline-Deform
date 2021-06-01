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
    public GUIStyle DirectionNodeStyle = new GUIStyle();

    private enum NodeType
    {
        Node,
        InNode,
        OutNode,
    }

    private NodeType m_NodeType;

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
            switch (m_NodeType)
            {
                case NodeType.Node:
                    {
                        Vector3 newPosition = Handles.PositionHandle(m_Selection.Position, Spline.transform.rotation);
                        if (newPosition != m_Selection.Position)
                        {
                            var delta = newPosition - m_Selection.Position;
                            m_Selection.InPoint += delta;
                            m_Selection.OutPoint += delta;
                            Spline.MoveNode(m_Selection, delta);
                        }
                    }
                    break;
                case NodeType.InNode:
                    {
                        m_Selection.InPoint = Handles.PositionHandle(m_Selection.InPoint, Quaternion.identity);
                        m_Selection.OutPoint = 2 * m_Selection.Position - m_Selection.InPoint;
                        Spline.MoveNode(m_Selection, Vector3.zero);
                    }
                    break;
                case NodeType.OutNode:
                    {
                        m_Selection.OutPoint = Handles.PositionHandle(m_Selection.OutPoint, Quaternion.identity);
                        m_Selection.InPoint = 2 * m_Selection.Position - m_Selection.OutPoint;
                        Spline.MoveNode(m_Selection, Vector3.zero);
                    }
                    break;
            }
        }

        Handles.BeginGUI();
        foreach (var node in Spline.Nodes)
        {
            if (NodeControlPoint(HandleUtility.WorldToGUIPoint(node.Position), new GUIContent(" " + Spline.Nodes.IndexOf(node).ToString()), m_Selection == node ? SelectedNodeStyle : NormalNodeStyle))
            {
                m_Selection = node;

                m_NodeType = NodeType.Node;
            }

            if (m_Selection == node)
            {
                Handles.color = new Color(1,1,0);
                Handles.DrawLine(HandleUtility.WorldToGUIPoint(node.InPoint), HandleUtility.WorldToGUIPoint(node.OutPoint));

                if (NodeControlPoint(HandleUtility.WorldToGUIPoint(m_Selection.InPoint), new GUIContent(), DirectionNodeStyle))
                {
                    m_NodeType = NodeType.InNode;
                }
                if (NodeControlPoint(HandleUtility.WorldToGUIPoint(m_Selection.OutPoint), new GUIContent(), DirectionNodeStyle))
                {
                    m_NodeType = NodeType.OutNode;
                }
            }
        }

        foreach (var curve in Spline.Curves)
        {
            var node = curve.Samples[curve.Samples.Count >> 1];
            if (NodeControlPoint(HandleUtility.WorldToGUIPoint(node.Position), new GUIContent(), VisualNodeStyle))
            {
                var scale = Vector3.Lerp(curve.node0.Scale, curve.node1.Scale, 0.5f);
                var direction = (curve.node1.InPoint - curve.node0.OutPoint).normalized;
                var newNode = new SplineNode() {  Position = node.Position, InPoint = node.Position - direction, OutPoint = node.Position + direction, Scale = scale };
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

        var t3 = new Texture2D(1, 1);
        t3.SetPixel(0, 0, new Color(1.0f, 1.0f, 0.0f, 1.0f));
        t3.Apply();
        DirectionNodeStyle = new GUIStyle();
        DirectionNodeStyle.normal.background = t3;
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
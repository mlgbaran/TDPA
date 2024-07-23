using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(IsoObjectSorterScript))]
public class IsoSortingEditorScriot : UnityEditor.Editor
{
    private void OnSceneGUI()
    {
        IsoObjectSorterScript line = (IsoObjectSorterScript)target;

        EditorGUI.BeginChangeCheck();

        float handleSizeA = HandleUtility.GetHandleSize(line.transform.position + (Vector3)line.pointA) * 0.1f;
        float handleSizeB = HandleUtility.GetHandleSize(line.transform.position + (Vector3)line.pointB) * 0.1f;

        Handles.color = Color.red;

        // Handles for pointA
        Vector3 handlePositionA = new Vector3(line.pointA.x, line.pointA.y, line.transform.position.z);
        var fmh_18_95_638573740332703772 = Quaternion.identity; Vector3 newPointA = Handles.FreeMoveHandle(line.transform.position + handlePositionA, handleSizeA / 2, Vector3.zero, Handles.DotHandleCap);

        // Handles for pointB
        Vector3 handlePositionB = new Vector3(line.pointB.x, line.pointB.y, line.transform.position.z);
        var fmh_22_95_638573740332730015 = Quaternion.identity; Vector3 newPointB = Handles.FreeMoveHandle(line.transform.position + handlePositionB, handleSizeB / 2, Vector3.zero, Handles.DotHandleCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Points");
            line.pointA = (Vector2)(newPointA - line.transform.position);
            line.pointB = (Vector2)(newPointB - line.transform.position);
            EditorUtility.SetDirty(line);
        }

        // Draw the line between points
        Handles.color = Color.red;
        Handles.DrawLine(line.transform.position + handlePositionA, line.transform.position + handlePositionB);
    }
}

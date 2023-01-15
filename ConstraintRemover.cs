using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class ConstraintRemover : EditorWindow
{
    private bool Fparent = false;
    private bool Fpositon = false;
    private bool Frotation = false;
    private bool Fscale = false;
    private bool Faim = false;
    private bool FlookAt = false;
    private bool Fgrouping = false;
    private Vector2 _scrollPosition = Vector2.zero;

    [MenuItem("Tools/ConstraintRemover")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(ConstraintRemover), false, "ConstraintRemover v1.0");
    }

    private GameObject targetObject = null;
    private class ConstraintInfo
    {
        public GameObject obj;
        public bool hasParentConstraint = false;
        public bool hasPositionConstraint = false;
        public bool hasRotationConstraint = false;
        public bool hasScaleConstraint = false;
        public bool hasAimConstraint = false;
        public bool hasLookAtConstraint = false;
        public bool hasConstraint = false;
        public bool isCheckedParent = false;
        public bool isCheckedPosition = false;
        public bool isCheckedRotation = false;
        public bool isCheckedScale = false;
        public bool isCheckedAim = false;
        public bool isCheckedLookAt = false;

        public ConstraintInfo(GameObject obj)
        {
            this.obj = obj;
            if (obj.GetComponent<ParentConstraint>() != null)
                hasConstraint = hasParentConstraint = true;
            if (obj.GetComponent<PositionConstraint>() != null)
                hasConstraint = hasPositionConstraint = true;
            if (obj.GetComponent<RotationConstraint>() != null)
                hasConstraint = hasRotationConstraint = true;
            if (obj.GetComponent<ScaleConstraint>() != null)
                hasConstraint = hasScaleConstraint = true;
            if (obj.GetComponent<AimConstraint>() != null)
                hasConstraint = hasAimConstraint = true;
            if (obj.GetComponent<LookAtConstraint>() != null)
                hasConstraint = hasLookAtConstraint = true;
        }

        public bool showCheck(bool Fparent, bool Fpositon, bool Frotation, bool Fscale, bool Faim, bool FlookAt)
        {
            if (Fparent && hasParentConstraint)
                return true;
            if (Fpositon && hasPositionConstraint)
                return true;
            if (Frotation && hasRotationConstraint)
                return true;
            if (Fscale && hasScaleConstraint)
                return true;
            if (Faim && hasAimConstraint)
                return true;
            if (FlookAt && hasLookAtConstraint)
                return true;
            return false;
        }

        public void normalize(bool Fparent, bool Fpositon, bool Frotation, bool Fscale, bool Faim, bool FlookAt)
        {
            if (!Fparent)
                isCheckedParent = false;
            if (!Fpositon)
                isCheckedPosition = false;
            if (!Frotation)
                isCheckedRotation = false;
            if (!Fscale)
                isCheckedScale = false;
            if (!Faim)
                isCheckedAim = false;
            if (!FlookAt)
                isCheckedLookAt = false;
        }
    }
    private List<ConstraintInfo> objects = new List<ConstraintInfo>();

    void OnGUI()
    {
        Fgrouping = EditorGUILayout.ToggleLeft("Grouping", Fgrouping);
        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            Fparent = EditorGUILayout.ToggleLeft("Parent Constraint", Fparent);
            if (GUILayout.Button("Select All"))
                selectAll("Parent", true);
            if (GUILayout.Button("UnSelect All"))
                selectAll("Parent", false);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            Fpositon = EditorGUILayout.ToggleLeft("Position Constraint", Fpositon);
            if (GUILayout.Button("Select All"))
                selectAll("Position", true);
            if (GUILayout.Button("UnSelect All"))
                selectAll("Position", false);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            Frotation = EditorGUILayout.ToggleLeft("Rotation Constraint", Frotation);
            if (GUILayout.Button("Select All"))
                selectAll("Rotation", true);
            if (GUILayout.Button("UnSelect All"))
                selectAll("Rotation", false);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            Fscale = EditorGUILayout.ToggleLeft("Scale Constraint", Fscale);
            if (GUILayout.Button("Select All"))
                selectAll("Scale", true);
            if (GUILayout.Button("UnSelect All"))
                selectAll("Scale", false);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            Faim = EditorGUILayout.ToggleLeft("Aim Constraint", Faim);
            if (GUILayout.Button("Select All"))
                selectAll("Aim", true);
            if (GUILayout.Button("UnSelect All"))
                selectAll("Aim", false);
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            FlookAt = EditorGUILayout.ToggleLeft("LookAt Constraint", FlookAt);
            if (GUILayout.Button("Select All"))
                selectAll("LookAt", true);
            if (GUILayout.Button("UnSelect All"))
                selectAll("LookAt", false);
        }
        EditorGUILayout.Space(10);
        targetObject = EditorGUILayout.ObjectField("Object", targetObject, typeof(GameObject), true) as GameObject;
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Listup") && targetObject != null)
            listup();

        writeLine(2f);

        foreach (var objInfo in objects)
            objInfo.normalize(Fparent, Fpositon, Frotation, Fscale, Faim, FlookAt);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        if (Fgrouping)
            viewG();
        else
            view();
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Delete") && objects.Count != 0)
        {
            foreach (var objInfo in objects)
            {
                if (objInfo.isCheckedParent)
                    GameObject.DestroyImmediate(objInfo.obj.GetComponent<ParentConstraint>());
                if (objInfo.isCheckedPosition)
                    GameObject.DestroyImmediate(objInfo.obj.GetComponent<PositionConstraint>());
                if (objInfo.isCheckedRotation)
                    GameObject.DestroyImmediate(objInfo.obj.GetComponent<RotationConstraint>());
                if (objInfo.isCheckedScale)
                    GameObject.DestroyImmediate(objInfo.obj.GetComponent<ScaleConstraint>());
                if (objInfo.isCheckedAim)
                    GameObject.DestroyImmediate(objInfo.obj.GetComponent<AimConstraint>());
                if (objInfo.isCheckedLookAt)
                    GameObject.DestroyImmediate(objInfo.obj.GetComponent<LookAtConstraint>());

            }
            listup();
        }
    }
    void viewG()
    {
        foreach (var objInfo in objects)
        {
            if (!objInfo.showCheck(Fparent, Fpositon, Frotation, Fscale, Faim, FlookAt))
                continue;
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                if (GUILayout.Button("Select All"))
                {
                    objInfo.isCheckedParent =
                        objInfo.isCheckedPosition =
                        objInfo.isCheckedRotation =
                        objInfo.isCheckedScale =
                        objInfo.isCheckedAim =
                        objInfo.isCheckedLookAt = true;
                    objInfo.normalize(Fparent, Fpositon, Frotation, Fscale, Faim, FlookAt);
                }
                if (GUILayout.Button("UnSelect All"))
                {
                    objInfo.isCheckedParent =
                        objInfo.isCheckedPosition =
                        objInfo.isCheckedRotation =
                        objInfo.isCheckedScale =
                        objInfo.isCheckedAim =
                        objInfo.isCheckedLookAt = false;
                    objInfo.normalize(Fparent, Fpositon, Frotation, Fscale, Faim, FlookAt);
                }
            }
            if (Fparent && objInfo.hasParentConstraint)
                objInfo.isCheckedParent = EditorGUILayout.ToggleLeft("Parent Constraint", objInfo.isCheckedParent);
            if (Fpositon && objInfo.hasPositionConstraint)
                objInfo.isCheckedPosition = EditorGUILayout.ToggleLeft("Position Constraint", objInfo.isCheckedPosition);
            if (Frotation && objInfo.hasRotationConstraint)
                objInfo.isCheckedRotation = EditorGUILayout.ToggleLeft("Rotation Constraint", objInfo.isCheckedRotation);
            if (Fscale && objInfo.hasScaleConstraint)
                objInfo.isCheckedScale = EditorGUILayout.ToggleLeft("Scale Constraint", objInfo.isCheckedScale);
            if (Faim && objInfo.hasAimConstraint)
                objInfo.isCheckedAim = EditorGUILayout.ToggleLeft("Aim Constraint", objInfo.isCheckedAim);
            if (FlookAt && objInfo.hasLookAtConstraint)
                objInfo.isCheckedLookAt = EditorGUILayout.ToggleLeft("LookAt Constraint", objInfo.isCheckedLookAt);
            EditorGUILayout.Space(10);
            writeLine(2f);
        }
    }
    void view()
    {
        if (Fparent)
        {
            EditorGUILayout.LabelField("Parent Constraint", EditorStyles.boldLabel);
            foreach (var objInfo in objects)
                if (objInfo.hasParentConstraint)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        objInfo.isCheckedParent = EditorGUILayout.ToggleLeft("", objInfo.isCheckedParent, GUILayout.MaxWidth(30f));
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                    }
        }
        if (Fpositon)
        {
            EditorGUILayout.LabelField("Position Constraint", EditorStyles.boldLabel);
            foreach (var objInfo in objects)
                if (objInfo.hasPositionConstraint)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        objInfo.isCheckedPosition = EditorGUILayout.ToggleLeft("", objInfo.isCheckedPosition, GUILayout.MaxWidth(30f));
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                    }
        }
        if (Frotation)
        {
            EditorGUILayout.LabelField("Rotation Constraint", EditorStyles.boldLabel);
            foreach (var objInfo in objects)
                if (objInfo.hasRotationConstraint)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        objInfo.isCheckedRotation = EditorGUILayout.ToggleLeft("", objInfo.isCheckedRotation, GUILayout.MaxWidth(30f));
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                    }
        }
        if (Fscale)
        {
            EditorGUILayout.LabelField("Scale Constraint", EditorStyles.boldLabel);
            foreach (var objInfo in objects)
                if (objInfo.hasScaleConstraint)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        objInfo.isCheckedScale = EditorGUILayout.ToggleLeft("", objInfo.isCheckedScale, GUILayout.MaxWidth(30f));
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                    }
        }
        if (Faim)
        {
            EditorGUILayout.LabelField("Aim Constraint", EditorStyles.boldLabel);
            foreach (var objInfo in objects)
                if (objInfo.hasAimConstraint)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        objInfo.isCheckedAim = EditorGUILayout.ToggleLeft("", objInfo.isCheckedAim, GUILayout.MaxWidth(30f));
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                    }
        }
        if (FlookAt)
        {
            EditorGUILayout.LabelField("LookAt Constraint", EditorStyles.boldLabel);
            foreach (var objInfo in objects)
                if (objInfo.hasLookAtConstraint)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        objInfo.isCheckedLookAt = EditorGUILayout.ToggleLeft("", objInfo.isCheckedLookAt, GUILayout.MaxWidth(30f));
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUILayout.ObjectField(objInfo.obj, typeof(GameObject), true);
                    }
        }
    }

    void listup()
    {
        objects.Clear();
        foreach (var obj in targetObject.GetComponentsInChildren<Transform>(true))
        {
            var info = new ConstraintInfo(obj.gameObject);
            if (info.hasConstraint)
                objects.Add(info);
        }
    }

    void writeLine(float height, Color color)
    {
        var colorBackup = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
        GUI.backgroundColor = colorBackup;
    }
    void writeLine(float height)
    {
        writeLine(height, Color.white);
    }

    void selectAll(string type, bool F)
    {
        foreach (var objInfo in objects)
        {
            if (type == "Parent")
                objInfo.isCheckedParent = F;
            if (type == "Position")
                objInfo.isCheckedPosition = F;
            if (type == "Rotation")
                objInfo.isCheckedRotation = F;
            if (type == "Scale")
                objInfo.isCheckedScale = F;
            if (type == "Aim")
                objInfo.isCheckedAim = F;
            if (type == "LookAt")
                objInfo.isCheckedLookAt = F;
        }
    }

}

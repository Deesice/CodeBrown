using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VariableLanguage)), CanEditMultipleObjects]
public class VariableLanguageEditor : Editor
{

    public SerializedProperty
        dataType,
        category,
        rus,
        eng,
        strRus,
        strEng,
        rusSound,
        engSound,
        strIt,
        strEs,
        strFr,
        strDe,
        strZh_TW,
        strZh_CN,
        strPt,
        strJa,
        strKo,
        strPl,
        strNl,
        strSv,
        strTr,
        strCs;

    void OnEnable()
    {
        // Setup the SerializedProperties
        dataType = serializedObject.FindProperty("dataType");
        category = serializedObject.FindProperty("category");
        rus = serializedObject.FindProperty("rus");
        eng = serializedObject.FindProperty("eng");
        strRus = serializedObject.FindProperty("strRus");
        strEng = serializedObject.FindProperty("strEng");
        rusSound = serializedObject.FindProperty("rusSound");
        engSound = serializedObject.FindProperty("engSound");
        strIt = serializedObject.FindProperty("strIt");
        strEs = serializedObject.FindProperty("strEs");
        strFr = serializedObject.FindProperty("strFr");
        strDe = serializedObject.FindProperty("strDe");
        strZh_TW = serializedObject.FindProperty("strZh_TW");
        strZh_CN = serializedObject.FindProperty("strZh_CN");
        strPt = serializedObject.FindProperty("strPt");
        strJa = serializedObject.FindProperty("strJa");
        strKo = serializedObject.FindProperty("strKo");
        strPl = serializedObject.FindProperty("strPl");
        strNl = serializedObject.FindProperty("strNl");
        strSv = serializedObject.FindProperty("strSv");
        strTr = serializedObject.FindProperty("strTr");
        strCs = serializedObject.FindProperty("strCs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(dataType);

        VariableLanguage.DataType st = (VariableLanguage.DataType)dataType.enumValueIndex;

        switch (st)
        {
            case VariableLanguage.DataType.Image:
                EditorGUILayout.PropertyField(rus, new GUIContent("rus"));
                EditorGUILayout.PropertyField(eng, new GUIContent("eng"));
                break;

            case VariableLanguage.DataType.Text:
                EditorGUILayout.PropertyField(strRus, new GUIContent("strRus"));
                EditorGUILayout.PropertyField(strEng, new GUIContent("strEng"));
                EditorGUILayout.PropertyField(strIt, new GUIContent("strIt"));
                EditorGUILayout.PropertyField(strEs, new GUIContent("strEs"));
                EditorGUILayout.PropertyField(strFr, new GUIContent("strFr"));
                EditorGUILayout.PropertyField(strDe, new GUIContent("strDe"));
                EditorGUILayout.PropertyField(strZh_TW, new GUIContent("strZh_TW"));
                EditorGUILayout.PropertyField(strZh_CN, new GUIContent("strZh_CN"));
                EditorGUILayout.PropertyField(strPt, new GUIContent("strPt"));
                EditorGUILayout.PropertyField(strJa, new GUIContent("strJa"));
                EditorGUILayout.PropertyField(strKo, new GUIContent("strKo"));
                EditorGUILayout.PropertyField(strPl, new GUIContent("strPl"));
                EditorGUILayout.PropertyField(strNl, new GUIContent("strNl"));
                EditorGUILayout.PropertyField(strSv, new GUIContent("strSv"));
                EditorGUILayout.PropertyField(strTr, new GUIContent("strTr"));
                EditorGUILayout.PropertyField(strCs, new GUIContent("strCs"));
                break;

            case VariableLanguage.DataType.Int:
                EditorGUILayout.PropertyField(category, new GUIContent("category"));
                break;

            case VariableLanguage.DataType.Sound:
                EditorGUILayout.PropertyField(rusSound, new GUIContent("rusSound"));
                EditorGUILayout.PropertyField(engSound, new GUIContent("engSound"));
                break;

        }


        serializedObject.ApplyModifiedProperties();
    }
}

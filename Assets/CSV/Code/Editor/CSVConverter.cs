using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CSVConverter
{
    static string CreateHeader(SerializedProperty element)
    {
        var fields = new List<string>();
        foreach (SerializedProperty property in element)
        {
            fields.Add(property.name);
        }
        return CSV.MakeCSVLine(fields);
    }

    static string CreateEntry(SerializedProperty element)
    {
        var values = new List<string>();
        foreach (SerializedProperty property in element)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    values.Add(property.stringValue);
                    break;
                case SerializedPropertyType.Float:
                    values.Add(property.floatValue.ToString());
                    break;
                case SerializedPropertyType.Integer:
                    values.Add(property.intValue.ToString());
                    break;
                case SerializedPropertyType.Boolean:
                    values.Add(property.boolValue.ToString());
                    break;
                case SerializedPropertyType.ObjectReference:
                    if (property.objectReferenceValue != null)
                    {
                        values.Add(property.objectReferenceValue.name);
                    }
                    else
                    {
                        values.Add("");
                    }
                    break;
                case SerializedPropertyType.Color:
                    Color color = property.colorValue;
                    values.Add(ColorUtility.ToHtmlStringRGBA(color));
                    break;
                case SerializedPropertyType.Enum:
                    int index = property.enumValueIndex;
                    values.Add(property.enumNames[index]);
                    break;
                default:
                    Debug.LogFormat(
                        "Unhandled property type '{0}'",
                        property.propertyType);
                    break;
            }
        }
        return CSV.MakeCSVLine(values);
    }

    public static List<string> ToCSV(SerializedProperty list)
    {
        List<string> csv = new List<string>();
        var element = list.GetArrayElementAtIndex(0);
        csv.Add(CreateHeader(element));
        for (int i = 0; i < list.arraySize; ++i)
        {
            element = list.GetArrayElementAtIndex(i);
            csv.Add(CreateEntry(element));
        }
        return csv;
    }

    static Object FindObjectFromPath(
        string path, 
        System.Type type,
        string name)
    {
        var objects = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (var asset in objects)
        {
            if (asset.GetType() == type && asset.name == name)
            {
                return asset;
            }
        }
        return null;
    }

    static Object FindObject(
        System.Reflection.FieldInfo type,
        string name)
    {
        string typeName = type.ToString();
        typeName = typeName.Replace("UnityEngine.", "");
        string [] guids = AssetDatabase.FindAssets(name);
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return FindObjectFromPath(path, type.FieldType, name);
        }
        Debug.LogErrorFormat(
            "{0} {1} not found",
            type,
            name);
        return null;
    }

    static void SetProperty(
        System.Type type,
        SerializedProperty element,
        string name,
        string value)
    {
        var property = element.FindPropertyRelative(name);
        Debug.AssertFormat(
            property != null,
            "property {0} not found",
            name);
        switch (property.propertyType)
        {
            case SerializedPropertyType.String:
                property.stringValue = value;
                break;
            case SerializedPropertyType.Float:
                float floatValue = 0.0f;
                if (float.TryParse(value, out floatValue))
                {
                    property.floatValue = floatValue;
                }
                else
                {
                    Debug.LogErrorFormat(
                        "value {0} is not a valid float",
                        value);
                }
                break;
            case SerializedPropertyType.Integer:
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    property.intValue = intValue;
                }
                else
                {
                    Debug.LogErrorFormat(
                        "value {0} is not a valid int",
                        value);
                }
                break;
            case SerializedPropertyType.Boolean:
                bool boolean = false;
                if (bool.TryParse(value, out boolean))
                {
                    property.boolValue = boolean;
                }
                else
                {
                    Debug.LogErrorFormat(
                        "value {0} is not a valid bool",
                        value);
                }
                break;
            case SerializedPropertyType.ObjectReference:
                var fieldType = type.GetField(property.name);
                property.objectReferenceValue = FindObject(fieldType, value);
                break;
            case SerializedPropertyType.Color:
                Color color = Color.white;
                value = string.Format("#{0}", value);
                if (ColorUtility.TryParseHtmlString(value, out color))
                {
                    property.colorValue = color;
                }
                break;
            case SerializedPropertyType.Enum:
                var enumNames = new List<string>(property.enumNames);
                Debug.AssertFormat(
                    enumNames.Contains(value),
                    "enum value {0} not found",
                    value);
                property.enumValueIndex = enumNames.IndexOf(value);
                break;
            default:
                Debug.LogFormat(
                    "Unhandled property type '{0}'",
                    property.propertyType);
                break;
        }
    }

    public static void FromCSV(List<string> csv, System.Type type, SerializedProperty list)
    {
        Debug.Assert(csv.Count > 0, "csv is empty");
        List<string> properties = CSV.SplitCSVLine(csv[0]);
        list.ClearArray();
        for (int i = 1; i < csv.Count; ++i)
        {
            List<string> values = CSV.SplitCSVLine(csv[i]);
            list.InsertArrayElementAtIndex(i - 1);
            var element = list.GetArrayElementAtIndex(i - 1);
            for (int j = 0; j < properties.Count; ++j)
            {
                SetProperty(type, element, properties[j], values[j]);
            }
        }
    }

    public static void ExportCSVFile(
        string filename,
        SerializedProperty list)
    {
        List<string> csv = ToCSV(list);
        using (StreamWriter file = File.CreateText(filename))
        {
            foreach (string line in csv)
            {
                file.WriteLine(line);
            }
        }
        AssetDatabase.Refresh();
    }

    public static void ExportCSV(
        string defaultName,
        SerializedProperty list)
    {
        string filename = EditorUtility.SaveFilePanelInProject(
            "Export CSV",
            defaultName,
            "csv",
            "enter a file name to export the data to");
        if (!string.IsNullOrEmpty(filename))
        {
            ExportCSVFile(filename, list);
        }
    }

    public static void ImportCSVFile<Type>(
        string filename,
        SerializedProperty list)
    {
        var csv = new List<string>();
        using (var reader = new StreamReader(filename))
        {
            string line;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    csv.Add(line);
                }
            }
            while (line != null);
        }
        CSVConverter.FromCSV(csv, typeof(Type), list);
        list.serializedObject.ApplyModifiedProperties();
    }

    public static void ImportCSV<Type>(string defaultFolder, SerializedProperty list)
    {
        string filename = EditorUtility.OpenFilePanelWithFilters(
            "Import CSV",
            defaultFolder,
            new string [] { "CSV files", "csv", "All files", "*" });
        if (!string.IsNullOrEmpty(filename))
        {
            ImportCSVFile<Type>(filename, list);
        }
    }
}

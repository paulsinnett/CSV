using System.Collections;
using System.Collections.Generic;
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
                    var objRef = property.objectReferenceValue;
                    var path = AssetDatabase.GetAssetPath(objRef);
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    if (objRef.GetType() == typeof(Sprite))
                    {
                        values.Add(
                            string.Format(
                                "{0}.{1}",
                                guid,
                                objRef.name));
                    }
                    else
                    {
                        values.Add(objRef.name);
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

    static void SetProperty(SerializedProperty element, string name, string value)
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
                break;
            case SerializedPropertyType.Color:
                break;
            case SerializedPropertyType.Enum:
                break;
            default:
                Debug.LogFormat(
                    "Unhandled property type '{0}'",
                    property.propertyType);
                break;
        }
    }

    public static void FromCSV(List<string> csv, SerializedProperty list)
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
                SetProperty(element, properties[j], values[j]);
            }
        }
    }
}
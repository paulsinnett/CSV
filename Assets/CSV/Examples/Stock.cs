using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenuAttribute(menuName = "Create Stock List")]
public class Stock : ScriptableObject
{
    public List<ShopItem> stock;

    [ContextMenu("Export To CSV")]
    public void ExportToCSV()
    {
        var serializedObject = new SerializedObject(this);
        var csv = CSVConverter.ToCSV(serializedObject.FindProperty("stock"));

        foreach (var line in csv)
        {
            Debug.Log(line);
        }
    }
}

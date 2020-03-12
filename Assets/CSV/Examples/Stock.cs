using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenuAttribute(menuName = "Create Stock List")]
public class Stock : ScriptableObject
{
    public List<ShopItem> stock;

#if UNITY_EDITOR
    [ContextMenu("Export to CSV")]
    public void ExportToCSV()
    {
        var serializedObject = new SerializedObject(this);
        var list = serializedObject.FindProperty("stock");
        CSVConverter.ExportCSV("stock", list);
    }

    [ContextMenu("Import from CSV")]
    public void ImportFromCSV()
    {
        var serializedObject = new SerializedObject(this);
        var list = serializedObject.FindProperty("stock");
        CSVConverter.ImportCSV<ShopItem>("stock", list);
    }
#endif
}

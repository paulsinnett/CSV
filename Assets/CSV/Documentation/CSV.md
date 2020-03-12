# CSV object import and export

This package contains code and examples for import and exporting data from a serialize-able list of any flat data structure. (A flat data structure has no nested data. Nested data wouldn't map onto the `CSV` format very well.)

## Installation

Import the package contents. For the functionality you only need to include to `Code` sub-folder. The `Documentation` and `Examples` are optional.

## Quick start export to CSV

If you have a ScriptableObject containing a `List` of a serialize-able data structure you can write that list out to `CSV` by adding an export function marked as `[ContextMenu("ExportFunctionName")]`. For example, given the class:

```csharp
[System.Serializable]
public class ShopItem
{
    public string name;
    public int price;
    public Sprite image;
}
```

And assuming you have a `ScriptableObject` containing a list of such items:

```csharp
[CreateAssetMenuAttribute(menuName = "Create Stock List")]
public class Stock : ScriptableObject
{
    public List<ShopItem> stock;
}
```

You can write out a `CSV` with a list of these entries by adding the following export function to your  `ScriptableObject` class:

```csharp
#if UNITY_EDITOR
    [ContextMenu("Export to CSV")]
    public void ExportToCSV()
    {
        var serializedObject = new SerializedObject(this);
        var list = serializedObject.FindProperty("stock");
        CSVConverter.ExportCSV("StockCSV", list);
    }
#endif
```

This will add `Export to CSV` as a context menu item to your `ScriptableObject` instance in Unity. To select it, first select the `ScriptableObject` instance in your `Project` view. Then right click at the top of the `Inspector` window or click the small cog icon at the top right.

Clicking on the `Export to CSV` option will open a file selector for you to choose where to save your `CSV` file. The resulting file might look something like this:

```csv
name,price,image
cookie,50,CookieIcon
apple,30,AppleIcon
fish,500,FishIcon
steak,1000,SteakIcon
```

## Quick start import from CSV

For this you will need an instance of your `ScriptableObject`. The result of the import will overwrite any existing content so be careful when choosing your object. Assuming a similar structure to the example above, you can add an `Import from CSV` options to your context menu by adding the following import function:

```csharp
#if UNITY_EDITOR
    [ContextMenu("Import from CSV")]
    public void ImportFromCSV()
    {
        var serializedObject = new SerializedObject(this);
        var list = serializedObject.FindProperty("stock");
        CSVConverter.ImportCSV<ShopItem>("NewStockCSV", list);
    }
#endif
```

To select the menu option, as before, first select the `ScriptableObject` in the `Project` view.  Then right click at the top of the `Inspector` window or click the small cog icon at the top right.

Clicking on the `Import from CSV` option will open a file selector to choose the `CSV` file containing the new data you want to write into your object. The file should automatically import and replace the existing contents.

## Import / Export to filename

You can provide your own file selection system and call the `CSV` import and export options directly.

```csharp
public static void ExportCSVFile(string filename, SerializedProperty list);
```

- `filename` path to the destination CSV file including extension
- `list` the list you want to export

```csharp
public static void ImportCSVFile<Type>(string filename, SerializedProperty list);
```

- `Type` the type of the data entries in the list, e.g. `ShopItem` in the example above
- `filename` path to the source CSV file including extension
- `list` the list you want to export

## Mapping CSV values to Unity objects

The mapping of the `CSV` values to Unity objects is automatic using the following conventions:

- the field names in the structure map to the `CSV` header line
- `string` these values are literally copied
- `int` these values are converted from number strings in the `CSV` to Unity `int` fields
- `float` these values are converted from number strings in the `CSV` to Unity `float` fields
- `bool` these values are converted from the strings `"True"` and `"False"` in the `CSV` to Unity `bool` fields
- `Color` these values are converted from `HTML` hex codes `RGBA` in the `CSV` to Unity `Color` values
- `Enum` these values are converted from strings matching the `enum` type definition in the Unity script to the actual Unity `enum` field.
- `Object` any Unity class based on `UnityEngine.Object` is converted from a string representing the object's name in the `CSV` to the first object that the Unity `AssetDatabase` can find matching the name and type of the object. For this reason, your `Object` references should have unique names to avoid any mismatches.

## Examples

The example scene, `Shop` displays a list of shop items based on an extended example `ShopItem` class. The `Store` class in this example has already been extended with the import and export context menu options.
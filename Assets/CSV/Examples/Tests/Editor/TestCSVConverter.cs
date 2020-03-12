using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestCSVExport
    {
        [Test]
        public void TestCSVExportSimple()
        {
            Stock stockAsset = Resources.Load<Stock>("TestExportStock");
            var serializedObject = new SerializedObject(stockAsset);
            var stock = serializedObject.FindProperty("stock");
            List<string> csv = CSVConverter.ToCSV(stock);
            Assert.AreEqual(2, csv.Count);
            Assert.AreEqual("name,price,image,colour,available,category,material,sound", csv[0]);
            Assert.AreEqual("cookie,50,Cookie,FF000000,True,Baked,Special,Funny", csv[1]);
            Resources.UnloadAsset(stockAsset);
        }
    }

    public class TestCSVImport
    {
        [Test]
        public void TestCSVImportSimple()
        {
            Stock stockAsset = Resources.Load<Stock>("TestImportStock");
            var serializedObject = new SerializedObject(stockAsset);
            var stock = serializedObject.FindProperty("stock");
            stock.ClearArray();
            serializedObject.ApplyModifiedProperties();
            Assert.AreEqual(0, stockAsset.stock.Count);
            TextAsset file = Resources.Load<TextAsset>("TestStockCSV");
            var csv = new List<string>();
            using (var reader = new StringReader(file.text))
            {
                string line = null;
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
            CSVConverter.FromCSV(csv, typeof(ShopItem), stock);
            serializedObject.ApplyModifiedProperties();
            Assert.AreEqual(5, stockAsset.stock.Count);
            Assert.AreEqual("cookie", stockAsset.stock[0].name);
            Assert.AreEqual(50, stockAsset.stock[0].price);
            Assert.NotNull(stockAsset.stock[0].image);
            Assert.AreEqual("Cookie", stockAsset.stock[0].image.name);
            Assert.AreEqual(Color.red, stockAsset.stock[0].colour);
            Assert.IsTrue(stockAsset.stock[0].available);
            Assert.AreEqual(
                ShopItem.Category.Baked,
                stockAsset.stock[0].category);
            Assert.NotNull(stockAsset.stock[0].material);
            Assert.AreEqual("Special", stockAsset.stock[0].material.name);
            Assert.NotNull(stockAsset.stock[0].sound);
            Assert.AreEqual("Funny", stockAsset.stock[0].sound.name);
            Assert.AreEqual("apple", stockAsset.stock[1].name);
            Assert.AreEqual("chicken leg", stockAsset.stock[2].name);
            Assert.AreEqual("fish", stockAsset.stock[3].name);
            Assert.AreEqual("steak", stockAsset.stock[4].name);
            Resources.UnloadAsset(stockAsset);
            Resources.UnloadAsset(file);
        }
    }
}

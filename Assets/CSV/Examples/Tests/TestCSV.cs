using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestCSV
    {
        string line;
        List<string> entries;

        // A Test behaves as an ordinary method
        [Test]
        public void TestCSVSimpleLine()
        {
            line = "abc,def,ghi";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(3, entries.Count);
            Assert.AreEqual("abc", entries[0]);
            Assert.AreEqual("def", entries[1]);
            Assert.AreEqual("ghi", entries[2]);
        }

        [Test]
        public void TestCSVLineWithQuotes()
        {
            line = "abc,\"def,ghi\",jkl";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(3, entries.Count);
            Assert.AreEqual("abc", entries[0]);
            Assert.AreEqual("def,ghi", entries[1]);
            Assert.AreEqual("jkl", entries[2]);
        }

        [Test]
        public void TestCSVLineWithQuotedQuotes()
        {
            line = "abc,\"\"\"def,ghi\"\"\",jkl";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(3, entries.Count);
            Assert.AreEqual("abc", entries[0]);
            Assert.AreEqual("\"def,ghi\"", entries[1]);
            Assert.AreEqual("jkl", entries[2]);
        }

        [Test]
        public void TestCSVSplitLine()
        {
            line = @"abc,def
ghi,jkl";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(3, entries.Count);
            Assert.AreEqual("abc", entries[0]);
            Assert.AreEqual("def\nghi", entries[1]);
            Assert.AreEqual("jkl", entries[2]);
        }

        [Test]
        public void TestStringBuilder()
        {
            StringBuilder builder = new StringBuilder("line1");
            builder.AppendLine();
            builder.Append("line2");
            Assert.AreEqual(1, builder.ToString().Count('\n'));
        }

        [Test]
        public void TestCSVStringReader()
        {
            StringReader reader = new StringReader("abc,\"def\nghi\",jkl");
            line = CSV.GetCSVLine(reader);
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(3, entries.Count);
            Assert.AreEqual("abc", entries[0]);
            Assert.AreEqual("def\nghi", entries[1]);
            Assert.AreEqual("jkl", entries[2]);
        }

        [Test]
        public void TestCSVMakeLine()
        {
            entries = new List<string>();
            entries.Add("abc");
            entries.Add("def\nghi");
            entries.Add("jkl");
            line = CSV.MakeCSVLine(entries);
            Assert.AreEqual("abc,\"def\nghi\",jkl", line);
        }
    }
}

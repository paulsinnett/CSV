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
            Assert.AreEqual(entries.Count, 3);
            Assert.AreEqual(entries[0], "abc");
            Assert.AreEqual(entries[1], "def");
            Assert.AreEqual(entries[2], "ghi");
        }

        [Test]
        public void TestCSVLineWithQuotes()
        {
            line = "abc,\"def,ghi\",jkl";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(entries.Count, 3);
            Assert.AreEqual(entries[0], "abc");
            Assert.AreEqual(entries[1], "def,ghi");
            Assert.AreEqual(entries[2], "jkl");
        }

        [Test]
        public void TestCSVLineWithQuotedQuotes()
        {
            line = "abc,\"\"\"def,ghi\"\"\",jkl";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(entries.Count, 3);
            Assert.AreEqual(entries[0], "abc");
            Assert.AreEqual(entries[1], "\"def,ghi\"");
            Assert.AreEqual(entries[2], "jkl");
        }

        [Test]
        public void TestCSVSplitLine()
        {
            line = @"abc,def
ghi,jkl";
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(entries.Count, 3);
            Assert.AreEqual(entries[0], "abc");
            Assert.AreEqual(entries[1], "def\nghi");
            Assert.AreEqual(entries[2], "jkl");
        }

        [Test]
        public void TestStringBuilder()
        {
            StringBuilder builder = new StringBuilder("line1");
            builder.AppendLine();
            builder.Append("line2");
            Assert.AreEqual(builder.ToString().Count('\n'), 1);
        }

        [Test]
        public void TestCSVStringReader()
        {
            StringReader reader = new StringReader("abc,\"def\nghi\",jkl");
            line = CSV.GetCSVLine(reader);
            entries = CSV.SplitCSVLine(line);
            Assert.AreEqual(entries.Count, 3);
            Assert.AreEqual(entries[0], "abc");
            Assert.AreEqual(entries[1], "def\nghi");
            Assert.AreEqual(entries[2], "jkl");
        }

        [Test]
        public void TestCSVMakeLine()
        {
            entries = new List<string>();
            entries.Add("abc");
            entries.Add("def\nghi");
            entries.Add("jkl");
            line = CSV.MakeCSVLine(entries);
            Assert.AreEqual(line, "abc,\"def\nghi\",jkl");
        }
    }
}

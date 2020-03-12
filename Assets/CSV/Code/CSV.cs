using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEditor;
using System.IO;
using System.Text;

public static class CSV
{
   	static string DosLineEnd = "\x0d\x0a";
	static string UnixLineEnd = "\x0a";

    public static int Count(this string line, char match)
    {
        int count = 0;
        foreach (char character in line)
        {
            if (character == match)
            {
                count++;
            }
        }
        return count;
    }

    public static bool IsEven(this int value)
    {
        return (value % 2) == 0;
    }

    static string TrimQuotes(string entry)
    {
        // trim any whitespace
        entry = entry.Trim();

        // if begins with a quote
        if (entry.Length > 0 && entry[0] == '"')
        {
            // and ends with a quote
            if (entry[entry.Length - 1] == '"')
            {
                // remove trailing quotes
                entry = entry.Substring(1, entry.Length - 2);

                // quoted quotes
                if (entry.Contains("\"\""))
                {
                    // de-double the quotes
                    entry = entry.Replace("\"\"", "\"");
                }
            }
            else
            {
                // quote mismatch
                Debug.LogWarningFormat("Quote mismatch in entry '{0}'", entry);
            }
        }

        return entry;
    }

    static string ReplaceDosLineEndings(string entry)
    {
		// Windows to Unix
		if (entry.Contains(DosLineEnd))
		{
			entry = entry.Replace(DosLineEnd, UnixLineEnd);
		}
        return entry;
    }

    static string ReplaceLineBreaks(string entry)
    {
        if (entry.Contains(DosLineEnd))
        {
            // escape Dos line breaks
            entry = entry.Replace(DosLineEnd, "\\n");
        }
        if (entry.Contains(UnixLineEnd))
        {
            // escape Unix line breaks
            entry = entry.Replace(UnixLineEnd, "\\n");
        }
        entry = entry.Replace("\\n", UnixLineEnd);
		return entry;
    }

    public static bool IsCompleteCSVLine(string line)
    {
        return line.Count('"').IsEven();
    }

    public static string GetCSVLine(this TextReader reader)
    {
        StringBuilder line = null;

        bool reading = true;
        while (reading)
        {
            string next = reader.ReadLine();
            if (next == null)
            {
                reading = false;
            }
            else
            {
                if (line == null)
                {
                    line = new StringBuilder(next);
                }
                else
                {
                    line.AppendLine();
                    line.Append(next);
                }

                if (IsCompleteCSVLine(line.ToString()))
                {
                    reading = false;
                }
            }
        }

        return (line != null)? line.ToString() : null;
    }

    public static List<string> SplitCSVLine(string line)
    {
        string[] commaSeparated = line.Split(new char [] { ',' });
        List<string> entries = new List<string>();
        string entry = string.Empty;
        foreach (string segment in commaSeparated)
        {
            entry += segment;
            if (entry.Count('"').IsEven())
            {
                entries.Add(ReplaceLineBreaks(TrimQuotes(entry)));
                entry = string.Empty;
            }
            else
            {
                // re-add the trimmed comma
                entry += ",";
            }
        }

        if (entry != string.Empty)
        {
            Debug.LogWarningFormat("Found trailing text '{0}' while parsing CSV", entry);
        }

        return entries;
    }

    static string QuoteOutput(string entry)
    {
        // replace single quotes with double quotes
        entry = entry.Replace("\"", "\"\"");
        if (entry.Contains(",") || entry.Contains("\"") || entry.Contains("\n"))
        {
            // quote the entry
            entry = string.Format("\"{0}\"", entry);
        }
        entry = ReplaceDosLineEndings(entry);
        return entry;
    }

    public static string MakeCSVLine(List<string> entries)
    {
        string line = string.Empty;
        bool first = true;

        foreach (string entry in entries)
        {
            if (!first)
            {
                line += ",";
            }
            line += QuoteOutput(entry);
            first = false;
        }

        return line;
    }
}

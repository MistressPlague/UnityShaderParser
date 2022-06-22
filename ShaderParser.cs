using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using UnityEngine;

public class ShaderParser
{
    public List<string> RawContents = null;

    public readonly List<string> Comments = null;

    public string Name = null;

    private List<string> _Properties = null;
    public List<string> Properties
    {
        get => _Properties;
        set
        {
            _Properties = value;

            // Remove Old
            var PropInfo = GetContentWithinBraces(RawContents, FragFunctionStartLine);

            RawContents.RemoveRange(PropInfo.Item2 + 1, PropInfo.Item1.Count);

            // Add New
            RawContents.InsertRange(PropInfo.Item2 + 1, _FragFunction);

            GetOffsets();
        }
    }

    private List<string> _SubShader = null;
    public List<string> SubShader
    {
        get => _SubShader;
        set
        {
            _SubShader = value;

            // Remove Old
            var PropInfo = GetContentWithinBraces(RawContents, FragFunctionStartLine);

            RawContents.RemoveRange(PropInfo.Item2 + 1, PropInfo.Item1.Count);

            // Add New
            RawContents.InsertRange(PropInfo.Item2 + 1, _FragFunction);

            GetOffsets();
        }
    }

    private List<string> _VertFunction = null;
    public List<string> VertFunction
    {
        get => _VertFunction;
        set
        {
            _VertFunction = value;

            // Remove Old
            var PropInfo = GetContentWithinBraces(RawContents, FragFunctionStartLine);

            RawContents.RemoveRange(PropInfo.Item2 + 1, PropInfo.Item1.Count);

            // Add New
            RawContents.InsertRange(PropInfo.Item2 + 1, _FragFunction);

            GetOffsets();
        }
    }

    private List<string> _FragFunction = null;
    public List<string> FragFunction
    {
        get => _FragFunction;
        set
        {
            _FragFunction = value;

            // Remove Old
            var PropInfo = GetContentWithinBraces(RawContents, FragFunctionStartLine);

            RawContents.RemoveRange(PropInfo.Item2 + 1, PropInfo.Item1.Count);

            // Add New
            RawContents.InsertRange(PropInfo.Item2 + 1, _FragFunction);

            GetOffsets();
        }
    }

    private int PropertiesStartLine;
    private int SubShaderStartLine;
    private int VertFunctionStartLine;
    private int FragFunctionStartLine;
    private string PathToFile;
    public ShaderParser(string PathToShaderFile)
    {
        PathToFile = PathToShaderFile;

        RawContents = File.ReadAllLines(PathToShaderFile).ToList();

        Comments = RawContents.Where(o => o.StartsWith("\\\\") || o.StartsWith("//")).ToList();

        var NameLine = RawContents.First(o => o.StartsWith("Shader \""));

        Name = NameLine.Substring(8, NameLine.Length - 11);

        GetOffsets();

        _Properties = GetContentWithinBraces(RawContents, PropertiesStartLine).Item1;

        _SubShader = GetContentWithinBraces(RawContents, SubShaderStartLine).Item1;

        _VertFunction = GetContentWithinBraces(RawContents, VertFunctionStartLine).Item1;

        _FragFunction = GetContentWithinBraces(RawContents, FragFunctionStartLine).Item1;
    }

    private void GetOffsets()
    {
        PropertiesStartLine = RawContents.FindIndex(o => RemoveSpacesAndTabs(o).StartsWith("Properties"));
        SubShaderStartLine = RawContents.FindIndex(o => RemoveSpacesAndTabs(o).StartsWith("SubShader"));
        VertFunctionStartLine = RawContents.FindIndex(o => o.Contains(" vert (") || o.Contains(" vert("));
        FragFunctionStartLine = RawContents.FindIndex(o => o.Contains(" frag (") || o.Contains(" frag("));
    }


    public void SaveChanges()
    {
        if (File.Exists(PathToFile + "_Backup"))
        {
            File.Delete(PathToFile + "_Backup");
        }

        File.Move(PathToFile, PathToFile + "_Backup");
        File.WriteAllLines(PathToFile, RawContents);
    }

    public string RemoveSpacesAndTabs(string text) => text.Replace(" ", "").Replace("		", "");

    // List, StartBrace, EndBrace
    public (List<string>, int, int) GetContentWithinBraces(List<string> Lines, int MethodLineIndex)
    {
        var Output = new List<string>();

        var Unclosed = 0;

        var StartBrace = MethodLineIndex;

        var EndBrace = Lines.Count - 1;

        var IsJSSyntax = Lines[MethodLineIndex].Contains("{");

        Unclosed++;

        if (IsJSSyntax)
        {
            for (var i = MethodLineIndex + 1; i < Lines.Count - 1; i++)
            {
                var lineData = Lines[i];

                if (lineData.Contains("{") && !lineData.Contains("}"))
                {
                    Unclosed++;
                }
                else if (!lineData.Contains("{") && lineData.Contains("}"))
                {
                    if (Unclosed == 1)
                    {
                        EndBrace = i;
                        break;
                    }

                    Unclosed--;
                }

                Output.Add(lineData);
            }
        }
        else
        {
            StartBrace++;

            for (var i = MethodLineIndex + 2; i < Lines.Count - 1; i++)
            {
                var lineData = Lines[i];

                if (lineData.Contains("{") && !lineData.Contains("}"))
                {
                    Unclosed++;
                }
                else if (!lineData.Contains("{") && lineData.Contains("}"))
                {
                    if (Unclosed == 1)
                    {
                        EndBrace = i;
                        break;
                    }

                    Unclosed--;
                }

                Output.Add(lineData);
            }
        }

        Debug.Log($"StartBrace: {StartBrace + 1}, EndBrace: {EndBrace + 1}");

        return (Output, StartBrace, EndBrace);
    }
}

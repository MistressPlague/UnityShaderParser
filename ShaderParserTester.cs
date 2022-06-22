#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShaderParserTester : Editor
{
    [MenuItem("Plague/Test ShaderParser")]
    private static void Test()
    {
        var parser = new ShaderParser("F:\\Users\\Poppy\\Documents\\Prev\\HSB\\Avatars 3.0 - Dynamics\\Assets\\RealFeel\\RealFeel Shader\\RealFeel.shader");

        if (parser.Comments != null)
            Debug.Log(string.Join("\r\n", parser.Comments));

        Debug.Log(string.Join("\r\n", parser.Name));
        Debug.Log(string.Join("\r\n", parser.Properties));
        Debug.Log(string.Join("\r\n", parser.SubShader));

        if (parser.FragFunction != null)
            Debug.Log(string.Join("\r\n", parser.FragFunction));
        if (parser.VertFunction != null)
            Debug.Log(string.Join("\r\n", parser.VertFunction));

        var Temp = parser.FragFunction.ToList();
        Temp.Insert(3, "				// This Is A Test!");
        parser.FragFunction = Temp;

        Debug.Log(string.Join("\r\n", parser.RawContents));
    }
}
#endif

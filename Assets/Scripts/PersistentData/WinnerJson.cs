using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WinnerJson : MonoBehaviour
{

    
    public static void WriteString(string fileName ,string textToWrite, bool append)
    {
        string _path = $"{Application.dataPath}/{fileName}.txt";
        StreamWriter writer = new StreamWriter(_path, append);
        
        writer.Write(textToWrite+":");
        writer.Close();
    }

    public static string[] ReadString(string fileName)
    {
        string _path = $"{Application.dataPath}/{fileName}.txt";
        StreamReader reader = new StreamReader(_path);
        string text;
        text = reader.ReadToEnd();
        string[] parsedText = text.Split(":");
        reader.Close();

        return parsedText;
    }

}

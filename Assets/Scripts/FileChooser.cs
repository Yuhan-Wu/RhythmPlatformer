using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FileChooser : MonoBehaviour
{
    public InputField FilePathTextArea;

    public void ShowFilePanel()
    {
        string path = EditorUtility.OpenFilePanel("Choose a song", "", "mp3, wav");
        FilePathTextArea.text = path;
    }
}

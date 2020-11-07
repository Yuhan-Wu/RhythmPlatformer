using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using System.IO;

public class FileChooser : MonoBehaviour
{
    public InputField FilePathTextArea;

    public void ShowFilePanel()
    {
        string path = EditorUtility.OpenFilePanel("Please choose a song", "", "mp3, wav");
        FilePathTextArea.text = path;
    }
}

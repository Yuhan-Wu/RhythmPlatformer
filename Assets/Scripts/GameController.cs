﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public FileChooser Chooser;

    public AudioSource AudioPlayer;
    private AudioClip AudioSample;

    private Generator MapGenerator;

    private void Start()
    {
        MapGenerator = GetComponent<Generator>();
    }

    public void LoadFile()
    {
        if(Chooser.FilePathTextArea.text != "")
        {
            string[] paths = Chooser.FilePathTextArea.text.Split('/');
            AudioSample = Resources.Load<AudioClip>(paths[paths.Length - 1].Split('.')[0]); ;
            if (AudioSample)
            {
                AudioPlayer.clip = AudioSample;
                AudioProcesser processer = new AudioProcesser(AudioSample);
                List<SpectralFluxInfo> infos = processer.Process().FindAll(x => x.isPeak == true);
                MapGenerator.Infos = infos;
                MapGenerator.Clip = AudioSample;
                Chooser.gameObject.SetActive(false);
                LoadMap();
            }
        }
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public void Restart(string p_Scene)
    {
        SceneManager.LoadScene(p_Scene, LoadSceneMode.Single);
    }

    public void Reload()
    {
        MapGenerator.Clean();
        LoadMap();
    }

    private void LoadMap()
    {
        MapGenerator.Generate();
        AudioPlayer.Play();
    }
}

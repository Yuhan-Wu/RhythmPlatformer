using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                MapGenerator.Generate();
                AudioPlayer.Play();
            }
        }
    }
}

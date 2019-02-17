using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SequenciaTutorial : MonoBehaviour
{
    public AudioSource radio;
    public AudioClip[] sonsTuto;
    public Image showTutorial;
    public Sprite[] spritesTutorial;

    private void ConfigAudio(int i)
    {
        radio.clip = sonsTuto[i];
        radio.Play();
    }
}

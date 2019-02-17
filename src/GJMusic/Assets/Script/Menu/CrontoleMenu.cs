using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrontoleMenu : MonoBehaviour
{
    public AudioSource tocador;

    public void IniciarJogo()
    {
        SceneManager.LoadScene(1);       
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TocarInstrucao()
    {
        tocador.Play();
    }
}

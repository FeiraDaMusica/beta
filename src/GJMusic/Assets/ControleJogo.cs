using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControleJogo : MonoBehaviour
{
    // Preparação do jogo - Tempo pro jogador saber que vai começar ( explicar como joga o jogo se for a primeira vez)
    // Ao começar, instrumentos estão todos equalizados. Interagir com os instrumentos, vai modificar a maneira
    // Objetivo é agradar o público mantendo a orquestra em <Harmonia>

    public enum FlowJogo { Prep, Durante, Fim };
    public FlowJogo tipoJogo;
    public float vida, vidaMaxima;
    public float tempoTotalJogo = 30;
    public float tempoEntreInt = 5;
    public int toqueTela = 5;
    public int tempoSegurandoTela = 2;

    [Header("Configuração")]
    public List<AudioSource> instrumentosAudio = new List<AudioSource>();
    public List<AudioClip> instrumentosSom = new List<AudioClip>();
    public Image vidaHud;
    public List<ListasMaestro> comentarios = new List<ListasMaestro>();

    Coroutine rotinaDesafino;
    bool ganhouJogo;

    [Header("Condição dos instrumentos")]
    public List<bool> volumeAlto = new List<bool>(5);
    public List<bool> volumeBaixo = new List<bool>(5);
    public List<bool> pitchAlterado = new List<bool>(5);
    public List<bool> parouTocar = new List<bool>(5);
    public List<int> qtdVoltarTocar = new List<int>(5);
    public List<bool> instDesafinado = new List<bool>(5);
    public List<Animator> musicos = new List<Animator>();
    public List<Animator> instrumentoVisual = new List<Animator>();

    [Header("Config inicial")]
    AudioSource somPrincipal;
    public AudioClip iniciarAudio;
    public List<ListaParticulas> part = new List<ListaParticulas>();

    void Start()
    {
        somPrincipal = GetComponent<AudioSource>();
        StartCoroutine(IniciarJogo());
        divisorTela[0] = Screen.width / (musicos.Count);
        IniciarJogoPreparar();
        for (int i = 1; i < 3; i++)
        {
            divisorTela[i] = divisorTela[0] * (i+1);
        }
    }

    void Update()
    {
        if (tipoJogo == FlowJogo.Durante)
        {
            ModificarVidaPlayer();
            AtualizacaoVida();
            if (vida <= 0)
                FinalizarJogo(false);

            VerificarToques();
        }    
        
        else if (tipoJogo == FlowJogo.Fim)
        {
            FinalizarJogo(true);
        }
    }

    // Método para receber o input de celular
    // Ao receber o input, verificar qual instrumento mais próximo (Input phase) 

    /// <summary>
    /// Engajamento do cliente, sebraecanvas.com
    /// </summary>

    IEnumerator IniciarJogo()
    {
        yield return new WaitForSeconds(3);
        tipoJogo = FlowJogo.Durante;
        rotinaDesafino = StartCoroutine(GeradorDeProblemas());
        IniciarMusicaGame();
    }

    IEnumerator GeradorDeProblemas()
    {
        while (tipoJogo == FlowJogo.Durante)
        {
            yield return new WaitForSeconds(tempoEntreInt);
            int i = GerarNumRandom();
            int f = UnityEngine.Random.Range(0, 3);
            if (i >= 0)
                ProblemasInstrumentos(i, f, true);
        }
    }

    private int GerarNumRandom()
    {
        int ran = UnityEngine.Random.Range(0, 4);
        if (instDesafinado[ran])
            return ran = -1;

        return ran;
    }

    private void ModificarVidaPlayer()
    {
        int qtdDesafinado = 0;
        for (int i = 0; i < instDesafinado.Count; i++)
        {
            if (instDesafinado[i])
                qtdDesafinado++;
        }
        if (qtdDesafinado == 0)
            vida += 1f;
        else
            vida -= qtdDesafinado;

        vida = Mathf.Clamp(vida, 0, vidaMaxima);
    }

    private void FinalizarJogo(bool fim)
    {
        
    }

    private void ProblemasInstrumentos(int qualInst, int qualPro, bool gerarPro)
    {
        switch (qualPro) {
            case (0):
                parouTocar[qualInst] = gerarPro;
                break;
            case (1):
                volumeAlto[qualInst] = gerarPro;
                break;
            case (2):
                volumeBaixo[qualInst] = gerarPro;
                break;
            case (3):
                pitchAlterado[qualInst] = gerarPro;
                break;
        }

       instDesafinado[qualInst] = gerarPro;

        if (gerarPro)
        {
            somPrincipal.clip = comentarios[qualInst].comentarioInstrumento[qualPro];
            somPrincipal.Play();
        }

        LigarParticulaAdequada(qualInst, qualPro, gerarPro);
        EfeitosProblemas(qualInst, qualPro, gerarPro);
    }
    
    private void EfeitosProblemas(int qualInst, int qualPro, bool gerarPro)
    {
        switch (qualPro) {
            case (0):
                instrumentosAudio[qualInst].mute = gerarPro;
                musicos[qualInst].SetTrigger((gerarPro ? "parou" : "normal"));
                instrumentoVisual[qualInst].SetTrigger((gerarPro ? "parou" : "normal"));
                break;
            case (1):
                instrumentosAudio[qualInst].volume = (gerarPro ? 1f : 0.25f);
                musicos[qualInst].SetTrigger((gerarPro ? "ruim" : "normal"));
                instrumentoVisual[qualInst].SetTrigger((gerarPro ? "ruim" : "normal"));
                break;
            case (2):
                instrumentosAudio[qualInst].volume = (gerarPro ? 0.1f : 0.25f); 
                musicos[qualInst].SetTrigger((gerarPro ? "ruim" : "normal"));
                instrumentoVisual[qualInst].SetTrigger((gerarPro ? "ruim" : "normal"));
                break;
            case (3):
                instrumentosAudio[qualInst].pitch = (gerarPro ? -1 : 1);
                musicos[qualInst].SetTrigger((gerarPro ? "ruim" : "normal"));
                instrumentoVisual[qualInst].SetTrigger((gerarPro ? "ruim" : "normal"));
                break;
        }
    }

    #region Controle Inputs 
    
    [Header("Inputs touch")]
    public int qualInstrumento;
    public int[] divisorTela = new int[4];
    public Vector2 posInicial;
    public int swipeDir;
    public float duracaoAfinar;
    
    private void VerificarToques()
    {
        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            IniciarTouch(toque);
           // IdentificarInstrumentoProximidade(toque.position);
            IdentificarInstrumentoParticao(posInicial.x);
            SwipeGesture(toque);
            HoldGesture(toque);
            AcordarMusico(toque);
        }
    }

    private void IdentificarInstrumentoParticao(float pos)
    {
        if (pos <= divisorTela[0])
            qualInstrumento = 0;
        else if (pos >= divisorTela[2])
            qualInstrumento = 3;
        else if (pos > divisorTela[0] && pos < divisorTela[1])
            qualInstrumento = 1;
        else if (pos > divisorTela[1] && pos < divisorTela[2])
            qualInstrumento = 2;

    }

    private void IniciarTouch(Touch toque)
    {
        if (toque.phase == TouchPhase.Began)
        {
            posInicial = toque.position;
        }
    }

    private void AcordarMusico(Touch toque)
    {
        if (toque.phase == TouchPhase.Began && parouTocar[qualInstrumento])
        {
            qtdVoltarTocar[qualInstrumento]++;
            if (qtdVoltarTocar[qualInstrumento] >= toqueTela)
            {
                qtdVoltarTocar[qualInstrumento] = 0;
                ProblemasInstrumentos(qualInstrumento, 0, false);
            }
        }
    }

    private void SwipeGesture(Touch toque)
    {
        if (toque.phase == TouchPhase.Moved)
        {
            Vector2 dir = toque.position - posInicial;

            if (dir.y < 0)
                swipeDir = 1;
            else if (dir.y > 0)
                swipeDir = 2;
            
            if ((swipeDir == 1 && volumeAlto[qualInstrumento]) || (swipeDir == 2 && volumeBaixo[qualInstrumento]))
                ProblemasInstrumentos(qualInstrumento,swipeDir, false);
        }
    }

    private void HoldGesture(Touch toq)
    {
        if (toq.phase == TouchPhase.Stationary)
        {
            duracaoAfinar += Time.deltaTime;
            if (duracaoAfinar > tempoSegurandoTela)
            {
                if (pitchAlterado[qualInstrumento])
                    ProblemasInstrumentos(qualInstrumento, 3, false);
            }
        }

        if (toq.phase == TouchPhase.Ended)
        {
            duracaoAfinar = 0;
        }
    }

    [Header("Time Lapse")]
    // public Text timerText;
    public AudioSource timeMusic;
    private float secondsCount;
    public float almostOver = 0.98f;
    public Image timeMusicBar;
    private Color m_MyColor;

    public void CountMusicTime()
    {
        var aux = ( (secondsCount - timeMusic.time)/ secondsCount);
        timeMusicBar.fillAmount = aux;

        if(aux < almostOver)
        {
            m_MyColor = Color.red;
            timeMusicBar.color = m_MyColor;
        }

        Debug.Log(aux);
    }

    #endregion

    #region Visuais
    public void AtualizacaoVida()
    {
        vidaHud.fillAmount = vida / vidaMaxima;
    }

    private void LigarParticulaAdequada(int qualInst, int qualPro, bool ativar)
    {
        if (ativar)
        {
            GameObject temp = part[qualInst].particulas[qualPro];
            temp.SetActive(ativar);
            temp.transform.position = musicos[qualInst].gameObject.transform.position + new Vector3(0,0.25f ,0);
        }
        else
            foreach (GameObject go in part[qualInst].particulas)
            {
                if (go.activeSelf)
                    go.SetActive(false);
            }
    }

    #endregion

    #region Controle de audio durante jogo
    private void IniciarJogoPreparar()
    {
        somPrincipal.clip = iniciarAudio;
        somPrincipal.Play();
    }

    private void IniciarMusicaGame()
    {
        for (int i = 0; i < musicos.Count; i++)
        {
            instrumentosAudio[i].Play();
        }
    }
    #endregion
}

[Serializable]
public class ListasMaestro
{
    public List<AudioClip> comentarioInstrumento = new List<AudioClip>();
}

[Serializable]
public class ListaParticulas {
    public List<GameObject> particulas = new List<GameObject>();
}
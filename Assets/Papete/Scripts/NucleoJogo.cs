using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class NucleoJogo : MonoBehaviour
{
    public enum TipoDeVerificacao
    {
        Vertical,
        Horizontal,
        Completa
    }
    private class Relatorio
    {
        private bool peEsquerdo;
        private List<Vector2> angulo;
        private List<Papete.Movimento> desejada;
        private List<Papete.Movimento> obtida;
        public Relatorio(bool peEsquerdo, int tamanhoEsperado)
        {
            this.peEsquerdo= peEsquerdo;
            angulo = new List<Vector2>();
            desejada = new List<Papete.Movimento>();
            obtida= new List<Papete.Movimento>();
        }

        public void Adicionar(Vector2 angulo, Papete.Movimento desejada, Papete.Movimento obtida)
        {
            this.angulo.Add(angulo);
            this.desejada.Add(desejada);
            this.obtida.Add(obtida);
        }

        public string ToCSV()
        {
            string s = "Frontal;Lateral;obtida;desejada;" + (peEsquerdo ? "E" : "D");
            for (int i = 0; i < angulo.Count; i++)
                s += "\n" + angulo[i].x.ToString("n2") + ";" + angulo[i].y.ToString("n2") + ";" + Papete.NomesInterno[(int)obtida[i]] + ";" + Papete.NomesInterno[(int)desejada[i]];
            return s;
        }
    }
    
    public Papete papete;
    public UnityEvent<float> EventoFimDeJogo;
    public float intervaloMedicaoRelatorio = 0.5f;
    public GameObject calibPrefab;

    public bool exibirUI = true;

    private bool peEsquerdo;
    private float tempoDeInicio;

    private uint momentoSequenciaAtual = 0;
    private float marco = 0f;
    private float pontuacao = 0f;
    private bool jogando = false;
    private float ultimaMedicaoRelatorio;
    private Relatorio relatorio;
    private Papete.Movimento[] sequenciaMovimento;
    private uint[] sequenciaTempo;

    private Action<int> EventoFimCalibracaoPadrao;


    private uint tempoTotal;

    private Vector2 posVisualJogador = new Vector2();
    private float velocidadeVisualJogador = 10f;

    #region métodos públicos
    public void IniciarExercicio(string fonte)
    {
        string[] linhas = fonte.Split('\n');
        sequenciaMovimento = new Papete.Movimento[linhas.Length];
        sequenciaTempo = new uint[linhas.Length];
        relatorio = new Relatorio(peEsquerdo, linhas.Length);

        tempoTotal= 0;

        uint lidos = 0;
        for (int i = 0; i < linhas.Length; i++)
        {
            string[] palavras = linhas[i].Split(';');
            if (palavras.Length > 1)
            {
                if (Papete.TryParsePos(palavras[0], out Papete.Movimento pos) && uint.TryParse(palavras[1],out uint tempo)){
                    sequenciaMovimento[lidos] = pos;
                    sequenciaTempo[lidos] = tempo;
                    tempoTotal += tempo;
                    lidos++;
                }

            }
        }
        this.peEsquerdo = papete.EhPeEsq();
        tempoDeInicio = Time.time;
        ultimaMedicaoRelatorio = tempoDeInicio;
        momentoSequenciaAtual = 0;
        marco = 0f;
        pontuacao = 0f;
        jogando = true;
        string buf = "Iniciando exercicio:\n";
        foreach (var item in sequenciaMovimento)
            buf += item + " ";
        print(buf);
    }
    /*
     Considerando as posicoes como uma cruz:
       E
     F R D
       I
    Em qualquer momento, acertar a posição vale 1.0; um pro lado, 0.5; uma diagonal, 0.25; o oposto, 0.0
     */
    public float ObterPontuacaoInstantanea()
    {
        Papete.Movimento prev = papete.ObterMovimento();
        if (sequenciaMovimento[momentoSequenciaAtual] == prev)
            return 1f;

        if (sequenciaMovimento[momentoSequenciaAtual] == Papete.Movimento.Dorsiflexao)
        {
            if (prev == Papete.Movimento.Repouso)
                return 0.5f;
            if (prev == Papete.Movimento.Flexao)
                return 0f;
        }
        if (sequenciaMovimento[momentoSequenciaAtual] == Papete.Movimento.Flexao)
        {
            if (prev == Papete.Movimento.Repouso)
                return 0.5f;
            if (prev == Papete.Movimento.Dorsiflexao)
                return 0f;
        }
        if (sequenciaMovimento[momentoSequenciaAtual] == Papete.Movimento.Eversao)
        {
            if (prev == Papete.Movimento.Repouso)
                return 0.5f;
            if (prev == Papete.Movimento.Inversao)
                return 0f;
        }
        if (sequenciaMovimento[momentoSequenciaAtual] == Papete.Movimento.Inversao)
        {
            if (prev == Papete.Movimento.Repouso)
                return 0.5f;
            if (prev == Papete.Movimento.Eversao)
                return 0f;
        }
        return 0.25f;
    }
    
    public Papete.Movimento ObterMovimentoDesejadaEm(float progresso)
    {
        uint busca = (uint)(progresso * tempoTotal);
        uint somatorio = 0;
        for (int i = 0; i < sequenciaTempo.Length; i++)
        {
            somatorio += sequenciaTempo[i];
            if (somatorio >= busca)
            {
                return sequenciaMovimento[i];
            }
        }
        return Papete.Movimento.Repouso;
    }

    public float ObterPontuacaoAtual()
    {
        return pontuacao;
    }

    public Papete.Movimento ObterMovimentoDesejadaAgora()
    {
        if (jogando)
            return sequenciaMovimento[momentoSequenciaAtual];
        return Papete.Movimento.Repouso;
    }

    public Papete.Movimento ObterMovimentoJogador()
    {
        return papete.ObterMovimento();
    }
    public string ObterRelatorio()
    {
        if (relatorio == null)
            return "";
        return relatorio.ToCSV();
    }
    public Vector2 ObterPosVisualJogador() {
        return posVisualJogador;
    }
    public uint ObterTempoTotal()
    {
        return tempoTotal;
    }
    public void AcionarColetaPadrao(Action<int> eventoFim, TipoDeVerificacao verif)
    {
        EventoFimCalibracaoPadrao = eventoFim;
        papete.Zerar();
        MenuCalibrador MC = Instantiate(calibPrefab).GetComponentInChildren<MenuCalibrador>();
        MC.funcaoFinalizar = RemoverObjetoCalibracaoPadrao;
        MC.papete = papete;
        MC.MudarTipoDeVerificacao(verif);
    }

    private void RemoverObjetoCalibracaoPadrao(int resultado)
    {
        EventoFimCalibracaoPadrao.Invoke(resultado);
        if (FindObjectOfType<MenuCalibrador>())
        {
            Destroy(FindObjectOfType<MenuCalibrador>().transform.root.gameObject);
        }
    }

    #endregion
    #region UnityMessages

    private void Start()
    {
        NucleoJogo[] objs = FindObjectsOfType<NucleoJogo>();
        foreach (var item in FindObjectsOfType<NucleoJogo>())
        {
            if (item != this)
                Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        papete = new();
        papete.ModoConexaoImediata(true);
        //IniciarExercicio(File.ReadAllText(Application.streamingAssetsPath + "\\Niveis\\vai_e_volta.txt"), FindObjectOfType<Papete>().PeEsquerdoEhMaisRecente());
    }
    void Update()
    {
        if (jogando)
        {
            if (momentoSequenciaAtual < sequenciaMovimento.Length-1 )
            {
                if(Time.time - tempoDeInicio > marco + sequenciaTempo[momentoSequenciaAtual])
                    marco += sequenciaTempo[momentoSequenciaAtual++];
                pontuacao += Time.deltaTime * ObterPontuacaoInstantanea();
            
                if(Time.time - ultimaMedicaoRelatorio > intervaloMedicaoRelatorio)
                {
                    Papete.Movimento atual = ObterMovimentoJogador();
                    relatorio.Adicionar(papete.ObterRotacaoV2(), ObterMovimentoDesejadaAgora(), atual);
                    ultimaMedicaoRelatorio = Time.time;
                }
                posVisualJogador = Vector2.Lerp(posVisualJogador, Papete.ObterMovimentoCruz(papete.ObterMovimento(),peEsquerdo), Time.deltaTime * velocidadeVisualJogador);
            }
            //bloco fim de jogo
            else
            {
                jogando= false;
                print("acabou o jogo");
                EventoFimDeJogo.Invoke(pontuacao);
            }
        }
    }
    private void OnGUI()
    {
        if(exibirUI)
        {
            // Set the text style
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.alignment = TextAnchor.LowerRight;
            textStyle.fontSize = 11;

            List<string> conexoes = papete.ListarConexoesAtuais();

            for (int i = 0; i < conexoes.Count; i++)
            {
                // Set the position and size of the text label
                Rect labelRect = new Rect(Screen.width - 150, Screen.height - 30, 150, 30);

                // Draw the text label
                GUI.Label(labelRect, conexoes[i], textStyle);
            }
        }
    }
    #endregion
}

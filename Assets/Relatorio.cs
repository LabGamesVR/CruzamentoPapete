using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Relatorio: MonoBehaviour
{
    public GameObject UIparent;
    public RectTransform corpoGrafico;
    public GameObject pontoGraficoPrefab;
    public GameObject linhaGraficoPrefab;

    public void Zerar()
    {
        momentosLinhaDoTempoPapete= new();
        posicoesPapete= new();
        momentosLinhaDoTempoExercicio = new();
        posicoesExercicio= new();
    }

    List<float> momentosLinhaDoTempoPapete = new List<float>();
    List<int> posicoesPapete = new List<int>();

    int ultimoMovimento = -1;

    public void RegistrarMovimento(int movimento)
    {
        ultimoMovimento = movimento;
        momentosLinhaDoTempoPapete.Add(Time.time);
        posicoesPapete.Add(movimento);
    }



    List<float> momentosLinhaDoTempoExercicio = new List<float>();
    List<int> posicoesExercicio = new List<int>();

    public void RegistrarMovimentoExercicio(int movimento)
    {
        momentosLinhaDoTempoExercicio.Add(Time.time);
        posicoesExercicio.Add(movimento);
    }

    public static unsafe bool IsNaN(float f)
    {
        int binary = *(int*)(&f);
        return ((binary & 0x7F800000) == 0x7F800000) && ((binary & 0x007FFFFF) != 0);
    }

    public void ExibirGrafico(string jogador)
    {
        if (momentosLinhaDoTempoExercicio.Count > 0 && momentosLinhaDoTempoPapete.Count > 0)
        {
            List<float> delays = new();
            int indexPapete = 0;
            float sumDelay = 0.0f;
            float sumSqrdDelay = 0.0f;
            for (int i = 0; i < posicoesExercicio.Count; i++)
            {
                while (posicoesPapete[indexPapete] != posicoesExercicio[i])
                    indexPapete++;
                delays.Add( momentosLinhaDoTempoPapete[indexPapete] - momentosLinhaDoTempoExercicio[i]);
                sumDelay += momentosLinhaDoTempoPapete[indexPapete] - momentosLinhaDoTempoExercicio[i];
                sumSqrdDelay += Mathf.Pow(momentosLinhaDoTempoExercicio[i] - momentosLinhaDoTempoPapete[indexPapete], 2);
            }
            float mediaDelay = sumDelay / posicoesExercicio.Count;
            float desvioPadraoDelay = Mathf.Sqrt(sumSqrdDelay / posicoesExercicio.Count);
            print("Média do delay: " + mediaDelay);
            print("DP do delay: " + desvioPadraoDelay);




            float primeiroMomento = Mathf.Min(momentosLinhaDoTempoPapete[0], momentosLinhaDoTempoExercicio[0]);
            float ultimoMomento = Mathf.Max(momentosLinhaDoTempoPapete[^1], momentosLinhaDoTempoExercicio[^1]);
            print(primeiroMomento + " - " + ultimoMomento);
            
            float larguraBase = corpoGrafico.rect.width - pontoGraficoPrefab.GetComponent<RectTransform>().rect.width;
            float alturaBase = corpoGrafico.rect.height- pontoGraficoPrefab.GetComponent<RectTransform>().rect.height;
            float proporcao = alturaBase/larguraBase;
            for (int i = 0; i < momentosLinhaDoTempoExercicio.Count; i++)
            {
                float x = Mathf.InverseLerp(primeiroMomento, ultimoMomento, momentosLinhaDoTempoExercicio[i]) - 1f;
                float y = posicoesExercicio[i] * 0.33f - 1f;
                GameObject ponto = Instantiate(pontoGraficoPrefab, corpoGrafico);
                ponto.GetComponent<Image>().color = Color.blue;
                RectTransform rt = ponto.GetComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.anchoredPosition = new Vector3(x * larguraBase, y *alturaBase, 0);

                if (i > 0)
                {
                    float xAnterior = Mathf.InverseLerp(primeiroMomento, ultimoMomento, momentosLinhaDoTempoExercicio[i - 1]) - 1f;
                    float yAnterior = posicoesExercicio[i - 1] * 0.33f - 1f;

                    float novoX = (xAnterior + x) * 0.5f;
                    float novoY = (yAnterior + y) * 0.5f;

                    float angulo = Mathf.Atan((y-novoY) * proporcao / ( x- novoX));
                    if (!IsNaN(angulo))
                    {
                        GameObject linha = Instantiate(linhaGraficoPrefab, corpoGrafico);
                        linha.GetComponent<Image>().color = Color.blue;
                        rt = linha.GetComponent<RectTransform>();
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.anchorMin = new Vector2(1, 1);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.anchoredPosition = new Vector3(novoX * larguraBase, novoY * alturaBase, 0);

                        float tamanho = 2 * Vector2.Distance(new Vector2( x * larguraBase, y*alturaBase), new Vector2(novoX * larguraBase, novoY*alturaBase));
                        rt.sizeDelta = new Vector2( tamanho, 5);
                        rt.rotation = Quaternion.Euler(0f,0f, angulo * 180f / Mathf.PI);
                        //muda pro inicio da hierarquia para ficar atras
                        rt.SetSiblingIndex(0);
                    }
                }
            }
            for (int i = 0; i < momentosLinhaDoTempoPapete.Count; i++)
            {
                float x = Mathf.InverseLerp(primeiroMomento, ultimoMomento, momentosLinhaDoTempoPapete[i]) - 1f;
                float y = posicoesPapete[i] * 0.33f - 1f + 0.01f;
                GameObject ponto = Instantiate(pontoGraficoPrefab, corpoGrafico);
                ponto.GetComponent<Image>().color = Color.red;
                RectTransform rt = ponto.GetComponent<RectTransform>();
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.anchoredPosition = new Vector3(x * larguraBase, y * alturaBase, 0);

                if (i > 0)
                {
                    float xAnterior = Mathf.InverseLerp(primeiroMomento, ultimoMomento, momentosLinhaDoTempoPapete[i - 1]) - 1f;
                    float yAnterior = posicoesPapete[i - 1] * 0.33f - 1f;

                    float novoX = (xAnterior + x) * 0.5f;
                    float novoY = (yAnterior + y) * 0.5f;

                    float angulo = Mathf.Atan((y - novoY) * proporcao / (x - novoX));
                    if (!IsNaN(angulo))
                    {
                        GameObject linha = Instantiate(linhaGraficoPrefab, corpoGrafico);
                        linha.GetComponent<Image>().color = Color.red;
                        rt = linha.GetComponent<RectTransform>();
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.anchorMin = new Vector2(1, 1);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.anchoredPosition = new Vector3(novoX * larguraBase, novoY * alturaBase, 0);

                        float tamanho = 2 * Vector2.Distance(new Vector2(x * larguraBase, y * alturaBase), new Vector2(novoX * larguraBase, novoY * alturaBase));
                        rt.sizeDelta = new Vector2(tamanho, 5);
                        rt.rotation = Quaternion.Euler(0f, 0f, angulo * 180f / Mathf.PI);

                        //muda pro inicio da hierarquia para ficar atras
                        rt.SetSiblingIndex(0);
                    }
                }
            }


            UIparent.SetActive(true);
        }
    }

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            momentosLinhaDoTempoExercicio.Add(Random.Range(0f, 100f));
            momentosLinhaDoTempoPapete.Add(Random.Range(0f, 100f));
            momentosLinhaDoTempoPapete.Sort();
            momentosLinhaDoTempoExercicio.Sort();

            posicoesExercicio.Add(Random.Range(0, 4));
            posicoesPapete.Add(Random.Range(0, 4));
        }

    }

}

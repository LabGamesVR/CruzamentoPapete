using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuCalibrador : MonoBehaviour
{
    public Animator anim;
    public Button[] botoes;
    public GameObject loadingScreen;
    public TMP_Text legendaLoadingScreen;

    public Action<int> funcaoFinalizar;

    private int[] quantidades = new int[] {0,0,0,0,0};
    private Stack<int> historicoMovimento = new();
    public Papete papete;
    

    private int dots = 0;
    private float lastDotTime = 0f;
    private float dotTimeInterval = 0.25f;
    private int direcaoDots = 1;

    private int QuantidadeTotal()
    {
        return quantidades[0] + quantidades[1] + quantidades[2] + quantidades[3] + quantidades[4];
    }
    private void ReescreverBotao(int id)
    {
        string seta = "·";
        switch (id)
        {
            case 0:
                seta = "↓";
                break;
            case 1:
                seta = "↑";
                break;
            case 3:
                seta = "→←";
                break;
            case 4:
                seta = "←→";
                break;
        }
        botoes[id].GetComponentInChildren<TMP_Text>().text = "<b>" + seta + "</b>\n" + Papete.NomesExterno[id] + "\n" + quantidades[id];
    }

    public void OnButtonHover(int id)
    {
        foreach (var nome in Papete.NomesInterno)
        {
            anim.SetBool(nome,false);
        }

        anim.SetBool(Papete.NomesInterno[id], true);
    }

    public void OnButtonClick(int id)
    {
        int qtd_original = QuantidadeTotal();
        if (papete.Registrar((Papete.Movimento)id) > qtd_original)
        {
            quantidades[id]++;
            historicoMovimento.Push(id);
            ReescreverBotao(id);
        }
    }

    public void CancelarCalibracao()
    {
        funcaoFinalizar.Invoke(0);
    }
    public void BotaoCalibrar()
    {
        loadingScreen.SetActive(true);
        papete.Retreinar(OnCalibracaoCompleta);
    }
    private void OnCalibracaoCompleta(int resultado)
    {
        funcaoFinalizar.Invoke(resultado);
    }

    public void Desfazer()
    {
        if(historicoMovimento.Count > 0)
        {
            int id = historicoMovimento.Pop();
            if (quantidades[id] > 0)
                quantidades[id]--;
            ReescreverBotao(id);
            papete.DesfazerUltimaColeta();
        }
    }
    public void MudarTipoDeVerificacao(NucleoJogo.TipoDeVerificacao verif)
    {
        for (int i = 0; i < 5; i++)
            botoes[i].gameObject.SetActive(false);
        
        if (verif == NucleoJogo.TipoDeVerificacao.Vertical || verif == NucleoJogo.TipoDeVerificacao.Completa)
        {
            botoes[0].gameObject.SetActive(true);
            botoes[1].gameObject.SetActive(true);
        }
        botoes[2].gameObject.SetActive(true);
        if (verif == NucleoJogo.TipoDeVerificacao.Horizontal || verif == NucleoJogo.TipoDeVerificacao.Completa)
        {
            botoes[3].gameObject.SetActive(true);
            botoes[4].gameObject.SetActive(true);
        }
    }


    private void Update()
    {
        if(Time.time - lastDotTime > dotTimeInterval)
        {
            lastDotTime = Time.time;
            dots+=direcaoDots;
            if (dots > 4 || dots <= 0)
                direcaoDots = -direcaoDots;

            legendaLoadingScreen.text = "Calibrando";
            for (int i = 0; i < dots; i++)
                legendaLoadingScreen.text += ".";
        }
    }

}

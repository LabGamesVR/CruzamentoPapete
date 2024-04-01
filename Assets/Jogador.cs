using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jogador : MonoBehaviour
{
    public GUIStyle labelStyle;

    float targetDirection = 0f;
    float rotSpeed = 30f;

    Animator animator;
    private GerenciadorCarros gerenciadorCarros;
    public Papete papete;
    
    private Papete.Movimento ultimaPos= Papete.Movimento.Repouso;
    private Papete.Movimento posAtual = Papete.Movimento.Repouso;
    private float inicioLeitura = 0f;
    private float minTempoLeitura = 0.1f;

    private float momentoMudancaPrioridade;
    private int ultimaPrioridade = -1;
    private float tempoMinReacaoAuto = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();   
        gerenciadorCarros = FindObjectOfType<GerenciadorCarros>();
        papete = new();
        papete.ModoConexaoImediata(true);
    }

    void rodarPara(int dir)
    {
        targetDirection = 180f + dir * -90f;
        animator.SetTrigger("jumpTrigger");
        gerenciadorCarros.Direcionar(dir);
    }
    void Update()
    {
        if(gerenciadorCarros.auto)
        {
            if(gerenciadorCarros.prioridade!=ultimaPrioridade)
            {
                ultimaPrioridade = gerenciadorCarros.prioridade;
                momentoMudancaPrioridade = Time.time;
            }
            else if(Time.time - momentoMudancaPrioridade > tempoMinReacaoAuto && gerenciadorCarros.prioridade!=-1)
                rodarPara(gerenciadorCarros.prioridade);
        }
        else
        {
            Papete.Movimento posNova = papete.ObterMovimento();
            if (posNova != posAtual)
            {
                posAtual = posNova;
                inicioLeitura = Time.time;
            }
            else
            {
                if(ultimaPos!=posAtual && Time.time - inicioLeitura > minTempoLeitura)
                {
                    ultimaPos = posAtual;
                    if(posNova == Papete.Movimento.Flexao)
                        rodarPara(0);
                    else if (posNova == Papete.Movimento.Dorsiflexao)
                        rodarPara(2);
                    else if(posNova == Papete.Movimento.Eversao)
                    {
                        if(papete.EhPeEsq()) rodarPara(3);
                        else rodarPara(1);
                    }
                    else if (posNova == Papete.Movimento.Inversao)
                    {
                        if (papete.EhPeEsq()) rodarPara(1);
                        else rodarPara(3);
                    }
                }
            }

            // fallback de controles no teclado
            if (Input.GetKeyDown(KeyCode.UpArrow))
                rodarPara(0);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                rodarPara(1);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                rodarPara(2);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                rodarPara(3);
        }

        Vector3 rot = transform.rotation.eulerAngles;
        rot.y = Mathf.LerpAngle(rot.y, targetDirection, Time.deltaTime*rotSpeed);
        transform.rotation = Quaternion.Euler(rot);
    }

    void OnGUI()
    {
        List<string> conexoesAtuais = papete.ListarConexoesAtuais();
        int i = 0;
        foreach (var item in conexoesAtuais)
        {
            
            // Set the color of the text to white
            GUI.color = Color.white;

            // Calculate the position of the label
            float labelWidth = labelStyle.CalcSize(new GUIContent(item)).x;
            float labelHeight = labelStyle.CalcSize(new GUIContent(item)).y;
            float xPosition = Screen.width - labelWidth - 10; // Adjust 10 for padding
            float yPosition = Screen.height - labelHeight - 10 - 10*i; // Adjust 10 for padding

            // Draw the label
            GUI.Label(new Rect(xPosition, yPosition, labelWidth, labelHeight), item, labelStyle);
            i++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TituloController : MonoBehaviour
{
    private Animator animator;
    private Relatorio relatorio;
    private InputJogador nomeJogadorInputField;
    private void Start()
    {
        animator = GetComponent<Animator>();
        relatorio = FindObjectOfType<Relatorio>();
        nomeJogadorInputField = FindObjectOfType<InputJogador>();
    }

    public void IniciarJogo()
    {
        GerenciadorCarros gerenciador = FindObjectOfType<GerenciadorCarros>();
        if (gerenciador.auto)
        {
            animator.Play("FadeOut");
            gerenciador.IniciarJogo();
        }
    }

    public void VoltarAoMenu()
    {
        animator.Play("FadeIn");
        relatorio.ExibirGrafico(nomeJogadorInputField.valor());
    }

    public void MostrarTutorial()
    {
        Tutorial[] t = Resources.FindObjectsOfTypeAll<Tutorial>();
        foreach (Tutorial t2 in t)
        {
            print(t2);
            t2.gameObject.SetActive(true);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TituloController : MonoBehaviour
{
    private Animator animator;
    private Relatorio relatorio;
    public TMP_InputField nomeJogadorInputField;
    private void Start()
    {
        animator = GetComponent<Animator>();
        relatorio = FindObjectOfType<Relatorio>();
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
        relatorio.ExibirGrafico(nomeJogadorInputField.text);
    }
}

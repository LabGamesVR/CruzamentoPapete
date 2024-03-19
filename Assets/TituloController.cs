using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TituloController : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void IniciarJogo()
    {
        animator.Play("FadeOut");
        FindObjectOfType<GerenciadorCarros>().IniciarJogo();
    }

    public void VoltarAoMenu()
    {
        animator.Play("FadeIn");

    }
}

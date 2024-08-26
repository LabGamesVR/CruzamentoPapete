using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TituloController : MonoBehaviour
{
    private Animator animator;
    public Button botaoInicio;
    private void Start()
    {
        animator = GetComponent<Animator>();
        botaoInicio.onClick.AddListener(IniciarJogo);
    }

    public void IniciarJogo()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.IniciarExercicio();
        animator.Play("FadeOut");
    }

    public void VoltarAoMenu()
    {
        animator.Play("FadeIn");
    }
    public void LiberarControleInicio(){
        botaoInicio.interactable = true;
    }

    public void MostrarTutorial()
    {
        Tutorial[] t = Resources.FindObjectsOfTypeAll<Tutorial>();
        foreach (Tutorial t2 in t)
        {
            t2.gameObject.SetActive(true);
        }
    }
}

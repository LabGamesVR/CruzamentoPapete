//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carro : MonoBehaviour
{
    public Color[] coresPossiveis;
    public Renderer[] carroceria;

    float maxVelocidade = 10f;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();

        Color corEscolhida = coresPossiveis[Random.Range(0, coresPossiveis.Length)];
        foreach(var parte in carroceria)
        {
            parte.material.color = corEscolhida;
        }
    }
    public void Seguir(Vector3 posInicial)
    {
        transform.parent = null;
        StartCoroutine(SeguirCoroutine(posInicial));
    }

    IEnumerator SeguirCoroutine(Vector3 posInicial)
    {
        while(transform.position != posInicial)
        {
            transform.position = Vector3.MoveTowards(transform.position, posInicial,maxVelocidade*Time.deltaTime);
            yield return 0;
        }
        animator.SetTrigger("Esquerda");
    }

    public void Destruir()
    {
        Destroy(this.gameObject);
    }
}

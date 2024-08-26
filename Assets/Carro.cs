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
    public Carro CarroDaFrente;
    public bool EstaEmPrimeiro = false, emMovimentoLivre = false, comPrioridade = false;
    public float espacamento, velocidade;
    Transform posPrioridade;
    void Update(){
        if(this.CarroDaFrente&&!emMovimentoLivre){
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, 
                comPrioridade?posPrioridade.localPosition:
                EstaEmPrimeiro?Vector3.zero:(CarroDaFrente.transform.localPosition + new Vector3(0f,0f,espacamento)),
                Time.deltaTime * velocidade);
        }
    }
    public void Instruir(Carro daFrente, float espacamento, float velocidade, bool ehPrimeiro, Transform posPrioridade){
        CarroDaFrente = daFrente;
        this.espacamento = espacamento;
        this.velocidade = velocidade;
        EstaEmPrimeiro = ehPrimeiro;
        this.posPrioridade = posPrioridade;
        transform.localPosition = EstaEmPrimeiro||daFrente==null?Vector3.zero:CarroDaFrente.transform.localPosition + new Vector3(0f,0f,espacamento);
    }
    public bool Priorizar(){
        if(!EstaEmPrimeiro)
            return false;
        comPrioridade = true;
        return true;
    }
    public bool Seguir(){
        if(!EstaEmPrimeiro)
            return false;
        
        StartCoroutine(SeguirCoroutine());
        EstaEmPrimeiro = false;
        transform.SetAsLastSibling();
        transform.parent.GetChild(1).GetComponent<Carro>().EstaEmPrimeiro = true;
        return true;
    }
    // public void Seguir(Vector3 posInicial)
    // {
    //     transform.parent = null;
    // }

    IEnumerator SeguirCoroutine()
    {
        while(transform.localPosition != posPrioridade.localPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, posPrioridade.localPosition, maxVelocidade*Time.deltaTime);
            yield return 0;
        }

        animator.SetTrigger("Esquerda");
        yield return new WaitForSeconds(2f);
        emMovimentoLivre = false;
        comPrioridade = false;
    }

    // public void Destruir()
    // {
    //     Destroy(this.gameObject);
    // }
}

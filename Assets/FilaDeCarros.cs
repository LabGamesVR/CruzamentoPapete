using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilaDeCarros : MonoBehaviour
{
    public GameObject carroPrefab;
    public Transform LocalFila;
    public Transform LocalEspera;
    float distanciaEntreCarros = 2f;
    private float velocidade = 3f;
    void Start(){
        InstanciarCarros(10);
    }
    public void InstanciarCarros(int n){
        Carro ultimo = null;
        for (int i = 0; i < n; i++)
        {
            Carro novoCarro = Instantiate(carroPrefab, LocalFila).GetComponent<Carro>();
            novoCarro.Instruir(ultimo,distanciaEntreCarros, velocidade, false, LocalEspera);
            ultimo = novoCarro;
        }
        LocalFila.GetChild(1).GetComponent<Carro>().Instruir(ultimo,distanciaEntreCarros, velocidade, true, LocalEspera);
    }
    public void MandarParaPrioridade(){
        LocalFila.GetChild(1).GetComponent<Carro>().Priorizar();
    }
    public void LiberarCarro(){
        LocalFila.GetChild(1).GetComponent<Carro>().Seguir();
    }
}

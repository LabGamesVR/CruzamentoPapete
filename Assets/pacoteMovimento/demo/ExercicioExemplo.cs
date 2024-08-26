using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExercicioExemplo : MonoBehaviour
{
    GerenciadorMovimentos mov;
    void Start(){
        mov = FindObjectOfType<GerenciadorMovimentos>();
    }
    void ExercicioFinalizado(int exercicio, float tempo){
        print("Exercicio finalizado em "+tempo+" segundos!");
    }
    void Neutralizado(int exercicio, float tempo){
        print(exercicio+ " foi para o neutro em "+tempo+" segundos!");
    }
    public void IniciarFrontalMax(){
        Queue<Movimento2Eixos.Direcao> fila = new();
        fila.Enqueue(Movimento2Eixos.Direcao.top);
        mov.IniciarSequenciaDeExercicios(fila,Neutralizado,ExercicioFinalizado);
    }
    public void IniciarFrontalMin(){
        Queue<Movimento2Eixos.Direcao> fila = new();
        fila.Enqueue(Movimento2Eixos.Direcao.bot);
        mov.IniciarSequenciaDeExercicios(fila,Neutralizado,ExercicioFinalizado);
    }
    public void IniciarLateralMax(){
        Queue<Movimento2Eixos.Direcao> fila = new();
        fila.Enqueue(Movimento2Eixos.Direcao.right);
        mov.IniciarSequenciaDeExercicios(fila,Neutralizado,ExercicioFinalizado);
    }
    public void IniciarLateralMin(){
        Queue<Movimento2Eixos.Direcao> fila = new();
        fila.Enqueue(Movimento2Eixos.Direcao.left);
        mov.IniciarSequenciaDeExercicios(fila,Neutralizado,ExercicioFinalizado);
    }
    public void IniciarSequencia(){
        Queue<Movimento2Eixos.Direcao> fila = new();
        fila.Enqueue(Movimento2Eixos.Direcao.top);
        fila.Enqueue(Movimento2Eixos.Direcao.right);
        fila.Enqueue(Movimento2Eixos.Direcao.right);
        fila.Enqueue(Movimento2Eixos.Direcao.left);
        fila.Enqueue(Movimento2Eixos.Direcao.bot);
        fila.Enqueue(Movimento2Eixos.Direcao.top);
        fila.Enqueue(Movimento2Eixos.Direcao.left);
        mov.IniciarSequenciaDeExercicios(fila,Neutralizado,ExercicioFinalizado);
    }
}

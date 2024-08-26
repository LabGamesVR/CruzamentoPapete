using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public List<Movimento2Eixos.Direcao> exercicio=new();
    public FilaDeCarros[] filas;
    
    //somente afeta exercicios gerados automaticamente. Gera automatico se não tiver nenhum na lista
    [Range(1,20)]
    public int carrosPorLadoNoExercicio = 3;
    private TituloController titulo;
    private Boneco boneco;
    private bool pronto;
    private GerenciadorMovimentos movimentos;

    public void GerarExercicio(){
        exercicio = new();
        print(exercicio);
        for (int i = 0; i < carrosPorLadoNoExercicio; i++)
        {
            exercicio.Add(Movimento2Eixos.Direcao.top);
            exercicio.Add(Movimento2Eixos.Direcao.bot);
            exercicio.Add(Movimento2Eixos.Direcao.left);
            exercicio.Add(Movimento2Eixos.Direcao.right);
        }
        //shuffle
        exercicio = exercicio.OrderBy(_ => Random.Range(0f,1f)).ToList();
    }
    void Start()
    {
        movimentos = FindObjectOfType<GerenciadorMovimentos>();  
        titulo = FindAnyObjectByType<TituloController>();      
        boneco = FindAnyObjectByType<Boneco>();      
    }
    void Update(){
        if(!pronto){
            if(movimentos.pronto){
                if(exercicio.Count==0){
                    GerarExercicio();   
                    titulo.LiberarControleInicio();
                    pronto = true;
                }
            }
        }
        if(movimentos.pronto){
            boneco.Apontar(movimentos.ObterDirecao());
        }
    }

    private void IniciarProximaPrioridade(int i, float t){
        Movimento2Eixos.Direcao ex = exercicio[i];
        switch (ex)
        {
            case Movimento2Eixos.Direcao.top:
                print("top");
                filas[0].MandarParaPrioridade();
                break;
            case Movimento2Eixos.Direcao.bot:
                print("bot");
                filas[1].MandarParaPrioridade();
            break;
            case Movimento2Eixos.Direcao.left:
                print("left");
                filas[2].MandarParaPrioridade();
                break;
            case Movimento2Eixos.Direcao.right:
                print("right");
                filas[3].MandarParaPrioridade();
                break;
        }
    }
    private void LiberarCarro(int i, float t){
            Movimento2Eixos.Direcao ex = exercicio[i];
            switch (ex)
            {
                case Movimento2Eixos.Direcao.top:
                    filas[0].LiberarCarro();
                    break;
                case Movimento2Eixos.Direcao.bot:
                    filas[1].LiberarCarro();
                break;
                case Movimento2Eixos.Direcao.left:
                    filas[2].LiberarCarro();
                    break;
                case Movimento2Eixos.Direcao.right:
                    filas[3].LiberarCarro();
                    break;

            }
        if(i==exercicio.Count-1){
            titulo.VoltarAoMenu();
            print("Acabou!");
        }
    }
    public void IniciarExercicio()
    {
        print(string.Join( ",", exercicio));
        movimentos.IniciarSequenciaDeExercicios(new Queue<Movimento2Eixos.Direcao>(exercicio),IniciarProximaPrioridade, LiberarCarro);
    }
}

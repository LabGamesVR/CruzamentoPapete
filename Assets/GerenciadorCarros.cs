using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GerenciadorCarros : MonoBehaviour
{
    static readonly float distanciaDeSpawn = 10f;
    static readonly float distanciaEntreCarros = 2f;
    static readonly float carroSpeed = 10f;
    
    static readonly Vector3 posDeEspera = new Vector3(-0.9f,0f,4f);
    static readonly Vector3 posDePrioridade = new Vector3(-0.9f,0f,2.6f);

    public bool auto = true;

    readonly Queue<int> filaDePrioridade = new();
    Queue<Carro>[] filas = { new(), new(), new(), new() };

    public int prioridade = -1;
    
    public GameObject carroPrefab;

    Transform[] carrosHolders = new Transform[4];

    public void Direcionar(int dir)
    {
        print(dir);
        if(dir == prioridade && filas[dir].Count>0)
        {
            Carro carro  = filas[dir].Dequeue();
            carro.Seguir(carrosHolders[dir].TransformPoint(posDePrioridade));

            prioridade = -1;
            Invoke("LiberarProximaPrioridade", 0.5f);
        }
    }

    public void IniciarJogo()
    {
        auto = false;
        for (int i = filas[0].Count + filas[1].Count + filas[2].Count + filas[3].Count; i < 20; i++)
        {
            int dir = Random.Range(0, 4);
            novoCarroNaFila(dir);
            filaDePrioridade.Enqueue(dir);
        }
    }

    void novoCarroNaFila(int dir) {
        Carro novoCarro = Instantiate(carroPrefab, carrosHolders[dir]).GetComponent<Carro>();
        novoCarro.transform.localPosition = posDeEspera + new Vector3(0f, 0f, distanciaDeSpawn + filas[dir].Count*distanciaEntreCarros);
        filas[dir].Enqueue(novoCarro);
    }
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            carrosHolders[i] = new GameObject(i.ToString()).transform;
            carrosHolders[i].position = transform.position;
            carrosHolders[i].rotation = Quaternion.Euler(0f, i * -90f, 0f);
        }
        Invoke("LiberarProximaPrioridade", 1f);
    }

    void LiberarProximaPrioridade()
    {
        if (filaDePrioridade.Count > 0)
            prioridade = filaDePrioridade.Dequeue();
        else
            prioridade = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (auto)
        {
            if (filas[0].Count + filas[1].Count + filas[2].Count + filas[3].Count < 10)
            {
                int dir = Random.Range(0, 4);
                novoCarroNaFila(dir);
                filaDePrioridade.Enqueue(dir);
            }
        }
        else
        {
            if (filas[0].Count + filas[1].Count + filas[2].Count + filas[3].Count == 0)
            {
                auto = true;
                FindObjectOfType<TituloController>().VoltarAoMenu();
            }

        }
        int filaIndex = 0;
        foreach (var fila in filas)
        {
            int carroIndex = 0;
            foreach (var carro in fila)
            {
                Vector3 dest = (carroIndex==0&&prioridade==filaIndex)?
                    posDePrioridade:
                    posDeEspera + new Vector3 (0f, 0f, distanciaEntreCarros*carroIndex);
                    ;
                carro.transform.localPosition = Vector3.MoveTowards(carro.transform.localPosition, dest, carroSpeed*Time.deltaTime);
                carroIndex++;
            }
            filaIndex++;
        }
    }
}

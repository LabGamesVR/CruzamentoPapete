using UnityEngine;
using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

public class ControladorSensores : MonoBehaviour
{
    public bool disponivel;
    public Vector2 dados;
    private Vector2 thread_save_dado;
    Mutex mutex = new Mutex();
    public MostradorStatusConexao mostrador;
    private string ultimoDispositivo;
    private Thread buscarDadosThread;
    private bool buscarDadosRunning = true;

    private StatusConexao novoStatus = StatusConexao.Desconectado;
    private StatusConexao statusConexao = StatusConexao.Desconectado;

    void Start()
    {
        if(FindObjectsOfType<ControladorSensores>().Length>1)
            Destroy(gameObject);
        else{
            DontDestroyOnLoad(gameObject);
            buscarDadosThread = new Thread(BuscarDadosPorta);
            buscarDadosThread.Start();
        }
    }

    void Update(){
        if(mostrador && novoStatus!=statusConexao){
            statusConexao = novoStatus;
            mostrador.Mostrar(statusConexao);
        }
            mutex.WaitOne();
            dados = thread_save_dado;  
            mutex.ReleaseMutex();
    }
    private static bool IsValidArduinoResponse(string response)
    {
        string pattern = @".*-?\d+\.\d+\t-?\d+\.\d+.*";
        return new Regex(pattern).IsMatch(response);
    }

    private SerialPort BuscarArduino()
    {
        while (buscarDadosRunning)
        {
            bool erro = false;
            SerialPort newSerialPort = null;
            foreach (string port in SerialPort.GetPortNames())
            {
                // Debug.Log("tentando na porta "+port);

                erro = false;
                try
                {
                    newSerialPort = new SerialPort(port, 9600)
                    {
                        ReadTimeout = 500
                    };
                    newSerialPort.Open();
                }
                catch (Exception)
                {
                    // Debug.Log(e);
                    erro = true;
                }
                if (!erro)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(100);
                        try
                        {
                            string response = newSerialPort.ReadLine();
                            if (IsValidArduinoResponse(response))
                                return newSerialPort;
                        }
                        catch (Exception)
                        {
                            // Debug.Log(e);
                        }
                    }
                }
            }
        }
        return null;
    }

    private void BuscarDadosPorta()
    {
        SerialPort serialPort = null;
        while (buscarDadosRunning)
        {
            if (serialPort == null)
            {
                if (mostrador != null)
                    novoStatus = StatusConexao.Desconectado;
                disponivel = false;

                serialPort = BuscarArduino();

                if (mostrador != null)
                    novoStatus = StatusConexao.Conectado;
                disponivel = true;
            }
            else
            {
                try
                {
                    string response = serialPort.ReadLine();
                    // print(response);
                    if (response == "")
                    {
                        serialPort = null;
                        disponivel = false;
                    }

                    if (IsValidArduinoResponse(response))
                    {
                        string[] f_reps = response.Split('\t');
                        string id = f_reps[0];
                        try
                        {
                            Vector2 v = new Vector2(
                                float.Parse(f_reps[1]),
                                float.Parse(f_reps[2])
                            );
                            // A leitura é feita em radianos pelo sensor.
                            // Como em Unity por padrão trabalhamos com graus,
                            // aqui fazemos a conversão
                            v*=180f / Mathf.PI;
                            
                            mutex.WaitOne();
                            thread_save_dado = v;  
                            mutex.ReleaseMutex();

                            if(ultimoDispositivo!=id)
                                ultimoDispositivo=id;
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception)
                {
                    // Debug.Log("ERRO: " + e);
                    serialPort = null;
                    disponivel = false;
                }
            }

            Thread.Sleep(100); // To avoid tight looping
        }
    }

    public string ObterDispostivoAtual(){
        return ultimoDispositivo;
    }

    private void OnApplicationQuit()
    {
        buscarDadosRunning = false;
        if (buscarDadosThread != null && buscarDadosThread.IsAlive)
        {
            buscarDadosThread.Join();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Papete
{
    public static string[] NomesExterno = new string[] { "Dorsiflexão", "Flexão", "Repouso", "Eversão", "Inversão" };
    public static string[] NomesInterno = new string[] { "Dorsiflexao", "Flexao", "Repouso", "Eversao", "Inversao" };

    public static Vector2 ObterPosicaoCruz(Movimento pos, bool peEsquerdo)
    {
        if (pos == Movimento.Dorsiflexao)
            return new Vector2(0f, 1f);
        if (pos == Movimento.Flexao)
            return new Vector2(0f, -1f);
        if (peEsquerdo)
            if (pos == Movimento.Eversao)
                return new Vector2(-1f, 0f);
        return new Vector2(0f, 0f);
    }
    public static bool TryParsePos(string s, out Movimento pos)
    {
        s = s.ToLower();
        for (int i = 0; i < 5; i++)
        {
            if (s.ToLower()[0] == NomesInterno[i].ToLower()[0])
            {
                pos = (Movimento)i;
                return true;
            }
        }

        pos = Movimento.Repouso;
        return false;
    }

    public static Vector2 ObterMovimentoCruz(Movimento mov, bool peEsquerdo)
    {
        if (mov == Movimento.Flexao)
        {
            return new Vector2(0f, 1f);
        }
        else if (mov == Movimento.Dorsiflexao)
        {
            return new Vector2(0f, -1f);
        }
        else if (mov == Movimento.Eversao)
        {
            if (peEsquerdo)
                return new Vector2(1f, 0f);
            return new Vector2(-1f, 0f);
        }
        else if (mov == Movimento.Inversao)
        {
            if (peEsquerdo)
                return new Vector2(-1f, 0f);
            return new Vector2(1f, 0f);
        }
        return new Vector2(0f, 0f);
    }

    public Vector2 ObterRotacaoV2()
    {
        return obter_rotacao(ptr_papete);
    }
    public int DesfazerUltimaColeta()
    {
        return deregistrar(ptr_papete);
    }
    public void Retreinar(Action<int> eventoCalibrado)
    {
        retreinar(ptr_papete);
        eventoCalibrado.Invoke(1);
    }

    public int Registrar(Movimento mov)
    {
        return registrar(ptr_papete, mov);
    }
    public void Zerar()
    {
        zerar(ptr_papete);
    }
    public bool EhPeEsq()
    {
        return obter_lado(ptr_papete);
    }








    public enum Movimento
    {
        Flexao,
        Dorsiflexao,
        Repouso,
        Eversao,
        Inversao
    }

    IntPtr ptr_papete;

    //construtor e destrutor
    public Papete()
    {
        ptr_papete = Papete.alocar_papete();
    }
    ~Papete()
    {
        liberar_papete(ptr_papete);
    }
    // metodos publicos

    public void AbrirMenuPapete()
    {
        throw new NotImplementedException();
        //MenuPapete menu = GameObject.Instantiate(new GameObject("MenuPapete")).AddComponent<MenuPapete>();
        //menu.papete = this;
    }
    public void FecharMenuPapete()
    {
        throw new NotImplementedException();
        //GameObject.Destroy(GameObject.FindObjectOfType<MenuPapete>());
    }

    public void IniciarSessao()
    {
        iniciar_sessao(ptr_papete);
    }

    public List<String> ListarConexoesDisponiveis()
    {
        List<String> list = new();
        unsafe
        {
            fixed (char* buffer = new char[300])
            {
                listar_conexoes_disponiveis(buffer);
                string res = Marshal.PtrToStringAnsi((IntPtr)buffer);
                foreach (var item in res.Split(','))
                {
                    if(item!="")
                        list.Add(item);
                }
            }
        }
        return list;
    }
    public List<String> ListarConexoesAtuais()
    {
        List<String> list = new();
        unsafe
        {
            fixed (byte* buffer = new byte[300])
            {
                listar_conexoes_atuais(ptr_papete, buffer);
                string res = Marshal.PtrToStringAnsi((IntPtr)buffer);
                foreach (var item in res.Split(','))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    public void ModoConexaoImediata(bool ativar)
    {
        if(ativar) 
            ativar_modo_conexao_imediata(ptr_papete);
        else
            desativar_modo_conexao_imediata(ptr_papete);
    }

    public bool Conectar(string porta)
    {
        return conectar(ptr_papete, porta);
    }

    public Movimento ObterMovimento()
    {
        return obter_movimento(ptr_papete);
    }


    //metodos rust
    [DllImport("papete.dll")]
    static extern IntPtr alocar_papete();

    [DllImport("papete.dll")]
    static extern void liberar_papete(IntPtr s);

    [DllImport("papete.dll")]
    static extern void iniciar_sessao(IntPtr s);

    [DllImport("papete.dll")]
    static extern unsafe int listar_conexoes_disponiveis(char* buffer);

    [DllImport("papete.dll")]
    static extern unsafe int listar_conexoes_atuais(IntPtr s, byte* buffer);

    [DllImport("papete.dll")]
    //MarshalAs transforma a String de C# em um char* de C
    static extern bool conectar(IntPtr s, [MarshalAs(UnmanagedType.LPUTF8Str)] string porta);

    [DllImport("papete.dll")]
    static extern Movimento obter_movimento(IntPtr s);

    [DllImport("papete.dll")]
    static extern void ativar_modo_conexao_imediata(IntPtr s);

    [DllImport("papete.dll")]
    static extern void desativar_modo_conexao_imediata(IntPtr s);

    [DllImport("papete.dll")]
    static extern Vector2 obter_rotacao(IntPtr s);

    [DllImport("papete.dll")]
    static extern bool obter_lado(IntPtr s);

    [DllImport("papete.dll")]
    static extern int registrar(IntPtr s, Movimento mov);

    [DllImport("papete.dll")]
    static extern int deregistrar(IntPtr s);

    [DllImport("papete.dll")]
    static extern int zerar(IntPtr s);

    [DllImport("papete.dll")]
    static extern bool retreinar(IntPtr s);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public RawImage[] pes;
    public Texture[] spritesEvInv;
    public bool peEsq = false;
    private bool lastPeEsq = false;
    private ControladorSensores sensor;
    private string ultimoDispositivo;
    private void Start()
    {
        sensor = FindObjectOfType<ControladorSensores>();
    }
    public void fechar()
    {
        this.gameObject.SetActive(false);
    }
    private void Update()
    {
        string dispositivo = sensor.ObterDispostivoAtual();
        if(dispositivo != ultimoDispositivo)
            switch (dispositivo)
            {
                case "papE":
                    foreach(RawImage img in pes)    
                        img.transform.localScale = new Vector3(-1f, 1f, 1f) ;
                    break;
                case "papD":
                    foreach(RawImage img in pes)    
                        img.transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
            }
    }
}

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
    Papete papete;
    private void Start()
    {
        papete = FindObjectOfType<Jogador>().papete;
    }
    public void fechar()
    {
        this.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (papete==null)
        {
            papete = FindObjectOfType<Jogador>().papete;
        }
        else
        {
            peEsq = papete.EhPeEsq();
            if (peEsq != lastPeEsq)
            {
                lastPeEsq = peEsq;
                foreach(RawImage img in pes)
                    img.transform.localScale = new Vector3(peEsq ? -1f:1f, 1f, 1f) ;

                pes[1].texture = spritesEvInv[peEsq?0:1];
                pes[3].texture = spritesEvInv[peEsq?1:0];
            }
        }
    }
}

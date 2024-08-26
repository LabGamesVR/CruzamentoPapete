using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boneco : MonoBehaviour
{
    float targetDirection = 0f;
    float rotSpeed = 30f;
    float neckSpeed = 10f;

    float inicioDessaDirecao;

    Movimento2Eixos.Direcao currentDir = Movimento2Eixos.Direcao.mid;
    Movimento2Eixos.Direcao lastNotMidDir = Movimento2Eixos.Direcao.bot;
    Animator animator;
    //private GerenciadorCarros gerenciadorCarros;
    public Transform neck;
    public Vector3 neckRotWaitTop;
    public Vector3 neckRotWaitBot;
    public Vector3 neckRotWaitLeft;
    public Vector3 neckRotWaitRight;


    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public bool Apontar(Movimento2Eixos.Direcao dir, bool force = false)
    {
        if (dir != currentDir)
        {
            CancelInvoke();
            Invoke(nameof(PularEApontar), 0.2f);
            lastNotMidDir = currentDir != Movimento2Eixos.Direcao.mid ? currentDir : lastNotMidDir;
            currentDir = dir;

            return true;
        }
        return false;
    }
    void PularEApontar()
    {
        try
        {
            targetDirection = currentDir switch
            {
                Movimento2Eixos.Direcao.top => 180f,
                Movimento2Eixos.Direcao.bot => 0f,
                Movimento2Eixos.Direcao.left => 90f,
                Movimento2Eixos.Direcao.right => -90f,
                _ => throw new System.Exception(),
            };
            animator.SetTrigger("jumpTrigger");
        }
        catch (System.Exception)
        { }
    }
    void Update()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y = Mathf.LerpAngle(rot.y, targetDirection, Time.deltaTime * rotSpeed);
        transform.rotation = Quaternion.Euler(rot);

        rot = neck.localRotation.eulerAngles;
        Vector3 targetNeckRot = currentDir == Movimento2Eixos.Direcao.mid ?
        lastNotMidDir switch
        {
            Movimento2Eixos.Direcao.bot => neckRotWaitBot,
            Movimento2Eixos.Direcao.left => neckRotWaitLeft,
            Movimento2Eixos.Direcao.right => neckRotWaitRight,
            _ => neckRotWaitTop,
        }
        : Vector3.zero;
        rot.x = Mathf.LerpAngle(rot.x, targetNeckRot.x, Time.deltaTime * neckSpeed);
        rot.y = Mathf.LerpAngle(rot.y, targetNeckRot.y, Time.deltaTime * neckSpeed);
        rot.z = Mathf.LerpAngle(rot.z, targetNeckRot.z, Time.deltaTime * neckSpeed);
        neck.localRotation = Quaternion.Euler(rot);
    }
}

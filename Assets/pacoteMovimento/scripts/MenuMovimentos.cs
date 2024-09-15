using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;

[RequireComponent(typeof(GerenciadorMovimentos))]
public class MenuMovimentos : MonoBehaviour
{
    public Button topBtn;
    public Button botBtn;
    public Button midBtn;
    public Button leftBtn;
    public Button rightBtn;

    public Color corComDado = Color.yellow;
    public Color corSemDado = Color.white;
    public RectTransform dotContainer;
    public RectTransform dot;
    public RectTransform inidicadorThresholdTop;
    public RectTransform inidicadorThresholdBot;
    public RectTransform inidicadorThresholdLeft;
    public RectTransform inidicadorThresholdRight;

    public GameObject IndicadotEixoFrontal;
    public GameObject IndicadotEixoLateral;

    private bool menuVisivel;
    public Transform menu;
    public float velocidadeMenu = 5f;
    public string pastaRelatorios = "relatorios";

    private GerenciadorMovimentos mov2;

    void Start()
    {
        mov2 = GetComponent<GerenciadorMovimentos>();
        menuVisivel = menu.localScale.x != 0f;
        pastaRelatorios = Application.persistentDataPath + "/" + pastaRelatorios;

        SetNomeRelatorio();

        //atualiza labels
        topBtn.GetComponentInChildren<TMP_Text>().text = mov2.topName;
        botBtn.GetComponentInChildren<TMP_Text>().text = mov2.botName;
        midBtn.GetComponentInChildren<TMP_Text>().text = mov2.midName;
        leftBtn.GetComponentInChildren<TMP_Text>().text = mov2.leftName;
        rightBtn.GetComponentInChildren<TMP_Text>().text = mov2.rightName;

        //atualiza acoes
        topBtn.onClick.AddListener(SetTop);
        botBtn.onClick.AddListener(SetBot);
        midBtn.onClick.AddListener(SetMid);
        leftBtn.onClick.AddListener(SetLeft);
        rightBtn.onClick.AddListener(SetRight);

    }

    void Update(){
        if(mov2.pronto){
            SetPos(mov2.analise.progressoLateral, mov2.analise.progressoFrontal, dotContainer, dot);
            IndicadotEixoFrontal.SetActive(mov2.analise.ProbabilidadeEixoLateral < 0.5f);
            IndicadotEixoLateral.SetActive(mov2.analise.ProbabilidadeEixoLateral > 0.5f);
            Movimento2Eixos.Thresholds t = mov2.GetThresholds();
            SetPos(0.5f, t.top, dotContainer, inidicadorThresholdTop);
            SetPos(0.5f, t.bot, dotContainer, inidicadorThresholdBot);
            SetPos(t.left, 0.5f, dotContainer, inidicadorThresholdLeft);
            SetPos(t.right, 0.5f, dotContainer, inidicadorThresholdRight);
        }
        else
        {
            IndicadotEixoFrontal.SetActive(false);
            IndicadotEixoLateral.SetActive(false);
        }
    }
    static void SetPos(float x, float y, RectTransform container, RectTransform target)
    {
        // Get the positions of the corners of the container RectTransform
        Vector3[] containerCorners = new Vector3[4];
        container.GetLocalCorners(containerCorners);

        // Calculate the target Y position for the target RectTransform
        float targetY = Mathf.Lerp(containerCorners[1].y, containerCorners[0].y, 1f - y);

        // Calculate the offset to align the target RectTransform correctly along the Y axis
        float yOffset = target.rect.height * target.pivot.y;
        targetY += yOffset * (0.5f - y) * 2f;

        // Calculate the target X position for the target RectTransform
        float targetX = Mathf.Lerp(containerCorners[0].x, containerCorners[2].x, x);

        // Calculate the offset to align the target RectTransform correctly along the X axis
        float xOffset = target.rect.width * target.pivot.x;
        targetX += xOffset * (0.5f - x) * 2f;

        // Adjust the position of the target RectTransform
        Vector3 newPos = target.localPosition;
        newPos.x = targetX;
        newPos.y = targetY;
        target.localPosition = newPos;
    }


    public void ToggleMenu()
    {
        menuVisivel = !menuVisivel;
        StartCoroutine(MoverMenu());
    }

    IEnumerator MoverMenu()
    {
        if (menuVisivel)
            menu.gameObject.SetActive(true);
        float target;
        do
        {
            target = menuVisivel ? 1f : 0f;
            menu.localScale = Vector3.one * Mathf.MoveTowards(menu.localScale.x, target, Time.deltaTime * velocidadeMenu);
            yield return 0;
        } while (menu.localScale.x != target);
        if (!menuVisivel)
            menu.gameObject.SetActive(false);
    }

    string SanitizeFilename(string filename)
    {
        return System.Text.RegularExpressions.Regex.Replace(filename, @"[\\/:*?""<>|]", "");
    }
    public void SetNomeRelatorio(string nome = "")
    {
        nome = SanitizeFilename(nome.Replace(" ", "-"));
        if (nome == "")
            nome = System.DateTime.Now.ToString("yyyy-dd-MM_HH_mm");
        mov2.SetSaveLocation(pastaRelatorios + "/" + nome + ".csv");
    }
    public void AbrirPastaRelatorios()
    {
        print(pastaRelatorios);
        System.IO.Directory.CreateDirectory(pastaRelatorios);
        print(Application.platform);
        // Detect the current platform
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                Process.Start("explorer.exe", pastaRelatorios);
                break;

            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                Process.Start("open", pastaRelatorios);
                break;

            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
                Process.Start("xdg-open", pastaRelatorios);
                break;

            default:
                break;
        }
    }

    void SetTop()
    {
        if (mov2.CalibrarDir(Movimento2Eixos.Direcao.top))
            topBtn.GetComponent<Image>().color = corComDado;
    }
    void SetBot()
    {
        if (mov2.CalibrarDir(Movimento2Eixos.Direcao.bot))
            botBtn.GetComponent<Image>().color = corComDado;
    }
    void SetMid()
    {
        if (mov2.CalibrarDir(Movimento2Eixos.Direcao.mid))
            midBtn.GetComponent<Image>().color = corComDado;
    }
    void SetLeft()
    {
        if (mov2.CalibrarDir(Movimento2Eixos.Direcao.left))
            leftBtn.GetComponent<Image>().color = corComDado;
    }
    void SetRight()
    {
        if (mov2.CalibrarDir(Movimento2Eixos.Direcao.right))
            rightBtn.GetComponent<Image>().color = corComDado;
    }

}
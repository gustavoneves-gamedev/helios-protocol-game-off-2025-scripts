using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Wiring")]
    public CanvasGroup dimmer;             // add CanvasGroup ao Dimmer_BG
    public GameObject frameFX;             // FrameFX (Image)
    public RectTransform titleHelios;      // Title_Helios
    public CanvasGroup titleHeliosCg;     // add CanvasGroup no TMP
    public RectTransform titleProtocol;    // Title_Protocol
    public CanvasGroup titleProtocolCg;   // add CanvasGroup no TMP
    public RectTransform btnPlay, btnHowToPlay, btnOptions, btnQuit;
    public CanvasGroup btnPlayCg, btnHowToPlayCg, btnOptionsCg, btnQuitCg;
    public TMP_Text footerHints;
    public GameObject optionsPanel;      // painel de opções
    public Button optionsBackBtn;    // botão voltar no painel
    public Slider volumeSlider;      // slider master
    public GameObject tutorialPanel;
    public GameObject tutorial1;
    public GameObject tutorial2;
    public GameObject tutorial3;

    [Header("Audio Info")]            
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider SFXVolume;

    [Header("Config")]
    public float introDelay = 0.2f;
    public float introDur = 0.6f;

    void Start()
    {
        // Estado inicial
        frameFX.SetActive(false);
        optionsPanel.SetActive(false);

        dimmer.alpha = 1f;
        titleHeliosCg.alpha = 0f; titleProtocolCg.alpha = 0f;
        btnPlayCg.alpha = 0f; btnHowToPlayCg.alpha = 0f; btnOptionsCg.alpha = 0f; btnQuitCg.alpha = 0f;

        titleHelios.anchoredPosition += new Vector2(0, 80);
        titleProtocol.anchoredPosition += new Vector2(0, -40);
        btnPlay.anchoredPosition += new Vector2(-80, 0);
        btnHowToPlay.anchoredPosition += new Vector2(-80, 0);
        btnOptions.anchoredPosition += new Vector2(-80, 0);
        btnQuit.anchoredPosition += new Vector2(-80, 0);

        // Intro
        LeanTween.value(gameObject, 1f, 0f, 2f).setOnUpdate((float a) => dimmer.alpha = a);

        frameFX.SetActive(true);
        //LeanTween.alpha(frameFX.GetComponent<RectTransform>(), 0.22f, 0.6f); // requer CanvasGroup se quiser

        // Títulos
        LeanTween.moveY(titleHelios, titleHelios.anchoredPosition.y - 80, introDur).setDelay(introDelay).setEaseOutCubic();
        LeanTween.value(gameObject, 0f, 1f, introDur).setDelay(introDelay).setOnUpdate((float a) => titleHeliosCg.alpha = a);

        LeanTween.moveY(titleProtocol, titleProtocol.anchoredPosition.y + 40, introDur).setDelay(introDelay + 0.1f).setEaseOutCubic();
        LeanTween.value(gameObject, 0f, 1f, introDur).setDelay(introDelay + 0.1f).setOnUpdate((float a) => titleProtocolCg.alpha = a);

        // Botões (stagger)
        AnimateButtonIn(btnPlay, btnPlayCg, 0.25f);
        AnimateButtonIn(btnOptions, btnOptionsCg, 0.35f);
        AnimateButtonIn(btnHowToPlay, btnHowToPlayCg, 0.45f);
        AnimateButtonIn(btnQuit, btnQuitCg, 0.55f);

        // Footer hint pulse
        PulseFooter();

        // Liga callbacks do Options
        //optionsBackBtn.onClick.AddListener(CloseOptions);
        //volumeSlider.onValueChanged.AddListener((v) => AudioListener.volume = v);
        AudioController.audioController.ChangeMasterVolume(volumeSlider.value);
    }

    void AnimateButtonIn(RectTransform rt, CanvasGroup cg, float delay)
    {
        LeanTween.moveX(rt, rt.anchoredPosition.x + 80, introDur).setDelay(delay).setEaseOutCubic();
        LeanTween.value(gameObject, 0f, 1f, introDur).setDelay(delay).setOnUpdate((float a) => cg.alpha = a);
        // leve bounce
        LeanTween.scale(rt, Vector3.one * 1.03f, 0.18f).setDelay(delay + introDur).setEaseOutQuad().setLoopPingPong(1);
    }

    void PulseFooter()
    {
        var cg = footerHints.GetComponent<CanvasGroup>();
        if (cg == null) cg = footerHints.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0.6f;
        LeanTween.value(gameObject, 0.6f, 0.2f, 1.2f).setLoopPingPong().setOnUpdate((float a) => cg.alpha = a);
    }

    // === Botões ===
    public void OnClickPlay()
    {
        // pequena animação de click nos 3 botões
        ClickPunch(btnPlay); ClickPunch(btnHowToPlay); ClickPunch(btnOptions); ClickPunch(btnQuit);

        // Antes de carregar a fase:
        if (GameController.gameController != null)
            GameController.gameController.ResetGame();

        // fade out e carregar cena
        LeanTween.value(gameObject, dimmer.alpha, 1f, 0.5f).setOnUpdate((float a) => dimmer.alpha = a)
                 .setOnComplete(() => SceneManager.LoadScene("DesertLevel2"));
    }

    #region Tutorial

    public void OnClickHowToPlay()
    {
        //tutorial1.SetActive(true);
        tutorial2.SetActive(false);
        tutorial3.SetActive(false);        
        
        ClickPunch(btnHowToPlay);
        tutorial1.SetActive(true);
        var cg = tutorial1.GetComponent<CanvasGroup>();
        if (cg == null) cg = tutorial1.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = tutorial1.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial1, 0f, 1f, 0.25f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack();
    }

    public void OnClickFirstNext()
    {
        AudioController.audioController.onClick.Play();
        var cg = tutorial1.GetComponent<CanvasGroup>();
        var rt = tutorial1.GetComponent<RectTransform>();
        LeanTween.value(tutorial1, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setOnComplete(() => tutorial1.SetActive(false));

        tutorial2.SetActive(true);
        var cg1 = tutorial2.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = tutorial2.AddComponent<CanvasGroup>();
        cg1.alpha = 0f;
        var rt1 = tutorial2.GetComponent<RectTransform>();
        rt1.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial2, 0f, 1f, 0.25f).setOnUpdate((float a) => cg1.alpha = a);
        LeanTween.scale(rt1, Vector3.one, 0.25f).setEaseOutBack();
    }

    public void OnClickSecondNext()
    {
        AudioController.audioController.onClick.Play();
        var cg = tutorial2.GetComponent<CanvasGroup>();
        var rt = tutorial2.GetComponent<RectTransform>();
        LeanTween.value(tutorial2, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setOnComplete(() => tutorial2.SetActive(false));

        tutorial3.SetActive(true);
        var cg1 = tutorial3.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = tutorial3.AddComponent<CanvasGroup>();
        cg1.alpha = 0f;
        var rt1 = tutorial3.GetComponent<RectTransform>();
        rt1.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial3, 0f, 1f, 0.25f).setOnUpdate((float a) => cg1.alpha = a);
        LeanTween.scale(rt1, Vector3.one, 0.25f).setEaseOutBack();
    }

    public void OnClickFirstPrevious()
    {
        AudioController.audioController.onClick.Play();
        var cg = tutorial2.GetComponent<CanvasGroup>();
        var rt = tutorial2.GetComponent<RectTransform>();
        LeanTween.value(tutorial2, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setOnComplete(() => tutorial2.SetActive(false));

        tutorial1.SetActive(true);
        var cg1 = tutorial1.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = tutorial1.AddComponent<CanvasGroup>();
        cg1.alpha = 0f;
        var rt1 = tutorial1.GetComponent<RectTransform>();
        rt1.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial1, 0f, 1f, 0.25f).setOnUpdate((float a) => cg1.alpha = a);
        LeanTween.scale(rt1, Vector3.one, 0.25f).setEaseOutBack();
    }

    public void OnClickSecondPrevious()
    {
        AudioController.audioController.onClick.Play();
        var cg = tutorial3.GetComponent<CanvasGroup>();
        var rt = tutorial3.GetComponent<RectTransform>();
        LeanTween.value(tutorial3, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setOnComplete(() => tutorial3.SetActive(false));

        tutorial2.SetActive(true);
        var cg1 = tutorial2.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = tutorial2.AddComponent<CanvasGroup>();
        cg1.alpha = 0f;
        var rt1 = tutorial2.GetComponent<RectTransform>();
        rt1.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial2, 0f, 1f, 0.25f).setOnUpdate((float a) => cg1.alpha = a);
        LeanTween.scale(rt1, Vector3.one, 0.25f).setEaseOutBack();
    }

    public void CloseTutorial()
    {
        AudioController.audioController.onClick.Play();
        var cg = tutorial3.GetComponent<CanvasGroup>();
        var rt = tutorial3.GetComponent<RectTransform>();
        LeanTween.value(tutorial3, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setOnComplete(() => tutorial3.SetActive(false));
    }


    #endregion

    public void OnClickOptions()
    {
        ClickPunch(btnOptions);
        optionsPanel.SetActive(true);        
        var cg = optionsPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = optionsPanel.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = optionsPanel.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(optionsPanel, 0f, 1f, 0.25f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack();
    }

    public void CloseOptions()
    {
        AudioController.audioController.onClick.Play();
        var cg = optionsPanel.GetComponent<CanvasGroup>();
        var rt = optionsPanel.GetComponent<RectTransform>();
        LeanTween.value(optionsPanel, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setOnComplete(() => optionsPanel.SetActive(false));
    }

    public void OnClickQuit()
    {
        ClickPunch(btnQuit);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // === UX: hover e click ===
    public void OnHoverEnter(RectTransform rt)
    {
        AudioController.audioController.onHover.Play();        
        LeanTween.scale(rt, Vector3.one * 1.04f, 0.12f).setEaseOutQuad();
    }
    public void OnHoverExit(RectTransform rt)
    {
        //PlayHover();
        LeanTween.scale(rt, Vector3.one, 0.12f).setEaseOutQuad();
    }

    void ClickPunch(RectTransform rt)
    {
        
        AudioController.audioController.onClick.Play();
        LeanTween.scale(rt, Vector3.one * 0.96f, 0.08f).setEaseOutQuad().setLoopPingPong(1);
    }
    
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SesionUI : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform hudRaiz;
    public RectTransform panelPerfilRaiz;

    RectTransform canvasRaiz;

    GameObject panelHistorial;
    TextMeshProUGUI textoHistorial;
    TextMeshProUGUI textoIndicadorPausa;
    TextMeshProUGUI textoIndicadorHistorial;
    TMP_FontAsset fuente;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        bool historialAbierto = panelHistorial != null && panelHistorial.activeSelf;

        if (historialAbierto && OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            OcultarHistorial();
            return;
        }

        if (!GameManager.Instance.SesionEnCurso())
            return;

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
            TogglePausa();

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            MostrarHistorial();
    }

    public void Configurar(RectTransform hud, RectTransform panelPerfil)
    {
        hudRaiz = hud;
        panelPerfilRaiz = panelPerfil;
        canvasRaiz = panelPerfil != null ? panelPerfil.parent as RectTransform : null;

        if (fuente == null)
            fuente = TMP_Settings.defaultFontAsset;

        EliminarHudObsoleto("BtnPausa");
        EliminarHudObsoleto("BtnHistorialHud");

        CrearIndicadorPausa();
        CrearIndicadorHistorialEnHud();
        CrearBotonHistorialEnPerfil();
        CrearPanelHistorial();
    }

    void EliminarHudObsoleto(string nombre)
    {
        if (hudRaiz == null)
            return;

        Transform viejo = hudRaiz.Find(nombre);
        if (viejo != null)
            Destroy(viejo.gameObject);
    }

    void CrearIndicadorPausa()
    {
        if (hudRaiz == null || hudRaiz.Find("IndicadorPausa") != null)
            return;

        textoIndicadorPausa = CrearIndicador(hudRaiz, "IndicadorPausa",
            new Vector2(-120f, 180f), new Vector2(130f, 40f),
            new Color(0.55f, 0.35f, 0.1f, 1f), "X: Pausar");
    }

    void CrearIndicadorHistorialEnHud()
    {
        if (hudRaiz == null || hudRaiz.Find("IndicadorHistorial") != null)
            return;

        textoIndicadorHistorial = CrearIndicador(hudRaiz, "IndicadorHistorial",
            new Vector2(120f, 180f), new Vector2(130f, 40f),
            new Color(0.15f, 0.35f, 0.55f, 1f), "A: Historial");
    }

    void CrearBotonHistorialEnPerfil()
    {
        if (panelPerfilRaiz == null || panelPerfilRaiz.Find("BtnHistorialPerfil") != null)
            return;

        Button boton = CrearBoton(panelPerfilRaiz, "BtnHistorialPerfil", "Ver Historial",
            new Vector2(0f, -220f), new Vector2(200f, 45f),
            new Color(0.15f, 0.35f, 0.55f, 1f));
        boton.onClick.AddListener(MostrarHistorial);
    }

    void CrearPanelHistorial()
    {
        if (panelHistorial != null)
            return;

        Transform padre = canvasRaiz != null ? canvasRaiz : hudRaiz;
        if (padre == null)
            return;

        panelHistorial = new GameObject("PanelHistorial", typeof(RectTransform), typeof(Image));
        panelHistorial.transform.SetParent(padre, false);
        panelHistorial.SetActive(false);
        VRUISetup.AsegurarLayerEnArbol(panelHistorial);

        RectTransform rect = panelHistorial.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(520f, 380f);
        rect.anchoredPosition = Vector2.zero;

        Image fondo = panelHistorial.GetComponent<Image>();
        fondo.color = new Color(0.08f, 0.1f, 0.14f, 0.95f);
        fondo.raycastTarget = false;

        CrearIndicador(panelHistorial.transform, "IndicadorCerrarHistorial",
            new Vector2(0f, -165f), new Vector2(160f, 36f),
            new Color(0.4f, 0.15f, 0.15f, 1f), "A: Cerrar");

        GameObject scrollGo = new GameObject("Scroll", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollGo.transform.SetParent(panelHistorial.transform, false);
        RectTransform scrollRect = scrollGo.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRect.pivot = new Vector2(0.5f, 0.5f);
        scrollRect.sizeDelta = new Vector2(480f, 280f);
        scrollRect.anchoredPosition = new Vector2(0f, 15f);
        scrollGo.GetComponent<Image>().color = new Color(0.12f, 0.14f, 0.18f, 1f);

        GameObject contenidoGo = new GameObject("Contenido", typeof(RectTransform));
        contenidoGo.transform.SetParent(scrollGo.transform, false);
        RectTransform contenidoRect = contenidoGo.GetComponent<RectTransform>();
        contenidoRect.anchorMin = new Vector2(0f, 1f);
        contenidoRect.anchorMax = new Vector2(1f, 1f);
        contenidoRect.pivot = new Vector2(0.5f, 1f);
        contenidoRect.sizeDelta = new Vector2(0f, 280f);

        GameObject textoGo = new GameObject("TextoHistorial", typeof(RectTransform), typeof(TextMeshProUGUI));
        textoGo.transform.SetParent(contenidoGo.transform, false);
        RectTransform textoRect = textoGo.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = new Vector2(12f, 8f);
        textoRect.offsetMax = new Vector2(-12f, -8f);

        textoHistorial = textoGo.GetComponent<TextMeshProUGUI>();
        if (fuente != null)
            textoHistorial.font = fuente;
        textoHistorial.fontSize = 16f;
        textoHistorial.alignment = TextAlignmentOptions.TopLeft;
        textoHistorial.color = Color.white;
        textoHistorial.enableWordWrapping = true;
        textoHistorial.raycastTarget = false;

        ScrollRect scroll = scrollGo.GetComponent<ScrollRect>();
        scroll.content = contenidoRect;
        scroll.viewport = scrollRect;
        scroll.horizontal = false;
        scroll.vertical = true;
    }

    TextMeshProUGUI CrearIndicador(Transform padre, string nombre, Vector2 posicion, Vector2 tamano, Color color, string etiqueta)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(padre, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = tamano;

        Image imagen = go.GetComponent<Image>();
        imagen.color = color;
        imagen.raycastTarget = false;

        GameObject textoGo = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
        textoGo.transform.SetParent(go.transform, false);
        RectTransform textoRect = textoGo.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;

        TextMeshProUGUI texto = textoGo.GetComponent<TextMeshProUGUI>();
        if (fuente != null)
            texto.font = fuente;
        texto.text = etiqueta;
        texto.fontSize = 17f;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = Color.white;
        texto.raycastTarget = false;

        VRUISetup.AsegurarLayerEnArbol(go);
        return texto;
    }

    Button CrearBoton(Transform padre, string nombre, string etiqueta, Vector2 posicion, Vector2 tamano, Color color)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(padre, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = tamano;

        go.GetComponent<Image>().color = color;

        GameObject textoGo = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
        textoGo.transform.SetParent(go.transform, false);
        RectTransform textoRect = textoGo.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;

        TextMeshProUGUI texto = textoGo.GetComponent<TextMeshProUGUI>();
        if (fuente != null)
            texto.font = fuente;
        texto.text = etiqueta;
        texto.fontSize = 18f;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = Color.white;
        texto.raycastTarget = false;

        VRUISetup.AsegurarLayerEnArbol(go);

        return go.GetComponent<Button>();
    }

    void TogglePausa()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.EstaPausada())
            GameManager.Instance.ReanudarSesion();
        else
            GameManager.Instance.PausarSesion();

        ActualizarTextoPausa();
    }

    void ActualizarTextoPausa()
    {
        if (textoIndicadorPausa == null || GameManager.Instance == null)
            return;

        textoIndicadorPausa.text = GameManager.Instance.EstaPausada()
            ? "X: Reanudar"
            : "X: Pausar";
    }

    void ToggleHistorial()
    {
        if (panelHistorial != null && panelHistorial.activeSelf)
            OcultarHistorial();
        else
            MostrarHistorial();
    }

    public void ActualizarUI()
    {
        ActualizarTextoPausa();
    }

    public void AlTerminarSesion()
    {
        RestaurarMenuPerfil();
        ActualizarTextoPausa();
    }

    public void RestaurarMenuPerfil()
    {
        OcultarHistorial();

        if (panelHistorial != null && canvasRaiz != null)
        {
            panelHistorial.transform.SetParent(canvasRaiz, false);
            panelHistorial.SetActive(false);
        }
    }

    void MostrarHistorial()
    {
        if (panelHistorial == null)
            CrearPanelHistorial();

        if (panelHistorial == null || textoHistorial == null)
            return;

        string nombre = "";
        if (PerfilManager.Instance != null && PerfilManager.Instance.perfilActual != null)
            nombre = PerfilManager.Instance.perfilActual.nombrePaciente;

        if (string.IsNullOrEmpty(nombre))
        {
            PerfilUI perfil = FindFirstObjectByType<PerfilUI>();
            if (perfil != null && perfil.inputNombre != null)
                nombre = perfil.inputNombre.text;
        }

        if (HistorialSesionesManager.Instance != null)
            textoHistorial.text = HistorialSesionesManager.Instance.FormatearHistorial(nombre);
        else
            textoHistorial.text = "Historial no disponible.";

        panelHistorial.SetActive(true);
        AsegurarPanelEnCanvasActivo();
        panelHistorial.transform.SetAsLastSibling();
    }

    void AsegurarPanelEnCanvasActivo()
    {
        if (panelHistorial == null)
            return;

        bool juegoActivo = hudRaiz != null && hudRaiz.gameObject.activeInHierarchy;
        Transform padre = juegoActivo ? hudRaiz : canvasRaiz;
        if (padre == null)
            padre = canvasRaiz != null ? canvasRaiz : hudRaiz;

        if (padre != null)
            panelHistorial.transform.SetParent(padre, false);

        VRUISetup.AsegurarLayerEnArbol(panelHistorial);
    }

    void OcultarHistorial()
    {
        if (panelHistorial != null)
            panelHistorial.SetActive(false);
    }
}

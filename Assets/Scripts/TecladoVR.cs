using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TecladoVR : MonoBehaviour
{
    [Header("Campos")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;

    [Header("Layout")]
    public float escalaTeclado = 0.68f;
    public float altoPanel = 175f;
    public float posicionVertical = 200f;

    TMP_InputField campoActivo;
    TMP_FontAsset fuente;
    GameObject tecladoRaiz;
    GameObject bloqueLetras;
    GameObject bloqueNumeros;
    GameObject botonEspacio;

    void Start()
    {
        if (inputNombre == null || inputEdad == null)
            BuscarReferencias();

        if (inputNombre == null || inputEdad == null)
        {
            Debug.LogWarning("[TecladoVR] No se encontraron los campos de perfil.");
            return;
        }

        fuente = inputNombre.textComponent.font;

        inputNombre.SetTextWithoutNotify("");
        inputEdad.SetTextWithoutNotify("");

        inputNombre.readOnly = true;
        inputEdad.readOnly = true;
        inputNombre.onSelect.AddListener(_ => MostrarParaNombre());
        inputEdad.onSelect.AddListener(_ => MostrarParaEdad());

        ConstruirTeclado();
        tecladoRaiz.SetActive(false);
    }

    public void AplicarPosicionVertical()
    {
        if (tecladoRaiz == null)
            return;

        RectTransform raizRect = tecladoRaiz.GetComponent<RectTransform>();
        raizRect.anchoredPosition = new Vector2(0f, posicionVertical);
    }

    void BuscarReferencias()
    {
        PerfilUI perfil = GetComponent<PerfilUI>();
        if (perfil == null)
            perfil = FindFirstObjectByType<PerfilUI>();

        if (perfil == null)
            return;

        inputNombre = perfil.inputNombre;
        inputEdad = perfil.inputEdad;
    }

    void ConstruirTeclado()
    {
        tecladoRaiz = new GameObject("TecladoVR", typeof(RectTransform));
        tecladoRaiz.transform.SetParent(transform, false);

        RectTransform raizRect = tecladoRaiz.GetComponent<RectTransform>();
        raizRect.anchorMin = new Vector2(0.5f, 0f);
        raizRect.anchorMax = new Vector2(0.5f, 0f);
        raizRect.pivot = new Vector2(0.5f, 0f);
        raizRect.anchoredPosition = new Vector2(0f, posicionVertical);
        raizRect.sizeDelta = new Vector2(620f, altoPanel);
        raizRect.localScale = Vector3.one * escalaTeclado;

        VerticalLayoutGroup vertical = tecladoRaiz.AddComponent<VerticalLayoutGroup>();
        vertical.childAlignment = TextAnchor.MiddleCenter;
        vertical.spacing = 4f;
        vertical.childControlWidth = false;
        vertical.childControlHeight = false;
        vertical.childForceExpandWidth = false;
        vertical.childForceExpandHeight = false;

        bloqueLetras = CrearBloqueLetras(tecladoRaiz.transform);
        bloqueNumeros = CrearBloqueNumeros(tecladoRaiz.transform);
        CrearFilaEspecial(tecladoRaiz.transform);
    }

    GameObject CrearBloqueLetras(Transform padre)
    {
        GameObject bloque = new GameObject("BloqueLetras", typeof(RectTransform));
        bloque.transform.SetParent(padre, false);

        RectTransform rect = bloque.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(600f, 130f);

        VerticalLayoutGroup vertical = bloque.AddComponent<VerticalLayoutGroup>();
        vertical.childAlignment = TextAnchor.MiddleCenter;
        vertical.spacing = 3f;
        vertical.childControlWidth = false;
        vertical.childControlHeight = false;

        CrearFilaTeclas(bloque.transform, "QWERTYUIOP", 42f);
        CrearFilaTeclas(bloque.transform, "ASDFGHJKL", 42f);
        CrearFilaTeclas(bloque.transform, "ZXCVBNM", 42f);

        return bloque;
    }

    GameObject CrearBloqueNumeros(Transform padre)
    {
        GameObject bloque = new GameObject("BloqueNumeros", typeof(RectTransform));
        bloque.transform.SetParent(padre, false);

        RectTransform rect = bloque.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(600f, 42f);

        CrearFilaTeclas(bloque.transform, "0123456789", 42f);
        bloque.SetActive(false);

        return bloque;
    }

    void CrearFilaTeclas(Transform padre, string teclas, float anchoBoton)
    {
        GameObject fila = CrearFila(padre, 38f);
        foreach (char c in teclas)
        {
            char letra = c;
            CrearBotonTecla(fila.transform, letra.ToString(), anchoBoton, () => AgregarTexto(letra.ToString()));
        }
    }

    void CrearFilaEspecial(Transform padre)
    {
        GameObject fila = CrearFila(padre, 38f);
        botonEspacio = CrearBotonTecla(fila.transform, "Espacio", 120f, () => AgregarTexto(" ")).gameObject;
        CrearBotonTecla(fila.transform, "Borrar", 85f, BorrarUltimo);
        CrearBotonTecla(fila.transform, "Enter", 95f, OcultarTeclado, new Color(0.1f, 0.55f, 0.25f, 1f));
    }

    GameObject CrearFila(Transform padre, float alto)
    {
        GameObject fila = new GameObject("Fila", typeof(RectTransform));
        fila.transform.SetParent(padre, false);

        RectTransform rect = fila.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(600f, alto);

        HorizontalLayoutGroup horizontal = fila.AddComponent<HorizontalLayoutGroup>();
        horizontal.childAlignment = TextAnchor.MiddleCenter;
        horizontal.spacing = 3f;
        horizontal.childControlWidth = false;
        horizontal.childControlHeight = false;
        horizontal.childForceExpandWidth = false;
        horizontal.childForceExpandHeight = false;

        return fila;
    }

    Button CrearBotonTecla(Transform padre, string etiqueta, float ancho, UnityEngine.Events.UnityAction accion, Color? color = null)
    {
        Button boton = CrearBotonBase(
            padre,
            etiqueta,
            new Vector2(ancho, 32f),
            color ?? new Color(0.22f, 0.28f, 0.38f, 1f)
        );
        boton.onClick.AddListener(accion);
        return boton;
    }

    Button CrearBotonBase(Transform padre, string etiqueta, Vector2 tamano, Color color)
    {
        GameObject go = new GameObject("Btn_" + etiqueta, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(padre, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = tamano;

        Image imagen = go.GetComponent<Image>();
        imagen.color = color;

        Button boton = go.GetComponent<Button>();
        ColorBlock colores = boton.colors;
        colores.highlightedColor = new Color(0.35f, 0.5f, 0.75f, 1f);
        colores.pressedColor = new Color(0.1f, 0.45f, 0.8f, 1f);
        boton.colors = colores;

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
        texto.fontSize = etiqueta.Length > 2 ? 14f : 18f;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = Color.white;

        return boton;
    }

    void MostrarParaNombre()
    {
        campoActivo = inputNombre;
        bloqueLetras.SetActive(true);
        bloqueNumeros.SetActive(false);
        if (botonEspacio != null)
            botonEspacio.SetActive(true);
        MostrarTeclado();
    }

    void MostrarParaEdad()
    {
        campoActivo = inputEdad;
        bloqueLetras.SetActive(false);
        bloqueNumeros.SetActive(true);
        if (botonEspacio != null)
            botonEspacio.SetActive(false);
        MostrarTeclado();
    }

    void MostrarTeclado()
    {
        tecladoRaiz.SetActive(true);
    }

    public void OcultarTeclado()
    {
        if (tecladoRaiz != null)
            tecladoRaiz.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    void AgregarTexto(string texto)
    {
        if (campoActivo == null)
            return;

        if (campoActivo == inputEdad && !EsDigito(texto))
            return;

        if (campoActivo == inputEdad && campoActivo.text.Length >= 3)
            return;

        campoActivo.text += texto;
    }

    void BorrarUltimo()
    {
        if (campoActivo == null || campoActivo.text.Length == 0)
            return;

        campoActivo.text = campoActivo.text.Substring(0, campoActivo.text.Length - 1);
    }

    bool EsDigito(string texto)
    {
        return texto.Length == 1 && char.IsDigit(texto[0]);
    }
}

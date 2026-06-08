using UnityEngine;

public class Contenedor : MonoBehaviour
{
    [Header("Color de este contenedor")]
    public Color colorContenedor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cristal"))
        {
            CristalMovimiento cristal = other.GetComponent<CristalMovimiento>();

            if (cristal != null && cristal.ColorMostrado())
            {
                Color colorCristal = cristal.GetColorAsignado();

                if (ColoresIguales(colorCristal, colorContenedor))
                {
                    Debug.Log("✅ Acierto!");
                    GameManager.Instance.SumarPunto();
                }
                else
                {
                    Debug.Log("❌ Error!");
                    GameManager.Instance.RestarPunto();
                }

                Destroy(other.gameObject);
            }
        }
    }

    bool ColoresIguales(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.1f &&
               Mathf.Abs(a.g - b.g) < 0.1f &&
               Mathf.Abs(a.b - b.b) < 0.1f;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "PacienteData", menuName = "FusionPlay/Paciente")]
public class PacienteData : ScriptableObject
{
    [Header("Datos Personales")]
    public string nombrePaciente;
    public int edad;

    [Header("Datos Clinicos")]
    public bool ojoDominanteDerecho = true;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;

public class Controlador : MonoBehaviour
{
    public float h;
    public float friccion;
    public float gravedad;
    public float fuerzaImanes;

    //Materiales para imanes
    public Material Positivo;
    public Material Negativo;
    public GameObject imanPrefab;

    //METODOS

    public List<Iman> imanes = new List<Iman>();

    public GameObject caja;
    void Start()
    {
        Iman[] imanesEnEscena = FindObjectsOfType<Iman>();
        imanes.AddRange(imanesEnEscena);

        foreach (Iman iman in imanes)
        {
            AsignarMaterial(iman);
        }
    }

    // Método para asignar el material según la polaridad del imán
    private void AsignarMaterial(Iman iman)
    {
        Renderer rend = iman.GetComponent<Renderer>();
        if (iman.polaridad == Iman.Polaridad.Positivo && Positivo != null)
        {
            rend.material = Positivo;
        }
        else if (iman.polaridad == Iman.Polaridad.Negativo && Negativo != null)
        {
            rend.material = Negativo;
        }
    }

    void Update()
    {
        foreach (Iman iman in imanes)
        {
            foreach (Iman otroIman in imanes)
            {
                if (iman != otroIman)
                {
                    // Calcular la fuerza entre el imán actual y otro imán
                    Vector3 fuerza = iman.CalcularFuerza(otroIman.transform.position, otroIman.polaridad, h, friccion, fuerzaImanes);

                    // Aplicar la fuerza al imán actual
                    iman.AplicarFuerza(fuerza, gravedad);
                }
            }
        }
    }
}

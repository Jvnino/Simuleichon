using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.tvOS;
using static UnityEditor.PlayerSettings;

public class Iman : MonoBehaviour
{
    //VARIABLES

    //Definir la polaridad del iman
    public enum Polaridad
    {
        Positivo,
        Negativo
    }

    //Asignar la selecci�n de la polaridad a una variable
    public Polaridad polaridad;

    //Distancia para que los imanes no se clipeen
    private float distanciaMinima = 1.0f;



    //Variables para crear el arraste de los imanes
    private bool isDragging = false;
    private Vector3 offset;

    private float distanciaMinimaEfectiva = 4.0f;

    //M�TODOS

    //  FUNCIONAMIENTO IM�N

    // M�todo para calcular la fuerza entre dos imanes
    public Vector3 CalcularFuerza(Vector3 posici�nOtroIm�n, Polaridad polaridadOtroIm�n, float h, float friccion, float fuerzaImanes)
    {
        //Se calcula la distancia entre los imanes
        float distancia = Vector3.Distance(transform.position, posici�nOtroIm�n);


        if (distancia > distanciaMinimaEfectiva)
        {
            return Vector3.zero; // No aplicar fuerza si est�n demasiado cerca
        }

        //Se asigna el atraer para las polaridades opuestas
        bool atraer = (polaridad == Polaridad.Positivo && polaridadOtroIm�n == Polaridad.Negativo) || (polaridad == Polaridad.Negativo && polaridadOtroIm�n == Polaridad.Positivo);

        //Ley de coulomb F= (k*q1*q1)/r*r
        float fuerza = fuerzaImanes / (distancia * distancia);
        
        //Si se repelen, la fuerza toma el lado contrario
        if (!atraer)
        {
            fuerza *= -1;
        }

        //Se calculan las direcciones a tomar
        Vector3 direcci�n = posici�nOtroIm�n - transform.position;
        direcci�n.Normalize();


        Vector3 fuerzaFinal = direcci�n * fuerza;


        // Aplicar efectos adicionales como h y fricci�n
        fuerzaFinal *= h;
        fuerzaFinal -= friccion * fuerzaFinal;

        // Si los imanes est�n muy cerca, evitan clippeos
        if (distancia < distanciaMinima)
        {
            Vector3 ajuste = direcci�n * (distanciaMinima - distancia) / 2f;
            transform.position -= ajuste;
        }

        return fuerzaFinal;
    }

    // M�todo para aplicar fuerza al im�n
    public void AplicarFuerza(Vector3 fuerza, float gravedad)
    {
        // Aplica fuerza solo si no est�s arrastrando el im�n
        if (!isDragging)
        {
            // Aplica fuerza
            transform.position += (fuerza + Vector3.down * gravedad) * Time.deltaTime;

            // Detecta colisi�n con el plano
            if (transform.position.y < transform.localScale.y / 2) // Si la posici�n Y del im�n est� por debajo del plano
            {
                transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z); // Ajusta la posici�n Y del im�n para que la esfera quede completamente sobre el plano
            }
        }
    }

    //FUNCIONAMIENTO ARRASTRE

    // M�todos para manejar el arrastre del im�n
    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
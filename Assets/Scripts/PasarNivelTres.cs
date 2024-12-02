using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasarNivelTres : MonoBehaviour
{
    // Detecta cuando el jugador entra en el área de la meta
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Verifica si es el jugador
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Carga el siguiente nivel
        }
    }
}

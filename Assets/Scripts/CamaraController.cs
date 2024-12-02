using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento = new Vector3(0, 5, -10);
    public float suavizado = 0.1f;

    // Tamaño de la cámara
    public float tamañoCamara = 8f; // Para ortográfica o campo de visión en perspectiva

    private Vector3 velocidad;
    private Camera camara;

    private void Start()
    {
        camara = GetComponent<Camera>();

        if (camara.orthographic)
        {
            camara.orthographicSize = tamañoCamara;
        }
        else
        {
            camara.fieldOfView = tamañoCamara;
        }
    }

    private void LateUpdate()
    {
        // Calcula la posición deseada con el desplazamiento
        Vector3 posicionDeseada = objetivo.position + desplazamiento;

        // Movimiento suave de la cámara
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
    }
}

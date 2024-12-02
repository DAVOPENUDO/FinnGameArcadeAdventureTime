using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento = new Vector3(0, 5, -10);
    public float suavizado = 0.1f;

    // Tama�o de la c�mara
    public float tama�oCamara = 8f; // Para ortogr�fica o campo de visi�n en perspectiva

    private Vector3 velocidad;
    private Camera camara;

    private void Start()
    {
        camara = GetComponent<Camera>();

        if (camara.orthographic)
        {
            camara.orthographicSize = tama�oCamara;
        }
        else
        {
            camara.fieldOfView = tama�oCamara;
        }
    }

    private void LateUpdate()
    {
        // Calcula la posici�n deseada con el desplazamiento
        Vector3 posicionDeseada = objetivo.position + desplazamiento;

        // Movimiento suave de la c�mara
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
    }
}

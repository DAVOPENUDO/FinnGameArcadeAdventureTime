using System.Collections;
using UnityEngine;

public class EnemyGorgon : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 12.0f;
    public float fuerzaRebote = 6f;
    public int vida = 3;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo;

    private Animator animator;
    private Vector3 escalaOriginal;

    private bool detectandoJugador; // Nuevo: Indica si el jugador está siendo detectado
    private bool persiguiendo; // Nuevo: Indica si el enemigo ya está persiguiendo
    private float tiempoDeteccion; // Nuevo: Tiempo en el que se detectó al jugador

    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 0.5f; // Ajusta la masa aquí
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        if (playerVivo && !muerto)
        {
            VerificarDeteccion();
            if (persiguiendo)
                Movimiento();
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerto", muerto);
    }

    private void VerificarDeteccion()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            if (!detectandoJugador)
            {
                detectandoJugador = true;
                tiempoDeteccion = Time.time; // Marca el tiempo actual
            }
            else if (Time.time - tiempoDeteccion >= 3f) // Verifica si han pasado 3 segundos
            {
                persiguiendo = true;
            }
        }
        else
        {
            detectandoJugador = false;
            persiguiendo = false;
        }
    }

    private void Movimiento()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x) * Mathf.Sign(direction.x), escalaOriginal.y, escalaOriginal.z);
        movement = new Vector2(direction.x, 0);
        enMovimiento = true;

        if (!recibiendoDanio)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime); // Incrementa speed si es necesario
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            playerScript.RecibeDanio(new Vector2(transform.position.x, 0), 1);
            playerVivo = !playerScript.muerto;
            if (!playerVivo)
                enMovimiento = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
            RecibeDanio(new Vector2(collision.gameObject.transform.position.x, 0), 1);
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio) return;

        vida -= cantDanio;
        recibiendoDanio = true;

        if (vida <= 0)
        {
            muerto = true;
            enMovimiento = false;
        }
        else
        {
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            StartCoroutine(DesactivaDanio());
        }
    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
    }

    public void EliminarCuerpo()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

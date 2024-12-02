using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 30.0f;
    public float speed = 2.0f;
    public float fuerzaRebote = 2f;
    public int vida = 3;

    public float sizeMultiplier = 1.5f; // Factor por el cual el enemigo se har� m�s ancho
    private float originalScaleX; // Escala original del enemigo en el eje X
    private bool isGrown = false; // Bandera para saber si el enemigo est� grande

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo;

    private Animator animator;

    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScaleX = transform.localScale.x; // Guardamos la escala original en el eje X
    }

    void Update()
    {
        if (playerVivo && !muerto)
        {
            Movimiento();
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerto", muerto);
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Cambia la escala del enemigo seg�n la direcci�n hacia el jugador
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-originalScaleX * (isGrown ? sizeMultiplier : 1), transform.localScale.y, 1);
            }
            else
            {
                transform.localScale = new Vector3(originalScaleX * (isGrown ? sizeMultiplier : 1), transform.localScale.y, 1);
            }

            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        if (!recibiendoDanio)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();

            playerScript.RecibeDanio(direccionDanio, 1);
            playerVivo = !playerScript.muerto;
            if (!playerVivo)
            {
                enMovimiento = false;
            }

            // El enemigo no debe cambiar de tama�o aqu�, solo lo hace si es necesario
            // Se puede llamar a HacerGrande solo si el enemigo debe cambiar de tama�o por alguna acci�n, no por colisi�n
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
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
    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
    }

    // Funci�n para hacer al enemigo m�s ancho
    public void HacerGrande()
    {
        if (!isGrown) // Evita que el enemigo crezca varias veces
        {
            isGrown = true;
            transform.localScale = new Vector3(originalScaleX * sizeMultiplier, transform.localScale.y, 1); // Solo cambia el tama�o en el eje X
        }
    }

    // Funci�n para devolver al enemigo a su tama�o original
    public void HacerNormal()
    {
        if (isGrown)
        {
            isGrown = false;
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, 1); // Restaura el tama�o solo en el eje X
        }
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

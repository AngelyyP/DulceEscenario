using UnityEngine;

public class FootstepSfx : MonoBehaviour
{
    public AudioClip[] clips;
    public float distanciaPaso = 0.75f;
    public float distanciaPasoCorriendo = 0.55f;
    public float velocidadCorrer = 5.5f;
    public Vector2 volumen = new Vector2(0.2f, 0.35f);
    public Vector2 pitch = new Vector2(0.85f, 1.12f);
    public float volumenGeneral = 1f;

    CharacterController controller;
    AudioSource fuente;
    float acumulado;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        fuente = GetComponent<AudioSource>();

        if (fuente == null)
        {
            fuente = gameObject.AddComponent<AudioSource>();
        }

        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 0f;
        fuente.spatialize = false;
        fuente.dopplerLevel = 0f;
        fuente.priority = 100;
    }

    void Update()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller == null || fuente == null) return;
        if (clips == null || clips.Length == 0) return;
        if (!controller.isGrounded) return;

        Vector3 velocidad = controller.velocity;
        velocidad.y = 0f;

        float rapidez = velocidad.magnitude;
        if (rapidez < 0.3f) return;

        float umbral = rapidez >= velocidadCorrer ? distanciaPasoCorriendo : distanciaPaso;
        acumulado += rapidez * Time.deltaTime;

        while (acumulado >= umbral)
        {
            acumulado -= umbral;
            Sonar();
        }
    }

    void Sonar()
    {
        fuente.pitch = Random.Range(pitch.x, pitch.y);
        fuente.PlayOneShot(clips[Random.Range(0, clips.Length)], Random.Range(volumen.x, volumen.y) * volumenGeneral);
    }
}

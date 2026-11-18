using UnityEngine;

public class AmbientRandomOneShot : MonoBehaviour
{
    public AudioClip[] clips;
    public Vector2 intervalo = new Vector2(6f, 16f);
    public Vector2 volumen = new Vector2(0.15f, 0.3f);
    public Vector2 pitch = new Vector2(0.85f, 1.15f);
    public float volumenGeneral = 1f;
    public float distanciaActivacion = 25f;
    public float distanciaMinima = 2f;
    public float distanciaMaxima = 18f;
    public int fuentesSimultaneas = 4;

    public bool usarArea = false;
    public Vector3 centroArea;
    public Vector3 tamanoArea = new Vector3(10f, 0f, 10f);

    AudioSource[] fuentes;
    Transform jugador;
    float siguiente;
    int indice;

    void Awake()
    {
        jugador = BuscarJugador();
        int cantidad = Mathf.Max(1, fuentesSimultaneas);
        fuentes = new AudioSource[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            GameObject hijo = new GameObject("OneShot_" + i);
            hijo.transform.SetParent(transform, false);
            AudioSource fuente = hijo.AddComponent<AudioSource>();
            ConfigurarFuente(fuente);
            fuentes[i] = fuente;
        }

        siguiente = Time.time + Random.Range(intervalo.x, intervalo.y);
    }

    void Update()
    {
        if (clips == null || clips.Length == 0) return;

        Vector3 centro = usarArea ? centroArea : transform.position;

        if (jugador != null && Vector3.Distance(jugador.position, centro) > distanciaActivacion)
        {
            siguiente = Mathf.Max(siguiente, Time.time + 1f);
            return;
        }

        if (Time.time < siguiente) return;

        Disparar();
        siguiente = Time.time + Random.Range(intervalo.x, intervalo.y);
    }

    void Disparar()
    {
        for (int i = 0; i < fuentes.Length; i++)
        {
            int k = (indice + i) % fuentes.Length;
            AudioSource fuente = fuentes[k];
            if (fuente.isPlaying) continue;

            indice = (k + 1) % fuentes.Length;
            fuente.transform.position = ElegirPosicion();
            fuente.clip = clips[Random.Range(0, clips.Length)];
            fuente.volume = Random.Range(volumen.x, volumen.y) * volumenGeneral;
            fuente.pitch = Random.Range(pitch.x, pitch.y);
            fuente.Play();
            return;
        }
    }

    Vector3 ElegirPosicion()
    {
        if (!usarArea) return transform.position;

        return new Vector3(
            centroArea.x + Random.Range(-tamanoArea.x * 0.5f, tamanoArea.x * 0.5f),
            centroArea.y + Random.Range(-tamanoArea.y * 0.5f, tamanoArea.y * 0.5f),
            centroArea.z + Random.Range(-tamanoArea.z * 0.5f, tamanoArea.z * 0.5f));
    }

    void ConfigurarFuente(AudioSource fuente)
    {
        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 1f;
        fuente.spatialize = false;
        fuente.dopplerLevel = 0f;
        fuente.rolloffMode = AudioRolloffMode.Custom;
        fuente.SetCustomCurve(AudioSourceCurveType.CustomRolloff, CurvaAtenuacion());
        fuente.minDistance = distanciaMinima;
        fuente.maxDistance = distanciaMaxima;
        fuente.priority = 128;
        fuente.spread = 0f;
        fuente.reverbZoneMix = 1f;
    }

    static AnimationCurve CurvaAtenuacion()
    {
        AnimationCurve curva = new AnimationCurve();
        curva.AddKey(0f, 1f);
        curva.AddKey(0.2f, 0.55f);
        curva.AddKey(0.5f, 0.25f);
        curva.AddKey(0.8f, 0.08f);
        curva.AddKey(1f, 0f);
        return curva;
    }

    static Transform BuscarJugador()
    {
        AudioListener escucha = Object.FindFirstObjectByType<AudioListener>();
        return escucha != null ? escucha.transform : null;
    }
}

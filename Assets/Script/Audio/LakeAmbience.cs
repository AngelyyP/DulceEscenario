using UnityEngine;

public class LakeAmbience : MonoBehaviour
{
    public AudioSource[] fuentes;
    public float volumen = 1f;
    public Vector2 rangoPitch = new Vector2(0.88f, 1.04f);
    public bool aleatorizarInicio = true;

    float[] volumenesBase;

    void Start()
    {
        if (fuentes == null || fuentes.Length == 0) return;

        volumenesBase = new float[fuentes.Length];

        for (int i = 0; i < fuentes.Length; i++)
        {
            AudioSource fuente = fuentes[i];
            if (fuente == null) continue;

            volumenesBase[i] = fuente.volume;
            fuente.loop = true;
            fuente.playOnAwake = false;
            fuente.pitch = Random.Range(rangoPitch.x, rangoPitch.y);

            if (aleatorizarInicio && fuente.clip != null)
            {
                fuente.time = Random.value * fuente.clip.length;
            }

            fuente.Play();
        }
    }

    void Update()
    {
        if (fuentes == null || volumenesBase == null) return;

        for (int i = 0; i < fuentes.Length; i++)
        {
            if (fuentes[i] != null)
            {
                fuentes[i].volume = volumenesBase[i] * volumen;
            }
        }
    }
}

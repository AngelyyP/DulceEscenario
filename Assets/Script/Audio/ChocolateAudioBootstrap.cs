using UnityEngine;

public static class ChocolateAudioBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Iniciar()
    {
        if (GameObject.Find("ChocolateLake") != null) return;

        GameObject viejo = GameObject.Find("ChocolateSound");
        if (viejo != null) Object.Destroy(viejo);

        AudioClip loopA = AsegurarMono(Cargar("Audio/Ambience/Chocolate_Loop_A"));
        AudioClip loopB = AsegurarMono(Cargar("Audio/Ambience/Chocolate_Loop_B"));
        AudioClip loopC = AsegurarMono(Cargar("Audio/Ambience/Chocolate_Loop_C"));
        AudioClip rumble = AsegurarMono(Cargar("Audio/Ambience/Chocolate_LowRumble"));

        if (loopA == null && loopB == null && loopC == null)
        {
            Debug.LogWarning("AUDIO: no se encontraron los loops del lago en Resources/Audio/Ambience.");
            return;
        }

        GameObject lago = new GameObject("ChocolateLake");
        lago.SetActive(false);
        lago.transform.position = new Vector3(-51f, 0f, 0.1f);

        AudioSource[] emisores = new AudioSource[6];
        int indice = 0;

        for (int xi = 0; xi < 3; xi++)
        {
            for (int zi = 0; zi < 2; zi++)
            {
                float x = -15f + xi * 15f;
                float z = -9f + zi * 18f;

                GameObject emisor = Nuevo("Lago_Emisor_" + (indice + 1), lago.transform, new Vector3(x, 0.35f, z));

                AudioClip clip;
                float volumen;

                if (indice % 3 == 0)
                {
                    clip = loopA;
                    volumen = 0.24f;
                }
                else if (indice % 3 == 1)
                {
                    clip = loopB;
                    volumen = 0.18f;
                }
                else
                {
                    clip = loopC;
                    volumen = 0.21f;
                }

                emisores[indice] = ConfigurarFuente3D(emisor, clip, volumen, 6f, 45f, 1f, 96, false, null);
                indice++;
            }
        }

        LakeAmbience ambiente = lago.AddComponent<LakeAmbience>();
        ambiente.fuentes = emisores;
        ambiente.volumen = 1f;
        ambiente.rangoPitch = new Vector2(0.88f, 1.04f);
        ambiente.aleatorizarInicio = true;

        AudioSource fuenteRumble = null;

        if (rumble != null)
        {
            GameObject objetoRumble = Nuevo("Lago_Rumble", lago.transform, new Vector3(0f, 0.2f, 0f));
            fuenteRumble = ConfigurarFuente3D(objetoRumble, rumble, 0.24f, 10f, 80f, 1f, 80, false, CurvaRumble());
        }

        AudioSource fuenteEspeso = null;

        if (loopC != null)
        {
            GameObject objetoEspeso = Nuevo("Lago_Espeso", lago.transform, new Vector3(0f, 0.3f, 0f));
            fuenteEspeso = ConfigurarFuente3D(objetoEspeso, loopC, 0.13f, 8f, 60f, 0.72f, 96, false, null);
        }

        AudioClip[] clipsGoteos = CargarVarios(
            "Audio/OneShots/Lake_Bubble_01",
            "Audio/OneShots/Lake_Bubble_02",
            "Audio/OneShots/Lake_Bubble_03",
            "Audio/OneShots/Lake_Slime_01",
            "Audio/OneShots/Lake_Slime_02",
            "Audio/OneShots/Lake_Slime_03",
            "Audio/OneShots/Lake_Slime_04",
            "Audio/OneShots/Lake_Slime_05",
            "Audio/OneShots/Lake_Slime_06",
            "Audio/OneShots/Lake_Splash_01",
            "Audio/OneShots/Lake_Splash_02",
            "Audio/OneShots/Lake_Splash_03",
            "Audio/OneShots/Lake_Plop_01",
            "Audio/OneShots/Lake_Plop_02");

        if (clipsGoteos.Length > 0)
        {
            GameObject goteos = Nuevo("Lago_Goteos", lago.transform, new Vector3(0f, 0.4f, 0f));
            AmbientRandomOneShot aleatorio = goteos.AddComponent<AmbientRandomOneShot>();
            aleatorio.clips = clipsGoteos;
            aleatorio.intervalo = new Vector2(1.5f, 4.5f);
            aleatorio.volumen = new Vector2(0.15f, 0.32f);
            aleatorio.pitch = new Vector2(0.7f, 1.15f);
            aleatorio.distanciaActivacion = 40f;
            aleatorio.distanciaMinima = 3f;
            aleatorio.distanciaMaxima = 30f;
            aleatorio.fuentesSimultaneas = 8;
            aleatorio.usarArea = true;
            aleatorio.centroArea = lago.transform.position + new Vector3(0f, 0.3f, 0f);
            aleatorio.tamanoArea = new Vector3(40f, 0f, 30f);
        }

        GameObject reverb = Nuevo("Lago_Reverb", lago.transform, Vector3.zero);
        AudioReverbZone zona = reverb.AddComponent<AudioReverbZone>();
        zona.reverbPreset = AudioReverbPreset.Cave;
        zona.minDistance = 8f;
        zona.maxDistance = 60f;

        AudioClip[] envolturas = CargarVarios(
            "Audio/Wrappers/Wrap_01",
            "Audio/Wrappers/Wrap_02",
            "Audio/Wrappers/Wrap_03",
            "Audio/Wrappers/Wrap_04");

        CrearAleatorio("Dulces_Envolturas_1", new Vector3(5.53f, 0.5f, -9.53f), envolturas,
            new Vector2(8f, 20f), new Vector2(0.12f, 0.25f), new Vector2(0.9f, 1.1f),
            22f, 2f, 18f, 3);

        CrearAleatorio("Dulces_Envolturas_2", new Vector3(0.91f, 3f, -0.3f), envolturas,
            new Vector2(8f, 20f), new Vector2(0.12f, 0.25f), new Vector2(0.9f, 1.1f),
            22f, 2f, 18f, 3);

        CrearAleatorio("Dulces_Envolturas_3", new Vector3(4.12f, 2.7f, 16.31f), envolturas,
            new Vector2(8f, 20f), new Vector2(0.12f, 0.25f), new Vector2(0.9f, 1.1f),
            22f, 2f, 18f, 3);

        AudioClip[] chispas = CargarVarios(
            "Audio/Sparkles/Sparkle_Bell_01",
            "Audio/Sparkles/Sparkle_Bell_02",
            "Audio/Sparkles/Sparkle_Bell_03");

        CrearAleatorio("Azucar_Sparkles_1", new Vector3(0.96f, 2.59f, 1.96f), chispas,
            new Vector2(4f, 10f), new Vector2(0.06f, 0.15f), new Vector2(1.8f, 2.4f),
            20f, 2f, 15f, 3);

        CrearAleatorio("Azucar_Sparkles_2", new Vector3(15.72f, 1.9f, -8.73f), chispas,
            new Vector2(4f, 10f), new Vector2(0.06f, 0.15f), new Vector2(1.8f, 2.4f),
            20f, 2f, 15f, 3);

        CrearAleatorio("Azucar_Sparkles_3", new Vector3(-9.94f, 1.6f, 6.84f), chispas,
            new Vector2(4f, 10f), new Vector2(0.06f, 0.15f), new Vector2(1.8f, 2.4f),
            20f, 2f, 15f, 3);

        GameObject musica = new GameObject("Musica");
        musica.SetActive(false);
        AudioSource fuenteMusica = musica.AddComponent<AudioSource>();
        AudioClip tema = Cargar("Audio/Music/TownTheme");

        if (tema != null)
        {
            fuenteMusica.clip = tema;
            fuenteMusica.loop = true;
            fuenteMusica.playOnAwake = false;
            fuenteMusica.volume = 0.12f;
            fuenteMusica.spatialBlend = 0f;
            fuenteMusica.spatialize = false;
            fuenteMusica.dopplerLevel = 0f;
            fuenteMusica.priority = 128;
            fuenteMusica.bypassReverbZones = true;
        }

        lago.SetActive(true);

        if (fuenteRumble != null) fuenteRumble.Play();
        if (fuenteEspeso != null) fuenteEspeso.Play();

        if (tema != null)
        {
            musica.SetActive(true);
            fuenteMusica.Play();
        }

        ConfigurarPasos();

        Debug.Log("AUDIO: espacio sonoro del lago de chocolate creado en " + lago.transform.position);
    }

    static void ConfigurarPasos()
    {
        GameObject rig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (rig == null) return;

        AudioSource fuentePasos = rig.GetComponent<AudioSource>();
        if (fuentePasos == null) fuentePasos = rig.AddComponent<AudioSource>();

        fuentePasos.playOnAwake = false;
        fuentePasos.loop = false;
        fuentePasos.spatialBlend = 0f;
        fuentePasos.spatialize = false;
        fuentePasos.dopplerLevel = 0f;
        fuentePasos.priority = 100;

        FootstepSfx pasos = rig.AddComponent<FootstepSfx>();
        pasos.clips = CargarVarios(
            "Audio/Footsteps/Step_Mud_01",
            "Audio/Footsteps/Step_Mud_02",
            "Audio/Footsteps/Step_Mud_03",
            "Audio/Footsteps/Step_Mud_05",
            "Audio/Footsteps/Step_Mud_07",
            "Audio/Footsteps/Step_Mud_09",
            "Audio/Footsteps/Step_Mud_12",
            "Audio/Footsteps/Step_Mud_15");
        pasos.distanciaPaso = 0.75f;
        pasos.distanciaPasoCorriendo = 0.55f;
        pasos.velocidadCorrer = 5.5f;
        pasos.volumen = new Vector2(0.18f, 0.3f);
        pasos.pitch = new Vector2(0.85f, 1.12f);
        pasos.volumenGeneral = 1f;
    }

    static void CrearAleatorio(string nombre, Vector3 posicion, AudioClip[] clips,
        Vector2 intervalo, Vector2 volumen, Vector2 pitch, float activacion, float minima, float maxima, int pool)
    {
        if (clips == null || clips.Length == 0) return;

        GameObject go = new GameObject(nombre);
        go.SetActive(false);
        go.transform.position = posicion;

        AmbientRandomOneShot comp = go.AddComponent<AmbientRandomOneShot>();
        comp.clips = clips;
        comp.intervalo = intervalo;
        comp.volumen = volumen;
        comp.pitch = pitch;
        comp.distanciaActivacion = activacion;
        comp.distanciaMinima = minima;
        comp.distanciaMaxima = maxima;
        comp.fuentesSimultaneas = pool;
        comp.usarArea = false;

        go.SetActive(true);
    }

    static AudioSource ConfigurarFuente3D(GameObject go, AudioClip clip, float volumen, float minima, float maxima,
        float pitch, int prioridad, bool autoPlay, AnimationCurve curva)
    {
        AudioSource fuente = go.AddComponent<AudioSource>();
        fuente.clip = clip;
        fuente.loop = true;
        fuente.playOnAwake = autoPlay;
        fuente.volume = volumen;
        fuente.pitch = pitch;
        fuente.spatialBlend = 1f;
        fuente.spatialize = false;
        fuente.spatializePostEffects = false;
        fuente.dopplerLevel = 0f;
        fuente.minDistance = minima;
        fuente.maxDistance = maxima;
        fuente.rolloffMode = AudioRolloffMode.Custom;
        fuente.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curva != null ? curva : CurvaLago());
        fuente.priority = prioridad;
        fuente.spread = 0f;
        fuente.reverbZoneMix = 1f;
        return fuente;
    }

    static AnimationCurve CurvaLago()
    {
        AnimationCurve curva = new AnimationCurve();
        curva.AddKey(0f, 1f);
        curva.AddKey(0.2f, 0.55f);
        curva.AddKey(0.5f, 0.25f);
        curva.AddKey(0.8f, 0.08f);
        curva.AddKey(1f, 0f);
        return curva;
    }

    static AnimationCurve CurvaRumble()
    {
        AnimationCurve curva = new AnimationCurve();
        curva.AddKey(0f, 1f);
        curva.AddKey(0.25f, 0.8f);
        curva.AddKey(0.6f, 0.45f);
        curva.AddKey(0.85f, 0.15f);
        curva.AddKey(1f, 0f);
        return curva;
    }

    static GameObject Nuevo(string nombre, Transform padre, Vector3 posicionLocal)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.transform.localPosition = posicionLocal;
        return go;
    }

    static AudioClip Cargar(string ruta)
    {
        AudioClip clip = Resources.Load<AudioClip>(ruta);
        if (clip == null) Debug.LogWarning("AUDIO: falta el clip " + ruta);
        return clip;
    }

    static AudioClip AsegurarMono(AudioClip clip)
    {
        if (clip == null || clip.channels <= 1) return clip;

        try
        {
            if (clip.loadState == AudioDataLoadState.Unloaded) clip.LoadAudioData();
            if (clip.loadState != AudioDataLoadState.Loaded) return clip;

            int canales = clip.channels;
            int muestras = clip.samples;
            float[] datos = new float[muestras * canales];

            if (!clip.GetData(datos, 0)) return clip;

            float[] mono = new float[muestras];

            for (int i = 0; i < muestras; i++)
            {
                float suma = 0f;

                for (int c = 0; c < canales; c++)
                {
                    suma += datos[i * canales + c];
                }

                mono[i] = suma / canales;
            }

            AudioClip nuevo = AudioClip.Create(clip.name + "_mono", muestras, 1, clip.frequency, false);
            nuevo.SetData(mono, 0);
            return nuevo;
        }
        catch
        {
            return clip;
        }
    }

    static AudioClip[] CargarVarios(params string[] rutas)
    {
        int validos = 0;
        AudioClip[] clips = new AudioClip[rutas.Length];

        for (int i = 0; i < rutas.Length; i++)
        {
            AudioClip clip = AsegurarMono(Cargar(rutas[i]));
            clips[i] = clip;
            if (clip != null) validos++;
        }

        if (validos == rutas.Length) return clips;

        AudioClip[] filtrados = new AudioClip[validos];
        int j = 0;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null)
            {
                filtrados[j] = clips[i];
                j++;
            }
        }

        return filtrados;
    }
}



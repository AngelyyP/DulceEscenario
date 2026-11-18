using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SetupAudio
{
    const string RutaEscena = "Assets/Scenes/SampleScene.unity";
    const string RutaAudio = "Assets/Resources/Audio";

    [MenuItem("DulceEscenario/Configurar audio espacial")]
    public static void RunAll()
    {
        try
        {
            GenerarRumble();
            AssetDatabase.Refresh();
            ConfigurarImportadores();
            ConfigurarEscena();
            Validar();
            Debug.Log("SETUPAUDIO_OK");
        }
        catch (Exception e)
        {
            Debug.LogError("SETUPAUDIO_ERROR: " + e);
            throw;
        }
    }

    static void GenerarRumble()
    {
        string carpeta = RutaAudio + "/Ambience";
        string ruta = carpeta + "/Chocolate_LowRumble.wav";
        Directory.CreateDirectory(carpeta);

        int frecuencia = 44100;
        int segundos = 20;
        int muestras = frecuencia * segundos;
        int fade = Mathf.RoundToInt(frecuencia * 0.75f);
        int total = muestras + fade;
        float[] datos = new float[total];

        System.Random rnd = new System.Random(20261118);
        float marron = 0f;
        float suave = 0f;

        for (int i = 0; i < total; i++)
        {
            float t = (float)i / frecuencia;
            float blanco = (float)(rnd.NextDouble() * 2.0 - 1.0);
            marron = Mathf.Clamp(marron * 0.9985f + blanco * 0.02f, -1f, 1f);
            suave += (marron - suave) * 0.08f;

            float lfo = 0.78f + 0.22f * Mathf.Sin(2f * Mathf.PI * 0.06f * t);
            float tono = Mathf.Sin(2f * Mathf.PI * (36f + 5f * Mathf.Sin(2f * Mathf.PI * 0.045f * t)) * t) * 0.12f;
            datos[i] = suave * 0.9f * lfo + tono * lfo;
        }

        for (int i = 0; i < fade; i++)
        {
            float mezcla = (float)i / fade;
            datos[i] = datos[i] * mezcla + datos[muestras + i] * (1f - mezcla);
        }

        float pico = 0.0001f;
        for (int i = 0; i < muestras; i++) pico = Mathf.Max(pico, Mathf.Abs(datos[i]));
        float escala = 0.7f / pico;

        using (FileStream fs = new FileStream(ruta, FileMode.Create, FileAccess.Write))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            int bytesDatos = muestras * 2;
            EscribirTexto(bw, "RIFF");
            bw.Write(36 + bytesDatos);
            EscribirTexto(bw, "WAVE");
            EscribirTexto(bw, "fmt ");
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(frecuencia);
            bw.Write(frecuencia * 2);
            bw.Write((short)2);
            bw.Write((short)16);
            EscribirTexto(bw, "data");
            bw.Write(bytesDatos);

            for (int i = 0; i < muestras; i++)
            {
                int valor = Mathf.Clamp(Mathf.RoundToInt(datos[i] * escala * 32767f), -32768, 32767);
                bw.Write((short)valor);
            }
        }

        AssetDatabase.ImportAsset(ruta, ImportAssetOptions.ForceUpdate);
        Debug.Log("SETUPAUDIO: rumble generado en " + ruta);
    }

    static void EscribirTexto(BinaryWriter bw, string texto)
    {
        bw.Write(Encoding.ASCII.GetBytes(texto));
    }

    static void ConfigurarImportadores()
    {
        ConfigurarClip("Ambience/Chocolate_Loop_A.wav", true, AudioClipLoadType.CompressedInMemory, 0.6f, true, true);
        ConfigurarClip("Ambience/Chocolate_Loop_B.wav", true, AudioClipLoadType.CompressedInMemory, 0.6f, true, true);
        ConfigurarClip("Ambience/Chocolate_Loop_C.ogg", true, AudioClipLoadType.CompressedInMemory, 0.6f, true, true);
        ConfigurarClip("Ambience/Chocolate_LowRumble.wav", true, AudioClipLoadType.CompressedInMemory, 0.5f, true, true);

        ConfigurarClip("OneShots/Lake_Bubble_01.wav", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Bubble_02.wav", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Bubble_03.wav", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Slime_01.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Slime_02.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Slime_03.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Slime_04.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Slime_05.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Slime_06.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Splash_01.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Splash_02.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Splash_03.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Plop_01.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        ConfigurarClip("OneShots/Lake_Plop_02.ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);

        int[] pasos = { 1, 2, 3, 5, 7, 9, 12, 15 };

        foreach (int numero in pasos)
        {
            ConfigurarClip("Footsteps/Step_Mud_" + numero.ToString("00") + ".ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        }

        for (int i = 1; i <= 4; i++)
        {
            ConfigurarClip("Wrappers/Wrap_" + i.ToString("00") + ".ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        }

        for (int i = 1; i <= 3; i++)
        {
            ConfigurarClip("Sparkles/Sparkle_Bell_" + i.ToString("00") + ".ogg", true, AudioClipLoadType.DecompressOnLoad, 0.7f, true, false);
        }

        ConfigurarClip("Music/TownTheme.mp3", false, AudioClipLoadType.CompressedInMemory, 0.7f, true, true);
    }

    static void ConfigurarClip(string relativa, bool mono, AudioClipLoadType carga, float calidad, bool precargar, bool segundoPlano)
    {
        string ruta = RutaAudio + "/" + relativa;
        AudioImporter imp = AssetImporter.GetAtPath(ruta) as AudioImporter;

        if (imp == null)
        {
            throw new Exception("No se pudo importar el audio: " + ruta);
        }

        imp.forceToMono = mono;
        imp.loadInBackground = segundoPlano;
        imp.ambisonic = false;

        AudioImporterSampleSettings ajustes = imp.defaultSampleSettings;
        ajustes.loadType = carga;
        ajustes.compressionFormat = AudioCompressionFormat.Vorbis;
        ajustes.quality = calidad;
        ajustes.preloadAudioData = precargar;
        ajustes.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;

        imp.defaultSampleSettings = ajustes;
        imp.SetOverrideSampleSettings("Android", ajustes);
        imp.SaveAndReimport();
    }

    static void ConfigurarEscena()
    {
        EditorSceneManager.OpenScene(RutaEscena, OpenSceneMode.Single);

        DestruirSiExiste("ChocolateSound");
        DestruirSiExiste("ChocolateLake");
        DestruirSiExiste("Dulces_Envolturas_1");
        DestruirSiExiste("Dulces_Envolturas_2");
        DestruirSiExiste("Dulces_Envolturas_3");
        DestruirSiExiste("Azucar_Sparkles_1");
        DestruirSiExiste("Azucar_Sparkles_2");
        DestruirSiExiste("Azucar_Sparkles_3");
        DestruirSiExiste("Musica");

        AudioClip loopA = Clip("Ambience/Chocolate_Loop_A.wav");
        AudioClip loopB = Clip("Ambience/Chocolate_Loop_B.wav");
        AudioClip loopC = Clip("Ambience/Chocolate_Loop_C.ogg");
        AudioClip rumble = Clip("Ambience/Chocolate_LowRumble.wav");

        GameObject lago = new GameObject("ChocolateLake");
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

        GameObject objetoRumble = Nuevo("Lago_Rumble", lago.transform, new Vector3(0f, 0.2f, 0f));
        ConfigurarFuente3D(objetoRumble, rumble, 0.24f, 10f, 80f, 1f, 80, true, CurvaRumble());

        GameObject objetoEspeso = Nuevo("Lago_Espeso", lago.transform, new Vector3(0f, 0.3f, 0f));
        ConfigurarFuente3D(objetoEspeso, loopC, 0.13f, 8f, 60f, 0.72f, 96, true, null);

        GameObject goteos = Nuevo("Lago_Goteos", lago.transform, new Vector3(0f, 0.4f, 0f));
        AmbientRandomOneShot aleatorioLago = goteos.AddComponent<AmbientRandomOneShot>();
        aleatorioLago.clips = new AudioClip[]
        {
            Clip("OneShots/Lake_Bubble_01.wav"),
            Clip("OneShots/Lake_Bubble_02.wav"),
            Clip("OneShots/Lake_Bubble_03.wav"),
            Clip("OneShots/Lake_Slime_01.ogg"),
            Clip("OneShots/Lake_Slime_02.ogg"),
            Clip("OneShots/Lake_Slime_03.ogg"),
            Clip("OneShots/Lake_Slime_04.ogg"),
            Clip("OneShots/Lake_Slime_05.ogg"),
            Clip("OneShots/Lake_Slime_06.ogg"),
            Clip("OneShots/Lake_Splash_01.ogg"),
            Clip("OneShots/Lake_Splash_02.ogg"),
            Clip("OneShots/Lake_Splash_03.ogg"),
            Clip("OneShots/Lake_Plop_01.ogg"),
            Clip("OneShots/Lake_Plop_02.ogg")
        };
        aleatorioLago.intervalo = new Vector2(1.5f, 4.5f);
        aleatorioLago.volumen = new Vector2(0.15f, 0.32f);
        aleatorioLago.pitch = new Vector2(0.7f, 1.15f);
        aleatorioLago.distanciaActivacion = 40f;
        aleatorioLago.distanciaMinima = 3f;
        aleatorioLago.distanciaMaxima = 30f;
        aleatorioLago.fuentesSimultaneas = 8;
        aleatorioLago.usarArea = true;
        aleatorioLago.centroArea = lago.transform.position + new Vector3(0f, 0.3f, 0f);
        aleatorioLago.tamanoArea = new Vector3(40f, 0f, 30f);

        GameObject reverb = Nuevo("Lago_Reverb", lago.transform, Vector3.zero);
        AudioReverbZone zona = reverb.AddComponent<AudioReverbZone>();
        zona.reverbPreset = AudioReverbPreset.Cave;
        zona.minDistance = 8f;
        zona.maxDistance = 60f;

        AudioClip[] envolturas =
        {
            Clip("Wrappers/Wrap_01.ogg"),
            Clip("Wrappers/Wrap_02.ogg"),
            Clip("Wrappers/Wrap_03.ogg"),
            Clip("Wrappers/Wrap_04.ogg")
        };

        CrearAleatorio("Dulces_Envolturas_1", new Vector3(5.53f, 0.5f, -9.53f), envolturas,
            new Vector2(8f, 20f), new Vector2(0.12f, 0.25f), new Vector2(0.9f, 1.1f),
            22f, 2f, 18f, 3, false, Vector3.zero, Vector3.zero);

        CrearAleatorio("Dulces_Envolturas_2", new Vector3(0.91f, 3f, -0.3f), envolturas,
            new Vector2(8f, 20f), new Vector2(0.12f, 0.25f), new Vector2(0.9f, 1.1f),
            22f, 2f, 18f, 3, false, Vector3.zero, Vector3.zero);

        CrearAleatorio("Dulces_Envolturas_3", new Vector3(4.12f, 2.7f, 16.31f), envolturas,
            new Vector2(8f, 20f), new Vector2(0.12f, 0.25f), new Vector2(0.9f, 1.1f),
            22f, 2f, 18f, 3, false, Vector3.zero, Vector3.zero);

        AudioClip[] chispas =
        {
            Clip("Sparkles/Sparkle_Bell_01.ogg"),
            Clip("Sparkles/Sparkle_Bell_02.ogg"),
            Clip("Sparkles/Sparkle_Bell_03.ogg")
        };

        CrearAleatorio("Azucar_Sparkles_1", new Vector3(0.96f, 2.59f, 1.96f), chispas,
            new Vector2(4f, 10f), new Vector2(0.06f, 0.15f), new Vector2(1.8f, 2.4f),
            20f, 2f, 15f, 3, false, Vector3.zero, Vector3.zero);

        CrearAleatorio("Azucar_Sparkles_2", new Vector3(15.72f, 1.9f, -8.73f), chispas,
            new Vector2(4f, 10f), new Vector2(0.06f, 0.15f), new Vector2(1.8f, 2.4f),
            20f, 2f, 15f, 3, false, Vector3.zero, Vector3.zero);

        CrearAleatorio("Azucar_Sparkles_3", new Vector3(-9.94f, 1.6f, 6.84f), chispas,
            new Vector2(4f, 10f), new Vector2(0.06f, 0.15f), new Vector2(1.8f, 2.4f),
            20f, 2f, 15f, 3, false, Vector3.zero, Vector3.zero);

        GameObject musica = new GameObject("Musica");
        musica.transform.position = Vector3.zero;
        AudioSource fuenteMusica = musica.AddComponent<AudioSource>();
        fuenteMusica.clip = Clip("Music/TownTheme.mp3");
        fuenteMusica.loop = true;
        fuenteMusica.playOnAwake = true;
        fuenteMusica.volume = 0.12f;
        fuenteMusica.spatialBlend = 0f;
        fuenteMusica.spatialize = false;
        fuenteMusica.dopplerLevel = 0f;
        fuenteMusica.priority = 128;
        fuenteMusica.bypassReverbZones = true;

        GameObject rig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (rig == null)
        {
            throw new Exception("No se encontro [BuildingBlock] Camera Rig en la escena.");
        }

        FootstepSfx pasosViejos = rig.GetComponent<FootstepSfx>();
        if (pasosViejos != null) UnityEngine.Object.DestroyImmediate(pasosViejos);

        AudioSource fuentePasos = rig.GetComponent<AudioSource>();
        if (fuentePasos == null) fuentePasos = rig.AddComponent<AudioSource>();
        fuentePasos.playOnAwake = false;
        fuentePasos.loop = false;
        fuentePasos.spatialBlend = 0f;
        fuentePasos.spatialize = false;
        fuentePasos.dopplerLevel = 0f;
        fuentePasos.priority = 100;

        FootstepSfx pasos = rig.AddComponent<FootstepSfx>();
        pasos.clips = new AudioClip[]
        {
            Clip("Footsteps/Step_Mud_01.ogg"),
            Clip("Footsteps/Step_Mud_02.ogg"),
            Clip("Footsteps/Step_Mud_03.ogg"),
            Clip("Footsteps/Step_Mud_05.ogg"),
            Clip("Footsteps/Step_Mud_07.ogg"),
            Clip("Footsteps/Step_Mud_09.ogg"),
            Clip("Footsteps/Step_Mud_12.ogg"),
            Clip("Footsteps/Step_Mud_15.ogg")
        };
        pasos.distanciaPaso = 0.75f;
        pasos.distanciaPasoCorriendo = 0.55f;
        pasos.velocidadCorrer = 5.5f;
        pasos.volumen = new Vector2(0.18f, 0.3f);
        pasos.pitch = new Vector2(0.85f, 1.12f);
        pasos.volumenGeneral = 1f;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        Debug.Log("SETUPAUDIO: escena configurada con lago en " + lago.transform.position);
    }

    static AmbientRandomOneShot CrearAleatorio(string nombre, Vector3 posicion, AudioClip[] clips,
        Vector2 intervalo, Vector2 volumen, Vector2 pitch, float activacion, float minima, float maxima,
        int pool, bool usarArea, Vector3 centro, Vector3 tamano)
    {
        GameObject go = new GameObject(nombre);
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
        comp.usarArea = usarArea;
        comp.centroArea = centro;
        comp.tamanoArea = tamano;
        return comp;
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

    static AudioClip Clip(string relativa)
    {
        string ruta = RutaAudio + "/" + relativa;
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(ruta);

        if (clip == null)
        {
            throw new Exception("Falta el clip de audio: " + ruta);
        }

        return clip;
    }

    static void DestruirSiExiste(string nombre)
    {
        GameObject go = GameObject.Find(nombre);
        if (go != null)
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    static void Validar()
    {
        AudioSource[] fuentes = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        int sinClip = 0;
        int espaciales = 0;

        foreach (AudioSource fuente in fuentes)
        {
            if (fuente.clip == null) sinClip++;
            else if (fuente.spatialBlend > 0.5f) espaciales++;
        }

        Debug.Log("SETUPAUDIO: AudioSources=" + fuentes.Length + " espaciales=" + espaciales + " sinClip=" + sinClip);
    }
}






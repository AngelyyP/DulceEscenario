using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

public static class SetupVR
{
    const string RutaEscena = "Assets/Scenes/SampleScene.unity";
    const string RutaApk = "Builds/DulceEscenario.apk";

    [MenuItem("DulceEscenario/Configurar proyecto y compilar Quest")]
    public static void RunAll()
    {
        try
        {
            ConfigurarXR();
            ConfigurarEscena();
            ConfigurarPlayerSettings();
            Compilar();
        }
        catch (Exception e)
        {
            Debug.LogError("SETUPVR_ERROR: " + e);
            throw;
        }
    }

    static void ConfigurarXR()
    {
        FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);

        MetaQuestFeature soporteQuest = FeatureHelpers.GetFeatureWithIdForBuildTarget(
            BuildTargetGroup.Android, MetaQuestFeature.featureId) as MetaQuestFeature;

        if (soporteQuest == null)
        {
            throw new Exception("No se encontro el feature Meta Quest Support para Android.");
        }

        if (!soporteQuest.enabled)
        {
            soporteQuest.enabled = true;
            EditorUtility.SetDirty(soporteQuest);
            Debug.Log("SETUPVR: Meta Quest Support activado (Android).");
        }

        OpenXRSettings ajustes = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
        if (ajustes != null)
        {
            EditorUtility.SetDirty(ajustes);
        }

        AssetDatabase.SaveAssets();
    }

    static void ConfigurarEscena()
    {
        EditorSceneManager.OpenScene(RutaEscena, OpenSceneMode.Single);

        GameObject rig = GameObject.Find("[BuildingBlock] Camera Rig");
        if (rig == null)
        {
            throw new Exception("No se encontro [BuildingBlock] Camera Rig en la escena.");
        }

        rig.transform.position = new Vector3(1.6f, 1.2f, -15.71f);
        rig.transform.rotation = Quaternion.identity;

        if (rig.GetComponent<VRLocomotion>() == null)
        {
            rig.AddComponent<VRLocomotion>();
            Debug.Log("SETUPVR: VRLocomotion agregado al Camera Rig.");
        }

        GameObject jugador = GameObject.Find("Player");
        if (jugador != null)
        {
            jugador.SetActive(false);
            Debug.Log("SETUPVR: Player de teclado desactivado.");
        }

        GameObject ojoIzquierdo = GameObject.Find("LeftEyeAnchor");
        if (ojoIzquierdo != null && ojoIzquierdo.tag == "MainCamera")
        {
            ojoIzquierdo.tag = "Untagged";
            Debug.Log("SETUPVR: tag MainCamera quitado de LeftEyeAnchor.");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void ConfigurarPlayerSettings()
    {
        PlayerSettings.productName = "DulceEscenario";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.angely.dulceescenario");
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel32;

        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[]
        {
            GraphicsDeviceType.Vulkan,
            GraphicsDeviceType.OpenGLES3
        });

        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

        AssetDatabase.SaveAssets();
        Debug.Log("SETUPVR: PlayerSettings Android configurados.");
    }

    static void Compilar()
    {
        Directory.CreateDirectory("Builds");

        BuildPlayerOptions opciones = new BuildPlayerOptions
        {
            scenes = new[] { RutaEscena },
            locationPathName = RutaApk,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        BuildReport reporte = BuildPipeline.BuildPlayer(opciones);
        BuildSummary resumen = reporte.summary;

        Debug.Log("BUILD_RESULT: " + resumen.result +
                  " errores=" + resumen.totalErrors +
                  " tamano=" + resumen.totalSize +
                  " salida=" + RutaApk);

        if (resumen.result != BuildResult.Succeeded)
        {
            throw new Exception("El build fallo: " + resumen.result);
        }
    }
}

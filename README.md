# DulceEscenario VR

Escenario de dulces en realidad virtual para Meta Quest, con un lago de chocolate y audio espacial 3D. Al acercarse al lago se escuchan burbujeos, goteos y plops; al alejarse, el volumen baja.

## Requisitos

- Unity **6000.0.48f1** con soporte de compilación para Android (SDK, NDK y OpenJDK instalados desde Unity Hub).
- Meta Quest 2, Quest 3 o Quest Pro con modo desarrollador activado.
- SideQuest, Meta Quest Developer Hub o ADB para instalar el APK.

## Abrir el proyecto

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/AngelyyP/DulceEscenario.git
   ```

2. En Unity Hub, seleccionar **Add project from disk** y elegir la carpeta clonada.
3. Abrirlo con Unity **6000.0.48f1**. Unity descargará los paquetes de `Packages/manifest.json`.
4. Abrir `Assets/Scenes/SampleScene.unity` y presionar Play.

El audio se crea automáticamente al iniciar la escena mediante `Assets/Script/Audio/ChocolateAudioBootstrap.cs`, así que no hace falta configurar nada extra.

## Compilar el APK

1. Ir a **File > Build Settings** y seleccionar **Android**; usar **Switch Platform** si es necesario.
2. Ejecutar el menú **DulceEscenario > Configurar proyecto y compilar Quest**.
   - Configura XR/OpenXR, la escena y los Player Settings.
   - Genera `Builds/DulceEscenario.apk`.
3. Alternativa: usar **Build** desde Build Settings con `Assets/Scenes/SampleScene.unity` como única escena.

El APK no se incluye en el repositorio; se genera localmente con los pasos anteriores.

## Instalar en las gafas

1. Activar el modo desarrollador en la app de Meta Horizon.
2. Conectar el Quest por USB y aceptar el permiso de depuración.
3. Instalar el APK con SideQuest / Meta Quest Developer Hub, o desde una terminal:

   ```bash
   adb install -r Builds/DulceEscenario.apk
   ```

4. Abrir **DulceEscenario** desde **Unknown Sources** en el Quest.

Paquete Android: `com.angely.dulceescenario`.

## Controles

- **Stick izquierdo**: moverse.
- **Grip izquierdo**: correr.
- **Botón A derecho**: saltar.
- **Stick derecho**: giro por pasos de 30 grados.

## Audio espacial

- El lago de chocolate está aproximadamente en `x = -51`, `z = 0.1`, al oeste de la sala de dulces. Caminar hacia el sonido y hacia esa zona.
- La atenuación es 3D: cerca del lago suena fuerte y al alejarse disminuye.
- Capas de audio:
  - Lago: loops de burbujeo y capa grave.
  - Goteos y plops aleatorios sobre la superficie.
  - Crujido de envolturas en las zonas de dulces.
  - Chispas de azúcar cerca de la zona `AZUCAR`.
  - Pasos pegajosos sobre el piso de chocolate.
  - Música dulce de fondo.
- Para ajustar el volumen durante Play, seleccionar los objetos creados (`ChocolateLake`, `Musica`, etc.) o los valores públicos:
  - `LakeAmbience.volumen`
  - `FootstepSfx.volumenGeneral`
  - `AmbientRandomOneShot.volumenGeneral`
- Opcional: el menú **DulceEscenario > Configurar audio espacial** hornea los emisores en la escena para verlos y editarlos directamente. Si se ejecuta, el bootstrap detecta la escena ya configurada y no duplica el audio.

## Estructura relevante

- `Assets/Script/Audio/ChocolateAudioBootstrap.cs`: crea el espacio sonoro al iniciar.
- `Assets/Script/Audio/LakeAmbience.cs`: loops del lago con inicio y pitch aleatorios.
- `Assets/Script/Audio/AmbientRandomOneShot.cs`: goteos, envolturas y chispas aleatorias.
- `Assets/Script/Audio/FootstepSfx.cs`: pasos pegajosos.
- `Assets/Resources/Audio/`: clips de audio.
- `Assets/Editor/SetupAudio.cs`: configuración opcional del audio en la escena.
- `Assets/Editor/SetupVR.cs`: configuración XR y compilación del APK.

## Créditos de audio (CC0)

Sonidos de [OpenGameArt](https://opengameart.org):

- **Bubble Sound Effects** por BMacZero.
- **25 CC0 mud sfx** por rubberduck.
- **40 CC0 water / splash / slime SFX** por rubberduck.
- **100 CC0 SFX** por rubberduck.
- **Town Theme RPG** por cynicmusic.

El rumble grave del chocolate se generó proceduralmente para este proyecto.

## Assets de terceros

El proyecto incluye paquetes y assets de terceros (por ejemplo, Stylized Environment - Free Pack, BOXOPHOBIC Skybox Cubemap Extended y el Meta XR SDK). Dichos assets conservan sus licencias originales y no se reclama propiedad sobre ellos.

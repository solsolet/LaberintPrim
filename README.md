# Juego Laberinto de Prim
Minijuego en Unity en el que el tablero se genere automáticamente mediante un algoritmo de generación de laberintos basado en Prim.

## TODO
### Básico
- [x] Implementar un generador procedural de laberintos
- [x] Construir un tablero jugable a partir del laberinto generado
- [x] Implementar físicas, sistema de puntuación y progresión de dificultad
- [x] Integrar control mediante teclado (PC) o giroscopio (en móvil)

### Opcional
- [x] Animaciones
- [x] Sonidos
- [x] Interfaz mejorada. <!--TODO: Cambiar palabras label por emojis-->
- [x] Efectos visuales.
- [ ] Minimapa.
- [x] Cámara dinámica
- [ ] Generación del laberinto paso a paso visualizada.
- [ ] Comparación con otro algoritmo (DFS, Kruskal).

## Demo
Se puede ver la demo del proyecto en la carpeta de este zip como demo_LaberintoPrim.mp4. Cualquier problema con la versión entregada por Moodle (tanto del proyecto como del README) se puede usar el repositorio donde se encuentra alojada la práctica: [https://github.com/solsolet/LaberintPrim.git](https://github.com/solsolet/LaberintPrim.git).

Para probar la aplicación directamente en un dispositivo **Android** (SDK <= 35) se puede instalar la APK: [LaberintPrim.apk](LaberintPrim.apk) o descargándola de [GitHub](https://github.com/solsolet/LaberintPrim/releases/tag/Android).

Y para probarla en un dispositivo Windows (Intel 64-bit architecture) se puede usar el zip: [LaberintPrim_Windows](LaberintPrim_Windows.zip) o descargándola de [GitHub](https://github.com/solsolet/LaberintPrim/releases/tag/Windows).

## 📋 Resumen de la arquitectura
```bash
Assets/
├── Prefabs/
│   ├── BallPrefab.prefab          # La pelota — Rigidbody + BallController + SphereCollider
│   ├── WallPrefab.prefab          # Muro del laberinto — BoxCollider sólido
│   ├── FloorPrefab.prefab         # Suelo del laberinto — BoxCollider sólido
│   ├── HolePrefab.prefab          # Agujero trampa — BoxCollider trigger + HoleTrap
│   ├── ExitPrefab.prefab          # Casilla de salida — BoxCollider trigger + ExitZone
│   ├── CollectableSmall.prefab    # Coleccionable pequeño (10 pts) — trigger + Collectable + FloatAnimation
│   ├── CollectableMedium.prefab   # Coleccionable mediano (25 pts) — trigger + Collectable + FloatAnimation
│   └── CollectableLarge.prefab    # Coleccionable grande  (50 pts) — trigger + Collectable + FloatAnimation
│
├── Scenes/
│   └── MainScene.unity            # Escena única del juego
│
└── Scripts/
    ├── GameManager.cs             # Singleton — gestiona vidas, puntuación, niveles y flujo de juego
    ├── UIManager.cs               # Actualiza el HUD y muestra/oculta los paneles de victoria y derrota
    ├── MazeGenerator.cs           # Algoritmo de Prim — genera la matriz 2D del laberinto
    ├── MazeRenderer.cs            # Instancia prefabs a partir de la matriz, coloca coleccionables y agujeros
    ├── BallController.cs          # Mueve la pelota con teclado (PC) y giroscopio (móvil)
    ├── Collectable.cs             # Al entrar en contacto con la pelota suma puntos y se destruye
    ├── HoleTrap.cs                # Al entrar en contacto con la pelota llama a GameManager.FallInHole()
    ├── ExitZone.cs                # Al entrar en contacto con la pelota llama a GameManager.ReachExit()
    ├── FloatAnimation.cs          # Animación de flotación y rotación para los coleccionables
    ├── CollectEffect.cs   # Burst de partículas al recoger un coleccionable
    ├── HoleEffect.cs      # Burst de partículas al caer en un agujero  
    ├── CameraFollow.cs    # Cámara que sigue la pelota con transición zoom-out/in
    └── SafeArea.cs                # Ajusta el HUD al área segura del dispositivo (notch, barra inferior)
```

## Implementación
### Decisiones de diseño
**Una sola escena.** En lugar de cargar una escena nueva por nivel, el juego borra y regenera el laberinto dentro de la misma escena. Esto evita tiempos de carga, simplifica la gestión del estado y hace que la transición entre niveles sea instantánea.
 
**Singleton para GameManager.** Se eligió el patrón Singleton para `GameManager` porque es el punto central de coordinación entre todos los sistemas (UI, laberinto, pelota). Desde cualquier script basta con `GameManager.Instance.MétodoQueSea()` sin necesidad de guardar referencias adicionales.
 
**Separación MazeGenerator / MazeRenderer.** El generador solo devuelve una matriz de enteros (`0` = muro, `1` = camino). El renderer es el único que sabe de prefabs y Unity. Esto permite cambiar el algoritmo de generación sin tocar el código de renderizado, y viceversa.
 
**Configuración de niveles serializada.** El array `LevelConfig[]` en `GameManager` es serializable, por lo que los parámetros de cada nivel (tamaño, agujeros, vidas, coleccionables) son editables directamente desde el Inspector sin tocar código.
 
**Control dual teclado + giroscopio.** `BallController` detecta en `Awake()` si el dispositivo tiene giroscopio (`SystemInfo.supportsGyroscope`). Si lo tiene, ambas entradas se suman, lo que permite probar en el editor con teclado y jugar en móvil con inclinación sin cambiar ningún ajuste.
 
**Cámara cenital adaptativa.** `MazeRenderer.PositionCamera()` calcula la altura de la cámara a partir del FOV y el aspect ratio real de la pantalla, de forma que el laberinto completo siempre cabe en pantalla independientemente del dispositivo y la orientación.

### Algoritmo
Se implementa la **versión de Prim para generación de laberintos**, que es una adaptación del algoritmo de árbol de expansión mínima de Prim aplicada a una rejilla.
 
**Representación:** una matriz 2D de enteros donde `0` es muro y `1` es camino. Las dimensiones deben ser siempre impares: las celdas de camino potencial ocupan posiciones `(impar, impar)` y los muros entre ellas ocupan las posiciones intermedias.
 
**Pasos del algoritmo:**
 
1. Inicializar toda la matriz como muro (`0`).
2. Marcar la celda `(1,1)` como camino y añadir sus vecinas a dos pasos de distancia a la lista de frontera.
3. Mientras la lista de frontera no esté vacía:
   - Elegir una celda aleatoria de la frontera.
   - Obtener sus vecinas ya visitadas (a dos pasos de distancia).
   - Si tiene al menos una vecina visitada:
     - Elegir una de ellas al azar.
     - Marcar la celda frontera y la celda intermedia (el muro entre ambas) como camino.
     - Añadir las nuevas vecinas no visitadas a la frontera.
4. Garantizar que la celda de salida `(ancho-2, alto-2)` es camino abriendo un pasillo si fuera necesario.
 
**Propiedades resultantes:** el algoritmo garantiza que existe exactamente un camino entre cualquier par de celdas (árbol de expansión), por lo que siempre hay un camino válido de entrada a salida. La aleatoriedad en la selección de la celda frontera produce laberintos con muchos pasillos cortos y ramificaciones frecuentes, diferente al DFS que tiende a generar pasillos largos y serpenteantes.

## Problemas encontrados
### Con Unity
**Fuente de letra no encontrada**
Al añadir componentes `TextMeshPro` por primera vez, Unity solicitó importar el paquete de recursos esenciales de TMP (ventana emergente *TMP Importer*). Sin aceptar esta importación las fuentes no se cargaban y los textos aparecían en blanco. Solución: aceptar la importación desde la ventana emergente o desde *Window → TextMeshPro → Import TMP Essential Resources*.

**Instalación del módulo de Android**
Unity Hub producía un error durante la descarga del módulo Android Build Support en la versión `6000.3.10f1`. Los logs de Hub (`app.log`) mostraban únicamente tráfico normal del gestor de paquetes UPM, sin un mensaje de error explícito. Las causas posibles son restricciones de red (proxy universitario) o espacio en disco insuficiente. Solución alternativa: descargar el instalador del módulo directamente desde el archivo de versiones de Unity (`unity.com/releases/editor/archive`) e instalarlo de forma independiente a Hub. Después de lograr instalarlo me temo que era culpa de mi conexión.

**Sistema de entrada desactualizado**
Al ejecutar el proyecto por primera vez aparecía el error `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package`. Unity 6 activa por defecto el nuevo Input System, pero `BallController` usaba la API antigua (`Input.GetAxis`). Solución: en *Edit → Project Settings → Player → Other Settings → Active Input Handling* cambiar a `Both`, lo que permite usar ambas APIs simultáneamente sin reescribir el código.

### Con la UI
**Interfaz no aparece**
Los textos del HUD eran invisibles durante el juego por dos motivos combinados: (1) el `RectTransform` del objeto HUD tenía tamaño cero porque sus anchors estaban en `(0,0)/(0,0)` en lugar de extenderse por toda la pantalla, y (2) el color del texto en los componentes `TextMeshPro` era blanco sobre el fondo claro del Panel, haciendo el texto indistinguible. Solución: ajustar los anchors del HUD a `(0,0)/(1,1)` para que ocupe toda la pantalla, y cambiar el color del texto a negro.
 
**Interfaz desproporcionada en móvil**
El `Canvas Scaler` estaba en modo `Constant Pixel Size`, por lo que los elementos UI tenían el mismo tamaño en píxeles en pantalla independientemente de la resolución del dispositivo. En un móvil de alta densidad (400+ ppi) los textos aparecían diminutos. Solución: cambiar el modo a `Scale With Screen Size` con resolución de referencia `1080×1920`.
 
**Espacio muerto en la parte superior (notch)**
En dispositivos con _notch_ (como es el caso del dispositivo que se ha usado para pruebas), el SO reserva una franja superior y la UI quedaba desplazada hacia arriba, dejando las etiquetas parcialmente ocultas. Solución: añadir el script `SafeArea.cs` al objeto HUD, que lee `Screen.safeArea` en cada frame y ajusta los anchors del `RectTransform` para que el contenido quede siempre dentro del área visible.

**Los emojis no se renderizaban**
Los caracteres emoji (Unicode > U+FFFF) no están incluidos en la fuente `Quando-Regular SDF` ni en los fallbacks disponibles. TMP los sustituía por □. Solución: reemplazar los emoji por símbolos Unicode del plano básico que sí están presentes en cualquier fuente estándar: ★ (U+2605) para puntos, ♥/♡ (U+2665/U+2661) para vidas y ◆ (U+25C6) para el nivel.

### Con la lógica del juego
**Los agujeros no triggereaban**
La pelota atravesaba los agujeros sin perder vida. El script `HoleTrap.cs` comprueba `other.CompareTag("Ball")` en `OnTriggerEnter`, pero el prefab `BallPrefab` tenía el tag establecido a `Untagged`. Al ser la pelota instanciada en tiempo de ejecución con `Instantiate`, el tag debe estar definido en el prefab, no en un objeto de escena. Solución: abrir `BallPrefab` en modo edición de prefab y cambiar el tag a `Ball` antes de guardar. Adicionalmente, el collider del `HolePrefab` tenía `Is Trigger` desmarcado en una revisión intermedia, lo que hacía que el agujero bloqueara físicamente el paso en lugar de detectar la colisión. Solución: verificar que `Is Trigger` esté activo en el collider del agujero y que `HoleTrap.cs` esté añadido como componente al prefab.

## Assets
Los assets de sonido de la carpeta `Audio` no son propios, se han descargado de [Freesound](freesound.org). Están bajo la licencia de Creatives Commons 0:
- Collectionable collect: [https://freesound.org/s/646672/](https://freesound.org/s/646672/)
- Woosh: [https://freesound.org/s/742833/](https://freesound.org/s/742833/)
- Victory: [https://freesound.org/s/580310/](https://freesound.org/s/580310/)
- Click: [https://freesound.org/s/629020/](https://freesound.org/s/629020/)
- Level up: [https://freesound.org/s/266100/](https://freesound.org/s/266100/)
- Game loop: [https://freesound.org/s/682484/](https://freesound.org/s/682484/)
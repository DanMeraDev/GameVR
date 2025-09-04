# GameVR
Juego de exploración sensorial

## Configuracion
* Instalar las herramientas de META XR del asset store (Meta XR All-in-One SDK)
* Poner el modo de build en android asegurando un API mayor o igual a la 32
* Instalar Meta quest link
* Instalar Meta Quest Developer Hub (Para el simulador)
* Instalar el XR plugin en unity en el project settings y escoger la opcion openXR
* Para probar en el meta quest 3 poner run and build y seleccionar un nombre para el APK
⚠️ Importante: Este proyecto requiere el asset "Real Stars Skybox" del Unity Asset Store.
Para usarlo, descarga el asset desde Unity Asset Store y cárgalo en la carpeta:
Assets/Real Stars Skybox/

### Uso de avatars
* Descargar las herramientas de Meta Avatars SDK. [Enlace a la Asset Store](https://assetstore.unity.com/packages/tools/integration/meta-avatars-sdk-271958) 
* Pese a que el GameObject 'Avatar' se encuentre dentro de 'VR Player', se debe modificar su posición dependiendo de la posición padre
* Por lo tanto, las componentes xyz del Position de 'Avatar' deben ser su inverso aditivo de las componentes xyz de 'VR Player'. Por ejemplo:
  * Si Position de 'VR Player' es (0, 3.4, 10), entonces el Position de 'Avatar' debe ser (0, -3.4, -10)
  * Si Position de 'VR Player' es (-23, 2, 5.4), entonces el Position de 'Avatar' debe ser (23, -2, -5.4)
* Este cambio de las componentes de Position de 'Avatar' se debe realizar en cada ocasión que se cambie la posición de 'VR Player'.

## Consideraciones
Aun no es necesario activar la particion del aplicativo en binarios obb o data.
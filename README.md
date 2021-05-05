# PrefabPlacerUnity
simple prefab placer for unity


UnityPackage file <img src="https://user-images.githubusercontent.com/52219910/117136413-020aa200-adb1-11eb-8404-60bca6ba1e35.png" width="50" height="50">
 https://github.com/AndrewKLoyd/PrefabPlacerUnity/raw/main/PrefabPlacer.unitypackage
1) To add PrefabPlacer press RMB in hierarchy and chose PrefabPlacer option

2) ![image](https://user-images.githubusercontent.com/52219910/117135063-1d74ad80-adaf-11eb-9f6b-3d9d0c33a489.png)

3)Drop your game object into prefab field

4)![image](https://user-images.githubusercontent.com/52219910/117135177-3b421280-adaf-11eb-9985-df7c42f5fb7d.png)

5)Adjust radius and amount of spawned items per radius and draw on surfaces

6)To draw prefabs simple set ur mouse on wanted surface and drag or click LMB

![image](https://user-images.githubusercontent.com/52219910/117135762-100bf300-adb0-11eb-9c51-9258ff08a82c.png)


7)To delete prefabs use Shift+LMB

![image](https://user-images.githubusercontent.com/52219910/117135783-1a2df180-adb0-11eb-8f3d-e2269db3f45a.png)


*Randomise Y rotation - will set Y rotation to random value

*Use geometry instead of pivot point(UGIPP) - if you wont use your pivot point as a spawned position, set UGIPP to true script will calculate the lowest vert in your mesh and set it as a spawn position + mouse position

*Y offset - change spawn position by Y

*Destroy spawned button - will destroy all kids of SpawnedObject you are working on

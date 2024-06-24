using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform carSpawn;
    private string[] cars = { "Chevy", "Ford", "Ferrari", "Bugatti" };


    /// <summary>
    /// Awake is being called when an enabled script instance is loaded, before the application starts
    /// </summary>
    void Awake()
    {
        GameObject carPrefab = null;
        CameraController camera;

        foreach (GameObject go in carPrefabs)
        {
            if (go.name == cars[PlayerPrefs.GetInt("CarSelected")])
            {
                carPrefab = go;
            }
        }

        if (carPrefab != null)
        {
            GameObject obj = Instantiate(carPrefab, carSpawn.position, carSpawn.rotation);
            camera = Camera.main.GetComponent<CameraController>();

            if (camera != null)
            {
                camera.player = obj.transform;
            }
            else
            {
                Debug.Log("Where camera?");
            }
        }
        else
        {
            Debug.Log("Where prefab?");
        }
    }
}

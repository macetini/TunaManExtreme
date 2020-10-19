using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject hero;
    public GameObject enemyPrefab;

    public List<GameObject> terrainLayers;
    public List<GameObject> enemyLayers;

    public Vector3 initOffset = new Vector3(8.8f, -0.55f, 0.0f);
    public Vector3 borderPositionOffset = new Vector3(11.2f, -5.6f, 0);

    public float borderJump = 15;

    public static int LAYER_GROUND = 8;
    public static int LAYER_POO = 9;

    private Transform _heroTransform;

    private List<GameObject> _activeTerrainLayers = new List<GameObject>();
    private Vector3 _lastTerrainLayerPos = new Vector2(0, 0);

    private List<GameObject> _activeEnemyLayers = new List<GameObject>();

    private double _currentBorder;

    // Start is called before the first frame update
    void Start()
    {
        _currentBorder = borderJump;

        _heroTransform = hero.transform;

        _activeTerrainLayers = InitLayers(terrainLayers, LAYER_GROUND);

        GameObject lastTerrainLayer = _activeTerrainLayers[_activeTerrainLayers.Count - 1];
        _lastTerrainLayerPos = lastTerrainLayer.transform.position;

        _activeEnemyLayers = InitLayers(enemyLayers);

        InitEnemys();
    }

    protected List<GameObject> InitLayers(List<GameObject> layers, int layerName = 0)
    {
        GameObject layer = null;

        int childrenCount = layers.Count;

        List<GameObject> processedLayers = new List<GameObject>(childrenCount);

        for (int i = 0; i < childrenCount; i++)
        {
            GameObject layerPrefab = layers[i];
            layer = Instantiate(layerPrefab);
            layer.transform.localPosition -= initOffset;
            SetLayerRecursively(layer, LAYER_GROUND);

            processedLayers.Add(layer);

            if (i > 0) ConnectLayers(processedLayers[i - 1], processedLayers[i]);
        }

        if (layer == null) throw new System.Exception("TerrainGenerator:: No layers found.");

        return processedLayers;
    }

    protected void InitEnemys()
    {
        int enemyLayersCount = _activeEnemyLayers.Count;

        for (int i = 0; i < enemyLayersCount; i++)
        {
            GameObject enemyLayer = _activeEnemyLayers[i];

            foreach (Transform enemy in enemyLayer.transform)
            {
                enemyLayer.SetActive(false);

                Vector2 enemyWorldPosition = (Vector2)Camera.main.transform.InverseTransformDirection(enemy.position);
                GameObject enemyInstance = Instantiate(enemyPrefab);
                enemyInstance.transform.position = enemyWorldPosition;
            }
        }
    }

    protected void ConnectLayers(GameObject firstElement, GameObject secondElement)
    {
        secondElement.transform.position = firstElement.transform.position + borderPositionOffset;
    }

    protected void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void Update()
    {
        double position = -_heroTransform.position.y;

        if (position > 0 && position >= _currentBorder)
        {
            _currentBorder += borderJump;
            /*
            int length = _activeTerrainLayers.Count - 1;

            GameObject firstLayer = _activeTerrainLayers[0];
            _activeTerrainLayers.RemoveAt(0);
            _activeTerrainLayers.Insert(length, firstLayer);

            _activeTerrainLayers[length].transform.position = _lastTerrainLayerPos + borderPositionOffset;
            */
            _lastTerrainLayerPos = ShiftLayers(_activeTerrainLayers, _lastTerrainLayerPos);//firstLayer.transform.position;
            //_lastTerrainLayerPos = ShiftLayers(_activeTerrainLayers, _lastTerrainLayerPos);//firstLayer.transform.position;
        }
    }

    protected Vector3 ShiftLayers(List<GameObject> layers, Vector3 lastPos)
    {
        int length = layers.Count - 1;

        GameObject firstLayer = layers[0];
        layers.RemoveAt(0);
        layers.Insert(length, firstLayer);

        layers[length].transform.position = lastPos + borderPositionOffset;

        lastPos = firstLayer.transform.position;

        return lastPos;
    }
}

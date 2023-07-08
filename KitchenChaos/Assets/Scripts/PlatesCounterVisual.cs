using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{

    [SerializeField] PlatesCounter platesCounter;

    [SerializeField] GameObject counterTopPoint;
    [SerializeField] GameObject platePrefab;

    private List<GameObject> plateVisualGameObjectList = new List<GameObject>();

    private void Start()
    {
        platesCounter.OnPlateSpwned += OnPlatesCounterPlateSpwned;
        platesCounter.OnPlateRemove += OnPlatesCounterPlateREmove;
    }

    private void OnDestroy()
    {
        platesCounter.OnPlateSpwned -= OnPlatesCounterPlateSpwned;
        platesCounter.OnPlateRemove -= OnPlatesCounterPlateREmove;
    }

    private void OnPlatesCounterPlateSpwned(object sender, System.EventArgs e)
    {
        var plateNewInstance = Instantiate(platePrefab, counterTopPoint.transform);

        var plateOffsetY = 0.1f;

        plateNewInstance.transform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);

        plateVisualGameObjectList.Add(plateNewInstance);
    }

    private void OnPlatesCounterPlateREmove(object sender, System.EventArgs e)
    {
        var plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        plateVisualGameObjectList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

}

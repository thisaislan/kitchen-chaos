using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private float turnX;
    private float turnY;
    private float turnZ;
    private float moveX;
    private float moveY;
    private float moveZ;
    private bool world;
    private float rotationTime = 0;
    private float rotationDuration = 3f;
    private float rotationSpeed = 0;

    void Start()
    {
        turnX = Random.Range(1, 100)/10f;
        turnY = Random.Range(1, 100)/10f;
        turnZ = Random.Range(1, 100)/10f;
        rotationTime = Random.Range(1, 10)/100f;
        rotationDuration = Random.Range(1, 100)/10f;
        rotationSpeed = Random.Range(1, 10)/1000f;
    }

    void Update()
    {
        if (world == true)
        {
            transform.Rotate(turnX * Time.deltaTime, turnY * Time.deltaTime, turnZ * Time.deltaTime, Space.World);
            transform.Translate(moveX * Time.deltaTime, moveY * Time.deltaTime, moveZ * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Rotate(turnX * Time.deltaTime, rotationSpeed * Time.deltaTime, turnZ * Time.deltaTime, Space.Self);
            transform.Translate(moveX * Time.deltaTime, moveY * Time.deltaTime, moveZ * Time.deltaTime, Space.Self);
        }

        rotationTime += Time.deltaTime;
        if (rotationTime > rotationDuration)
        {
            rotationSpeed = Random.Range(3, 300);
            rotationTime = 0;
        }
    }

}


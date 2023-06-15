using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSped = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    private void Update()
    {
        var inputVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) { inputVector.y++; }
        if (Input.GetKey(KeyCode.S)) { inputVector.y--; }
        if (Input.GetKey(KeyCode.A)) { inputVector.x--; }
        if (Input.GetKey(KeyCode.D)) { inputVector.x++; }

        SetMovement(inputVector.normalized);
    }

    private void SetMovement(Vector2 inputVector) 
    {
        var movementVector = new Vector3(inputVector.x, 0f, inputVector.y);

        transform.position += movementVector * moveSped * Time.deltaTime;

        SetRotation(movementVector);
    }


    private void SetRotation(Vector3 direction) =>
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * rotateSpeed);

}

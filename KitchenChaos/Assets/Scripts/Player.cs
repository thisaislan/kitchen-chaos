using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSped = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private GameInput gameInput;

    private bool isWalking;

    private void Update() =>
        SetMovement(gameInput.GetMovementVectorNormalized());

    private void SetMovement(Vector2 inputVector) 
    {
        var movementVector = new Vector3(inputVector.x, 0f, inputVector.y);

        transform.position += movementVector * moveSped * Time.deltaTime;

        SetRotation(movementVector);
        SetWalkingState(movementVector != Vector3.zero);
    }

    private void SetRotation(Vector3 direction) =>
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * rotateSpeed);

    private void SetWalkingState(bool isWalking) =>
        this.isWalking = isWalking;

    public bool IsWalking() =>
        isWalking;

}

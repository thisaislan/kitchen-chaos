using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;

    [SerializeField] private GameInput gameInput;

    private bool isWalking;

    private void Update() =>
        SetMovement(gameInput.GetMovementVectorNormalized());

    private void SetMovement(Vector2 inputVector) 
    {
        // refactor this
        var movementVector = new Vector3(inputVector.x, 0f, inputVector.y);
        var moveDistance = moveSpeed * Time.deltaTime;

        SetRotation(movementVector);

        var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementVector, moveDistance);

        if (!canMove)
        {
            var moveDirX = new Vector3(movementVector.x, 0, 0).normalized;
            
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                movementVector = moveDirX;
            }
            else
            {
                var moveDirZ = new Vector3(0, 0, movementVector.z).normalized;

                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    movementVector = moveDirZ;
                }
            }
        }

        if (canMove)
        { 
            transform.position += movementVector * moveSpeed * Time.deltaTime;
            SetWalkingState(movementVector != Vector3.zero);
        }

    }

    private void SetRotation(Vector3 direction) =>
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * rotateSpeed);

    private void SetWalkingState(bool isWalking) =>
        this.isWalking = isWalking;

    public bool IsWalking() =>
        isWalking;

}

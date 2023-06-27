using System;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private Vector3 lastInteractDirection;

    public bool IsWalking { get; private set; }

    private void Update()
    {
        var directionVector = GetDirectionVector(gameInput.GetMovementVectorNormalized());

        HandleMovement(directionVector);
        HandleInteractions(directionVector);
    }

    private Vector3 GetDirectionVector(Vector2 inputVector) =>
        new Vector3(inputVector.x, 0f, inputVector.y);

    private void HandleInteractions(Vector3 directionVector)
    {
        if (directionVector != Vector3.zero)
        {
            lastInteractDirection = directionVector;
        }

        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                clearCounter.Interact();
            }            
        }
    }

    private void HandleMovement(Vector3 directionVector)
    {        
        var movementDistance = movementSpeed * Time.deltaTime;
        var movementDirection = GetMovementDirection(directionVector, movementDistance);

        SetRotation(directionVector);
        SetMovement(movementDirection, movementDistance);

        IsWalking = movementDirection != Vector3.zero;
    }

    private Vector3 GetMovementDirection(Vector3 directionVector, float movementDistance)
    {
        var canMove = CanMove(directionVector, movementDistance);

        if (!canMove)
        {
            var movementDirectionX = new Vector3(directionVector.x, 0, 0).normalized;

            canMove = CanMove(movementDirectionX, movementDistance);

            if (canMove)
            {
                directionVector = movementDirectionX;
            }
            else
            {
                var movementDirectionZ = new Vector3(0, 0, directionVector.z).normalized;

                canMove = CanMove(movementDirectionZ, movementDistance);

                if (canMove) { directionVector = movementDirectionZ; }
                else { return Vector3.zero; }
            }
        }

        return directionVector;
    }

    private bool CanMove(Vector3 direction, float distance) =>
        !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, direction, distance);

    private void SetRotation(Vector3 direction) =>
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * rotateSpeed);

    private void SetMovement(Vector3 movementDirection, float movementDistance)
    {
        if (movementDirection != Vector3.zero)
        {
            transform.position += movementDirection * movementDistance;
        }
    }

}

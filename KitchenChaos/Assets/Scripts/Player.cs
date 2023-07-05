using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedCounter;
    }

    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    
    [SerializeField] private GameObject kitchenObjectHoldPoint;

    public bool IsWalking { get; private set; }

    private BaseCounter selectedClearCounter;

    private Vector3 lastInteractDirection;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Debug.LogError("There are more then one istance of the player"); }
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInputOnInteractAction;
    }

    private void OnDestroy()
    {
        gameInput.OnInteractAction -= GameInputOnInteractAction;
    }

    private void Update()
    {
        var directionVector = GetDirectionVector(gameInput.GetMovementVectorNormalized());

        HandleMovement(directionVector);
        HandleInteractions(directionVector);
    }

    public void ClearKitchenObject() =>
        kitchenObject = null;

    public Transform GetKitchenObjectFollowTransform() =>
        kitchenObjectHoldPoint.transform;

    public bool HasKitchenObject() =>
        kitchenObject != null;

    public void SetKitchenObject(KitchenObject kitchenObject) =>
        this.kitchenObject = kitchenObject;

    private void GameInputOnInteractAction(object sender, EventArgs e) =>
         selectedClearCounter?.Interact(this);

    public KitchenObject GetKitchenObject() =>
        kitchenObject;

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
            if (raycastHit.transform.TryGetComponent(out BaseCounter counter))
            {
                if (selectedClearCounter != counter)
                {
                    SetSelectedCounter(counter);
                }
            }
            else 
            {
                SetSelectedCounter();
            }
        }
        else
        {
            SetSelectedCounter();
        }

    }

    private void SetSelectedCounter(BaseCounter selectedCounter = null)
    {
        selectedClearCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedChangedEventArgs
        {
            SelectedCounter = this.selectedClearCounter
        }) ;
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

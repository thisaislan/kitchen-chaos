using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawn;
    public static event EventHandler OnAnyPickedSomething;

    public event EventHandler OnPickedSomething;

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
        
    [SerializeField] private LayerMask countersLayerMask;
    
    [SerializeField] private GameObject kitchenObjectHoldPoint;

    public bool IsWalking { get; private set; }

    private BaseCounter selectedClearCounter;

    private Vector3 lastInteractDirection;

    private KitchenObject kitchenObject;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += OnGameInputInteractAction;
        GameInput.Instance.OnInteractAlternateAction += OnGameInputInteractAlternateAction;
    }

    public override void OnDestroy()
    {
        GameInput.Instance.OnInteractAction -= OnGameInputInteractAction;
        GameInput.Instance.OnInteractAlternateAction -= OnGameInputInteractAlternateAction;

        base.OnDestroy();
    }

    private void Update()
    {
        if (IsOwner)
        { 
            var directionVector = GetDirectionVector(GameInput.Instance.GetMovementVectorNormalized());

            HandleMovement(directionVector);
            HandleInteractions(directionVector);
        }
    }

    public void ClearKitchenObject() =>
        kitchenObject = null;

    public Transform GetKitchenObjectFollowTransform() =>
        kitchenObjectHoldPoint.transform;

    public bool HasKitchenObject() =>
        kitchenObject != null;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        OnPickedSomething?.Invoke(this, EventArgs.Empty);
        OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
    }

    private void OnGameInputInteractAction(object sender, EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.GamePlaying)
        {
            selectedClearCounter?.Interact(this);
        }
    }

    private void OnGameInputInteractAlternateAction(object sender, EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.GamePlaying)
        {
            selectedClearCounter?.InteractAlternate(this);
        }
    }

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
            
            canMove = CanMove(movementDirectionX, directionVector.x, movementDistance);

            if (canMove)
            {
                directionVector = movementDirectionX;
            }
            else
            {
                var movementDirectionZ = new Vector3(0, 0, directionVector.z).normalized;

                canMove = CanMove(movementDirectionZ, directionVector.z, movementDistance);

                if (canMove) { directionVector = movementDirectionZ; }
                else { return Vector3.zero; }
            }
        }

        return directionVector;
    }

    private bool CanMove(Vector3 direction, float axis, float distance) =>
        (axis < -0.4f || axis > +0.4f) && CanMove(direction, distance);

    private bool CanMove(Vector3 direction, float distance) =>
        !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, direction, distance);

    private void SetRotation(Vector3 direction) =>
        transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * rotateSpeed);

    private void SetMovement(Vector3 movementDirection, float movementDistance) =>
        transform.position += movementDirection * movementDistance;

}

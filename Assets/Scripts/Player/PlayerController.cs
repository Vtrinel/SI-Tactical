using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged += DebugLifeAmount;
        damageReceiptionSystem.OnLifeReachedZero += LifeReachedZero;
        damageReceiptionSystem.OnReceivedDamages += OnReceivedDamages;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged -= DebugLifeAmount;
        damageReceiptionSystem.OnLifeReachedZero -= LifeReachedZero;
        damageReceiptionSystem.OnReceivedDamages -= OnReceivedDamages;
    }

    private void Start()
    {
        damageReceiptionSystem.SetUpSystem(true);
        navMeshAgent.isStopped = true;
        positionStamp = transform.position;
    }

    private void Update()
    {
        if (moving)
            CheckForDestinationReached();

        if (ableToAct)
            UpdateInputs();

        Vector3 currentPos = transform.position;
        if (positionStamp != currentPos)
        {
            positionStamp = currentPos;
            GameManager.Instance.OnPlayerPositionChanged?.Invoke(currentPos);
            GameManager.Instance.CheckForCompetencesUsability();
            DiscManager.Instance.CheckAllDiscsProximity(transform.position);
        }
    }

    [Header("References")]
    [SerializeField] NavMeshAgent navMeshAgent = default;
    public NavMeshAgent GetNavMeshAgent => navMeshAgent;
    public DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;

    #region Life
    [Header("Damage Reception")]
    [SerializeField] EffectZone rageEffectZonePrefab = default;
    [SerializeField] float rageEffectZoneVerticalOffset = 1f;

    public Action<bool> OnPlayerReceivedDamages = default;

    public void OnReceivedDamages(int currentLife, int lifeDifferential)
    {
        TurnManager.Instance.InterruptEnemiesTurn();
        OnPlayerReceivedDamages?.Invoke(currentLife == 0);
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }

    public void PlayRage()
    {
        EffectZone newRageEffectZone = Instantiate(rageEffectZonePrefab);
        newRageEffectZone.StartZone(transform.position + Vector3.up * rageEffectZoneVerticalOffset);
    }

    public void LifeReachedZero()
    {
        GameManager.Instance.LoseGame();
    }

    public void DebugLifeAmount(int amount)
    {
        //Debug.Log("Current life : " + amount);
    }
    #endregion

    #region Inputs
    [Header("Inputs")]
    [SerializeField] KeyCode clickActionKey = KeyCode.Mouse0;
    [SerializeField] KeyCode selectMoveInput = KeyCode.A;
    [SerializeField] KeyCode throwCompetenceInput = KeyCode.Z;
    [SerializeField] KeyCode recallCompetenceInput = KeyCode.E;
    [SerializeField] KeyCode specialCompetenceInput = KeyCode.R;
    [SerializeField] KeyCode passTurnInput = KeyCode.Return;
    [SerializeField] KeyCode competenceMenuInput = KeyCode.Tab;
    [Space]
    [SerializeField] KeyCode camForwardInput = KeyCode.UpArrow;
    [SerializeField] KeyCode camBackwardInput = KeyCode.DownArrow;
    [SerializeField] KeyCode camRightInput = KeyCode.RightArrow;
    [SerializeField] KeyCode camLeftInput = KeyCode.LeftArrow;
    public Vector2 GetCameraMoveKeyboardInput => new Vector2(
        (Input.GetKey(camRightInput) ? 1 : 0) - (Input.GetKey(camLeftInput) ? 1 : 0), 
        (Input.GetKey(camForwardInput)? 1 : 0) - (Input.GetKey(camBackwardInput) ? 1 : 0));

    [SerializeField] float cursorMinCameraHorizontalMovementCoeff = 0.8f;
    [SerializeField] float cursorMaxCameraHorizontalMovementCoeff = 0.95f;
    [SerializeField] float cursorMinCameraVerticalMovementCoeff = 0.9f;
    [SerializeField] float cursorMaxCameraVerticalMovementCoeff = 0.98f;

    bool ableToAct = false;

    public KeyCode GetCompetenceMenuInput => competenceMenuInput;

    public void SetAbleToAct(bool able)
    {
        ableToAct = able;
    }

    public void UpdateInputs()
    {
        #region Action Selection
        if (Input.GetKeyDown(selectMoveInput))
            GameManager.Instance.SelectAction(ActionType.Move);
        else if (Input.GetKeyDown(throwCompetenceInput))
            GameManager.Instance.SelectAction(ActionType.Throw);
        else if (Input.GetKeyDown(recallCompetenceInput))
            GameManager.Instance.SelectAction(ActionType.Recall);
        else if (Input.GetKeyDown(specialCompetenceInput))
            GameManager.Instance.SelectAction(ActionType.Special);
        #endregion

        if (Input.GetKeyDown(clickActionKey))
            GameManager.Instance.OnPlayerClickAction();

        if (Input.GetKeyDown(passTurnInput))
        {
            GameManager.Instance.SelectAction(ActionType.None);
            TurnManager.Instance.EndPlayerTurn();
            CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
        }



            Vector2 camKeyboardInputs = GetCameraMoveKeyboardInput;
        if (camKeyboardInputs != Vector2.zero)
        {
            CameraManager.instance.GetPlayerCamera.MovePlayerCamera(camKeyboardInputs, true);
        }
        else
        {
            if (!GameManager.Instance.GetCurrentWorldMouseResult.mouseIsOnUI)
            {
                Vector2 unclampedMousePosition = Input.mousePosition;
                Vector2 clampedMousePosition = unclampedMousePosition;
                clampedMousePosition.x = Mathf.Clamp(clampedMousePosition.x, 0, Screen.width);
                clampedMousePosition.y = Mathf.Clamp(clampedMousePosition.y, 0, Screen.height);
                float cursorHorizontalCoeff = ((clampedMousePosition.x - Screen.width / 2) / (Screen.width / 2));
                float cursorVerticalCoeff = ((clampedMousePosition.y - Screen.height / 2) / (Screen.height / 2));

                float cursorHorizontalInput = Mathf.Clamp(
                    1 - ((cursorMaxCameraHorizontalMovementCoeff - Mathf.Abs(cursorHorizontalCoeff)) / (cursorMaxCameraHorizontalMovementCoeff - cursorMinCameraHorizontalMovementCoeff))
                    , 0, 1) * Mathf.Sign(cursorHorizontalCoeff);

                float cursorVerticalInput = Mathf.Clamp(
                   1 - ((cursorMaxCameraVerticalMovementCoeff - Mathf.Abs(cursorVerticalCoeff)) / (cursorMaxCameraVerticalMovementCoeff - cursorMinCameraVerticalMovementCoeff))
                   , 0, 1) * Mathf.Sign(cursorVerticalCoeff);

                if (unclampedMousePosition.x < 0 || unclampedMousePosition.x > Screen.width || unclampedMousePosition.y < 0 || unclampedMousePosition.y > Screen.height)
                {
                    cursorHorizontalInput = 0;
                    cursorVerticalInput = 0;
                }

                Vector2 camCursorInput = new Vector2(cursorHorizontalInput, cursorVerticalInput);
                if (camCursorInput != Vector2.zero)
                    CameraManager.instance.GetPlayerCamera.MovePlayerCamera(camCursorInput, false);
            }
        }
    }
    #endregion

    #region Movement
    Vector3 positionStamp = Vector3.zero;
    bool moving = false;

    public Action<bool> OnMoveChange;

    public void MoveTo(Vector3 targetPosition)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
        moving = true;
        OnMoveChange?.Invoke(moving);
    }
    public void CheckForDestinationReached()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
        {
            navMeshAgent.isStopped = true;
            moving = false;
            OnMoveChange?.Invoke(moving);
            OnPlayerReachedMovementDestination?.Invoke();
        }
    }
    public Action OnPlayerReachedMovementDestination;
    #endregion

    #region Others
    TimerSystem lookRotationTimer = new TimerSystem();
    AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    Vector3 startLookDirection = Vector3.zero;
    Vector3 targetLookDirection = Vector3.zero;

    public void StartLookAt(Vector3 position)
    {
        lookRotationTimer.ChangeTimerValue(0.3f);
        startLookDirection = transform.forward;
        targetLookDirection = position - transform.position;
        targetLookDirection.y = 0;
        targetLookDirection.Normalize();

        StartCoroutine(LookAtPosition());
    }

    IEnumerator LookAtPosition()
    {
        lookRotationTimer.StartTimer();

        while (!lookRotationTimer.TimerOver)
        {
            lookRotationTimer.UpdateTimer();
            Vector3 currentLookDir = Vector3.Slerp(startLookDirection, targetLookDirection, lookRotationTimer.GetTimerCoefficient);
            float rotY = Mathf.Atan2(currentLookDir.x, currentLookDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotY, 0);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (knockbackReceiptionSystem.IsBeingKnockbacked || moving)
        {
            if (other.gameObject.layer == 11)
            {
                DiscScript hitDisc = other.GetComponent<DiscScript>();
                if (hitDisc != null)
                {
                    hitDisc.RetreiveByPlayer();
                }
            }
        }
    }
}

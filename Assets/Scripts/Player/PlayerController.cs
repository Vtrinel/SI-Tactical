using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] EffectZone test = default;

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged += DebugLifeAmount;
        damageReceiptionSystem.OnLifeReachedZero += LifeReachedZero;
        damageReceiptionSystem.OnReceivedDamages += StartPlayerRage;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerLifeAmountChanged -= DebugLifeAmount;
        damageReceiptionSystem.OnLifeReachedZero -= LifeReachedZero;
        damageReceiptionSystem.OnReceivedDamages -= StartPlayerRage;
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
        }

        #region DEBUG
        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector3 randomDir = Random.onUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();

            knockbackReceiptionSystem.ReceiveKnockback(DamageTag.Enemy, new KnockbackParameters(10, 0.08f, 0.2f), randomDir);
            Debug.DrawRay(transform.position + Vector3.up, randomDir * 3, Color.red);
        }

        if (Input.GetKeyDown(KeyCode.D))
            damageReceiptionSystem.ReceiveDamage(DamageTag.Enemy, 1);

        if (Input.GetMouseButtonDown(1))
        {
            EffectZone newEffectZone = Instantiate(test);
            newEffectZone.StartZone(GameManager.Instance.GetCurrentWorldMouseResult.mouseWorldPosition);
        }
        #endregion
    }

    [Header("References")]
    [SerializeField] NavMeshAgent navMeshAgent = default;
    public DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;

    #region Life
    [Header("Damage Reception")]
    [SerializeField] EffectZone rageEffectZonePrefab = default;
    [SerializeField] float rageEffectZoneVerticalOffset = 1f;

    public void StartPlayerRage(int currentLife, int lifeDifferential)
    {
        if (currentLife == 0)
            return;

        Debug.Log("RAGE");

        EffectZone newRageEffectZone = Instantiate(rageEffectZonePrefab);
        newRageEffectZone.StartZone(transform.position + Vector3.up * rageEffectZoneVerticalOffset);

        TurnManager.Instance.InterruptEnemiesTurn();
    }

    public void LifeReachedZero()
    {
        GameManager.Instance.LoseGame();
    }

    public void DebugLifeAmount(int amount)
    {
        Debug.Log("Current life : " + amount);
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

    bool ableToAct = false;
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
        }
    }
    #endregion

    #region Movement
    Vector3 positionStamp = Vector3.zero;
    bool moving = false;
    public void MoveTo(Vector3 targetPosition)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
        moving = true;
    }
    public void CheckForDestinationReached()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
        {
            navMeshAgent.isStopped = true;
            moving = false;
            OnPlayerReachedMovementDestination?.Invoke();
        }
    }
    public System.Action OnPlayerReachedMovementDestination;
    #endregion    
}

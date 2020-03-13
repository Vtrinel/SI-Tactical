using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] EnemyType enemyType = EnemyType.TouniBase;
    public EnemyType GetEnemyType => enemyType;
    [SerializeField] int goldGain = 10;

    private void Start()
    {
        SpawnEnemy(transform.position, _lootedDiscType);
        if(_lootedDiscType != DiscType.None)
        {
            TouniFourrureBasique.material = TouniFourrureAndMaskAlt;
            TouniMaskBasique.material = TouniFourrureAndMaskAlt;
        }

        enemyHoverCirle.SetColor(new Color(enemyHoverCircleColor.r, enemyHoverCircleColor.g, enemyHoverCircleColor.b, enemyUnhoveredCircleAlpha));
        enemyHoverCirle.SetHovered(false);

        myIA.SetAnimator(enemyAnimator, animationEventContainer);
    }

    [Header("References")]
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;
    [SerializeField] Transform lifeBarParent;
    [SerializeField] Animator lifeBarAnimator;
    [SerializeField] GameObject lifeBarEnemyPrefab;
    [SerializeField] Animator enemyAnimator = default;
    [SerializeField] AnimationEventsContainer animationEventContainer = default;
    [SerializeField] Transform deathParticlePosition = default;
    [SerializeField] List<MeshRenderer> meshesToDisolveOnDeath = default;
    [SerializeField] List<SkinnedMeshRenderer> skinnedMeshesToDisolveOnDeath = default;
    [SerializeField] AnimationCurve disolveAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float deathDisolveAnimationDuration = 0.5f;
    [SerializeField] float deathDisolveAnimationDelay = 0.2f;
    [SerializeField] float deathDisolveMaxValue = 0.2f;
    TimerSystem deathDissolveTimer = default;
    List<Image> lifeBarList = new List<Image>();

    public MeshRenderer TouniFourrureBasique;
    public MeshRenderer TouniMaskBasique;
    public Material TouniFourrureAndMaskAlt;


    void InitLifeBar(int lifeNumber)
    {
        for(int i=0; i < lifeNumber; i++)
        {
            lifeBarList.Add(Instantiate(lifeBarEnemyPrefab, lifeBarParent).GetComponent<Image>());
        }
    }

    public void UpdateLifeBarFill(int currentAmount, int damageDelta)
    {
        int i = 1;
        foreach(Image bar in lifeBarList)
        {
            bar.GetComponentInChildren<Animator>().SetBool("IsAlive", !(currentAmount < i));
            //bar.enabled = !(currentAmount < i);
            i++;
        }
    }

    public void ReceivedDamages(int currentAmount, int damageDelta)
    {
        enemyAnimator.SetTrigger("Hit");
        enemyAnimator.SetInteger("RandomHit", UnityEngine.Random.Range(0, 2));
        PostProcessAnimEnemyDamaged.instance.PlayPostProcessAnim();
        FxManager.Instance.CreateFx(FxType.enemyDamage, gameObject.transform.position);
        ShakeScriptableObjectManager.instance.LoadShake("ShakeSetting_EnnemyDamage");

        tooltipCollider.SetValueInInfos(currentAmount);
    }

    public Action<EnemyBase> OnEnemyDeath;
    public void Die()
    {
        CheckForLootedDisc();

        //Debug.Log(name + " (Enemy) is dead");
        spawned = false;
        setedUpInitiative = false;
        
        OnEnemyDeath?.Invoke(this);
        PlayerExperienceManager.Instance.GainGold(goldGain);
        SoundManager.Instance.PlaySound(Sound.EnemyDeath, gameObject.transform.position);
        //FxManager.Instance.CreateFx(FxType.enemyDeath, gameObject.transform.position);
        //Destroy(gameObject);
        enemyHoverCirle.gameObject.SetActive(false);
        animationEventContainer.SetEvent2(DestroyEnemy);
        enemyAnimator.SetBool("Dead", true);

        StartCoroutine(DeathDissolve());
    }   

    IEnumerator DeathDissolve()
    {
        yield return new WaitForSeconds(deathDisolveAnimationDelay);
        deathDissolveTimer = new TimerSystem(deathDisolveAnimationDuration, null);
        deathDissolveTimer.StartTimer();
        while (!deathDissolveTimer.TimerOver)
        {
            deathDissolveTimer.UpdateTimer();
            foreach (MeshRenderer mesh in meshesToDisolveOnDeath)
            {
                mesh.material.SetFloat("_DissolveAmount", Mathf.Lerp(0, deathDisolveMaxValue, disolveAnimationCurve.Evaluate(deathDissolveTimer.GetTimerCoefficient)));
            }

            foreach (SkinnedMeshRenderer mesh in skinnedMeshesToDisolveOnDeath)
            {
                mesh.material.SetFloat("_DissolveAmount", Mathf.Lerp(0, deathDisolveMaxValue, disolveAnimationCurve.Evaluate(deathDissolveTimer.GetTimerCoefficient)));
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void DestroyEnemy()
    {
        Vector3 fxPos = transform.position;
        if (deathParticlePosition != null)
            fxPos = deathParticlePosition.position;

        //FxManager.Instance.CreateFx(FxType.enemyDeath, fxPos);

        Destroy(gameObject);
    }

    [Header("Common Values")]
    [SerializeField] int baseInitiative = 1;
    float enemyInstanceInitiative = 1;
    public float GetEnemyInitiative => enemyInstanceInitiative;
    bool setedUpInitiative = false;

    [Header("Loot")]
    [SerializeField] DiscType _lootedDiscType = DiscType.None;
    [SerializeField] GameObject lootDiscIndicator = default;

    public void CheckForLootedDisc()
    {
        if(_lootedDiscType != DiscType.None)
        {
            DiscScript newDisc = DiscManager.Instance.GetDiscFromPool(_lootedDiscType);
            if(newDisc != null)
            {
                newDisc.transform.position = transform.position;
            }
        }
    }

    bool spawned = false;
    public void SpawnEnemy(Vector3 position, DiscType lootedDiscType)
    {
        if (spawned)
            return;

        spawned = true;

        damageReceiptionSystem.SetUpSystem(false);
        transform.position = position;
        gameObject.SetActive(true);
        SetUpInitiative();
        _lootedDiscType = lootedDiscType;

        if (_lootedDiscType != DiscType.None)
            Debug.Log(name + " will loot " + _lootedDiscType + " disc");

        if (lootDiscIndicator != null)
        {
            lootDiscIndicator.SetActive(_lootedDiscType != DiscType.None);
        }

        InitLifeBar(damageReceiptionSystem.GetCurrentLifeAmount);
        tooltipCollider.SetValueInInfos(damageReceiptionSystem.GetCurrentLifeAmount);
        enemyAnimator.SetBool("Dead", false);
    }

    public void SetUpInitiative()
    {
        if (setedUpInitiative)
            return;

        setedUpInitiative = true;
        enemyInstanceInitiative = baseInitiative + UnityEngine.Random.Range(0f, 1f);

        name = name + " - " +  GetEnemyInitiative.ToString();
    }

    #region Turn management

    public void StartTurn()
    {
        myIA.myNavAgent.avoidancePriority = 10;
        myIA.isPlaying = true;
        PlayMyTurn();
    }

    public void EndTurn()
    {
        myIA.myNavAgent.avoidancePriority = 50;
        myIA.isPlaying = false;
        if(TurnManager.Instance.GetCurrentTurnState != TurnState.EnemyTurn)
        {
            return;
        }

        TurnManager.Instance.EndEnemyTurn(this, GetPlayerDetected);
    }

    public void InterruptAllAction()
    {
        //Debug.Log("Interrupt " + name + "'s actions");
        // TO DO : interrupt action of the linked AI, without calling EndTurn 
    }
    #endregion

    #region IA
    [Header("IA")]

    public IAEnemyVirtual myIA = default;
    public void SetPlayerDetected(bool detected)
    {
        myIA.haveDetectPlayer = detected;
    }
    public bool GetPlayerDetected => myIA.haveDetectPlayer;

    void PlayMyTurn()
    {
        if (myIA == null)
            return;

        myIA.PlayerTurn();
    }
    #endregion

    private void OnEnable()
    {
        damageReceiptionSystem.OnLifeAmountChanged += UpdateLifeBarFill;
        damageReceiptionSystem.OnReceivedDamages += ReceivedDamages;
        damageReceiptionSystem.OnLifeReachedZero += Die;
        TurnManager.Instance.OnEnemyTurnInterruption += InterruptAllAction;

        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip += StartHovering;
            tooltipCollider.OnEndTooltip += EndHovering;
        }

        if (myIA == null)
            return;
        myIA.OnFinishTurn += EndTurn;
    }

    private void OnDisable()
    {
        damageReceiptionSystem.OnLifeAmountChanged -= UpdateLifeBarFill;
        damageReceiptionSystem.OnReceivedDamages -= ReceivedDamages;
        damageReceiptionSystem.OnLifeReachedZero -= Die;
        TurnManager.Instance.OnEnemyTurnInterruption -= InterruptAllAction;

        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip -= StartHovering;
            tooltipCollider.OnEndTooltip -= EndHovering;
        }

        if (myIA == null)
            return;

        myIA.OnFinishTurn -= EndTurn;
    }

    public void DisplayAndActualisePreviewAttack(Transform target)
    {
        myIA.myShowPath.SetValue(myIA.distanceOfDeplacement, myIA.attackRange);
        myIA.myShowPath.ShowOrHide(true);
        myIA.myShowPath.SetTargetPosition(target);
    }

    public void HidePreview(bool value)
    {
        myIA.myShowPath.ShowOrHide(value);
    }

    [Header("Tooltips")]
    [SerializeField] TooltipCollider tooltipCollider = default;
    [SerializeField] HoverCircle enemyHoverCirle = default;
    [SerializeField] Color enemyHoverCircleColor = Color.red;
    [SerializeField] float enemyUnhoveredCircleAlpha = 0.2f;
    [SerializeField] float enemyHoveredCircleAlpha = 0.8f;

    public void StartHovering()
    {
        enemyHoverCirle.SetColor(new Color(enemyHoverCircleColor.r, enemyHoverCircleColor.g, enemyHoverCircleColor.b, enemyHoveredCircleAlpha));
        enemyHoverCirle.SetHovered(true);
        lifeBarAnimator.SetBool("Hover", true);
    }

    public void EndHovering()
    {
        enemyHoverCirle.SetColor(new Color(enemyHoverCircleColor.r, enemyHoverCircleColor.g, enemyHoverCircleColor.b, enemyUnhoveredCircleAlpha));
        enemyHoverCirle.SetHovered(false);
        lifeBarAnimator.SetBool("Hover", false);
    }

    [Header("Other feedbacks")]
    [SerializeField] GameObject willBeHitIndicator = default;

    public void ShowWillBeHitIndicator()
    {
        willBeHitIndicator.gameObject.SetActive(true);
    }

    public void HideWillBeHitIndicator()
    {
        willBeHitIndicator.gameObject.SetActive(false);
    }
}

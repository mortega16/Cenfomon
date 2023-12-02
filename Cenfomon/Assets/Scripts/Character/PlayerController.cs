using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    public float moveSpeed;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private bool isMoving;
    private Vector2 input;

    public static PlayerController i {  get; private set; }

    private Animator animator;

    private void Awake()
    {
        i = this;
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Restringir Movimineto Diagonal
            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));

                OnMoveOver();
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(Interact());
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos,0.2f,GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer)!= null)
        {
            return false;
        }
        return true;
    }

    IPlayerTriggerable currentlyInTrigger;

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, GameLayers.i.TriggerableLayers);

        IPlayerTriggerable triggerable = null;
        foreach(var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;

                animator.SetBool("isMoving", false);
                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainersView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            animator.SetBool("isMoving", false);
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            cenfomons = GetComponent<CenfomonParty>().Cenfomons.Select(p=>p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        //Restaurar posición
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        //Restaurar Party
        GetComponent<CenfomonParty>().Cenfomons = saveData.cenfomons.Select(s=>new Cenfomon(s)).ToList();
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<CenfomonSaveData> cenfomons;
}

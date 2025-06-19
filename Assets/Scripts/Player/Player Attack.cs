using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTimer = 1f;

    [Header("Stance Settings")]
    [Tooltip("How long to remain in stance after each attack completes.")]
    [SerializeField] private float stanceDuration = 3f;
    private float stanceTimer = 0f;

    private int comboStep = 0;
    private float lastAttackTime;

    private void Update()
    {
        // 1) Check for left mouse button press
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttack();
        }

        // 2) If we're in stance, count down and exit if time is up
        if (animator.GetBool("InStance"))
        {
            stanceTimer -= Time.deltaTime;
            if (stanceTimer <= 0f)
            {
                ExitStance();
            }
        }
    }

    private void HandleAttack()
    {
        // Whether you are in normal idle or stance, you can start/continue the combo
        float timeSinceLastAttack = Time.time - lastAttackTime;

        // If we haven't clicked recently or are at combo step 0, begin new combo
        if (timeSinceLastAttack > comboResetTimer || comboStep == 0)
        {
            animator.SetTrigger("Attack1");
            comboStep = 1;
        }
        else
        {
            // If we are on the first attack, do the second
            if (comboStep == 1)
            {
                animator.SetTrigger("Attack2");
                comboStep = 2;
            }
            // (If you had more attacks, you'd keep going with else if)
        }

        // If we’re already in stance, optionally refresh the stance timer on each new attack:
        if (animator.GetBool("InStance"))
        {
            stanceTimer = stanceDuration;
        }

        lastAttackTime = Time.time;
    }

    /// <summary>
    /// Call this at the end of each *Recover animation* 
    /// (Hit1Recover or Hit2Recover) via Animation Event.
    /// </summary>
    public void EnterStance()
    {
        animator.SetBool("InStance", true);
        stanceTimer = stanceDuration;
    }

    public void ExitStance()
    {
        animator.SetBool("InStance", false);
        ResetCombo();
    }

    public void ResetCombo()
    {
        comboStep = 0;
    }
}

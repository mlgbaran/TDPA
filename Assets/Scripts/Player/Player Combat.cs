using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    public float comboWindow = 0.4f;      // Time window to press combo
    public float stanceDuration = 0.5f;   // Optional: stance hold time

    public Animator animator;

    private bool inComboWindow = false;
    private bool isAttacking = false;
    private Coroutine comboRoutine;

    [Header("Dash Settings")]
    public float dashDistanceHit1 = 1.0f;
    public float dashDurationHit1 = 0.15f;
    public float dashDistanceHit2 = 0.3f;
    public float dashDurationHit2 = 0.1f;
    private Coroutine dashRoutine;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                // First attack
                animator.SetTrigger("attack");
                isAttacking = true;
                if (dashRoutine != null) StopCoroutine(dashRoutine);
                dashRoutine = StartCoroutine(DashRight(dashDistanceHit1, dashDurationHit1));
            }
            else if (inComboWindow)
            {
                // Combo attack
                animator.SetTrigger("attack");
                EndComboWindow(); // prevents multiple triggers
                if (dashRoutine != null) StopCoroutine(dashRoutine);
                dashRoutine = StartCoroutine(DashRight(dashDistanceHit2, dashDurationHit2));
            }
        }
    }

    private IEnumerator DashRight(float distance, float duration)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.right * distance;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }

    // Called by Animation Event at last frame of Hit1
    public void StartComboWindow()
    {
        if (comboRoutine != null) StopCoroutine(comboRoutine);
        comboRoutine = StartCoroutine(ComboWindowCoroutine());
    }

    private IEnumerator ComboWindowCoroutine()
    {
        inComboWindow = true;
        yield return new WaitForSeconds(comboWindow);
        inComboWindow = false;
        // Animator transitions to Hit1Recover automatically after ComboWait finishes
    }

    // Optional: Called by Animation Event at start of Hit2
    public void EndComboWindow()
    {
        if (comboRoutine != null) StopCoroutine(comboRoutine);
        inComboWindow = false;
    }

    // Called by Animation Event at start of Hit1Recover/Hit2Recover
    public void LetAttackAgain()
    {
        isAttacking = false;

    }

   
}

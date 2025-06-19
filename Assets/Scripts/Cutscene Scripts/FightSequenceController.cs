using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightSequenceController : MonoBehaviour
{
    [Header("References")]
    public Animator fightAnimator;  // This animator has the 20 fight animations
    public QTEManager qteManager;   // A separate QTE Manager (optional, see below)

    [Header("Fight Steps")]
    public List<FightStep> fightSteps;   // Data about each step in the sequence

    private int currentStepIndex = 0;

    void Start()
    {
        // Optionally, start automatically
        // or you can call StartFightSequence() from outside
        StartFightSequence();
    }

    public void StartFightSequence()
    {
        currentStepIndex = 0;
        if (fightSteps.Count > 0)
        {
            PlayStep(fightSteps[currentStepIndex]);
        }
    }

    // Called when a step finishes (success or fail)
    private void OnStepComplete()
    {
        currentStepIndex++;
        if (currentStepIndex < fightSteps.Count)
        {
            PlayStep(fightSteps[currentStepIndex]);
        }
        else
        {
            Debug.Log("Fight sequence complete!");
            // Do any wrap-up logic here:
            // e.g., turn off this cutscene object,
            // re-enable the normal player, etc.
        }
    }

    private void PlayStep(FightStep step)
    {
        // In your Animator, you might have each fight step as a named state
        // or you can call animator.Play("SomeStateName", 0, 0f)
        fightAnimator.Play(step.animatorStateName);

        // If the step involves a QTE:
        if (step.isQTE)
        {
            // Option A: Use a QTE Manager
            qteManager.StartQTE(step.requiredKey, step.timeWindow, 
                                onSuccess: () => {
                                    // Trigger success animation/transition
                                    if (!string.IsNullOrEmpty(step.successTrigger))
                                        fightAnimator.SetTrigger(step.successTrigger);

                                    OnStepComplete();
                                },
                                onFailure: () => {
                                    // Trigger fail animation/transition
                                    if (!string.IsNullOrEmpty(step.failTrigger))
                                        fightAnimator.SetTrigger(step.failTrigger);

                                    OnStepComplete();
                                });
        }
        else
        {
            // If no QTE, we just wait for the Animator to reach the end of the clip.
            // E.g., set up an Animation Event at the end of the clip to call StepComplete
            // Or do a coroutine that waits for clip length:
            StartCoroutine(WaitForClipEnd(step.clipLength));
        }
    }

    IEnumerator WaitForClipEnd(float length)
    {
        yield return new WaitForSeconds(length);
        OnStepComplete();
    }
}

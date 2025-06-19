using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;


public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;                      // The main camera
    public GameObject playerGameObject;            // The player's GameObject
    public GameObject cameraPosGameObject;              // The FollowPlayer script on "CameraPos"
    public GameObject crosshair;                   // The crosshair GameObject in the scene

    [Header("UI")]
    public GameObject conversationPanel;               // Panel for dialogue
    public TextMeshProUGUI conversationText;           // TMP text field for lines

    public GameObject smsPanel;             // the cloned panel for SMS
    public TextMeshProUGUI smsDialogueText; // the text field in the SMS panel

    [Header("Camera Transition")]
    public float cameraTransitionTime = 0.5f;        // Smooth transition duration

    [Header("Options")]
    public float bubbleOffsetY = 0;
    public float bubbleOffsetX = 0;
    public float typingSpeed = 0.05f; // Time delay between each character



    // The data for the current conversation
    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool isAnimationPlaying = false;
    private bool isCurrentLineUnskippable;

    // We get these in runtime
    private PlayerMovement playerMovement;
    private FollowPlayer followPlayer;
    private PlayerRotation playerRotation;
    private Animator playerAnimator;

    private bool isDialogueActive = false;

    private Coroutine typingCoroutine;

    public event Action OnDialogueEnded;
    public event Action OnDialogueStarted;

    private RectTransform dialogueRect;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;
    private Vector2 originalAnchoredPosition;

    void Awake()
    {

        dialogueRect = conversationPanel.GetComponent<RectTransform>();

        // Store these at startup so we can restore them later
        originalAnchorMin = dialogueRect.anchorMin;
        originalAnchorMax = dialogueRect.anchorMax;
        originalPivot = dialogueRect.pivot;
        originalAnchoredPosition = dialogueRect.anchoredPosition;
        // Get the PlayerMovement component from the dragged-in Player GameObject
        if (playerGameObject != null)
        {
            playerMovement = playerGameObject.GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogWarning($"{playerGameObject.name} has no PlayerMovement component!");
            }

            playerRotation = playerGameObject.GetComponent<PlayerRotation>();
            if (playerRotation == null)
            {
                Debug.LogWarning($"{playerGameObject.name} has no PlayerRotation component!");
            }

            followPlayer = cameraPosGameObject.GetComponent<FollowPlayer>();
            if (followPlayer == null)
            {
                Debug.LogWarning($"{cameraPosGameObject.name} has no FollowPlayer component!");
            }
            playerAnimator = playerGameObject.GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogWarning($"{playerGameObject.name} has no Animator component!");
            }

        }
        else
        {
            Debug.LogWarning("No playerGameObject assigned in DialogueManager!");
        }

        conversationPanel.SetActive(false);
        smsPanel.SetActive(false);
    }

    void Update()
    {
        // If the dialogue is active and the player left-clicks, go to next line
        if (isDialogueActive && Input.GetMouseButtonDown(0) && !isAnimationPlaying && !isCurrentLineUnskippable)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;

                // Instantly show the full text
                DialogueLine line = currentDialogue.lines[currentLineIndex];
                SpeakerInfo speaker = currentDialogue.participants[line.speakerIndex];
                string speakerName = (speaker != null) ? speaker.speakerName : "Unknown";
                conversationText.text = $"{line.text}";
            }
            else
            {
                GoToNextLine();
            }
        }
    }


    /// <summary>
    /// Called by an NPC or some trigger to start a specific DialogueData
    /// </summary>
    /// 
    public void StartDialogue(DialogueData dialogue)
    {

        if (dialogue == null)
        {
            Debug.LogWarning("No DialogueData provided to StartDialogue!");
            return;
        }


        currentDialogue = dialogue;
        currentLineIndex = 0;

        // Hide both panels just in case
        conversationPanel.SetActive(false);
        smsPanel.SetActive(false);

        // Disable player movement so they can't run around during dialogue
        if (playerMovement != null)
        {
            //stop moving so the animator does not fuck up
            playerMovement.StopMoving();
            playerMovement.enabled = false;
        }

        OnDialogueStarted?.Invoke();


        //enable player lookatlookdirection
        if (currentDialogue.isSMS)
        {
            // SHOW the SMS panel, hide the normal one
            smsPanel.SetActive(true);
        }
        else
        {
            // SHOW the normal conversation panel
            conversationPanel.SetActive(true);

            // If you want NPC rotation, camera centering, etc. for normal dialogues
            if (playerRotation != null)
                playerRotation.lookAtLookDirection = true;
        }


        isDialogueActive = true;
        crosshair.SetActive(false);


        followPlayer.isDialogueMode = true;

        if (currentDialogue.isSMS)
        {

            // Adjust these values as you like

            // Trigger the phone-taking-out animation
            if (playerAnimator != null)
                playerAnimator.SetTrigger("takingOutPhone");

            OnCutsceneStarted();

            if (playerGameObject != null)
            {
                StartCoroutine(SmoothCenterCamera(playerGameObject.transform.position));
            }
        }
        else
        {

            // Center camera on participants
            Vector3 centroid = GetParticipantsCentroid(dialogue);
            if (centroid != Vector3.zero)
            {
                StartCoroutine(SmoothCenterCamera(centroid));
            }
        }

        ShowLine();
    }

    private void ShowLine()
    {
        // If there's no data or no lines, end immediately
        if (currentDialogue == null || currentDialogue.lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            // We're past the end of the list
            EndDialogue();
            return;
        }



        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (line.speakerIndex < 0 || line.speakerIndex >= currentDialogue.participants.Count)
        {
            Debug.LogWarning("Line has invalid speakerIndex!");
            EndDialogue();
            return;
        }

        isCurrentLineUnskippable = line.unskippable;

        SpeakerInfo speaker = currentDialogue.participants[line.speakerIndex];


        //string speakerName = (speaker != null) ? speaker.speakerName : "Unknown";


        TextMeshProUGUI activeTextField = (currentDialogue.isSMS) ? smsDialogueText : conversationText;

        string fullText = $"{line.text}";

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(fullText, activeTextField));

        //if not texting
        if (!currentDialogue.isSMS)
        {
            // The part that "finds" the GameObject:
            if (speaker != null && !string.IsNullOrEmpty(speaker.sceneObjectName))
            {
                GameObject speakerGO = GameObject.Find(speaker.sceneObjectName);
                if (speakerGO != null)
                {
                    // For example, center camera here
                    PositionconversationPanelOverSpeaker(speakerGO);
                }
                else
                {
                    Debug.LogWarning($"No GameObject named '{speaker.sceneObjectName}' found in the scene.");
                }
            }
        }




        //if not texting
        if (!currentDialogue.isSMS)
        {
            if (line.targetIndex >= 0 && line.targetIndex < currentDialogue.participants.Count)
            {
                SpeakerInfo targetSpeaker = currentDialogue.participants[line.targetIndex];
                if (targetSpeaker != null && !string.IsNullOrEmpty(targetSpeaker.sceneObjectName))
                {
                    GameObject targetGO = GameObject.Find(targetSpeaker.sceneObjectName);
                    if (targetGO != null)
                    {
                        Transform targetLookDir = targetGO.transform.Find("LookDirection");
                        if (targetLookDir != null && speaker != null)
                        {
                            GameObject spGO = GameObject.Find(speaker.sceneObjectName);
                            if (spGO != null)
                            {
                                targetLookDir.position = spGO.transform.position;
                            }
                        }
                    }
                }

                // Also make the Speaker face the Target
                if (speaker != null && targetSpeaker != null)
                {
                    GameObject spGO = GameObject.Find(speaker.sceneObjectName);
                    GameObject targetGO = GameObject.Find(targetSpeaker.sceneObjectName);

                    if (spGO != null && targetGO != null)
                    {
                        Transform speakerLookDir = spGO.transform.Find("LookDirection");
                        if (speakerLookDir != null)
                        {
                            speakerLookDir.position = targetGO.transform.position;
                        }
                    }
                }
            }
            else
            {
                // targetIndex == -1 => do nothing special for the speaker
            }

            // 4) Make all OTHER participants face the speaker
            for (int i = 0; i < currentDialogue.participants.Count; i++)
            {
                if (i == line.speakerIndex || i == line.targetIndex)
                    continue; // skip speaker & target

                SpeakerInfo other = currentDialogue.participants[i];
                if (other == null || string.IsNullOrEmpty(other.sceneObjectName))
                    continue;

                GameObject otherGO = GameObject.Find(other.sceneObjectName);
                if (otherGO == null)
                    continue;

                Transform otherLookDir = otherGO.transform.Find("LookDirection");
                if (otherLookDir != null && speaker != null)
                {
                    GameObject spGO = GameObject.Find(speaker.sceneObjectName);
                    if (spGO != null)
                    {
                        otherLookDir.position = spGO.transform.position;
                    }
                }
            }
        }




        // 3) PERFORM ACTIONS
        if (line.actions != null)
        {
            foreach (DialogueAction action in line.actions)
            {
                PerformAction(action);
            }
        }

    }

    private IEnumerator SmoothCenterCamera(Vector3 targetPosition)
    {
        if (!followPlayer)
            yield break;

        // Temporarily disable followPlayer's normal movement so it doesn't fight the tween


        bool originalDialogueMode = followPlayer.isDialogueMode;

        followPlayer.isDialogueMode = false;

        Vector3 startPos = followPlayer.transform.position;
        float elapsed = 0f;

        while (elapsed < cameraTransitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraTransitionTime);

            followPlayer.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        // Snap to exact final position
        followPlayer.transform.position = targetPosition;

        // Restore original isDialogueMode
        followPlayer.isDialogueMode = originalDialogueMode;

    }

    public void GoToNextLine()
    {
        currentLineIndex++;
        ShowLine();
    }

    private void CenterCameraOn(Transform t)
    {
        Vector3 newPos = t.position;
        newPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = newPos;
    }

    private void EndDialogue()
    {
        // Hide the UI
        if (currentDialogue != null && currentDialogue.isSMS)
        {
            // Hide SMS panel
            smsPanel.SetActive(false);

            // If you want "puttingDownPhone" animation
            if (playerAnimator != null)
                playerAnimator.SetTrigger("puttingDownPhone");
        }
        else
        {
            // Hide normal panel
            conversationPanel.SetActive(false);
        }
        isDialogueActive = false;
        crosshair.SetActive(true);

        followPlayer.isDialogueMode = false;

        OnDialogueEnded?.Invoke();

        if (!currentDialogue.isSMS)
        {
            if (playerMovement != null) playerMovement.enabled = true;
        }
        else
        {
            // For SMS, trigger "puttingDownPhone"
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("puttingDownPhone");
            }
            // DO NOT re-enable movement here; wait for the animation to finish
            dialogueRect.anchorMin = originalAnchorMin;
            dialogueRect.anchorMax = originalAnchorMax;
            dialogueRect.pivot = originalPivot;
            dialogueRect.anchoredPosition = originalAnchoredPosition;
        }

        // Re-enable player movement

        playerRotation.lookAtLookDirection = false;

        // Reset
        currentDialogue = null;
        currentLineIndex = 0;


    }

    private void PerformAction(DialogueAction action)
    {
        switch (action.actionType)
        {
            case DialogueActionType.TriggerAnimation:
                TriggerAnimationAction(action);
                isAnimationPlaying = true; // Animation starts
                break;
            // case DialogueActionType.PlaySound:
            //     ...
            //     break;
            default:
                Debug.LogWarning($"Unhandled action type {action.actionType}");
                break;
        }
    }

    private void TriggerAnimationAction(DialogueAction action)
    {
        // 1) find which participant is doing the animation
        if (action.targetIndex < 0 || action.targetIndex >= currentDialogue.participants.Count)
        {
            Debug.LogWarning("Invalid targetIndex for animation action!");
            return;
        }

        SpeakerInfo targetSpeaker = currentDialogue.participants[action.targetIndex];
        if (targetSpeaker == null || string.IsNullOrEmpty(targetSpeaker.sceneObjectName))
        {
            Debug.LogWarning("No SpeakerInfo or sceneObjectName for the target of animation action!");
            return;
        }

        // 2) find the speaker's GameObject
        GameObject targetGO = GameObject.Find(targetSpeaker.sceneObjectName);
        if (!targetGO)
        {
            Debug.LogWarning($"No GameObject named '{targetSpeaker.sceneObjectName}' found in scene!");
            return;
        }

        // 3) get an Animator
        Animator anim = targetGO.GetComponent<Animator>();
        if (!anim)
        {
            Debug.LogWarning($"No Animator on {targetGO.name}!");
            return;
        }

        // 4) set the trigger
        if (!string.IsNullOrEmpty(action.animationTriggerName))
        {
            Debug.Log("Triggering:");
            Debug.Log(action.animationTriggerName);
            anim.SetTrigger(action.animationTriggerName);
        }
        else
        {
            Debug.LogWarning("Animation trigger name is empty!");
        }
    }

    private void PositionconversationPanelOverSpeaker(GameObject speakerGO)
    {
        if (!speakerGO) return;

        // 1) Add a small world offset or not, as desired
        Vector3 worldPos = speakerGO.transform.position + new Vector3(bubbleOffsetX, bubbleOffsetY, 0);

        // 2) Convert world position to screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        // 3) Convert screen position to canvas local position
        RectTransform panelRect = conversationPanel.GetComponent<RectTransform>();
        Canvas canvas = conversationPanel.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 localPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out localPos))
        {
            // Set the dialogue panel's local position relative to the canvas
            panelRect.localPosition = localPos;
        }


    }


    /// <summary>
    /// Returns the average (centroid) position of all participants in the given dialogue.
    /// If no valid participant GameObjects are found, returns Vector3.zero.
    /// </summary>
    private Vector3 GetParticipantsCentroid(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.participants.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (SpeakerInfo speaker in dialogue.participants)
        {
            if (speaker != null && !string.IsNullOrEmpty(speaker.sceneObjectName))
            {
                GameObject speakerGO = GameObject.Find(speaker.sceneObjectName);
                if (speakerGO != null)
                {
                    sum += speakerGO.transform.position;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            return sum / count; // average
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void OnAnimationFinished()
    {
        isAnimationPlaying = false; // Animation has finished

    }

    private IEnumerator TypeText(string fullText, TextMeshProUGUI textField)
    {
        textField.text = ""; // Clear the text field
        foreach (char c in fullText)
        {
            textField.text += c; // Add one character at a time
            yield return new WaitForSeconds(typingSpeed); // Wait before adding the next character
        }

        typingCoroutine = null;
    }

    public void OnCutsceneFinished()
    {
        playerAnimator.SetBool("isInCutscene", false);
    }

    public void OnCutsceneStarted()
    {
        playerAnimator.SetBool("isInCutscene", true);
    }


}

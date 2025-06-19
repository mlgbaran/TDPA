using UnityEngine;
using System;
using System.Collections;

public class QTEManager : MonoBehaviour
{
    public void StartQTE(KeyCode key, float window, Action onSuccess, Action onFailure)
    {
        Debug.Log($"QTE started! Press {key} within {window} seconds...");

        // Start the coroutine that waits for input
        StartCoroutine(QTECoroutine(key, window, onSuccess, onFailure));
    }

    private IEnumerator QTECoroutine(KeyCode key, float window, Action onSuccess, Action onFailure)
    {
        float timer = 0f;
        bool succeeded = false;

        while (timer < window)
        {
            timer += Time.deltaTime;
            if (Input.GetKeyDown(key))
            {
                succeeded = true;
                Debug.Log("QTE Success! You pressed " + key);
                break;
            }
            yield return null;
        }

        if (!succeeded)
        {
            Debug.Log("QTE Failed. Time ran out or wrong key.");
            onFailure?.Invoke();
        }
        else
        {
            onSuccess?.Invoke();
        }
    }
}

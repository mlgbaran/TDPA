using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeakerInfo", menuName = "Dialogue/Speaker Info")]
public class SpeakerInfo : ScriptableObject
{
    public string speakerName;          // e.g., "Security"
    public string sceneObjectName;      // e.g., the name of the GameObject in the scene

    // Additional fields if you like (portrait, etc.)
}

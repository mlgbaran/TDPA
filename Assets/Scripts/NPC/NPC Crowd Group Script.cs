using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCrowdGroupScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform person1;
    public Transform person2;
    public Transform person3;
    public Transform person4;

    [SerializeField]
    private bool _groupIsTalking = true;

    public bool GroupIsTalking
    {
        get { return _groupIsTalking; }
        set
        {
            if (_groupIsTalking != value)
            {
                _groupIsTalking = value;
                SetIsTalkingForAll(value);
            }
        }
    }

    public float speakerChangeFreq;

    private int personCount;

    private Transform theSpeaker;

    public Transform[] npcTransforms;

    private void SetIsTalkingForAll(bool isTalking)
    {
        //Debug.Log("group talk value changed");
        // Set isTalking property for all NPCs
        foreach (Transform npcTransform in npcTransforms)
        {
            if (npcTransform != null)
            {
                //Debug.Log(npcTransform + " is not null");
                NPCTalk npcTalk = npcTransform.GetComponent<NPCTalk>();
                if (npcTalk != null)
                {
                    npcTalk.isTalking = isTalking;
                }
            }
        }


    }

    void Awake()
    {
        // Create a list to store non-null Transforms
        List<Transform> validTransforms = new List<Transform>();

        // Add non-null Transforms to the list
        if (person1 != null) validTransforms.Add(person1);
        if (person2 != null) validTransforms.Add(person2);
        if (person3 != null) validTransforms.Add(person3);
        if (person4 != null) validTransforms.Add(person4);

        // Assign the list to npcTransforms array
        npcTransforms = validTransforms.ToArray();

        //Debug.Log("Populated npcTransforms count: " + npcTransforms.Length);

        personCount = npcTransforms.Length;

    }


    void Start()
    {
        InvokeRepeating("ChangeSpeaker", 0f, 3f);
    }


    void ChangeSpeaker()
    {

        int randomTalkNumber = Random.Range(0, personCount);

        //Debug.Log(npcTransforms[randomTalkNumber].name + " is the speaker! everybody else in the group are looking at him!");

        for (int i = 0; i < personCount; i++)
        {
            if (i != randomTalkNumber)
            {
                //Debug.Log(npcTransforms[i].name + "'s look position has been set to the speaker.");
                npcTransforms[i].Find("LookDirection").position = npcTransforms[randomTalkNumber].position;

            }

        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Column : MonoBehaviour
{
    public SpriteRenderer Button;
    public KeyCode input { get; set; }

    public Note notePrefab;
    public List<Note> notes;

    public List<OsuToRlc.IHitObject> hitObjects = new();

    private int inputIndex;
    private int spawnIndex;

    void Start()
    {
        spawnIndex = 0;
        inputIndex = 0;
    }

    void Update()
    {
        if (spawnIndex < hitObjects.Count)
        {
            float time;
            while ((time = hitObjects[spawnIndex].Time / 1000f) - 1 <= Game.Time)
            {
                var note = Instantiate(notePrefab, transform);
                note.SpawnTime = time - 1;
                note.TapTime = time;
                notes.Add(note);
                spawnIndex++;
            }
        }

        if (inputIndex < hitObjects.Count)
        {
            float tapTime = hitObjects[inputIndex].Time / 1000f;
            float marginOfError = 0.1f;
            float time = Game.Time;

            if (Input.GetKeyDown(input))
            {
                if (Mathf.Abs(time - tapTime) < marginOfError)
                {
                    Debug.Log("Cool!");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else
                {
                    Debug.Log("Oops!");
                }
            }

            if (tapTime + marginOfError <= time)
            {
                Debug.Log("Miss!");
                inputIndex++;
            }
        }

        if (Input.GetKeyDown(input))
        {
            Button.color = Color.black;
        }

        else if (Input.GetKeyUp(input))
        {
            Button.color = Color.gray;
        }
    }
}

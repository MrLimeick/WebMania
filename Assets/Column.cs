using System.Collections;
using System.Collections.Generic;
using OsuToRlc;
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
        {
            float time;
            while (spawnIndex < hitObjects.Count && (time = hitObjects[spawnIndex].Time / 1000f) - 1 <= Game.Time)
            {
                var note = Instantiate(notePrefab, transform);
                note.SpawnTime = time - 1;
                note.TapTime = time;
                if (hitObjects[spawnIndex] is HoldNote holdNote)
                    note.UntapTime = holdNote.EndTime / 1000f;
                else
                    note.UntapTime = time;

                notes.Add(note);
                spawnIndex++;
            }
        }

        if (inputIndex < hitObjects.Count)
        {
            float tapTime = hitObjects[inputIndex].Time / 1000f;

            float marginOfError = 0.1f;
            float time = Game.Time;

            if (hitObjects[inputIndex] is HoldNote holdNote) 
            {
                float untapTime = holdNote.EndTime / 1000f;

                if (notes.Count - 1 >= inputIndex) // Hold нота существует
                {
                    if (Input.GetKeyDown(input))
                    {
                        if (Mathf.Abs(time - tapTime) < marginOfError)
                        {
                            // Нажата в правельное время
                            Debug.Log("Cool Down!");
                            notes[inputIndex].IsDown = true;
                        }
                        else
                        {
                            // Нажата в неправельное время
                            Debug.Log("Oops Down!");
                        }
                    }

                    if (notes[inputIndex].IsDown) // Hold нота была зажата до этого
                    {
                        if (Input.GetKeyUp(input))
                        {
                            // Ноту отжали

                            if (Mathf.Abs(time - untapTime) < marginOfError)
                            {
                                // Отжата в правильное время
                                Debug.Log("Cool Up!");
                            }
                            else
                            {
                                // Отжата в неправельное время
                                Debug.Log("Miss up!");
                            }

                            Destroy(notes[inputIndex].gameObject);
                            inputIndex++;
                        }
                        else
                        {
                            // Нота зажата
                        }
                    }
                }

                if (untapTime + marginOfError <= time) // Hold нота не была зажата в правильное время
                {
                    Debug.Log("Miss Hold!");
                    inputIndex++;
                }
            }
            else
            {
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

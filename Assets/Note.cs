using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{
	public float SpawnTime;
	public float TapTime;
	public float UntapTime;

	private const float spawnY = 6;
	private const float tapY = -4;
	private const float despawnY = -6;

	public SpriteRenderer SpriteRenderer;
	private float lenght;

	public bool IsDown = false;

    private void Start()
    {
		transform.localPosition = Vector2.up * spawnY;

        lenght = (UntapTime - TapTime) * (Mathf.Abs(spawnY) + Mathf.Abs(tapY));
        SpriteRenderer.size = new(1, lenght + 1);
        SpriteRenderer.transform.localPosition = new(0, lenght / 2);
    }

    void Update()
	{
		float t = Game.Time - SpawnTime;

		if(!IsDown) transform.localPosition = Vector2.LerpUnclamped(Vector2.up * spawnY, Vector2.up * tapY, t);
		else
		{
			transform.localPosition = Vector2.up * tapY;
            lenght = (UntapTime - Game.Time) * (Mathf.Abs(spawnY) + Mathf.Abs(tapY));
            SpriteRenderer.size = new(1, lenght + 1);
            SpriteRenderer.transform.localPosition = new(0, lenght / 2);
        }


		if (transform.localPosition.y + lenght < despawnY) Destroy(gameObject);
	}
}


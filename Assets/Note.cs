using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{
	public float SpawnTime;
	public float TapTime;

	public float spawnY = 6;
	public float tapY = -4;
	public float despawnY = -6;

    private void Awake()
    {
		transform.localPosition = Vector2.up * spawnY;
    }

    void Update()
	{
		float t = Game.Time - SpawnTime;

		transform.localPosition = Vector2.LerpUnclamped(Vector2.up * spawnY, Vector2.up * tapY, t);

		if (transform.localPosition.y < despawnY) Destroy(gameObject);
	}
}


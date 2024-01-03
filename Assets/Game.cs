using UnityEngine;
using System.Collections;
using OsuToRlc;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public Beatmap beatmap;

	public Column columnPrefab;
	public Column[] Columns { get; private set; } = null!;
	public int ColumnCount { get; private set; } = 4;

	public IReadOnlyList<KeyCode>[] Inputs =
	{
		new List<KeyCode>() { KeyCode.Space },
		new List<KeyCode>() { KeyCode.F, KeyCode.J },
		new List<KeyCode>() { KeyCode.F, KeyCode.Space, KeyCode.J },
		new List<KeyCode>() { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K },
		new List<KeyCode>() { KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K },
		new List<KeyCode>() { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L },
		new List<KeyCode>() { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L },
		new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon },
        new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon },
        new List<KeyCode>() { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.V, KeyCode.N, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon },
    };

	public string pathToBeatmap;

	private static Game Instance;

	public AudioSource musicSource;
	public static float Time => Instance.musicSource.time;

	public RawImage Background;

    void Start()
	{
		if (Instance != null) Destroy(Instance);
		
		Instance = this;
		Debug.Log(Application.streamingAssetsPath);
		StartCoroutine(LoadBeatmap((beatmap, clip, texture) =>
        {
			this.beatmap = beatmap;
			SetBackground(texture);
			StartGame(clip);
		}));
	}

	IEnumerator LoadBeatmap(Action<Beatmap, AudioClip, Texture2D> action)
	{
#if UNITY_WEBGL
		string mapDir = Application.streamingAssetsPath + "/Map/";
#else
        string mapDir = "file://" + Application.streamingAssetsPath + "/Map/";
#endif

		string beatmapUri = mapDir + "Map.osu";
        Beatmap beatmap = null!;
		using (UnityWebRequest beatmapRequest = UnityWebRequest.Get(beatmapUri))
		{
            yield return beatmapRequest.SendWebRequest();
            if (beatmapRequest.result != UnityWebRequest.Result.Success) throw new Exception("Beatmap download failed");
			else
			{
                using MemoryStream stream = new(beatmapRequest.downloadHandler.data);
				beatmap = new(stream);
            }
        }

        AudioClip audioClip = null!;
		string audioUri = mapDir + beatmap.General.AudioFilename;

        using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(audioUri, AudioType.UNKNOWN))
		{
			yield return audioRequest.SendWebRequest();
			if (audioRequest.result != UnityWebRequest.Result.Success) throw new Exception("audio download failed");
			else audioClip = DownloadHandlerAudioClip.GetContent(audioRequest);
        }

        Texture2D background = null!;
		string backgroundUri = mapDir + beatmap.BackgroundFilename;

        using (UnityWebRequest backgroundRequest = UnityWebRequestTexture.GetTexture(backgroundUri))
		{
			yield return backgroundRequest.SendWebRequest();
			if (backgroundRequest.result != UnityWebRequest.Result.Success) throw new Exception("background download failed");
			else background = DownloadHandlerTexture.GetContent(backgroundRequest);
        }

		action?.Invoke(beatmap, audioClip, background);
	}

	//IEnumerator LoadTexture(string uri, Action<Texture2D> texture)
	//{
 //       using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri);
 //       yield return webRequest.SendWebRequest();

 //       if (webRequest.result == UnityWebRequest.Result.Success) texture?.Invoke(DownloadHandlerTexture.GetContent(webRequest));
 //       else throw new Exception("Texture get error! " + webRequest.error);
 //   }

	void SetBackground(Texture2D texture)
	{
		Background.texture = texture;
		Background.GetComponent<AspectRatioFitter>().aspectRatio = texture.width / (float)texture.height;
	}

	//IEnumerator LoadAudio(string uri, Action<AudioClip> clip)
	//{
	//	using UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN);
	//	yield return webRequest.SendWebRequest();

	//	if (webRequest.result == UnityWebRequest.Result.Success) clip?.Invoke(DownloadHandlerAudioClip.GetContent(webRequest));
	//	else throw new Exception("Audio clip get error! " + webRequest.error);
	//}

    void StartGame(AudioClip clip)
	{
		musicSource.Stop();

        LoadColumns();

		musicSource.clip = clip;
		musicSource.time = 0;
        musicSource.Play();
    }

	void LoadColumns()
	{
		ColumnCount = Mathf.RoundToInt(beatmap.Difficulty.CircleSize);
        Columns = new Column[ColumnCount];

        IReadOnlyList<KeyCode> inputs = Inputs[ColumnCount - 1];

		float w = columnPrefab.transform.lossyScale.x;
		float offsetX = (w * ColumnCount / 2) + (0.1f * (ColumnCount - 1) / 2) - (w / 2);

		for(int i = 0; i < ColumnCount; i++)
		{
			var column = Instantiate(columnPrefab);
			column.input = inputs[i];
			column.transform.position = new(-offsetX + (w + 0.1f) * i , 0);

			Columns[i] = column;
		}

		foreach(var hitObject in beatmap.HitObjects)
		{
			int columnIndex = Mathf.FloorToInt(hitObject.Position.x * ColumnCount / 512);
			Columns[columnIndex].hitObjects.Add(hitObject);
        }
    }
}


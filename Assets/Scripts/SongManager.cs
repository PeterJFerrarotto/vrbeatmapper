using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VRBeatMapper
{
    public class SongManager : MonoBehaviour
    {
        private static SongManager instance;

        public static SongManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SongManager>();
                }
                return instance;
            }
        }

        private float beatsPerMinute;
        private float beatsPerSecond;

        public GameObject loadingOverlay;

        public AudioSource AudioObject;

        public UnityEngine.UI.Slider TimeSlider;

        public UnityEngine.UI.Text BeatCounter;

        private float songDuration;
        [SerializeField]
        public float songCurrentTime;

        private float secondsPerBeat;

        private decimal beatCount;

        [SerializeField]
        private decimal beatNum;

        private float beatNumRaw;

        public Transform[] lanePositions;

        public GameObject mapOffset;

        public GameObject BlueCubePrefab;
        public GameObject RedCubePrefab;

        private bool audioLoaded = false;
        // Start is called before the first frame update
        void Start()
        {
            loadingOverlay.SetActive(true);
            if (AudioObject != null)
            {
                StartCoroutine("LoadAudioAndMap");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (AudioObject.isPlaying)
            {
                songCurrentTime = AudioObject.time;
                TimeSlider.value = AudioObject.time;
                beatNumRaw = songCurrentTime * beatsPerSecond;
            } else
            {
                songCurrentTime = TimeSlider.value;
                AudioObject.time = songCurrentTime;
                beatNumRaw = TimeSlider.value * beatsPerSecond;
            }
            beatNum = decimal.Round((decimal)beatNumRaw, 2);
            BeatCounter.text = "Beat: " + beatNum.ToString();
            mapOffset.transform.localPosition = new Vector3(0, 0, -beatNumRaw);
        }

        public void PlayPauseSong()
        {
            Debug.Log("Pressed play/pause");
            if (AudioObject.isPlaying)
            {
                AudioObject.Pause();
            }
            else
            {
                AudioObject.UnPause();
            }
        }

        public void StartDrag()
        {
            Debug.Log("started dragging");
            AudioObject.Pause();
        }

        public void StopDrag()
        {
            Debug.Log("stopped dragging");
            AudioObject.time = TimeSlider.value;
            AudioObject.UnPause();
        }

        private IEnumerator LoadAudioAndMap()
        {
            var downloadHandler = new DownloadHandlerAudioClip(GameManager.Instance.songToUse.audioFilePath, AudioType.OGGVORBIS);
            downloadHandler.compressed = false;
            downloadHandler.streamAudio = true;
            var uwr = new UnityWebRequest(
                    GameManager.Instance.songToUse.audioFilePath,
                    UnityWebRequest.kHttpVerbGET,
                    downloadHandler,
                    null);
            Debug.Log(GameManager.Instance.songToUse.audioFilePath);
            var request = uwr.SendWebRequest();
            while (!request.isDone)
                yield return null;
            AudioObject.clip = DownloadHandlerAudioClip.GetContent(uwr);
            songDuration = AudioObject.clip.length;
            beatsPerMinute = (float)GameManager.Instance.songToUse.songInfo.BeatsPerMinute;
            beatsPerSecond = beatsPerMinute / 60;
            secondsPerBeat = 1 / beatsPerSecond;
            beatCount = decimal.Round((decimal)(beatsPerSecond * songDuration), 2);
            beatNum = 0.0m;
            TimeSlider.maxValue = songDuration;
            TimeSlider.value = 0;
            LoadMap();
            audioLoaded = true;
            loadingOverlay.SetActive(false);
            AudioObject.Play();
        }

        private void LoadMap()
        {
            foreach (Note noteBlock in GameManager.Instance.difficultyToEdit.Notes)
            {
                int spawnerIndex = (noteBlock.LineLayer * 4) + noteBlock.LineIndex;
                GameObject block;
                if (noteBlock.Type == (int)NoteType.LEFT)
                {
                    block = GameObject.Instantiate(RedCubePrefab, lanePositions[spawnerIndex]);
                } else
                {
                    block = GameObject.Instantiate(BlueCubePrefab, lanePositions[spawnerIndex]);
                }
                block.transform.localPosition = new Vector3(0, 0, (float)(decimal.Round((decimal)noteBlock.Time, 2)));
                float rotation = 0f;

                switch (noteBlock.CutDirection)
                {
                    case (int)CutDirection.TOP:
                        rotation = 0f;
                        break;
                    case (int)CutDirection.BOTTOM:
                        rotation = 180f;
                        break;
                    case (int)CutDirection.LEFT:
                        rotation = 270f;
                        break;
                    case (int)CutDirection.RIGHT:
                        rotation = 90f;
                        break;
                    case (int)CutDirection.TOPLEFT:
                        rotation = 315f;
                        break;
                    case (int)CutDirection.TOPRIGHT:
                        rotation = 45f;
                        break;
                    case (int)CutDirection.BOTTOMLEFT:
                        rotation = 225f;
                        break;
                    case (int)CutDirection.BOTTOMRIGHT:
                        rotation = 125f;
                        break;
                    case (int)CutDirection.NONDIRECTION:
                        rotation = 0f;
                        break;
                    default:
                        break;
                }
                block.transform.Rotate(transform.forward, rotation);
            }
        }

    }
}
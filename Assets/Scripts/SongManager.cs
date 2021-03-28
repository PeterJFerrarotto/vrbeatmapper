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

        private bool isCloserToGreaterAngle(float angle1, float angle2, float currentAngle)
        {
            return (angle2 - currentAngle) < (currentAngle - angle1);
        }

        private float determineClosestAngle(float angle1, float angle2, float currentAngle)
        {
            if (isCloserToGreaterAngle(angle1, angle2, currentAngle))
            {
                return angle2;
            }
            return angle1;
        }

        private CutDirection GetCutDirection(float angle)
        {
            switch (angle)
            {
                case 0.0f:
                    return CutDirection.TOP;
                case 45.0f:
                    return CutDirection.TOPRIGHT;
                case 90.0f:
                    return CutDirection.RIGHT;
                case 135.0f:
                    return CutDirection.BOTTOMRIGHT;
                case 180.0f:
                    return CutDirection.BOTTOM;
                case 225.0f:
                    return CutDirection.BOTTOMLEFT;
                case 270.0f:
                    return CutDirection.LEFT;
                case 315.0f:
                    return CutDirection.TOPLEFT;
                default:
                    return CutDirection.NONDIRECTION;
            }
        }

        public void SaveSong()
        {
            GameManager.Instance.SaveSongChanges();
        }

        public void AddNote(int laneIndex, NoteType noteType, Vector3 direction)
        {
            float timeToUse = (float)beatNum;
            GameObject noteBlock;
            if (noteType == NoteType.LEFT)
            {
                noteBlock = Instantiate(RedCubePrefab, lanePositions[laneIndex]);
            }
            else
            {
                noteBlock = Instantiate(BlueCubePrefab, lanePositions[laneIndex]);
            }

            noteBlock.transform.localPosition = new Vector3(0, 0, (float)beatNum);

            float angle = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x);
            if (angle < 0)
            {
                angle += 360.0f;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            CutDirection cutDirection;
            Debug.Log(angle);
            if (angle <= 45)
            {
                angle = determineClosestAngle(0, 45, angle);
            }
            else if (angle > 45 && angle <= 90)
            {
                angle = determineClosestAngle(45, 90, angle);
            }
            else if (angle > 90 && angle <= 125)
            {
                angle = determineClosestAngle(90, 135, angle);
            }
            else if (angle > 125 && angle <= 180)
            {
                angle = determineClosestAngle(135, 180, angle);
            }
            else if (angle > 180 && angle <= 225)
            {
                angle = determineClosestAngle(180, 225, angle);
            }
            else if (angle > 225 && angle <= 270)
            {
                angle = determineClosestAngle(225, 270, angle);
            }
            else if (angle > 270 && angle <= 315)
            {
                angle = determineClosestAngle(270, 315, angle);
            }
            else if (angle > 315)
            {
                angle = 315;
            }
            noteBlock.transform.Rotate(transform.forward, angle);
            cutDirection = GetCutDirection(angle);
            Note note = new Note();
            note.CutDirection = (int)cutDirection;
            note.Time = timeToUse;
            note.LineLayer = laneIndex / 4;
            Debug.Log(note.LineLayer);
            note.LineIndex = laneIndex % 4;
            note.Type = (int)noteType;
            GameManager.Instance.difficultyToEdit.Notes.Add(note);
        }

    }
}
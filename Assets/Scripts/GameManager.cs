using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace VRBeatMapper
{
    public class GameManager : MonoBehaviour
    {
#if DEBUG
        private string songPath = "./Assets/BeatMaps/";
#else
        private string songPath = Application.persistentDataPath;
#endif


        public Dictionary<string, BeatMap> loadedSongs
        {
            get;
            private set;
        }

        public bool SongsLoaded
        {
            get;
            private set;
        }

        public BeatMap songToUse
        {
            get;
            private set;
        }
        public SongDifficulty difficultyToEdit
        {
            get;
            private set;
        }

        private string difficultyName;

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (FindObjectsOfType<GameManager>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LoadSongs());
        }

        public void ForceLoad()
        {
            StartCoroutine(LoadSongs());
        }

        private IEnumerator LoadSongs()
        {
            SongsLoaded = false;
            loadedSongs = new Dictionary<string, BeatMap>();
            foreach (string d in System.IO.Directory.GetDirectories(songPath))
            {
                Debug.Log(d);
                if (System.IO.File.Exists(d + "/info.dat"))
                {
                    Debug.Log("found info.dat");
                    BeatMap beatMap = new BeatMap();
                    beatMap.path = d;
                    SongInfo info = LoadSongInfo(d + "/info.dat");
                    beatMap.songInfo = info;
                    beatMap.audioFilePath = System.IO.Path.GetFullPath(d +"/" + info.SongFilename);
                    beatMap.difficultyMaps = LoadDifficulties(info, d);
                    loadedSongs.Add(info.SongName, beatMap);
                }
            }
            SongsLoaded = true;
            yield return null;
        }

        private Dictionary<string, SongDifficulty> LoadDifficulties(SongInfo info, string path)
        {
            Dictionary<string, SongDifficulty> difficulties = new Dictionary<string, SongDifficulty>();
            foreach (DifficultyBeatmapSet set in info.DifficultyBeatmapSets)
            {
                string modeName = set.BeatmapCharacteristicName;
                foreach (DifficultyBeatmap diff in set.DifficultyBeatmaps)
                {
                    string diffName = modeName + " | " + diff.Difficulty;
                    string filePath = System.IO.Path.GetFullPath(path + "/" + diff.BeatmapFilename);
                    SongDifficulty loadedDiff = LoadDifficulty(filePath);
                    difficulties.Add(diffName, loadedDiff);
                }
            }
            return difficulties;
        }

        private SongDifficulty LoadDifficulty(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception("File does not exist");
            }
            SongDifficulty difficulty = JsonConvert.DeserializeObject<SongDifficulty>(System.IO.File.ReadAllText(filePath));
            difficulty.filePath = filePath;
            return difficulty;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void GoToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        public void GoToEditor()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SongEdit");
        }

        private SongInfo LoadSongInfo(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception("File does not exist");
            }
            Debug.Log("loading song info...");
            SongInfo songInfo = JsonConvert.DeserializeObject<SongInfo>(System.IO.File.ReadAllText(filePath));
            return songInfo;
        }

        public void SelectSong(string songName)
        {
            if (loadedSongs.ContainsKey(songName))
            {
                songToUse = loadedSongs[songName];
            }
        }

        public void SelectDifficulty(string difficulty)
        {
            Debug.Log(difficulty);
            if (songToUse.difficultyMaps.ContainsKey(difficulty))
            {
                difficultyToEdit = songToUse.difficultyMaps[difficulty];
                difficultyName = difficulty;
            }
        }

        public void SaveSongChanges()
        {
            string toWrite = JsonConvert.SerializeObject(difficultyToEdit);
            System.IO.File.WriteAllText(difficultyToEdit.filePath, toWrite);
        }

        //private IEnumerator loadAudioClip(string audioFilePath)
        //{
        //    Debug.Log("loading audio clip...");
        //    if (!System.IO.File.Exists(audioFilePath))
        //    {
        //        Debug.Log("file does not exist!");
        //        throw new System.Exception(audioFilePath + " does not exist!");
        //    }
           
        //    AudioClip temp = null;
        //    UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, AudioType.OGGVORBIS);
        //    yield return www.SendWebRequest();
        //        Debug.Log("sending request for local file...");
        //        yield return www.SendWebRequest();
        //        if (www.result == UnityWebRequest.Result.ConnectionError)
        //        {
        //            Debug.Log(www.error);
        //        }
        //        else
        //        {
        //            temp = DownloadHandlerAudioClip.GetContent(www);
        //        }
        //    yield return temp;
        //}
    }
}
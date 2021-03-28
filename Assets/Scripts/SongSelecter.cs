using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VRBeatMapper
{
    public class SongSelecter : MonoBehaviour
    {
        [SerializeField]
        public UnityEngine.UI.Dropdown songSelection;
        [SerializeField]
        public UnityEngine.UI.Dropdown difficultySelection;
        [SerializeField]
        public UnityEngine.UI.Button editButton;
        [SerializeField]
        public GameObject loadingOverlay;
        private IEnumerator PopulateSongList()
        {
            do
            {
                songSelection.enabled = false;
                difficultySelection.enabled = false;
                editButton.enabled = false;
                loadingOverlay.SetActive(true);
                yield return null;
            } while (!GameManager.Instance.SongsLoaded);
            songSelection.ClearOptions();
            PopulateSongDropDown();
            songSelection.enabled = true;
            difficultySelection.enabled = true;
            editButton.enabled = true;
            loadingOverlay.SetActive(false);
        }

        private void PopulateSongDropDown()
        {

            List<string> songNames = new List<string>();
            foreach (string songName in GameManager.Instance.loadedSongs.Keys)
            {
                songNames.Add(songName);
            }
            songSelection.AddOptions(songNames);
            GameManager.Instance.SelectSong(songSelection.options[0].text);
            PopulateDifficultyDropDown();
        }

        private void PopulateDifficultyDropDown()
        {
            difficultySelection.ClearOptions();
            List<string> difficultyNames = new List<string>();
            foreach(string difficultyName in GameManager.Instance.songToUse.difficultyMaps.Keys)
            {
                difficultyNames.Add(difficultyName);
            }
            difficultySelection.AddOptions(difficultyNames);
            GameManager.Instance.SelectDifficulty(difficultySelection.options[0].text);
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(PopulateSongList());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SelectSong(int value)
        {
            string selection = songSelection.options[value].text;
            GameManager.Instance.SelectSong(selection);
            PopulateDifficultyDropDown();
        }

        public void SelectDifficulty(int value)
        {
            string selection = difficultySelection.options[value].text;
            GameManager.Instance.SelectDifficulty(selection);
        }

        public void EditSong()
        {
            GameManager.Instance.GoToEditor();
        }
    }
}
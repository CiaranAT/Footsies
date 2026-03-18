using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Footsies

{
    public class GameManager : Singleton<GameManager>
    {
        public enum SceneIndex
        {
            Title = 1,
            Battle = 2,
        }

        public enum GameMode
        {
            Tutorial = 0,
            PvsP = 1,
            VsILCPU = 2,
            VsBaseCPU = 3,
            ILCPUVsILCPU = 4,
        }

        public AudioClip menuSelectAudioClip;

        public SceneIndex currentScene { get; private set; }
        public GameMode gameMode { get; private set; }
        public bool isFilewriteEnabled { get; private set; }
        public bool isInfiniteMatchEnabled { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            LoadTitleScene();
            isFilewriteEnabled = true;
            isInfiniteMatchEnabled = false;

            loadTrainingEnv(); //used for training build, remove when building for release
        }

        private void Update()
        {
            if(currentScene == SceneIndex.Battle)
            {
                if(Input.GetButtonDown("Cancel"))
                {
                    LoadTitleScene();
                }
            }
        }

        public void LoadTitleScene()
        {
            SceneManager.LoadScene((int)SceneIndex.Title);
            currentScene = SceneIndex.Title;
        }

        public void LoadVsPlayerScene()
        {
            gameMode = GameMode.PvsP;
            LoadBattleScene();
        }

        public void LoadVsCPUScene()
        {
            gameMode = GameMode.VsILCPU;
            LoadBattleScene();
        }

        public void LoadCPUVsCPUScene()
        {
            gameMode = GameMode.ILCPUVsILCPU;
            LoadBattleScene();
        }

        public void toggleSettingsMenu() 
        {
            Transform settingsMenu = GameObject.Find("TitleCanvas").transform.Find("SettingsMenuScreen");

            if (settingsMenu.gameObject.active)
            {
                settingsMenu.gameObject.SetActive(false);
            }
            else settingsMenu.gameObject.SetActive(true);
        }

        public bool toggleFilewrite()
        {
            if (isFilewriteEnabled)
            {
                isFilewriteEnabled = false;
            }
            else
            {
                isFilewriteEnabled = true;
            }

            return isFilewriteEnabled;
        }

        public bool toggleMatchLooping()
        {
            if (isInfiniteMatchEnabled)
            {
                isInfiniteMatchEnabled = false;
            }
            else
            {
                isInfiniteMatchEnabled = true;
            }

            return isInfiniteMatchEnabled;
        }

        private void LoadBattleScene()
        {
            SceneManager.LoadScene((int)SceneIndex.Battle);
            currentScene = SceneIndex.Battle;

            if(menuSelectAudioClip != null)
            {
                SoundManager.Instance.playSE(menuSelectAudioClip);
            }
        }

        public bool checkCanFilewrite()
        {
            //don't write to files or store data if match is looping to avoid memory issues, or during the tutorial as it isn't a real match
            return isFilewriteEnabled && gameMode != GameMode.Tutorial && !isInfiniteMatchEnabled;
        }

        private void loadTrainingEnv()
        {
            isFilewriteEnabled = false;
            isInfiniteMatchEnabled = true;
            LoadCPUVsCPUScene();
        }
    }

}
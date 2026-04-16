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
            Tutorial = 0, //Added for honours project, basic tutorial against cpus with linear deterministic behaviour, used to onboard players during user testing
            PvsP = 1,
            VsAgent = 2, //Added for honours project
            VsBaseCPU = 3, //Amended for honours project, plays a match against the game's original CPU
            AgentVsAgent = 4, //Added for honours project, both player 1 and 2 are the trained playing agent. Can be used for training agents at once by adjusting the behaviour parameters of the agent game objects in battleScene
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

            //loadTrainingEnv(); //automatically loads agent training gamemode on game launch, used for training build, remove when building for release
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

        public void LoadVsAgentScene()
        {
            gameMode = GameMode.VsAgent;
            LoadBattleScene();
        }

        public void LoadAgentVsAgentScene()
        {
            gameMode = GameMode.AgentVsAgent;
            LoadBattleScene();
        }

        public void LoadVsBaseCPU()
        {
            gameMode = GameMode.VsBaseCPU;
            LoadBattleScene();
        }

        public void LoadTutorial()
        {
            gameMode = GameMode.Tutorial;
            LoadBattleScene();
        }

        public void toggleSettingsMenu() // Added for honours, opens the settings menu on the title screen that can toggle filewrite and enable looping matches for use in project demo
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
            LoadAgentVsAgentScene();
        }
    }

}
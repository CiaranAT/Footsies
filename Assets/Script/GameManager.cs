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
            isFilewriteEnabled = false;
            isInfiniteMatchEnabled = false;
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

        private void LoadBattleScene()
        {
            SceneManager.LoadScene((int)SceneIndex.Battle);
            currentScene = SceneIndex.Battle;

            if(menuSelectAudioClip != null)
            {
                SoundManager.Instance.playSE(menuSelectAudioClip);
            }
        }
    }

}
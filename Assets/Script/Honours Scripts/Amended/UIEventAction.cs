using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Footsies
{
    public class UIEventAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public enum Action
        {
            LoadVsAgent,
            LoadVsPlayer,
            LoadVsBaseCPU,
            LoadTutorial,
            ExitGame,
            BGMToggle,
            SEToggle,
            SettingsMenuToggle,
            LoadAgentVsAgent,
            FileToggle,
            LoopToggle
        }

        public Action action;

        private void Awake()
        {
            Toggle toggle = null;
            switch (action)
            {
                case Action.BGMToggle:
                    toggle = gameObject.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = SoundManager.Instance.isBGMOn;
                    }
                    break;
                case Action.FileToggle:
                    toggle = gameObject.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = GameManager.Instance.isFilewriteEnabled;
                    }
                    break;
                case Action.LoopToggle:
                    toggle = gameObject.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = GameManager.Instance.isInfiniteMatchEnabled;
                    }
                    break;


            }
        }

        public void InvokeAction()
        {
            switch(action)
            {
                case Action.LoadVsAgent:
                    LoadVsAgent();
                    break;
                case Action.LoadVsPlayer:
                    LoadVsPlayer();
                    break;
                case Action.ExitGame:
                    ExitGame();
                    break;
                case Action.BGMToggle:
                    toggleBGM();
                    break;
                case Action.SettingsMenuToggle:
                    toggleSettingsMenu();
                    break;
                case Action.SEToggle:
                    break;
                case Action.LoadAgentVsAgent:
                    LoadAgentVsAgent();
                    break;
                case Action.LoopToggle:
                    toggleMatchLooping();
                    break;
                case Action.FileToggle:
                    toggleFilewrite();
                    break;
                case Action.LoadVsBaseCPU:
                    LoadVsBaseCPU();
                    break;
                case Action.LoadTutorial:
                    LoadTutorial();
                    break;
            }
        }

        public void LoadVsAgent()
        {
            GameManager.Instance.LoadVsAgentScene();
        }

        public void LoadVsPlayer()
        {
            GameManager.Instance.LoadVsPlayerScene();
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void LoadAgentVsAgent()
        {
            GameManager.Instance.LoadAgentVsAgentScene();
        }

        public void LoadVsBaseCPU()
        {
            GameManager.Instance.LoadVsBaseCPU();
        }

        public void LoadTutorial()
        {
            GameManager.Instance.LoadTutorial();
        }

        public void toggleBGM()
        {
            var isOn = SoundManager.Instance.toggleBGM();
            var toggle = gameObject.GetComponent<Toggle>();
            if(toggle != null)
            {
                toggle.isOn = isOn;
            }
        }

        public void toggleSettingsMenu()
        {
            GameManager.Instance.toggleSettingsMenu();
        }
        public void toggleFilewrite()
        {
            var isOn = GameManager.Instance.toggleFilewrite();
            var toggle = gameObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = isOn;
            }
        }

        public void toggleMatchLooping() { 
            var isOn = GameManager.Instance.toggleMatchLooping();
            var toggle = gameObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = isOn;
            }
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }

}
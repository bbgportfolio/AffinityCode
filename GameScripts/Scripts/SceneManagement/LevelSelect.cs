using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    public class LevelSelect : MonoBehaviour
    {
        public List<Color> LevelColors;
        public TextMeshProUGUI Text;
        public Button RightButton;
        public Button LeftButton;
        public Button StartButton;
        private SceneController sceneController;
        private int selectedLevel;
        private int maxIndex;

        // Start is called before the first frame update
        void Start()
        {
            UpdateLevel();
        }

        void OnEnable()
        {
            Reset();
            UpdateLevel();
        }

        private void Reset()
        {
            StartButton.interactable = true;
        }

        public void SetSceneIndex(SceneController sceneControl, int index)
        {
            sceneController = sceneControl;
            maxIndex = index;
            selectedLevel = index;
        }

        public void Left()
        {
            selectedLevel--;
            if (selectedLevel < 1)
                selectedLevel = 1;

            UpdateLevel();
        }

        public void Right()
        {
            selectedLevel++;
            if (selectedLevel > maxIndex)
                selectedLevel = maxIndex;

            UpdateLevel();
        }

        private void UpdateLevel()
        {
            if (selectedLevel == maxIndex)
            {
                RightButton.interactable = false;
            }
            else
            {
                RightButton.interactable = true;
            }
            if (selectedLevel == 1)
            {
                LeftButton.interactable = false;
            }
            else
            {
                LeftButton.interactable = true;
            }
            Text.text = selectedLevel.ToString();
            Text.color = LevelColors[selectedLevel];
        }

        public void PushStart()
        {
            sceneController.FadeAndLoadScene("Level" + selectedLevel);
            sceneController.SceneIndex = selectedLevel + 1;
            StartButton.interactable = false;
        }
    }
}

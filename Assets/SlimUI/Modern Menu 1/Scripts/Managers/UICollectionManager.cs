using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SlimUI.ModernMenu
{
    public class UICollectionManager : MonoBehaviour
    {
        public enum Platform { Desktop, Mobile }
        public enum MenuPage { General, Advanced }

        [Header("General Settings Panel (Grid)")]
        public GameObject generalSettingsPanel; // assign parent GameObject with GridLayoutGroup

        [Header("Advanced Settings Panel (Grid)")]
        public GameObject advancedSettingsPanel; // assign parent GameObject with GridLayoutGroup

        [Header("Platform Settings")]
        public Platform platform;

        private MenuPage _currentPage = MenuPage.General;

        private void Start()
        {
            ShowPage(_currentPage);
        }

        #region Menu Navigation
        public void ShowGeneral()
        {
            ShowPage(MenuPage.General);
        }

        public void ShowAdvanced()
        {
            ShowPage(MenuPage.Advanced);
        }

        private void ShowPage(MenuPage page)
        {
            _currentPage = page;
            generalSettingsPanel.SetActive(page == MenuPage.General);
            advancedSettingsPanel.SetActive(page == MenuPage.Advanced);
        }
        #endregion

        #region Helpers
        private void SetFullScreen(bool isOn)
        {
            Screen.fullScreen = isOn;
        }
        #endregion
    }
}

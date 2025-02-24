using System.Collections.Generic;
using UnityEngine;

namespace EEA.UI
{
    public enum eCanvasType
    {
        Hud = 0,
        Overlay,
        Popup,
        Count
    }

    public class UICanvasGroup : MonoBehaviour
    {
        [SerializeField] List<Canvas> _canvases = new List<Canvas>();

        private void Start()
        {
            //int screenWidth = 1480;
            //int screenHeight = 720;

            //Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.FullScreenWindow);
        }

        public Canvas GetCanvas(eCanvasType type)
        {
            if ((int)type < 0 || (int)type >= _canvases.Count)
                return null;

            return _canvases[(int)type];
        }
    }
}

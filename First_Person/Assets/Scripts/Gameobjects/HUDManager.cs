using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private Text[] textElements;         // 0 - Left weapon count. 1 - Right weapon count

    private RectTransform reticuleTransform;
    private ushort[] reticuleSizes;                         // 0 - Regular. 1 - Zoomed
    #endregion

    #region Initialization
    private void Awake()
    {
        reticuleTransform = GameObject.Find("Reticle").GetComponent<RectTransform>();

        // Assigning initial reticule size
        reticuleSizes = new ushort[2] { 95, 305 };
    }
    #endregion

    #region Public Interface
    public void ToggleReticuleSize()
    {
        byte index = (reticuleTransform.rect.height == reticuleSizes[0]) ? (byte)1 : (byte)0;
        reticuleTransform.sizeDelta = new Vector2(reticuleSizes[index], reticuleSizes[index]);
    }
    public void ToggleTextElementVisibility()
    {
        // Decides if elements (round counts for the weapons) should be visible or not
        bool result = (textElements[0].enabled) ? false : true;

        // Sets visibility
        for (int index = 0; index < 2; ++index)
            textElements[index].enabled = result;
    }
    public void UpdateRoundCount(byte _index, byte _numberOfRounds) { textElements[_index].text = _numberOfRounds.ToString(); }
    #endregion
}
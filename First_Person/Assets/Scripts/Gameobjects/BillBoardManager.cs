using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class BillBoardManager : MonoBehaviour
{
    #region Variables
    // Inspector-assignable variables
    [SerializeField] private GameObject[] bombNumberetxtMeshes;
    [SerializeField] private GameObject[] livesNumberTextMeshes;
    [SerializeField] private GameObject[] scoreNumberTextMeshes;
    #endregion

    #region Public Interface                                                                                                                                  
    public void UpdateBillBoardText<T>(byte _index, T _value)
    {
        // Update whichever textmesh group that is selected
        switch (_index)
        {
            case 0:
                foreach (GameObject textMesh in bombNumberetxtMeshes)
                    textMesh.GetComponent<TextMesh>().text = _value.ToString();
                break;
            case 1:
                foreach (GameObject textMesh in livesNumberTextMeshes)
                    textMesh.GetComponent<TextMesh>().text = _value.ToString();
                break;
            case 2:
                foreach (GameObject textMesh in scoreNumberTextMeshes)
                    textMesh.GetComponent<TextMesh>().text = _value.ToString();
                break;
        }
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfficerManager : MonoBehaviour
{
    public List<GameObject> officerList = new List<GameObject>();
    public bool noGuys = false;

    public void setPos(int pos)
    {
        if (noGuys)
        {
            SceneManager.LoadScene(3);
            return;
        }

        if (officerList.Count > 0)
        {
            if (pos < 0)
            {
                officerList[0].GetComponent<OfficerAI>().moveToLeftPos = true;
            }
            else
            {
                officerList[0].GetComponent<OfficerAI>().moveToRightPos = true;
            }

            if (officerList.Count == 1) //if it is the last officer that is called, then the distances and trueAttack must be reset in the case he is called to move while firing
                officerList[0].GetComponent<OfficerAI>().resetDis();

            officerList.RemoveAt(0);
        }
        else
        {
            noGuys = true;
        }
    }

    public void setWarning()
    {
        foreach (GameObject off in officerList)
        {
            var tempColor = Color.white;
            off.GetComponent<OfficerAI>().warning.color = tempColor;
            off.GetComponent<OfficerAI>().warned = true;
        }
    }
}

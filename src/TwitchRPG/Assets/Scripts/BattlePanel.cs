using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : MonoBehaviour, IResponseProcessor
{
    public GameObject DirectionLabelPrefab;

	void Start ()
	{
	    SetVisible(false);
	}

    void Update()
    {
        
    }

    public void SetVisible(bool state)
    {
        GetComponent<Image>().enabled = state;
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(state);
        }
    }

    public string TypeName { get { return "AddVote"; } }
    public bool ProcessResponse(JsonRequest response)
    {
        if (response.data["direction"].IsString)
        {
            try
            {
                
            }
            catch (Exception )
            {
            }
        }

        return true;
    }
}

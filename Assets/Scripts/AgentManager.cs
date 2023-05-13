using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    private GameObject agentObject;
    private int attckDmg;



    private void Awake()
    {
        agentObject = Resources.Load<GameObject>("Enemy");
    }


    public bool stunned;
    public bool dead;
    


}

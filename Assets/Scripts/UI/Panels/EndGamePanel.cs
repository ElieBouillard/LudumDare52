using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    public void LeaveGame()
    {
        NetworkManager.Instance.Leave();
    }
}

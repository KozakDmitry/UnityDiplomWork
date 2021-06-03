using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PanelManager : MonoBehaviour
{
    private void Start()
    {
        
    }

    public void setTexts(List<PlayerController> players)
    {
        PlayerController[] topPlayers = players
            .Where(p => !p.isDead)
            .OrderBy(p => p.score)
            .Take(5)
            .ToArray();

        for(int i=0; i < topPlayers.Length; i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = (i+1) + ". " + topPlayers[i].photonView.Owner.NickName + "    " + topPlayers[i].score;
        }
    }
}

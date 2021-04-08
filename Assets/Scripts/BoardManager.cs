using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private int[][] board;
    [SerializeField]
    private Tilemap map;
    [SerializeField]
    private List<Tile> ListOfTiles;



    private int size = 10;
    // Start is called before the first frame update
    void Start()
    {
        BoardGenerator(size);
    }
    public void setSize(int size)
    {
        this.size = size;
    }
    // Update is called once per frame
    void Update()
    {
        
    }





    private void BoardGenerator(int size)
    {
      
    }
}

﻿using UnityEngine;
using System.Collections;
using System.IO;

public class loadMap : MonoBehaviour {

    public StreamReader sr;
    mapSquare[,] map;
    int row;
    int column;

    //objects to create in unity
    public GameObject unblockedSquare;
    public GameObject partiallyBlockedSquare;
    public GameObject blockedSquare;
    public GameObject horizontalHighway;
    public GameObject verticalHighway;
    public GameObject upperHighway;
    public GameObject leftHighway;

    int lOnce = 0;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        //load an old map
        if (Input.GetKey(KeyCode.L)&&lOnce==0)
        {
            lOnce = 1;
            loadAnotherMap();
            displaySquares();
        }
    }

    void loadAnotherMap()
    {
        sr = new StreamReader("output.txt");
        if (sr != null)
        {
            string startingSquare = sr.ReadLine();
            string endingSquare = sr.ReadLine();
            string partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            partiallyBlockedSquared = sr.ReadLine();
            string rowcolumn = sr.ReadLine();
            string[] rowscolumns = rowcolumn.Split(',');
             row = int.Parse(rowscolumns[0]);
            column = int.Parse(rowscolumns[1]);
            Debug.Log("row:" + row);
            Debug.Log("column:" + column);
            map = new mapSquare[row, column];
            string[] line;
            string type;
            int r = 0;
            int c = 0;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine().Split(',');
                c = 0;
                for(int i=0; i<line.Length; i++)
                {
                    type = line[i];
                    //put type in right position
                    if (type == "0")
                    {
                        map[r, c] = new mapSquare();
                        map[r, c].type = 0;
                    }
                    else if (type == "1")
                    {
                        map[r, c] = new mapSquare();
                        map[r, c].type = 1;
                    }
                    else if (type == "2")
                    {
                        map[r, c] = new mapSquare();
                        map[r, c].type = 2;
                    }
                    else if (type == "a")
                    {
                        map[r, c] = new mapSquare();
                        map[r, c].type = 1;
                        map[r, c].typeHighway = 1;
                    }
                    else if (type == "b")
                    {
                        map[r, c] = new mapSquare();
                        map[r, c].type = 2;
                        map[r, c].typeHighway = 1;
                    }
                    c++;
                }
                r++;
            }

            sr.Close();
        }
    }

    //display all squares in unity
    void displaySquares()
    {
        //go through map and place the right squares in the right places
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                if (map[r, c].type == 0)
                {
                    blockedSquare.SetActive(true);
                    Object temp = Instantiate(blockedSquare, new Vector3(r, 0, c), Quaternion.identity);
                    blockedSquare.SetActive(false);
                }
                else if (map[r, c].type == 1)
                {
                    unblockedSquare.SetActive(true);
                    Object temp = Instantiate(unblockedSquare, new Vector3(r, 0, c), Quaternion.identity);
                    unblockedSquare.SetActive(false);
                }
                else if (map[r, c].type == 2)
                {
                    partiallyBlockedSquare.SetActive(true);
                    Object temp = Instantiate(partiallyBlockedSquare, new Vector3(r, 0, c), Quaternion.identity);
                    partiallyBlockedSquare.SetActive(false);
                }
                //place highways
                if (map[r, c].typeHighway == 1)
                {
                    horizontalHighway.SetActive(true);
                    Object temp = Instantiate(horizontalHighway, new Vector3(r, 0.5f, c), Quaternion.identity);
                    horizontalHighway.SetActive(false);
                }
                else if (map[r, c].typeHighway == 2)
                {
                    verticalHighway.SetActive(true);
                    Object temp = Instantiate(verticalHighway, new Vector3(r, 0.5f, c), Quaternion.AngleAxis(90, Vector3.up));

                    verticalHighway.SetActive(false);
                }
                //3:_| upper left highway
                else if (map[r, c].typeHighway == 3)
                {
                    upperHighway.SetActive(true);
                    Object temp = Instantiate(upperHighway, new Vector3(r, 0.5f, c - .25f), Quaternion.identity);
                    upperHighway.SetActive(false);
                    leftHighway.SetActive(true);
                    Object temp2 = Instantiate(leftHighway, new Vector3(r + .25f, 0.5f, c), Quaternion.identity);
                    leftHighway.SetActive(false);
                }
                //4:|_ upper right highway
                else if (map[r, c].typeHighway == 4)
                {
                    upperHighway.SetActive(true);
                    Object temp = Instantiate(upperHighway, new Vector3(r, 0.5f, c - .25f), Quaternion.identity);
                    upperHighway.SetActive(false);
                    leftHighway.SetActive(true);
                    Object temp2 = Instantiate(leftHighway, new Vector3(r - .25f, 0.5f, c), Quaternion.identity);
                    leftHighway.SetActive(false);
                }
                //5:   lower left highway
                else if (map[r, c].typeHighway == 5)
                {
                    upperHighway.SetActive(true);
                    Object temp = Instantiate(upperHighway, new Vector3(r, 0.5f, c + .25f), Quaternion.identity);
                    upperHighway.SetActive(false);
                    leftHighway.SetActive(true);
                    Object temp2 = Instantiate(leftHighway, new Vector3(r + .25f, 0.5f, c), Quaternion.identity);
                    leftHighway.SetActive(false);
                }
                //6:   lower right highway
                else if (map[r, c].typeHighway == 6)
                {
                    upperHighway.SetActive(true);
                    Object temp = Instantiate(upperHighway, new Vector3(r, 0.5f, c + .25f), Quaternion.identity);
                    upperHighway.SetActive(false);
                    leftHighway.SetActive(true);
                    Object temp2 = Instantiate(leftHighway, new Vector3(r - .25f, 0.5f, c), Quaternion.identity);
                    leftHighway.SetActive(false);
                }
            }
        }
    }

}

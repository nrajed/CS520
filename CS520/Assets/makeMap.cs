﻿using UnityEngine;
using System.Collections;
using System.IO;

public class makeMap : MonoBehaviour {

    public int numRows=20;//120
    public int numCols=20;//160
    public int pathMax;

    //matrix storage of all coordinates
    public mapSquare[,] map;

    //objects to create in unity
    public GameObject unblockedSquare;
    public GameObject partiallyBlockedSquare;
    public GameObject blockedSquare;
    public GameObject horizontalHighway;
    public GameObject verticalHighway;
    public GameObject upperHighway;
    public GameObject leftHighway;

    //starting and goal locations
    public Vector2 startLocation;
    public Vector2 goalLocation;
    public GameObject startObject;
    public GameObject goalObject;

    //maximum size of paths(100)
    public int maxPath;

    //list of all centers of partially blocked squares
    public Vector2[] centerPartiallyBlocked = new Vector2[8];

    public ArrayList paths = new ArrayList();
    public ArrayList currentPath = new ArrayList();
    public StreamWriter tw;

    //see if n was already pressed
    int nOnce = 0;
    int numberAStar = 0;
    int maxPathsDone;

    // Use this for initialization
    void Start () {
        int smallMap = 3;
        if (smallMap == 1)
        {
            maxPath = 2;//100;
            numRows = 5;//120;
            numCols = 5;//160;
            pathMax = 2;//20
            maxPathsDone = 1;//4
        }else if (smallMap == 2)
        {
            maxPath = 25;//100;
            numRows = 30;//120;
            numCols = 40;//160;
            pathMax = 5;//20
            maxPathsDone = 1;//4
        }
        else {
            maxPath = 100;//100;
            numRows = 120;//120;
            numCols = 160;//160;
            pathMax = 20;//20
            maxPathsDone = 4;//4
        }

        
        numberAStar = 0;
    }

    void Update()
    {
        //make new map
        if (Input.GetKey(KeyCode.N) && nOnce == 0)
        {
            makeMapGreat();
            //display squares
            displaySquares();


            //convert to text
            convertToText();
        }
        //reload start and end location
        if (Input.GetKey(KeyCode.R))
        {
            generateStartGoal();
            updateStarterGoalSquares(startLocation, goalLocation,0);
        }
        if (Input.GetKey(KeyCode.K))
        {
            //Debug.Log("get K: all the paths");

            float[] results = new float[76];
            float[] allResults = new float[76];

            for (int f = 0; f < 76; f++)
            {
                allResults[f] = 0;
            }

            AStarAlgorithm astar = this.gameObject.GetComponent<AStarAlgorithm>();
            Phase2AStar_Sequential seqastar = this.gameObject.GetComponent<Phase2AStar_Sequential>();
            Phase2AStar_Integrated intastar = this.gameObject.GetComponent<Phase2AStar_Integrated>();

            int numIterations=10;

            for (int i = 0; i < numIterations; i++)
            {
                Debug.Log("running it for i: " + i + "th time");
                results = new float[76];
                //if (numberAStar == 0 || numberAStar == 3)
                //{
                //generate new map
                map = new mapSquare[numRows, numCols];
                generateUnblocked();
                generateStartGoal();
                updateStarterGoalSquares(startLocation, goalLocation,0);
                generatePartiallyBlocked();
                generateHighwayPoints();
                assignEachPointOnHighway();
                generateBlocked();
                //display squares
                displaySquares();
                //convert to text
                convertToText();
                //numberAStar = 0;
                Debug.Log("We generated new map with start: " + startLocation + " and goal: " + goalLocation);
                //}

                Debug.Log("Weighted A*-------------------------------------------");
                //start with weighted astar
                astar.enabled = true;
                seqastar.enabled = false;
                intastar.enabled = false;

                results=astar.calculateAllPathsAStar(results);
                

                Debug.Log("Sequential A*-------------------------------------------");
                //sequential astar
                astar.enabled = false;
                seqastar.enabled = true;
                intastar.enabled = false;

                results=seqastar.calculateAllPathsAStar(results);
                

                Debug.Log("Integrated A*-------------------------------------------");
                //integrated astar
                astar.enabled = false;
                seqastar.enabled = false;
                intastar.enabled = true;

                results=intastar.calculateAllPathsAStar(results);
                for (int f = 0; f < 76; f++)
                {
                    allResults[f] = allResults[f] + results[f];
                }


                


            }


            for (int f = 0; f < 76; f++)
            {
                Debug.Log("result "+f +":"+ allResults[f]/ numIterations);
            }

        }    
            
    }


    public void makeMapGreat()
    {
        nOnce = 1;

        map = new mapSquare[numRows, numCols];
        Debug.Log("generateMap");

        //generate map
        generateUnblocked();
        //generateStartGoal();
        
        startLocation = new Vector2((int)Random.Range(0, numRows-1), (int)Random.Range(0, numCols-1));
        updateStarterGoalSquares(startLocation, goalLocation,0);
        goalObject.SetActive(false);
        generatePartiallyBlocked();
        generateHighwayPoints();
        assignEachPointOnHighway();
        generateBlocked();
    }

    public GameObject path;

    //display all squares in unity
    public void displaySquaresGround(Vector2[] locations, int numLocations, int height)
    {
               //go through map and place the right squares in the right places
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
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
               
                if (map[r, c].typeHighway >=1)
                {
                    verticalHighway.SetActive(true);
                    Object temp = Instantiate(verticalHighway, new Vector3(r, 0.5f, c), Quaternion.AngleAxis(90, Vector3.up));
                    verticalHighway.SetActive(false);
                }

            }
        }


        for(int i = 0; i <numLocations; i++)
        {
            path.SetActive(true);
            Object temp = Instantiate(path, new Vector3(locations[i].x, height, locations[i].y), Quaternion.identity);
            path.SetActive(false);

        }
    }




    //generate all unblocked squares: all in the beginning
    public void generateUnblocked()
    {
        //initialize the map to unblocked squares
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                map[r, c] = new mapSquare();
                //map[r, c].type = 1;
            }
        }
    }

    //generate the start and goal locations
    public void generateStartGoal()
    {
        //generate start and goal states
       int startIsBlocked = 1;
         int goalIsBlocked = 1;

        mapSquare[,] maptemp;
        mapSquare[,] map1 = transform.gameObject.GetComponent<loadMap>().map;
        mapSquare[,] map2 = transform.gameObject.GetComponent<makeMap>().map;
        if (map1 != null)
        {
            maptemp = map1;
        }
        else if (map2 != null)
        {
            maptemp = map2;
        }
        else
        {
            return;
        }

        while (startIsBlocked != 0 || goalIsBlocked != 0) {

             startIsBlocked = 1;
             goalIsBlocked = 1;

             //start
             int startx = (int)(39 * Random.value);
             if (startx >= 20)
             {
                 startx += 80;
             }
             int starty = (int)(39 * Random.value);
             if (starty >= 20)
             {
                 starty += 120;
             }
             Vector2 startVector = new Vector2(startx, starty);
             startLocation = startVector;

           

            if (maptemp[(int)startLocation.x, (int)startLocation.y].type != 0)
             {
                //Debug.Log("startlocation: type!=0 so unblocked");
                //Debug.Log("start.x:" + startLocation.x);
                //Debug.Log("start.y:" + startLocation.y);
                startIsBlocked = 0;
             }

             //goal
             int goalx = (int)(39 * Random.value);
             if (goalx >= 20)
             {
                 goalx += 80;
             }
             int goaly = (int)(39 * Random.value);
             if (goaly >= 20)
             {
                 goaly += 80;
             }
             Vector2 goalVector = new Vector2(goalx, goaly);
             while ((int)((goalVector - startVector).magnitude) < 100)
             {
                 startx = (int)(39 * Random.value);
                 if (startx >= 20)
                 {
                     startx += 80;
                 }

                 starty = (int)(39 * Random.value);
                 if (starty >= 20)
                 {
                     starty += 120;
                 }
                 startVector = new Vector2(startx, starty);

                 goalx = (int)(39 * Random.value);
                 if (goalx >= 20)
                 {
                     goalx += 80;
                 }

                 goaly = (int)(39 * Random.value);
                 if (goaly >= 20)
                 {
                     goaly += 80;
                 }

                 goalVector = new Vector2(goalx, goaly);
             }

             goalLocation = goalVector;
             if (maptemp[(int)goalLocation.x, (int)goalLocation.y].type != 0)
             {
                //Debug.Log("goalLocation: type!=0 so unblocked");
                //Debug.Log("goal.x:" + goalLocation.x);
                //Debug.Log("goal.y:" + goalLocation.y);
                goalIsBlocked = 0;
             }
         }
    }

    //generate all partially blocked squares
    public void generatePartiallyBlocked()
    {
        //place partially blocked squares by randomly deciding on 8 coordinates
        for (int i = 0; i < 8; i++)
        {
            float randx = (numRows - 1) * Random.value;
            float randz = (numCols - 1) * Random.value;
            centerPartiallyBlocked[i] = new Vector2((int)randx, (int)randz);
            for (int x = (int)randx - 15; x <= randx + 15; x++)
            {
                for (int z = (int)randz - 15; z <= randz + 15; z++)
                {
                    //this is out of bounds
                    if (x < 0 || x > (numRows - 1) || z < 0 || z > (numCols - 1))
                    {
                        continue;
                    }
                    //otherwise do probabilities
                    else
                    {
                        float partiallyBlocked = Random.value;
                        if (partiallyBlocked < .5)
                        {
                            map[x, z].type = 2;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }

    //generate the highway turning points so that can later can fill in what is between them
    public void generateHighwayPoints()
    {
        int stoppingTime = 0;

        //while we have not done all 4 paths
        int pathsDone = 0;
        int currentPathDone = 0;//1 done, 2 restart
        while (pathsDone < maxPathsDone)
        {
            stoppingTime++;
            if (stoppingTime == 10000)
            {
                //Debug.Log("hit 1000");
            }
            if (stoppingTime == 50000)
            {
                //Debug.Log("hit 10000");
            }
            if (stoppingTime == 100000)
            {
                //Debug.Log("hit 50000");
            }

            //1,000,000
            if (stoppingTime == 1000000)
            {
                pathsDone = 0;
                stoppingTime = 0;
                currentPathDone = 0;
                paths.Clear();
                continue;
            }


            currentPathDone = 0;
            currentPath.Clear();
            //do 1 path
            //while still can do path:
            //-----------------------------------------------------do path-----------------------------------------------------

            //-----------------------------------------------------do starting path component-----------------------------------------------------
            //first find starting point:
            Vector2 startHighway; //x,z
            int count = 0; //counts how many squares in the highway so far
            int perimiter = numCols * 2 + numRows * 2 - 4;
            int boundarySide = (int)((perimiter - 1) * Random.value);
            if (boundarySide < numCols)
            {
                //then our point is that number
                startHighway = new Vector2(0, boundarySide);
            }
            else if (boundarySide < numCols + numRows - 2)
            {
                startHighway = new Vector2(boundarySide - (numCols - 1), numCols - 1);
            }
            else if (boundarySide < numCols + numRows + numRows - 4)
            {
                startHighway = new Vector2(boundarySide - numCols - numRows + 3, 0);
            }
            else
            {
                startHighway = new Vector2(numRows - 1, boundarySide - numCols - numRows - numRows + 4);
            }
            currentPath.Add(startHighway);
            //so we have start position
            //map[(int)startHighway.x, (int)startHighway.y].typeA = 1;
            //now decide to which position this will go:
            int direction;
            while (true)
            {
                direction = (int)(Random.value * 3);
                //right
                if (direction == 0)
                {
                    //is this direction possible?
                    if (startHighway.y + 1 < numCols)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y + i - 1;
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                        }
                        if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y + pathMax - 1);
                        currentPath.Add(startHighway);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                //left
                if (direction == 1)
                {
                    //is this direction possible?
                    if (startHighway.y - 1 >= 0)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y - i + 1;
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                        }
                        if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y - pathMax + 1);
                        currentPath.Add(startHighway);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                //down
                if (direction == 2)
                {
                    //is this direction possible?
                    if (startHighway.x + 1 < numRows)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            int xCoord = (int)startHighway.x + i - 1;
                            int zCoord = (int)startHighway.y;
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }

                        }
                        if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x + pathMax - 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                //up
                if (direction == 3)
                {
                    //is this direction possible?
                    if (startHighway.x - 1 >= 0)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            int xCoord = (int)startHighway.x - i + 1;
                            int zCoord = (int)startHighway.y;
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                        }
                        if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x - pathMax + 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            //if current path needs to restart, restart
            if (currentPathDone == 2)
            {
                continue;
            }

            count = pathMax;
            //--------------------------------------------------finish starting path component-----------------------------------------------------

            //-----------------------------------------------------do other direction components---------------------------------------------------
            //do rest
            //while the path doesnt cross itself or hits border before reaching 100
            while (true)
            {
                float direction2 = Random.value;
                //move in same directions
                if (direction2 < .6f)
                {
                    direction2 = direction;
                    //right
                    if (direction == 0)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y + i - 1;
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord - 1));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                            //map[xCoord, zCoord].typeA = i;
                            //map[xCoord, zCoord].typeHighway = 1;
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y + pathMax - 1);
                        currentPath.Add(startHighway);
                    }
                    //left
                    if (direction == 1)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y - i + 1;
                            //if it is a highway, then restart
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord + 1));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y - pathMax + 1);
                        currentPath.Add(startHighway);
                    }
                    //down
                    if (direction == 2)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x + i - 1;
                            int zCoord = (int)startHighway.y;
                            //if it is a highway, then restart
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord - 1, zCoord));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x + pathMax - 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                    }
                    //up
                    if (direction == 3)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x - i + 1;
                            int zCoord = (int)startHighway.y;
                            //if it is a highway, then restart
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord + 1, zCoord));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x - pathMax + 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                    }
                }
                //move perpendicular in one direction: up or right
                else if (direction2 < .8f)
                {
                    //if direction is up or down, then we move right
                    if (direction == 2 || direction == 3)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y + i - 1;
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord - 1));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                            //map[xCoord, zCoord].typeA = i;
                            //map[xCoord, zCoord].typeHighway = 1;
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y + pathMax - 1);
                        currentPath.Add(startHighway);
                        direction = 0;
                    }
                    //if direction is left or right, then we move up
                    if (direction == 0 || direction == 1)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x - i + 1;
                            int zCoord = (int)startHighway.y;
                            //if it is a highway, then restart
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord + 1, zCoord));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x - pathMax + 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        direction = 3;
                    }
                    //otherwise start over
                    //break;

                }
                //move perpendicular in one direction: down or left
                else
                {
                    //if direction is up or down, then we move left
                    if (direction == 2 || direction == 3)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y - i + 1;
                            //if it is a highway, then restart
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord + 1));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y - pathMax + 1);
                        currentPath.Add(startHighway);
                        direction = 1;
                    }
                    //if direction is left or right, then we move down
                    if (direction == 0 || direction == 1)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= pathMax; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x + i - 1;
                            int zCoord = (int)startHighway.y;
                            //if it is a highway, then restart
                            if (checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= maxPath)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord - 1, zCoord));
                                    break;
                                }
                                else
                                {
                                    currentPathDone = 2;
                                    //restart path
                                    break;
                                }
                            }
                        }
                        if (currentPathDone == 1)
                        {
                            break;
                        }
                        else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x + pathMax - 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        direction = 2;
                    }
                    //otherwise start over
                    if (currentPathDone == 1)
                    {
                        break;
                    }
                    else if (currentPathDone == 2)
                    {
                        break;
                    }
                    //break;
                }


            }

            //if current path needs to restart or isn't done, restart
            if (currentPathDone == 2 || currentPathDone == 0)
            {
                continue;
            }

            //-----------------------------------------------------finish other direction components---------------------------------------------------

            pathsDone++;
            paths.Add(currentPath);
            currentPath = new ArrayList();

        }
    }

    //fills in all points in between the points generated by generateHighwayPoints
    public void assignEachPointOnHighway()
    {
        //Debug.Log("start printing---------------------------------------");
        int countPoints = 0;
        for (int i = 0; i < paths.Count; i++)
        {
           // Debug.Log("next path-------------------------");
            if (paths[i] != null)
            {
                ArrayList temp = (ArrayList)paths[i];
                //go through all points....
                Vector2 previousPoint = (Vector2)temp[0];
                Vector2 currentPoint = previousPoint;
                if (previousPoint == null)
                {
                    continue;
                }
                //Debug.Log("1st point" + currentPoint);
                countPoints = 1;
                //go through each point in temp
                for (int j = 1; j < temp.Count; j++)
                {
                    previousPoint = currentPoint;
                    currentPoint = (Vector2)temp[j];
                    //Debug.Log(currentPoint);
                    //connect previousPoint and currentPoint
                    int x1 = (int)previousPoint.x;
                    int z1 = (int)previousPoint.y;
                    int x2 = (int)currentPoint.x;
                    int z2 = (int)currentPoint.y;
                    //if the x's are the same and the z's change: horizontal
                    if (x1 == x2)
                    {
                        int x = x1;
                        if (z1 < z2)
                        {
                            for (int z = z1; z <= z2; z++)
                            {
                                map[x, z].count = countPoints;
                                map[x, z].typeHighway = 1;
                                countPoints++;
                            }
                        }
                        else
                        {
                            for (int z = z2; z <= z1; z++)
                            {
                                map[x, z].count = countPoints;
                                map[x, z].typeHighway = 1;
                                countPoints++;
                            }
                        }
                    }
                    //if the z's are the same and the x's change: vertical
                    else
                    {
                        int z = z1;
                        if (x1 < x2)
                        {
                            for (int x = x1; x <= x2; x++)
                            {
                                map[x, z].count = countPoints;
                                map[x, z].typeHighway = 2;
                                countPoints++;
                            }
                        }
                        else
                        {
                            for (int x = x2; x <= x1; x++)
                            {
                                map[x, z].count = countPoints;
                                map[x, z].typeHighway = 2;
                                countPoints++;
                            }
                        }
                    }
                }

            }
        }

    }

    //generate all blocked squares
    public void generateBlocked()
    {
        //place blocked squares by randomly selecting non highway cells
        int numBlocked = 0;
        while (numBlocked < numRows*numCols*.2)
        {
            int x = Random.Range(0, numRows);
            int z = Random.Range(0, numCols);
            if (map[x, z].typeHighway == 0&& !(x==startLocation.x&& z == startLocation.y) && !(x == goalLocation.x && z == goalLocation.y))
            {
                map[x, z].type = 0;
                numBlocked++;
            }
        }

    }

    //display all squares in unity
    void displaySquares()
    {
        //go through map and place the right squares in the right places
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
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

    //generate the text file
    void convertToText()
    {
        mapSquare[,] maptemp;
        mapSquare[,] map1 = transform.gameObject.GetComponent<loadMap>().map;
        mapSquare[,] map2 = map;
        if (map1 != null)
        {
            maptemp = map1;
        }
        else if (map2 != null)
        {
            maptemp = map2;
        }
        else
        {
            return;
        }

        tw = new StreamWriter("output.txt");
        tw.Write(""+ startLocation);
        tw.WriteLine();
        tw.Write("" + goalLocation);
        tw.WriteLine();

        for(int i=0; i<8; i++)
        {
            tw.Write(centerPartiallyBlocked[i]);
            tw.WriteLine();
        }


        tw.Write("" +numRows+","+numCols);
        tw.WriteLine();

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                if (j != 0)
                {
                    tw.Write(",");
                }
                //tw.Write(map[i, j]);
                if (maptemp[i, j].typeHighway == 0)
                {

                    if (maptemp[i, j].type == 2)
                    {
                        tw.Write("2");
                    }
                    else if (maptemp[i, j].type == 1)
                    {
                        tw.Write("1");
                    }
                    else if (maptemp[i, j].type == 0)
                    {
                        tw.Write("0");
                    }
                }
                else
                {
                    string unicodeString = "";
                    /*
                    int count = map[i, j].count;
                    //while count>=0
                    while (count >= 1)
                    {
                        switch (count % 10)
                        {
                            case 0:
                                unicodeString =  "\x2080"+ unicodeString;
                                break;
                            case 1:
                                unicodeString =  "\x2081" + unicodeString;
                                break;
                            case 2:
                                unicodeString =  "\x2082" + unicodeString;
                                break;
                            case 3:
                                unicodeString = "\x2083" + unicodeString;
                                break;
                            case 4:
                                unicodeString = "\x2084" + unicodeString;
                                break;
                            case 5:
                                unicodeString = "\x2085" + unicodeString;
                                break;
                            case 6:
                                unicodeString = "\x2086" + unicodeString;
                                break;
                            case 7:
                                unicodeString = "\x2087" + unicodeString;
                                break;
                            case 8:
                                unicodeString = "\x2088" + unicodeString;
                                break;
                            case 9:
                                unicodeString = "\x2089" + unicodeString;
                                break;
                        }
                        count = count / 10;
                    }*/

                    if (maptemp[i, j].type == 1)
                    {
                        tw.Write("a"+ unicodeString);
                    }
                    else if (maptemp[i, j].type == 2)
                    {
                        tw.Write("b"+ unicodeString);
                    }
                    else
                    {
                        tw.Write("ERROR!");
                    }
                }

            }
            tw.WriteLine();
        }
        tw.Close();
    }

    //helper methods
    public void updateStarterGoalSquares(Vector2 start, Vector2 goal, int height)
    {
        //map[(int)start.x, (int)start.y].start = 1;
        //map[(int)start.x, (int)start.y].goal = 1;
        startObject.transform.position = new Vector3(start.x, height, start.y);
        goalObject.transform.position = new Vector3(goal.x, height, goal.y);
        Debug.Log("End Location: " + goalObject.transform.position);
        goalObject.SetActive(true);
        //convert to text
        convertToText();
    }

    //method that checks whether there is a highway in the point specified
    bool checkIfHighway(int x, int z)
    {
        //go through paths, and see if this point is between any of the coords
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i] != null)
            {
                ArrayList temp = (ArrayList)paths[i];
                //go through all points....
                Vector2 previousPoint = (Vector2)temp[0];
                Vector2 currentPoint = previousPoint;
                if (previousPoint == null)
                {
                    continue;
                }
                //go through each point in temp
                for (int j = 1; j < temp.Count; j++)
                {
                    previousPoint = currentPoint;
                    currentPoint = (Vector2)temp[j];
                    //connect previousPoint and currentPoint
                    int x1 = (int)previousPoint.x;
                    int z1 = (int)previousPoint.y;
                    int x2 = (int)currentPoint.x;
                    int z2 = (int)currentPoint.y;
                    //if the x's are the same and the z's change: horizontal
                    if (x1 < x2)
                    {
                        if (z1 < z2)
                        {
                            if (x >= x1 && x <= x2 && z >= z1 && z <= z2)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (x >= x1 && x <= x2 && z >= z2 && z <= z1)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (z1 < z2)
                        {
                            if (x >= x2 && x <= x1 && z >= z1 && z <= z2)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (x >= x2 && x <= x1 && z >= z2 && z <= z1)
                            {
                                return true;
                            }
                        }
                    }

                }

            }
        }

        //go through current path too
        //go through all points....
        Vector2 previousPoint2 = (Vector2)currentPath[0];
        Vector2 currentPoint2 = previousPoint2;
        if (previousPoint2 == null)
        {
            return false;
        }
        //go through each point in temp
        for (int j = 1; j < currentPath.Count-1; j++)
        {
            previousPoint2 = currentPoint2;
            currentPoint2 = (Vector2)currentPath[j];
            //connect previousPoint and currentPoint
            int x1 = (int)previousPoint2.x;
            int z1 = (int)previousPoint2.y;
            int x2 = (int)currentPoint2.x;
            int z2 = (int)currentPoint2.y;
            //if the x's are the same and the z's change: horizontal
            if (x1 < x2)
            {
                if (z1 < z2)
                {
                    if (x >= x1 && x <= x2&& z >= z1 && z <= z2)
                    {
                        return true;
                    }
                }else
                {
                    if (x >= x1 && x <= x2&& z >= z2 && z <= z1)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (z1 < z2)
                {
                    if (x >= x2 && x <= x1 && z >= z1 && z <= z2)
                    {
                        return true;
                    }
                }else
                {
                    if (x >= x2 && x <= x1 && z >= z2 && z <= z1)
                    {
                        return true;
                    }
                }
            }
   

        }


        return false;
    }


    
    //FOR A*----------------------------------------------------------------------

    //returns -1 if unblocked square is the neighbor
   
}

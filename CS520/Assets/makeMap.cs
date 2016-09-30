using UnityEngine;
using System.Collections;

public class makeMap : MonoBehaviour {

    public int numRows=120;//120
    public int numCols=160;//160

    mapSquare[,] map;
    public GameObject unblockedSquare;
    public GameObject partiallyBlockedSquare;
    public GameObject blockedSquare;
    public GameObject horizontalHighway;
    public GameObject verticalHighway;
    public GameObject upperHighway;
    public GameObject leftHighway;

    // Use this for initialization
    void Start () {
        map = new mapSquare[numRows, numCols];
    
        //initialize the map to unblocked squares
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                map[r, c] = new mapSquare();
                //map[r, c].type = 1;
            }
        }

        //place partially blocked squares by randomly deciding on 8 coordinates
        for(int i=0; i < 8; i++)
        {
            float randx = (numRows-1) *Random.value;
            float randz = (numCols-1) * Random.value;
            for(int x=(int)randx-15; x<=randx+15; x++)
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
                        }else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        //place highways
        ArrayList paths = new ArrayList();
        ArrayList currentPath = new ArrayList();

        //while we have not done all 4 paths
        int pathsDone = 0;
        int currentPathDone = 0;//1 done, 2 restart
        while (pathsDone < 4)
        {
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
            Debug.Log("adding starting point: " + startHighway);
            //so we have start position
            //map[(int)startHighway.x, (int)startHighway.y].typeA = 1;
            //now decide to which position this will go:
            int direction;
            while (true)
            {
                direction = (int)(Random.value * 3);
                Debug.Log("direction:" + direction);
                //right
                if (direction == 0)
                {
                    //is this direction possible?
                    if (startHighway.y + 1 < numCols)
                    {
                        //for 20 blocks move out
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y + 20 - 1);
                        int xCoord = (int)startHighway.x;
                        int zCoord = (int)startHighway.y + 20 - 1;
                        //if leave boundary:
                        if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                        {
                            currentPathDone = 2;
                            //restart path
                            break;
                        }
                        currentPath.Add(startHighway);
                        Debug.Log("adding second point: " + startHighway);
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
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y - 20 + 1);
                        int xCoord = (int)startHighway.x;
                        int zCoord = (int)startHighway.y - 20 + 1;
                        //if leave boundary:
                        if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                        {
                            currentPathDone = 2;
                            //restart path
                            break;
                        }
                        currentPath.Add(startHighway);
                        Debug.Log("adding second point: "+startHighway);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                    //down
                }
                if (direction == 2)
                {
                    //is this direction possible?
                    if (startHighway.x + 1 < numRows)
                    {
                        //for 20 blocks move out
                        startHighway = new Vector2((int)startHighway.x + 20 - 1, (int)startHighway.y);
                        int xCoord = (int)startHighway.x+20-1;
                        int zCoord = (int)startHighway.y;
                        //if leave boundary:
                        if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                        {
                            currentPathDone = 2;
                            //restart path
                            break;
                        }
                        currentPath.Add(startHighway);
                        Debug.Log("adding second point: " + startHighway);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                    //up
                }
                if (direction == 3)
                {
                    //is this direction possible?
                    if (startHighway.x - 1 >= 0)
                    {
                        //for 20 blocks move out
                        startHighway = new Vector2((int)startHighway.x - 20 + 1, (int)startHighway.y);
                        int xCoord = (int)startHighway.x-20+1;
                        int zCoord = (int)startHighway.y;
                        //if leave boundary:
                        if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                        {
                            currentPathDone = 2;
                            //restart path
                            break;
                        }
                        currentPath.Add(startHighway);
                        Debug.Log("adding second point: " + startHighway);
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

            count = 20;
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
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord-1));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord, zCoord-1));
                                    break;
                                }else
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
                        }else if (currentPathDone == 2)
                        {
                            break;
                        }
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y + 20 - 1);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //left
                    if (direction == 1)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
                        {
                            count++;
                            int xCoord = (int)startHighway.x;
                            int zCoord = (int)startHighway.y - i + 1;
                            //if it is a highway, then restart
                            if(checkIfHighway(xCoord, zCoord))
                            {
                                currentPathDone = 2;
                                //restart path
                                break;
                            }
                            //if leave boundary:
                            if (xCoord < 0 || zCoord < 0 || xCoord >= numRows || zCoord >= numCols)
                            {
                                //if count>100, we are good and path is done
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord+1));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord, zCoord+1));
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
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y - 20 + 1);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //down
                    if (direction == 2)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord-1, zCoord));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord-1, zCoord));
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
                        startHighway = new Vector2((int)startHighway.x + 20 - 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //up
                    if (direction == 3)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord+1, zCoord));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord+1, zCoord));
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
                        startHighway = new Vector2((int)startHighway.x - 20 + 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                }
                //move perpendicular in one direction: up or right
                else if (direction2 < .8f)
                {
                    //if direction is up or down, then we move right
                    if (direction ==2||direction==3)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord-1));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord, zCoord-1));
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
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y + 20 - 1);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //if direction is left or right, then we move up
                    if (direction ==0||direction==1)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord+1, zCoord));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord+1, zCoord));
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
                        startHighway = new Vector2((int)startHighway.x - 20 + 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //otherwise start over
                    break;
                       
                }
                //move perpendicular in one direction: down or left
                else
                {
                    //if direction is up or down, then we move left
                    if (direction == 2 || direction == 3)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord, zCoord+1));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord, zCoord+1));
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
                        startHighway = new Vector2((int)startHighway.x, (int)startHighway.y - 20 + 1);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //if direction is left or right, then we move down
                    if (direction == 0 || direction == 1)
                    {
                        //for 20 blocks move out
                        for (int i = 1; i <= 20; i++)
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
                                if (count >= 100)
                                {
                                    currentPathDone = 1;
                                    //path done!!
                                    currentPath.Add(new Vector2(xCoord-1, zCoord));
                                    Debug.Log("adding boundary point: " + new Vector2(xCoord-1, zCoord));
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
                        startHighway = new Vector2((int)startHighway.x + 20 - 1, (int)startHighway.y);
                        currentPath.Add(startHighway);
                        Debug.Log("adding more points: " + startHighway);
                    }
                    //otherwise start over
                    break;
                }
            }

            //if current path needs to restart or isn't done, restart
            if (currentPathDone == 2||currentPathDone==0)
            {
                continue;
            }
            
            //-----------------------------------------------------finish other direction components---------------------------------------------------

            pathsDone++;
            paths.Add(currentPath);
            currentPath = new ArrayList();

        }


        //TEMPORARY print arraylist--------------------------------------------

        int countPoints = 0;
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
                    Debug.Log("next path-------------------------");
                    continue;
                }
                Debug.Log("1st point"+currentPoint);
                countPoints = 1;
                //go through each point in temp
                for (int j = 1; j < temp.Count; j++)
                {
                    previousPoint = currentPoint;
                    currentPoint = (Vector2)temp[j];
                    Debug.Log(currentPoint);
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
                            for(int z=z1; z < z2; z++)
                            {
                                map[x, z].typeA = countPoints;
                                map[x, z].typeHighway = 1;
                                countPoints++;
                            }
                        }
                        else
                        {
                            for (int z = z2; z > z1; z--)
                            {
                                map[x, z].typeA = countPoints;
                                map[x, z].typeHighway = 1;
                                countPoints++;
                            }
                        }
                    }
                    //if the z's are the same and the x's change: vertical
                    else
                    {
                        int z = z1;
                        if (x1 <x2)
                        {
                            for (int x = x1; x < x2; x++)
                            {
                                map[x, z].typeA = countPoints;
                                map[x, z].typeHighway =2;
                                countPoints++;
                            }
                        }
                        else
                        {
                            for (int x = x2; x > x1; x--)
                            {
                                map[x, z].typeA = countPoints;
                                map[x, z].typeHighway = 2;
                                countPoints++;
                            }
                        }
                    }

                }

            }
            Debug.Log("next path-------------------------");
        }

        //Debug.Log(paths);
        //--------------------------------------------------------


        //go through paths and fill mapSquares with types
        

        


        //go through map and place the right squares in the right places
        for (int r =0; r< numRows; r++)
        {
            for(int c=0; c< numCols; c++)
            {
                if (map[r, c].type == 0)
                {
                    blockedSquare.SetActive(true);
                    Object temp = Instantiate(blockedSquare, new Vector3(r, 0, c), Quaternion.identity);
                    blockedSquare.SetActive(false);
                }else if (map[r, c].type == 1)
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
                if(map[r, c].typeHighway == 1)
                {
                    horizontalHighway.SetActive(true);
                    Object temp = Instantiate(horizontalHighway, new Vector3(r, 0.5f, c), Quaternion.identity);
                    horizontalHighway.SetActive(false);
                }
                else if (map[r, c].typeHighway == 2)
                {
                    verticalHighway.SetActive(true);
                    Object temp = Instantiate(verticalHighway, new Vector3(r, 0.5f, c), Quaternion.identity);
                    verticalHighway.SetActive(false);
                }
                //3:_| upper left highway
                else if (map[r, c].typeHighway == 3)
                {
                    upperHighway.SetActive(true);
                    Object temp = Instantiate(upperHighway, new Vector3(r, 0.5f, c-.25f), Quaternion.identity);
                    upperHighway.SetActive(false);
                    leftHighway.SetActive(true);
                    Object temp2 = Instantiate(leftHighway, new Vector3(r+.25f, 0.5f, c), Quaternion.identity);
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
	
    //method that checks whether there is a highway in the point specified
    bool checkIfHighway(int x, int z)
    {
        return false;
    }

    // Update is called once per frame
    void Update () {
	
	}
}

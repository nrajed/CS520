using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Threading;

public class Phase2AStar_Integrated : MonoBehaviour {
    mapSquare[,] map;
    Vector2 startLocation;
    Vector2 goalLocation;

    public GameObject pathObject;
    Process proc;
    long mem;
    public float w;
    public int h; //heuristic number
    Stopwatch stopwatch = new Stopwatch();
    int maxMem;
    int numNodes;
    float pathCost;
    //path that A* follows
    //array list of vectors where path passes through
    public ArrayList path = new ArrayList();
    //temporary pathobject in path
    ArrayList tempPathObjs = new ArrayList();

    public PriorityQueue fringe = new PriorityQueue();
    public PriorityQueue closed = new PriorityQueue();
    public float w2; //secondary heuristic value

    //FOR SEQUENTIAL A*
    public int n;
    public PriorityQueue[] OPEN;
    public PriorityQueue[] CLOSED;
    public PriorityQueue CLOSED_anchor;
    public PriorityQueue CLOSED_inad;
    public ArrayList mapList = new ArrayList();

    public Vector2[] centers = new Vector2[8];

    // Use this for initialization
    void Start()
    {
        maxMem = 0;
        numNodes = 0;
        proc = Process.GetCurrentProcess();
        mem = proc.PrivateMemorySize64;

        //set weight of A*
        //f=g+wh
        //w = 2.5f; //weight
        //h = 1; //heuristic
        //w2 = 1.25f;

        n = 4; //num heuristics minus 1
        OPEN = new PriorityQueue[n + 1];
        CLOSED = new PriorityQueue[n + 1];

        //temporary pathobject in path
        tempPathObjs = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            stopwatch.Start();
            path = new ArrayList();


            fringe = new PriorityQueue();
            closed = new PriorityQueue();
            //compute A*
            mapSquare[,] map1 = transform.gameObject.GetComponent<loadMap>().map;
            mapSquare[,] map2 = transform.gameObject.GetComponent<makeMap>().map;
            if (map1 != null)
            {
                map = map1;
                startLocation = transform.gameObject.GetComponent<loadMap>().startLocation;
                goalLocation = transform.gameObject.GetComponent<loadMap>().goalLocation;
                centers = transform.gameObject.GetComponent<loadMap>().centers;
            }
            else
            {
                map = map2;
                startLocation = transform.gameObject.GetComponent<makeMap>().startLocation;
                goalLocation = transform.gameObject.GetComponent<makeMap>().goalLocation;
                centers = transform.gameObject.GetComponent<makeMap>().centerPartiallyBlocked;
            }
            
            if (integratedAStar())
            {
                Vector2 curr = goalLocation;
                while (curr != startLocation)
                {
                    path.Add(curr);
                    curr = (Vector2)map[(int)curr.x, (int)curr.y].parent;
                }
                path.Add(curr);
                stopwatch.Stop();
                //UnityEngine.Debug.Log("stopwatch:" + stopwatch.ElapsedMilliseconds);
                //UnityEngine.Debug.Log("pathSize:" + path.Count);
                //UnityEngine.Debug.Log("numNodes:" + numNodes);
                //mem = proc.PrivateMemorySize64;

                //UnityEngine.Debug.Log("mem:" + mem);
                //UnityEngine.Debug.Log("visualizing");
                visualizeResetPaths();
            }
        }
    }

    float g(Vector2 curr)
    {
        if (curr.x == -1) { return 0; }
        return map[(int)curr.x, (int)curr.y].g;
        // map[(int)curr.x, (int)curr.y].g = map[(int)parent.x, (int)parent.y].g + findCost((int)curr.x, (int)curr.y,(int)parent.x, (int)parent.y);
        //return (Mathf.Sqrt(Mathf.Pow((curr - startLocation).x,2)+ Mathf.Pow((curr - startLocation).y, 2));
    }

    float g(Vector2 curr, int i)
    {
        if (curr.x == -1) { return 0; }

        return (float)map[(int)curr.x, (int)curr.y].SequentialG[i];
        // map[(int)curr.x, (int)curr.y].g = map[(int)parent.x, (int)parent.y].g + findCost((int)curr.x, (int)curr.y,(int)parent.x, (int)parent.y);
        //return (Mathf.Sqrt(Mathf.Pow((curr - startLocation).x,2)+ Mathf.Pow((curr - startLocation).y, 2));
    }

    //Prioritize longer axis
    public float h1(Vector2 s, Vector2 g)
    {
        float a = Mathf.Sqrt(2) * Mathf.Min(Mathf.Abs(s.x - g.x), Mathf.Abs(s.y - g.y));
        float b = Mathf.Max(Mathf.Abs(s.x - g.x), Mathf.Abs(s.y - g.y));
        float c = Mathf.Min(Mathf.Abs(s.x - g.x), Mathf.Abs(s.y - g.y));
        return a + b - c;
    }
    //Manhattan distance
    float h2(Vector2 s, Vector2 g)
    {
        Vector2 diff = new Vector2(Mathf.Abs(g.x - s.x), Mathf.Abs(g.y - s.y));
        return (int)(diff.x + diff.y);
    }
    //Euclidean distance
    float h3(Vector2 s, Vector2 g)
    {
        return .25f * Vector2.Distance(s, g);
    }
    //Euclidean distance+distances to partial block centers
    float h4(Vector2 s, Vector2 g)
    {
        float sum = 0;
        for (int i = 0; i < centers.Length; i++)
        {
            sum += Vector2.Distance(s, centers[i]);
        }
        return .25f * Vector2.Distance(s, g) + 1 / sum;

    }
    //Euclidean distance+number of unblocked neighbors
    float h5(Vector2 s, Vector2 g)
    {
        float count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2 curr = new Vector2(s.x + i, s.y + j);

                //if out of bounds
                if (curr.x < 0 || curr.y < 0 || curr.x >= map.GetLength(0) || curr.y >= map.GetLength(1))
                {
                    continue;
                }
                else
                {
                    if (map[(int)curr.x, (int)curr.y].type == 1)
                    {
                        count++;
                    }
                }
            }
        }
        return .25f * Vector2.Distance(s, g) + 1 / count;
    }


    //For Part 3

    public float[] calculateAllPathsAStar(float[] results)
    {

        numNodes = 0;
        proc = Process.GetCurrentProcess();
        mem = proc.PrivateMemorySize64;

        //set weight of A*
        //f=g+wh
        //w = 2.5f; //weight
        //h = 1; //heuristic
        //w2 = 1.25f;

        n = 4; //num heuristics minus 1
        OPEN = new PriorityQueue[n + 1];
        CLOSED = new PriorityQueue[n + 1];

        //temporary pathobject in path
        tempPathObjs = new ArrayList();

        //UnityEngine.Debug.Log("computing astar path---------------------------");
        long stopWatchSumw1w1 = 0;
        int pathCountSumw1w1 = 0;
        int numNodesSumw1w1 = 0;
        float pathCostSumw1w1 = 0;

        long stopWatchSumw1w2 = 0;
        int pathCountSumw1w2 = 0;
        int numNodesSumw1w2 = 0;
        float pathCostSumw1w2 = 0;

        long stopWatchSumw2w1 = 0;
        int pathCountSumw2w1 = 0;
        int numNodesSumw2w1 = 0;
        float pathCostSumw2w1 = 0;

        long stopWatchSumw2w2 = 0;
        int pathCountSumw2w2 = 0;
        int numNodesSumw2w2 = 0;
        float pathCostSumw2w2 = 0;




        w = 1.5f;
        w2 = 1.25f;
        calculateAStar();
        stopWatchSumw1w1 += stopwatch.ElapsedMilliseconds;
        pathCountSumw1w1 += path.Count;
        numNodesSumw1w1 += numNodes;
        pathCostSumw1w1 += pathCost;
        // UnityEngine.Debug.Log("Integrated. w1=1.5, w2=1.25. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=1.25. memory: " + maxMem);
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=1.25. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=1.25. path cost: " + pathCost);
        results[60] = stopwatch.ElapsedMilliseconds;
        results[61] = maxMem;
        results[62] = numNodes;
        results[63] = pathCost;

        w = 1.5f;
        w2 = 2f;
        calculateAStar();
        stopWatchSumw1w2 += stopwatch.ElapsedMilliseconds;
        pathCountSumw1w2 += path.Count;
        numNodesSumw1w2 += numNodes;
        pathCostSumw1w2 += pathCost;
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=2. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=2. memory: " + maxMem);
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=2. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("Integrated. w1=1.5, w2=2. path cost: " + pathCost);
        results[64] = stopwatch.ElapsedMilliseconds;
        results[65] = maxMem;
        results[66] = numNodes;
        results[67] = pathCost;


        w = 2.5f;
        w2 = 1.25f;
        calculateAStar();
        stopWatchSumw2w1 += stopwatch.ElapsedMilliseconds;
        pathCountSumw2w1 += path.Count;
        numNodesSumw2w1 += numNodes;
        pathCostSumw2w1 += pathCost;
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=1.25. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=1.25. memory: " + maxMem);
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=1.25. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=1.25. path cost: " + pathCost);
        results[68] = stopwatch.ElapsedMilliseconds;
        results[69] = maxMem;
        results[70] = numNodes;
        results[71] = pathCost;

        w = 2.5f;
        w2 = 2f;
        calculateAStar();
        stopWatchSumw2w2 += stopwatch.ElapsedMilliseconds;
        pathCountSumw2w2 += path.Count;
        numNodesSumw2w2 += numNodes;
        pathCostSumw2w2 += pathCost;
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=2. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=2. memory: " + maxMem);
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=2. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("Integrated. w1=2.5, w2=2. path cost: " + pathCost);
        results[72] = stopwatch.ElapsedMilliseconds;
        results[73] = maxMem;
        results[74] = numNodes;
        results[75] = pathCost;

        return results;
    }

    void calculateAStar()
    {
        maxMem = 0;
        pathCost = 0;
        numNodes = 0;
        stopwatch.Reset();
        stopwatch.Start();
        path = new ArrayList();


        OPEN = new PriorityQueue[n + 1];
        CLOSED = new PriorityQueue[n + 1];
        CLOSED_anchor = new PriorityQueue();
        CLOSED_inad = new PriorityQueue();


        //compute A*
        mapSquare[,] map1 = transform.gameObject.GetComponent<loadMap>().map;
        mapSquare[,] map2 = transform.gameObject.GetComponent<makeMap>().map;
        if (map1 != null)
        {
            map = map1;
            startLocation = transform.gameObject.GetComponent<loadMap>().startLocation;
            goalLocation = transform.gameObject.GetComponent<loadMap>().goalLocation;
            centers = transform.gameObject.GetComponent<loadMap>().centers;
        }
        else
        {
            map = map2;
            startLocation = transform.gameObject.GetComponent<makeMap>().startLocation;
            goalLocation = transform.gameObject.GetComponent<makeMap>().goalLocation;
            centers = transform.gameObject.GetComponent<makeMap>().centerPartiallyBlocked;
        }
        if (integratedAStar())
        {
            Vector2 curr = goalLocation;
            pathCost = g(goalLocation);//map[(int)curr.x, (int)curr.y].g;
            while (curr != startLocation)
            {
                path.Add(curr);
                curr = map[(int)curr.x, (int)curr.y].parent;
            }
            path.Add(curr);
            stopwatch.Stop();

            //mem = proc.PrivateMemorySize64;

            //UnityEngine.Debug.Log("mem:" + mem);

            //visualizeResetPaths();
        }
    }

    bool integratedAStar()
    {
        //g(s_start) = 0;
        map[(int)startLocation.x, (int)startLocation.y].g = 0f;
        //g(s_goal) = inf;
        map[(int)goalLocation.x, (int)goalLocation.y].g = float.MaxValue;

        //g(s_start) = NULL; //actualy (-2,-1)
        map[(int)startLocation.x, (int)startLocation.y].parent = new Vector2(-2,-1);
        //g(s_goal) = NULL; //actualy (-2,-1)
        map[(int)goalLocation.x, (int)goalLocation.y].parent = new Vector2(-2, -1);

        //for 0,1...n do:
        //Open[i] initialized in Start()
        for(int i=0; i<=n; i++)
        {
            OPEN[i] = new PriorityQueue();
            //Insert s_start in Open_i with priority = key(s_start,i)
            OPEN[i].Insert(startLocation, Key(startLocation, i));
            if (OPEN[i].getSize() > maxMem)
            {
                maxMem = OPEN[i].getSize();
            }
        }

        //initialized CLOSED_anchor
        //initialize CLOSED_inad
        CLOSED_anchor = new PriorityQueue();
        CLOSED_inad = new PriorityQueue();

        //while OPEN[0].Minkey()<inf do
        while (OPEN[0].getMin() < float.MaxValue)
        {
            //for i=1,2...n do
            for(int i = 1; i <= n; i++)
            {
                //if open[i].minkey()<=w2*Open[0].Minkey()
                if (OPEN[i].getMin()!=-1f && OPEN[i].getMin() <= w2 * OPEN[0].getMin())
                {
                    //if g(s_goal)<=OPEN[i].Minkey then
                    if((float)(g(goalLocation)) <= OPEN[i].getMin())
                    {
                        //if g(s_goal)<inf then
                        if((float)(g(goalLocation)) < float.MaxValue)
                        {
                            return true;//path found
                        }
                    }else
                    {
                        numNodes++;
                        Vector2 s = OPEN[i].Pop();
                        ExpandState(s);
                        CLOSED_inad.Insert(s, Key(s, i));
                        if (CLOSED_inad.getSize() > maxMem)
                        {
                            maxMem = CLOSED_inad.getSize();
                        }
                    }
                }else
                {
                    //if g(s_goal)<=OPEN[0].Minkey() then
                    if((float)(g(goalLocation)) <= OPEN[0].getMin())
                    {
                        //if g(s_goal)<inf then
                        if ((float) (g(goalLocation)) < float.MaxValue)
                        {
                            return true;
                        }
                    }else
                    {
                        numNodes++;
                        Vector2 s = OPEN[0].Pop();
                        ExpandState(s);
                        CLOSED_anchor.Insert(s, Key(s, 0));
                        if (CLOSED_anchor.getSize() > maxMem)
                        {
                            maxMem = CLOSED_anchor.getSize();
                        }
                    }
                }
            }


        }


            return false;
    }

    //Helper Methods
    float Key(Vector2 s, int i)
    {
        switch (i)
        {
            case 0:
                return g(s) + w * h3(s, goalLocation);
            case 1:
                return g(s) + w * h1(s, goalLocation);
            case 2:
                return g(s) + w * h2(s, goalLocation);
            case 3:
                return g(s) + w * h4(s, goalLocation);
            default:
                return g(s) + w * h5(s, goalLocation);
        }
    }

    void ExpandState(Vector2 s)
    {
        //remove s from OPEN for all i={0,1...n}
        for (int i = 0; i <= n; i++)
        {
            OPEN[i].Remove(s);
        }

        //foreach s' in Succ(s) do
        for (int j = -1; j <= 1; j++)
        {
            for (int k = -1; k <= 1; k++)
            {
                //curr=s'
                Vector2 curr = new Vector2(s.x + j, s.y + k);
                //has to be in bounds
                if ((j != 0 || k != 0) && (s.x + j >= 0 && s.y + k >= 0) && (s.x + j < map.GetLength(0) && s.y + k < map.GetLength(1)))
                {
                    //check that the curr is not blocked
                    if (map[(int)curr.x, (int)curr.y].type != 0)
                    {
                        //if s' was never generated then
                        if (!CLOSED_anchor.contains(curr) && !CLOSED_inad.contains(curr))
                        {
                            int openContains = 0;
                            for (int i = 0; i <= n; i++)
                            {
                                if (OPEN[i].contains(curr))
                                {
                                    openContains = 1;
                                }
                            }
                            //if open does not contain s' than s' was never generated 
                            if (openContains == 0)
                            {
                                //g(s')=inf
                                map[(int)curr.x, (int)curr.y].g = float.MaxValue;
                                //bp(s')=NULL
                                map[(int)curr.x, (int)curr.y].parent = new Vector2(-2, -1);
                            }
                        }
                        //if g(s')>g(s)+c(s,s') then
                        if (g(curr) > g(s) + findCost((int)s.x, (int)s.y, (int)curr.x, (int)curr.y))
                        {
                            //g(s')=g(s)+c(s,s')
                            map[(int)curr.x, (int)curr.y].g = g(s)+ findCost((int)s.x, (int)s.y, (int)curr.x, (int)curr.y);
                            //bp(s')=s
                            map[(int)curr.x, (int)curr.y].parent = s;
                            //if s' is not in CLOSED_anchor then
                            if (!CLOSED_anchor.contains(curr))
                            {
                                //Insert s' in OPEN[0] with Key(s',0)
                                OPEN[0].Insert(curr, Key(curr, 0));
                                if (OPEN[0].getSize() > maxMem)
                                {
                                    maxMem = OPEN[0].getSize();
                                }
                                if (!CLOSED_inad.contains(curr))
                                {
                                    for(int i=1; i<=n; i++)
                                    {
                                        if(Key(curr, i) <= w2 * Key(curr, 0))
                                        {
                                            OPEN[i].Insert(curr, Key(curr, i));
                                            if (OPEN[i].getSize() > maxMem)
                                            {
                                                maxMem = OPEN[i].getSize();
                                            }
                                        }
                                    }
                                }

                            }
                        }



                    }
                    //if s' was never generated then
                    //so s' not in open lists and not in CLOSED_inad and CLOSED_
                    // if (!(OPEN[i].contains(curr) || CLOSED[i].contains(curr)))
                    //   {

                    // }
                }
            }

            /*
            //For all neighbors in succ(s)

                    Vector2 curr = new Vector2(s.x + i, s.y + j);
                    if (!(OPEN[i].contains(curr) || CLOSED[i].contains(curr)))
                    {
                        map[curr.x, curr.y].g = float.MaxValue;
                        map[curr.x, curr.y].g = null;
                    }
                    if (g(curr) > g(s) + findCost(s.x, s.y, curr.x, curr.y))
                    {
                        map[curr.x, curr.y].g = g(s) + findCost(s.x, s.y, curr.x, curr.y);
                        map[curr.x, curr.y].parent = s;

                        //If not in open list
                        if (!CLOSED[i].contains(curr))
                        {
                            OPEN[i].Insert(curr, Key(s, i)); //add to open list
                        }
                    }
               */
        }
    }

    //-----------------------------------------------------------




    //finds cost given parent node(x,z) and neighbor node (neighborX, neighborZ)
    float findCost(int x, int z, int neighborX, int neighborZ)
    {
        float cost = 0;
        //if blocked return error: -1
        //Debug.Log("x:"+x);
        //Debug.Log("z:" + z);
        //Debug.Log("neighborX:" + neighborX);
        //Debug.Log("neighborZ:" + neighborZ);


        if (map[x, z].type == 0 || map[neighborX, neighborZ].type == 0)
        {
            return -1;
        }

        //if you are unblocked
        if (map[x, z].type == 1)
        {
            //options: unblocked, partially blocked, blocked, unblocked highway, partially blocked highway, diagonal for each
            //unblocked
            if (map[neighborX, neighborZ].type == 1)
            {
                //if diagonal:
                if (x != neighborX && z != neighborZ)
                {
                    cost = Mathf.Sqrt(2);
                }
                else
                {
                    cost = 1;
                    if (map[x, z].typeHighway != 0 && map[neighborX, neighborZ].typeHighway != 0)
                    {
                        cost = .25f;
                    }

                }
                return cost;
            }
            //partially blocked
            if (map[neighborX, neighborZ].type == 2)
            {
                //if diagonal:
                if (x != neighborX && z != neighborZ)
                {
                    cost = (Mathf.Sqrt(2) + Mathf.Sqrt(8)) / 2;
                }
                else
                {
                    cost = 1.5f;
                    if (map[x, z].typeHighway != 0 && map[neighborX, neighborZ].typeHighway != 0)
                    {
                        cost = .375f;
                    }
                }
                return cost;
            }
        }

        //if you are partially blocked
        if (map[x, z].type == 2)
        {
            //options: unblocked, partially blocked, blocked, unblocked highway, partially blocked highway, diagonal for each
            //unblocked
            if (map[neighborX, neighborZ].type == 1)
            {
                //if diagonal:
                if (x != neighborX && z != neighborZ)
                {
                    cost = (Mathf.Sqrt(2) + Mathf.Sqrt(8)) / 2;

                }
                else
                {
                    cost = 1.5f;
                    //check if highway
                    if (map[x, z].typeHighway != 0 && map[neighborX, neighborZ].typeHighway != 0)
                    {
                        cost = .375f;
                    }
                }
                return cost;
            }
            //partially blocked
            if (map[neighborX, neighborZ].type == 2)
            {
                //if diagonal:
                if (x != neighborX && z != neighborZ)
                {
                    cost = Mathf.Sqrt(8);
                }
                else
                {
                    cost = 2f;
                    if (map[x, z].typeHighway != 0 && map[neighborX, neighborZ].typeHighway != 0)
                    {
                        cost = .5f;
                    }
                }
                return cost;
            }
        }

        return -1;
    }

    //resets any old path and visualizes new path
    void visualizeResetPaths()
    {
        //input=array of path
        //input=path object
        //input=temporary path object arrayList
        //this is temporary for input in future
        //ArrayList path = new ArrayList();

        //stores objects that were used to build path:
        //ArrayList tempPathObjs = new ArrayList();

        //delete all objects in the tempPathObjs
        for (int i = 0; i < tempPathObjs.Count; i++)
        {
            Destroy((GameObject)tempPathObjs[i]);
        }
        tempPathObjs.Clear();


        //Loop through whole path

        Vector2 previous = (Vector2)path[0];
        Vector2 current = previous;
        int angle;

        for (int i = 0; i < path.Count; i++)
        {
            previous = current;
            current = (Vector2)path[i];

            //connect previous with current
            //8 cases:
            //upper left
            if (previous.x - 1 == current.x && previous.y - 1 == current.y)
            {
                angle = 45;//?
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x - .5f, 0.55f, previous.y - .5f), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //upper
            else if (previous.x - 1 == current.x && previous.y == current.y)
            {
                angle = 90;
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x - .5f, 0.55f, previous.y), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //upper right
            else if (previous.x - 1 == current.x && previous.y + 1 == current.y)
            {
                angle = -45;//?
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x - .5f, 0.55f, previous.y + .5f), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //left
            else if (previous.x == current.x && previous.y - 1 == current.y)
            {
                angle = 0;
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x, 0.55f, previous.y - .5f), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //right
            else if (previous.x == current.x && previous.y + 1 == current.y)
            {
                angle = 0;
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x, 0.55f, previous.y + .5f), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //lower left
            else if (previous.x + 1 == current.x && previous.y - 1 == current.y)
            {
                angle = -45;//?
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x + .5f, 0.55f, previous.y - .5f), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //lower right
            else if (previous.x + 1 == current.x && previous.y + 1 == current.y)
            {
                angle = 45;//?
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x + .5f, 0.55f, previous.y + .5f), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }
            //lower
            else if (previous.x + 1 == current.x && previous.y == current.y)
            {
                angle = 90;
                pathObject.SetActive(true);
                Object temp = Instantiate(pathObject, new Vector3(previous.x + .5f, 0.55f, previous.y), Quaternion.AngleAxis(angle, Vector3.up));
                pathObject.SetActive(false);
                tempPathObjs.Add(temp);
            }


        }


    }
}

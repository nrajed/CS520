using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Threading;

public class AStarAlgorithm : MonoBehaviour {

    mapSquare[,] map;
    Vector2 startLocation;
    Vector2 goalLocation;

    public GameObject pathObject;
    Process proc;
    long mem;
    public float w;
	public int h; //heuristic number
    Stopwatch stopwatch = new Stopwatch();
    float pathCost;
    int maxMem;
    int numNodes;

    //path that A* follows
    //array list of vectors where path passes through
    public ArrayList path = new ArrayList();
    //temporary pathobject in path
    ArrayList tempPathObjs = new ArrayList();

    public PriorityQueue fringe = new PriorityQueue();
    public PriorityQueue closed = new PriorityQueue();
	

	//FOR SEQUENTIAL A*
	public int n;
	public PriorityQueue[] OPEN;
	public PriorityQueue[] CLOSED;
	public ArrayList mapList = new ArrayList ();

    public Vector2[] centers = new Vector2[8];

    // Use this for initialization
    void Start () {
       numNodes=0;
        proc = Process.GetCurrentProcess();
        mem = proc.PrivateMemorySize64;

        //set weight of A*
        //f=g+wh
        //w = 2.5f; //weight
		//h = 1; //heuristic
		

		n=4; //num heuristics minus 1
		OPEN = new PriorityQueue[n];
		CLOSED = new PriorityQueue[n];

        //temporary pathobject in path
        tempPathObjs = new ArrayList();
        


    }
	
    public float[] calculateAllPathsAStar(float[] results)
    {
        UnityEngine.Debug.Log("computing astar path---------------------------");
        long stopWatchSumw0h3 = 0;
        int pathCountSumw0h3 = 0;
        int numNodesSumw0h3 = 0;
        float pathCostSumw0h3 = 0;

        float pathCostSumw1_5h1 = 0;
        long stopWatchSumw1_5h1 = 0;
        int pathCountSumw1_5h1 = 0;
        int numNodesSumw1_5h1 = 0;

        float pathCostSumw2_5h1 = 0;
        long stopWatchSumw2_5h1 = 0;
        int pathCountSumw2_5h1 = 0;
        int numNodesSumw2_5h1 = 0;

        float pathCostSumw1_5h2 = 0;
        long stopWatchSumw1_5h2 = 0;
        int pathCountSumw1_5h2 = 0;
        int numNodesSumw1_5h2 = 0;

        float pathCostSumw2_5h2 = 0;
        long stopWatchSumw2_5h2 = 0;
        int pathCountSumw2_5h2 = 0;
        int numNodesSumw2_5h2 = 0;

        float pathCostSumw1_5h3 = 0;
        long stopWatchSumw1_5h3 = 0;
        int pathCountSumw1_5h3 = 0;
        int numNodesSumw1_5h3 = 0;

        float pathCostSumw2_5h3 = 0;
        long stopWatchSumw2_5h3 = 0;
        int pathCountSumw2_5h3 = 0;
        int numNodesSumw2_5h3 = 0;

        float pathCostSumw1_5h4 = 0;
        long stopWatchSumw1_5h4 = 0;
        int pathCountSumw1_5h4 = 0;
        int numNodesSumw1_5h4 = 0;

        float pathCostSumw2_5h4 = 0;
        long stopWatchSumw2_5h4 = 0;
        int pathCountSumw2_5h4 = 0;
        int numNodesSumw2_5h4 = 0;

        float pathCostSumw1_5h5 = 0;
        long stopWatchSumw1_5h5 = 0;
        int pathCountSumw1_5h5 = 0;
        int numNodesSumw1_5h5 = 0;

        float pathCostSumw2_5h5 = 0;
        long stopWatchSumw2_5h5 = 0;
        int pathCountSumw2_5h5 = 0;
        int numNodesSumw2_5h5 = 0;

      




            w = 0;
            h = 3;
        //Profiler.BeginSample("AStarAlgorithm");
            calculateAStar();
        //Profiler.EndSample();

            stopWatchSumw0h3 += stopwatch.ElapsedMilliseconds;
            pathCountSumw0h3 += path.Count;
            numNodesSumw0h3 += numNodes;
            pathCostSumw0h3 += pathCost;
        //     UnityEngine.Debug.Log("w=0, h=3. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=0, h=3. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=0, h=3. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=0, h=3. path cost: " + pathCost);

        results[0] = stopwatch.ElapsedMilliseconds;
        results[1] = maxMem;
        results[2] = numNodes;
        results[3] = pathCost;


        w = 1.5f;
            h = 1;
            calculateAStar();
            stopWatchSumw1_5h1 += stopwatch.ElapsedMilliseconds;
            pathCountSumw1_5h1 += path.Count;
            numNodesSumw1_5h1 += numNodes;
            pathCostSumw1_5h1 += pathCost;
        //    UnityEngine.Debug.Log("w=1.5, h=1. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=1.5, h=1. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=1.5, h=1. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=1.5, h=1. path cost: " + pathCost);
        results[4] = stopwatch.ElapsedMilliseconds;
        results[5] = maxMem;
        results[6] = numNodes;
        results[7] = pathCost;


        w = 2.5f;
            h = 1;
            calculateAStar();
            stopWatchSumw2_5h1 += stopwatch.ElapsedMilliseconds;
            pathCountSumw2_5h1 += path.Count;
            numNodesSumw2_5h1 += numNodes;
            pathCostSumw2_5h1 += pathCost;
        //    UnityEngine.Debug.Log("w=2.5, h=1. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=2.5, h=1. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=2.5, h=1. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=2.5, h=1. path cost: " + pathCost);
        results[8] = stopwatch.ElapsedMilliseconds;
        results[9] = maxMem;
        results[10] = numNodes;
        results[11] = pathCost;


        w = 1.5f;
            h = 2;
            calculateAStar();
            stopWatchSumw1_5h2 += stopwatch.ElapsedMilliseconds;
            pathCountSumw1_5h2 += path.Count;
            numNodesSumw1_5h2 += numNodes;
            pathCostSumw1_5h2 += pathCost;
        //    UnityEngine.Debug.Log("w=1.5, h=2. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=1.5, h=2. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=1.5, h=2. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=1.5, h=2. path cost: " + pathCost);
        results[12] = stopwatch.ElapsedMilliseconds;
        results[13] = maxMem;
        results[14] = numNodes;
        results[15] = pathCost;


        w = 2.5f;
            h = 2;
            calculateAStar();
            stopWatchSumw2_5h2 += stopwatch.ElapsedMilliseconds;
            pathCountSumw2_5h2 += path.Count;
            numNodesSumw2_5h2 += numNodes;
            pathCostSumw2_5h2 += pathCost;
        //    UnityEngine.Debug.Log("w=2.5, h=2. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=2.5, h=2. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=2.5, h=2. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=2.5, h=2. path cost: " + pathCost);
        results[16] = stopwatch.ElapsedMilliseconds;
        results[17] = maxMem;
        results[18] = numNodes;
        results[19] = pathCost;


        w = 1.5f;
            h = 3;
            calculateAStar();
            stopWatchSumw1_5h3 += stopwatch.ElapsedMilliseconds;
            pathCountSumw1_5h3 += path.Count;
            numNodesSumw1_5h3 += numNodes;
            pathCostSumw1_5h3 += pathCost;
        //    UnityEngine.Debug.Log("w=1.5, h=3. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=1.5, h=3. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=1.5, h=3. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=1.5, h=3. path cost: " + pathCost);
        results[20] = stopwatch.ElapsedMilliseconds;
        results[21] = maxMem;
        results[22] = numNodes;
        results[23] = pathCost;


        w = 2.5f;
            h = 3;
            calculateAStar();
            stopWatchSumw2_5h3 += stopwatch.ElapsedMilliseconds;
            pathCountSumw2_5h3 += path.Count;
            numNodesSumw2_5h3 += numNodes;
            pathCostSumw2_5h3 += pathCost;
        //    UnityEngine.Debug.Log("w=2.5, h=3. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=2.5, h=3. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=2.5, h=3. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=2.5, h=3. path cost: " + pathCost);
        results[24] = stopwatch.ElapsedMilliseconds;
        results[25] = maxMem;
        results[26] = numNodes;
        results[27] = pathCost;


        w = 1.5f;
            h = 4;
            calculateAStar();
            stopWatchSumw1_5h4 += stopwatch.ElapsedMilliseconds;
            pathCountSumw1_5h4 += path.Count;
            numNodesSumw1_5h4 += numNodes;
            pathCostSumw1_5h4 += pathCost;
        //   UnityEngine.Debug.Log("w=1.5, h=4. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=1.5, h=4. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=1.5, h=4. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=1.5, h=4. path cost: " + pathCost);
        results[28] = stopwatch.ElapsedMilliseconds;
        results[29] = maxMem;
        results[30] = numNodes;
        results[31] = pathCost;



        w = 2.5f;
            h = 4;
            calculateAStar();
            stopWatchSumw2_5h4 += stopwatch.ElapsedMilliseconds;
            pathCountSumw2_5h4 += path.Count;
            numNodesSumw2_5h4 += numNodes;
            pathCostSumw2_5h4 += pathCost;
        //  UnityEngine.Debug.Log("w=2.5, h=4. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=2.5, h=4. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=2.5, h=4. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=2.5, h=4. path cost: " + pathCost);
        results[32] = stopwatch.ElapsedMilliseconds;
        results[33] = maxMem;
        results[34] = numNodes;
        results[35] = pathCost;


        w = 1.5f;
            h = 5;
            calculateAStar();
            stopWatchSumw1_5h5 += stopwatch.ElapsedMilliseconds;
            pathCountSumw1_5h5 += path.Count;
            numNodesSumw1_5h5 += numNodes;
            pathCostSumw1_5h5 += pathCost;
        //  UnityEngine.Debug.Log("w=1.5, h=5. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=1.5, h=5. memory: " + maxMem);
        //UnityEngine.Debug.Log("w=1.5, h=5. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=1.5, h=5. path cost: " + pathCost);
        results[36] = stopwatch.ElapsedMilliseconds;
        results[37] = maxMem;
        results[38] = numNodes;
        results[39] = pathCost;


        w = 2.5f;
            h = 5;
            calculateAStar();
            stopWatchSumw2_5h5 += stopwatch.ElapsedMilliseconds;
            pathCountSumw2_5h5 += path.Count;
            numNodesSumw2_5h5 += numNodes;
            pathCostSumw2_5h5 += pathCost;
        //  UnityEngine.Debug.Log("w=2.5, h=5. time: " + stopwatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("w=2.5, h=5. memory: " + maxMem);
        //     UnityEngine.Debug.Log("w=2.5, h=5. number of nodes: " + numNodes);
        //UnityEngine.Debug.Log("w=2.5, h=5. path cost: " + pathCost);
        results[40] = stopwatch.ElapsedMilliseconds;
        results[41] = maxMem;
        results[42] = numNodes;
        results[43] = pathCost;

        return results;
    }


    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.G))
        {
            calculateAStar();
            //UnityEngine.Debug.Log("time: " + stopwatch.ElapsedMilliseconds);
            //UnityEngine.Debug.Log("path size: " + path.Count);
            //UnityEngine.Debug.Log("number of nodes expanded: " + numNodes);
            //UnityEngine.Debug.Log("path cost: " + pathCost);

        }
    }

    void calculateAStar()
    {
        maxMem = 0;
        pathCost = 0;
        numNodes = 0;
        stopwatch.Reset();
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
        if (aStarComputation())
        {
            Vector2 curr = goalLocation;
            pathCost = map[(int)curr.x, (int)curr.y].g;
            while (curr != startLocation)
            {
                path.Add(curr);
                curr = map[(int)curr.x, (int)curr.y].parent;
            }
            path.Add(curr);
            stopwatch.Stop();
            
            //mem = proc.PrivateMemorySize64;

            //UnityEngine.Debug.Log("mem:" + mem);
           
            visualizeResetPaths();
        }
    }

    //g: cost
    float g(Vector2 curr)
    {
        if(curr.x==-1) { return 0; }
        return map[(int)curr.x, (int)curr.y].g;
       // map[(int)curr.x, (int)curr.y].g = map[(int)parent.x, (int)parent.y].g + findCost((int)curr.x, (int)curr.y,(int)parent.x, (int)parent.y);
        //return (Mathf.Sqrt(Mathf.Pow((curr - startLocation).x,2)+ Mathf.Pow((curr - startLocation).y, 2));
    }
	float g(Vector2 curr, int i)
	{
		if(curr.x==-1) { return 0; }

		return map[(int)curr.x, (int)curr.y].SequentialG[i];
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
        return .25f*Vector2.Distance(s, g);
    }
    //Euclidean distance+distances to partial block centers
    float h4(Vector2 s, Vector2 g)
    {
        float sum = 0;
         for (int i=0; i< centers.Length; i++)
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
        return .25f * Vector2.Distance(s, g)+1/count;
    }

    float findH(Vector2 s, Vector2 g)
    {
        switch (h)
        {
            case 2:
                return h1(s, g);
            case 3:
                return h3(s, g);
            case 4:
                return h4(s, g);
            case 5:
                return h5(s, g);
            default:
                return h2(s, g);

        }
    }


    //computes A*
    bool aStarComputation()
    {
       
        map[(int)startLocation.x, (int)startLocation.y].g = 0;
        map[(int)startLocation.x, (int)startLocation.y].parent = startLocation;
        path.Clear();
      
        fringe.Insert(startLocation, g(startLocation) + w* findH(startLocation,goalLocation));
        if (fringe.getSize() > maxMem)
        {
            maxMem = fringe.getSize();
        }

        //...do all pseudocode here
        while (!fringe.isEmpty())
        {
            numNodes++;
            Vector2 s = fringe.Pop();
            if(s==goalLocation)
            {
                return true; //path found
            }
            closed.Insert(s, g(s)+w* findH(s,goalLocation));
            if (closed.getSize() > maxMem)
            {
                maxMem = closed.getSize();
            }


            for (int i=-1; i<=1; i++) {
                for(int j=-1; j<=1; j++) {
                    Vector2 curr = new Vector2(s.x + i, s.y + j);
                   
                    if ((i!=0 || j!=0) && (s.x+i>=0 && s.y+j>=0) && (s.x+i<map.GetLength(0) && s.y+j< map.GetLength(1))) {
                        if (map[(int)curr.x, (int)curr.y].type == 0)
                        {
                            continue;
                        }
                        if (!closed.contains(curr)) {
                            if(!fringe.contains(curr)) {
                                map[(int)curr.x, (int)curr.y].g = float.MaxValue;
                                map[(int)curr.x, (int)curr.y].parent = new Vector2(-2,-1);
                            }
                           
                            updateVertex(s, curr);
                        }
                    }
                }
            }
        }

        UnityEngine.Debug.Log("Path not found");
        return false;
    }

    //updates Vertex for A*
    void updateVertex(Vector2 parent, Vector2 curr)
    {


        if (g(parent) + findCost((int)curr.x,(int)curr.y,(int)parent.x,(int)parent.y) < g(curr))
        {
            map[(int)curr.x, (int)curr.y].g = (g(parent) + findCost((int)curr.x, (int)curr.y, (int)parent.x, (int)parent.y));
            map[(int)curr.x, (int)curr.y].parent = parent;
        }
        if(fringe.contains(curr))
        {
            fringe.Remove(curr);
        }
        map[(int)curr.x, (int)curr.y].f = map[(int)curr.x, (int)curr.y].g+w * findH(curr, goalLocation);
        map[(int)curr.x, (int)curr.y].h = findH(curr, goalLocation);
        fringe.Insert(curr, map[(int)curr.x, (int)curr.y].g + w* findH(curr,goalLocation));
        if(fringe.getSize()> maxMem)
        {
            maxMem = fringe.getSize();
        }

    }

	//For Part 2
	/*int sequentialAStar() {
		
		for (int i = 0; i <= n; i++) {
			ANYTHING IN THIS COMMENT BLOCK REMAINS UNIMPLEMENTED

			//different g-value for each search
			map [(int)startLocation.x, (int)startLocation.y].SequentialG.Add(0f);
			map [(int)goalLocation.x, (int)goalLocation.y].SequentialG.Add(float.MaxValue);

			//start and goal don't have parents
			map [(int)startLocation.x, (int)startLocation.y].SequentialParent.Add(new Vector2(-2,-1));
			map [(int)goalLocation.x, (int)goalLocation.y].SequentialParent.Add(new Vector2(-2,-1));

			OPEN[i].Insert (startLocation, Key (startLocation, i));
		}

		Vector2 s = OPEN [0].Pop ();
		float minKey0 = Key (s, 0);
		while(minKey0<float.MaxValue)
		{
			for (int i = 1; i <= n; i++) {
				float minKeyI = Key (OPEN [i].Pop, i);
				if(minKeyI <= w2*minKey0){
					if(map[(int)goalLocation.x,(int)goalLocation.y].SequentialG[i] <= minKeyI) {
						if(map[(int)goalLocation.x,(int)goalLocation.y].SequentialG[i] <= float.MaxValue) {
							return i;
						}
					}
					else {
						s = OPEN [i].Pop ();
						ExpandState (s, i);
						CLOSED [i].Insert (s, Key(s,i));
					}
				}
				else {
					if(g(goalLocation) <= minKey0){
						if(g(goalLocation) < float.MaxValue) {
							return 0;
						}

					}
					else {
						s = OPEN [0].Pop ();
						ExpandState (s, 0);
						CLOSED [0].Insert (s, Key(s,0));
					}
				}
			}
		}
	}

	//Helper Methods
	float Key(Vector2 s, int i)
	{
		switch(h)
		{
			case 1:
				return g(s,i) + w * h1 (s, goalLocation);
			case 2:
				return g(s,i) + w * h2 (s, goalLocation);
			case 3:
				return g(s,i) + w * h3 (s, goalLocation);
			case 4:
				return g(s,i) + w * h4 (s, goalLocation);
			default:
				return g(s,i) + w * h5 (s, goalLocation);
		}
	}

	void ExpandState(Vector2 s, int i)
	{
		OPEN[i].Remove (s);

		//For all neighbors in succ(s)
		for (int j = -1; j <= 1; j++) {
			for (int k = -1; k <= 1; k++) {
				Vector2 curr = new Vector2 (s.x + i, s.y + j);
				if (!(OPEN [i].contains (curr) || CLOSED[i].contains(curr))) {
					map [curr.x, curr.y].SequentialG[i] = float.MaxValue;
					map [curr.x, curr.y].SequentialParent[i] = new Vector2(-2,-1);
				}
				if (g (curr) > g (s) + findCost (s.x, s.y, curr.x, curr.y)) {
					map [curr.x, curr.y].SequentialG[i] = g (s) + findCost (s.x, s.y, curr.x, curr.y);
					map [curr.x, curr.y].SequentialParent[i] = s;

					//If not in open list
					if (!CLOSED [i].contains (curr)) { 
						OPEN [i].Insert (curr, Key (s, i)); //add to open list
					}
				}
			}
		}
	}*/

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
        for(int i=0; i<tempPathObjs.Count;i++){
            Destroy((GameObject)tempPathObjs[i]);
         }
         tempPathObjs.Clear();
        

        //Loop through whole path
        
        Vector2 previous=(Vector2)path[0];
        Vector2 current = previous;
        int angle;

        for(int i=0; i<path.Count; i++){
             previous=current;
             current=(Vector2)path[i];

             //connect previous with current
             //8 cases:
             //upper left
             if(previous.x-1==current.x&&previous.y-1==current.y){
                 angle=45;//?
                 pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x-.5f, 0.55f, previous.y-.5f), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //upper
             else if(previous.x-1==current.x&&previous.y==current.y){
                 angle=90;
                 pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x-.5f, 0.55f, previous.y), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //upper right
             else if(previous.x-1==current.x&&previous.y+1==current.y){
                 angle=-45;//?
                 pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x-.5f, 0.55f, previous.y+.5f), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //left
             else if(previous.x==current.x&&previous.y-1==current.y){
                 angle=0;
                 pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x, 0.55f, previous.y-.5f), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //right
             else if(previous.x==current.x&&previous.y+1==current.y){
                 angle=0;
                 pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x, 0.55f, previous.y+.5f), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //lower left
             else if(previous.x+1==current.x&&previous.y-1==current.y){
                angle = -45;//?
                pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x+.5f, 0.55f, previous.y-.5f), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //lower right
             else if(previous.x+1==current.x&&previous.y+1==current.y){
                angle = 45;//?
                pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x+.5f, 0.55f, previous.y+.5f), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }
             //lower
             else if(previous.x+1==current.x&&previous.y==current.y){
                 angle=90;
                 pathObject.SetActive(true);
                 Object temp = Instantiate(pathObject, new Vector3(previous.x+.5f, 0.55f, previous.y), Quaternion.AngleAxis(angle, Vector3.up));
                 pathObject.SetActive(false);
                 tempPathObjs.Add(temp);
             }


        }

        
    }
}

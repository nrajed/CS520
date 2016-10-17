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
    Stopwatch stopwatch = new Stopwatch();

    int numNodes;

    //path that A* follows
    //array list of vectors where path passes through
    public ArrayList path = new ArrayList();
    //temporary pathobject in path
    ArrayList tempPathObjs = new ArrayList();

    public PriorityQueue fringe = new PriorityQueue();
    public PriorityQueue closed = new PriorityQueue();

    public Vector2[] centers = new Vector2[8];

    // Use this for initialization
    void Start () {
       numNodes=0;
        proc = Process.GetCurrentProcess();
        mem = proc.PrivateMemorySize64;

        //set weight of A*
        //f=g+wh
        w = 1f; //weight 

        //temporary pathobject in path
        tempPathObjs = new ArrayList();
    }
	
	// Update is called once per frame
	void Update () {
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
                centers= transform.gameObject.GetComponent<loadMap>().centers;
            }
            else
            {
                map = map2;
                startLocation = transform.gameObject.GetComponent<makeMap>().startLocation;
                goalLocation = transform.gameObject.GetComponent<makeMap>().goalLocation;
                centers = transform.gameObject.GetComponent<makeMap>().centerPartiallyBlocked;
            }
            if(aStarComputation())
            {
                Vector2 curr = goalLocation;
                while (curr != startLocation)
                {
                    path.Add(curr);
                    curr = map[(int)curr.x, (int)curr.y].parent;
                }
                path.Add(curr);
                stopwatch.Stop();
                UnityEngine.Debug.Log("stopwatch:" + stopwatch.ElapsedMilliseconds);
                UnityEngine.Debug.Log("pathSize:" + path.Count);
                UnityEngine.Debug.Log("numNodes:" + numNodes);
                //mem = proc.PrivateMemorySize64;
                
                //UnityEngine.Debug.Log("mem:" + mem);
                UnityEngine.Debug.Log("visualizing");
                visualizeResetPaths();
            }
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

    //computes A*
    bool aStarComputation()
    {
        map[(int)startLocation.x, (int)startLocation.y].g = 0;
        map[(int)startLocation.x, (int)startLocation.y].parent = startLocation;
        path.Clear();
      
        fringe.Insert(startLocation, g(startLocation) + w*h1(startLocation,goalLocation));

        //...do all pseudocode here
        while(!fringe.isEmpty())
        {
            numNodes++;
            Vector2 s = fringe.Pop();
            if(s==goalLocation)
            {
                return true; //path found
            }
            closed.Insert(s, g(s)+w*h1(s,goalLocation));
           
            for(int i=-1; i<=1; i++) {
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

        fringe.Insert(curr, map[(int)curr.x, (int)curr.y].g + w*h1(curr,goalLocation));

    }

	//For Part 2
	void sequentialAStar() {
		
	}

	//Helper Methods
	int Key(Vector2 s, int i)
	{
		
	}

	void ExpandState(Vector2 s, int i)
	{

	}

	void IntegratedHeuristicAStar() {
	
	}

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

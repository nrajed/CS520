using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Threading;

public class Phase2AStar_Sequential : MonoBehaviour {

	mapSquare[,] map;
	Vector2 startLocation;
	Vector2 goalLocation;

	public GameObject pathObject;
	Process proc;
	long mem;
	public float w;
	public int h; //heuristic number
	Stopwatch stopwatch = new Stopwatch();

	int numNodes;

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
	public ArrayList mapList = new ArrayList();

	public Vector2[] centers = new Vector2[8];

	// Use this for initialization
	void Start()
	{
		numNodes = 0;
		proc = Process.GetCurrentProcess();
		mem = proc.PrivateMemorySize64;

		//set weight of A*
		//f=g+wh
		w = 1f; //weight
		h = 1; //heuristic
		w2 = 1f;

		n = 4; //num heuristics minus 1
		OPEN = new PriorityQueue[n+1];
		CLOSED = new PriorityQueue[n+1];

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
			int index = sequentialAStar();
			if (index != -1)
			{ 
				Vector2 curr = goalLocation;
				while (curr != startLocation)
				{
					path.Add(curr);
					curr = (Vector2)map[(int)curr.x, (int)curr.y].SequentialParent[index];
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


	//For Part 2
	int sequentialAStar()
	{
		//map[(int)startLocation.x, (int)startLocation.y].SequentialG = new ArrayList();
		//map[(int)startLocation.x, (int)startLocation.y].SequentialParent = new ArrayList();
		//map[(int)goalLocation.x, (int)goalLocation.y].SequentialG = new ArrayList();
		//map[(int)goalLocation.x, (int)goalLocation.y].SequentialParent = new ArrayList();

		for (int i = 0; i <= n; i++)
		{
			/**ANYTHING IN THIS COMMENT BLOCK REMAINS UNIMPLEMENTED**/



			//different g-value for each search
			map[(int)startLocation.x, (int)startLocation.y].SequentialG[i]=0f;
			map[(int)goalLocation.x, (int)goalLocation.y].SequentialG[i]=float.MaxValue;

			//start and goal don't have parents
			map[(int)startLocation.x, (int)startLocation.y].SequentialParent[i]=new Vector2(-2, -1);
			map[(int)goalLocation.x, (int)goalLocation.y].SequentialParent[i]=new Vector2(-2, -1);

			OPEN[i] = new PriorityQueue();
			CLOSED[i] = new PriorityQueue();
			OPEN[i].Insert(startLocation, Key(startLocation, i));
		}

		//Vector2 s = OPEN[0].Pop();
		//float minKey0 = Key(s, 0);
		while (OPEN[0].getMin() < float.MaxValue)
		{
			for (int i = 1; i <= n; i++)
			{
				//float minKeyI = Key(OPEN[i].Pop(), i);
				if (OPEN[i].getMin() <= w2 * OPEN[0].getMin())
				{
					if ((float)(map[(int)goalLocation.x, (int)goalLocation.y].SequentialG[i]) <= OPEN[i].getMin())
					{
						if ((float)(map[(int)goalLocation.x, (int)goalLocation.y].SequentialG[i]) < float.MaxValue)
						{
							return i;
						}
					}
					else
					{
						Vector2 s = OPEN[i].Pop();
						ExpandState(s, i);
						CLOSED[i].Insert(s, Key(s, i));
					}
				}
				else
				{
					if (g(goalLocation,0) <= OPEN[0].getMin())
					{
						if (g(goalLocation,0) < float.MaxValue)
						{
							return 0;
						}

					}
					else
					{
						Vector2 s = OPEN[0].Pop();
						ExpandState(s, 0);
						CLOSED[0].Insert(s, Key(s, 0));
					}
				}
			}
		}
		return -1;
	}

	//Helper Methods
	float Key(Vector2 s, int i)
	{
		switch (i)
		{
		case 0:
			return g(s, i) + w * h1(s, goalLocation);
		case 1:
			return g(s, i) + w * h2(s, goalLocation);
		case 2:
			return g(s, i) + w * h3(s, goalLocation);
		case 3:
			return g(s, i) + w * h4(s, goalLocation);
		default:
			return g(s, i) + w * h5(s, goalLocation);
		}
	}

	void ExpandState(Vector2 s, int i)
	{
		OPEN[i].Remove(s);

		//For all neighbors in succ(s)
		for (int j = -1; j <= 1; j++)
		{
			for (int k = -1; k <= 1; k++)
			{
				Vector2 curr = new Vector2(s.x + j, s.y + k);
				if((j != 0 || k != 0) && (s.x + j >= 0 && s.y + k >= 0) && (s.x + j < map.GetLength(0) && s.y + k < map.GetLength(1))) {
					if (map [(int)curr.x, (int)curr.y].type != 0) {
						if (!(OPEN [i].contains (curr) || CLOSED [i].contains (curr))) {
							//map[(int)curr.x, (int)curr.y].SequentialG = new ArrayList();
							//map[(int)curr.x, (int)curr.y].SequentialParent = new ArrayList();
							map [(int)curr.x, (int)curr.y].SequentialG [i] = float.MaxValue;
							map [(int)curr.x, (int)curr.y].SequentialParent [i] = new Vector2 (-2, -1);
						}
						if (g (curr, i) > g (s, i) + findCost ((int)s.x, (int)s.y, (int)curr.x, (int)curr.y)) {
							map [(int)curr.x, (int)curr.y].SequentialG [i] = g (s, i) + findCost ((int)s.x, (int)s.y, (int)curr.x, (int)curr.y);
							map [(int)curr.x, (int)curr.y].SequentialParent [i] = s;

							//If not in open list
							if (!CLOSED [i].contains (curr)) {
								OPEN [i].Insert (curr, Key (curr, i)); //add to open list
							}
						}
					}
				}
			}
		}
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
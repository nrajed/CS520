using UnityEngine;
using System.Collections;
using System.IO;

public class ViterbiAlgorithm : MonoBehaviour
{
    //number of states
    public int k = 8;

    //number of observations
    public int t = 1;

    public double[,] T1;
    public int[,] T2;
    public int numIterations;
    public mapSquare[,] map;

    // Use this for initialization
    void Start()
    {
        //setUpSmallMap();
        //setUpTextbookProb();
        //setUpBigMap();

        //question A,B
        map = new mapSquare[3, 3];
        map[0, 0] = new mapSquare();
        map[0, 1] = new mapSquare();
        map[0, 2] = new mapSquare();
        map[1, 0] = new mapSquare();
        map[1, 1] = new mapSquare();
        map[1, 2] = new mapSquare();
        map[2, 0] = new mapSquare();
        map[2, 1] = new mapSquare();
        map[2, 2] = new mapSquare();

        map[0, 0].type = 1;
        map[0, 0].typeHighway = 1;
        map[1, 0].type = 1;
        map[2, 0].type = 1;
        map[0, 1].type = 1;
        map[0, 1].typeHighway = 1;
        map[0, 2].type = 2;
        map[1, 1].type = 1;
        map[1, 2].type = 1;
        map[2, 1].type = 0;
        map[2, 2].type = 1;
        map[2, 2].typeHighway = 1;

        //setUpSmallMap();
        //setUpQuestionA();
        scaleUpFilter();
    }

    int[] actions;
    int[] observations;

    Vector2[] locations;

    int[] errorPlot;

    void scaleUpFilter()
    {
        errorPlot = new int[100];
        //type of action and sensor
        string[] O = { "N", "T", "H" };
        string[] A = { "R", "L", "U", "D" };


        this.gameObject.transform.GetComponent<makeMap>().makeMapGreat();
        map = this.gameObject.transform.GetComponent<makeMap>().map;

        int numRows = map.GetLength(0);
        int numCols = map.GetLength(1);

        Vector2 startLocation = this.gameObject.transform.GetComponent<makeMap>().startLocation;
        
        
        locations= new Vector2[100];


        //Question C:

        //1:Right, 2:Left, 3:Down,4:Up
        actions = new int[100];

        //1:unblocked, 2: partially blocked, 3:highway
        observations = new int[100];

        Vector2 currentLocation = startLocation;

        //Debug.Log("Starting Location: " + startLocation);

        for (int i = 0; i < 100; i++)
        {
            actions[i] = Random.Range(1, 5);
            //find actual coordinates

            float randNum = Random.value;
            //with .9 probability we execute the action
            if (randNum < .9f)
            {
                //right
                if (actions[i] == 1)
                {
                    if (currentLocation.y + 1 < numCols && map[(int)currentLocation.x, (int)currentLocation.y + 1].type != 0)
                    {
                        currentLocation = new Vector2(currentLocation.x, currentLocation.y + 1);
                    }
                }
                //left
                else if (actions[i] == 2)
                {
                    if (currentLocation.y - 1 >= 0 && map[(int)currentLocation.x, (int)currentLocation.y - 1].type != 0)
                    {
                        currentLocation = new Vector2(currentLocation.x, currentLocation.y - 1);
                    }
                }
                //down
                else if (actions[i] == 3)
                {
                    if (currentLocation.x - 1 >= 0 && map[(int)currentLocation.x - 1, (int)currentLocation.y].type != 0)
                    {
                        currentLocation = new Vector2(currentLocation.x - 1, currentLocation.y);
                    }
                }
                //up
                else if (actions[i] == 4)
                {
                    if (currentLocation.x + 1 < numRows && map[(int)currentLocation.x + 1, (int)currentLocation.y].type != 0)
                    {
                        currentLocation = new Vector2(currentLocation.x + 1, currentLocation.y);
                    }
                }

            }

            locations[i] = currentLocation;
            map[(int)currentLocation.x, (int)currentLocation.y].path = 1;

            //get observation
            randNum = Random.value;
            if (randNum < .9)
            {
                if (map[(int)currentLocation.x, (int)currentLocation.y].typeHighway >= 1)
                {
                    observations[i] = 3;
                }
                else
                {
                    observations[i] = map[(int)currentLocation.x, (int)currentLocation.y].type;
                }

            }
            else if (randNum < .95f)
            {
                //three possibilities 1 for each type
                if (map[(int)currentLocation.x, (int)currentLocation.y].typeHighway >= 1)
                {
                    observations[i] = 1;
                }
                else if (map[(int)currentLocation.x, (int)currentLocation.y].type == 1)
                {
                    observations[i] = 2;
                }
                else
                {
                    observations[i] = 3;
                }
            }
            else
            {
                //three possibilities 1 for each type
                if (map[(int)currentLocation.x, (int)currentLocation.y].typeHighway >= 1)
                {
                    observations[i] = 2;
                }
                else if (map[(int)currentLocation.x, (int)currentLocation.y].type == 1)
                {
                    observations[i] = 3;
                }
                else
                {
                    observations[i] = 1;
                }
            }


            //Debug.Log("Action: " + A[actions[i] - 1] + " gives us coordinates: " + locations[i] + "with observations: " + O[observations[i] - 1] + " when actual observation should be: " + O[map[(int)currentLocation.x, (int)currentLocation.y].type - 1]);

        }



        convertToText(startLocation, locations, actions, observations, A, O, map);

        //this.gameObject.transform.GetComponent<makeMap>().displaySquaresGround();

        numIterations = 0;


        //go through all the actions and find the actuall coordingates

        //1:unblocked, 2: partially blocked, 3:highway
        //int[] observations = { 1, 1, 3, 3 };
        //filtering(actions, observations);
    }



    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            map = this.gameObject.transform.GetComponent<makeMap>().map;
            //go on to next step in 
            numIterations++;

            if (numIterations == 1)
            {
                //do 10 iterations
                int[] a = new int[10];
                int[] o = new int[10];
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = actions[i];
                    o[i] = observations[i];
                }
                filtering(a, o);
                this.gameObject.transform.GetComponent<makeMap>().displaySquaresGround(locations,10,6);
                this.gameObject.transform.GetComponent<makeMap>().updateStarterGoalSquares(this.gameObject.transform.GetComponent<makeMap>().startLocation, locations[9], 7);

            }
            else if (numIterations == 2)
            {
                //do 50 iterations
                int[] a = new int[50];
                int[] o = new int[50];
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = actions[i];
                    o[i] = observations[i];
                }
                filtering(a, o);
                this.gameObject.transform.GetComponent<makeMap>().displaySquaresGround(locations,50,6);
                this.gameObject.transform.GetComponent<makeMap>().updateStarterGoalSquares(this.gameObject.transform.GetComponent<makeMap>().startLocation, locations[49], 7);
            }
            else
            {
                //do 100 iterations
                int[] a = new int[100];
                int[] o = new int[100];
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = actions[i];
                    o[i] = observations[i];
                }
                filtering(a, o);
                this.gameObject.transform.GetComponent<makeMap>().displaySquaresGround(locations,100,6);
                this.gameObject.transform.GetComponent<makeMap>().updateStarterGoalSquares(this.gameObject.transform.GetComponent<makeMap>().startLocation, locations[99], 7);
                for(int i=0; i< errorPlot.Length; i++)
                {
                    Debug.Log(errorPlot[i]);
                }
            }

        }

    }



    public StreamWriter tw;

    //generate the text file
    void convertToText(Vector2 startLocation, Vector2[] locations, int[] actions, int[] observations, string[] A, string[] O, mapSquare[,] map)
    {

        tw = new StreamWriter("QuestionC.txt");

        tw.Write("Starting Location");
        tw.Write("" + startLocation);
        tw.WriteLine();

        tw.Write("Path");
        for (int i = 0; i < 100; i++)
        {
            tw.Write("" + locations[i]);
        }
        tw.WriteLine();

        tw.Write("Actions");
        for (int i = 0; i < 100; i++)
        {

            tw.Write("" + A[actions[i] - 1]);
        }
        tw.WriteLine();

        tw.Write("Observations");
        for (int i = 0; i < 100; i++)
        {
            tw.Write("" + O[observations[i] - 1]);
        }
        tw.WriteLine();


        tw.Close();
    }



    void setUpQuestionA()
    {
        //1:Right, 2:Left, 3:Down,4:Up
        int[] actions = { 1, 1, 3, 3 };
        //1:unblocked, 2: partially blocked, 3:highway
        int[] observations = { 1, 1, 3, 3 };
        filtering(actions, observations);
    }

    void filtering(int[] actions, int[] observations)
    {
        float maxProb = 0;
        Vector2 maxProbLoc = new Vector2(0,0);

        int row = map.GetLength(0);
        int column = map.GetLength(1);
        float[,] prevBeliefs = new float[row, column];
        float[,] newBeliefs = new float[row, column];
        float sumProbabilitiesBeliefs = 0;

        int numUnblocked = 0;
        //initialize the starting point probabilities
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                if (map[r, c].type == 0)
                {
                }
                else
                {
                    numUnblocked++;
                }
            }
        }
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                prevBeliefs[r, c] = 1f / (numUnblocked);
            }
        }

        //for each action
        for (int i = 0; i < actions.Length; i++)
        {
            sumProbabilitiesBeliefs = 0;

            //go through all squares in the map
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    //if the square is blocked, we do nothing
                    if (map[r, c].type == 0)
                    {
                        continue;
                    }

                    //transition model: get 
                    //two possibilities: 
                    //1 above, below, right or up
                    //2 same
                    float[] transitionSummation = new float[2];
                    float[] previousBelief = new float[2];

                    //right so either from left or same
                    if (actions[i] == 1)
                    {
                        if (c - 1 >= 0 && map[r, c - 1].type != 0)
                        {
                            transitionSummation[0] = .9f;
                            if (c + 1 < column && map[r, c + 1].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = prevBeliefs[r, c - 1];
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                        else
                        {
                            transitionSummation[0] = 0;
                            if (c + 1 < column && map[r, c + 1].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = 0;
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                    }
                    //left so either from right or same
                    else if (actions[i] == 2)
                    {
                        if (c + 1 < column && map[r, c + 1].type != 0)
                        {
                            transitionSummation[0] = .9f;
                            if (c - 1 >= 0 && map[r, c - 1].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = prevBeliefs[r, c + 1];
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                        else
                        {
                            transitionSummation[0] = 0;
                            if (c - 1 >= 0 && map[r, c - 1].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = 0;
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                    }
                    //down so either from up or same
                    else if (actions[i] == 4)
                    {
                        if (r - 1 >= 0 && map[r - 1, c].type != 0)
                        {
                            transitionSummation[0] = .9f;
                            if (r + 1 < row && map[r + 1, c].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = prevBeliefs[r - 1, c];
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                        else
                        {
                            transitionSummation[0] = 0;
                            if (r + 1 < row && map[r + 1, c].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = 0;
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                    }
                    //up so either from down or same
                    else
                    {
                        if (r + 1 < row && map[r + 1, c].type != 0)
                        {
                            transitionSummation[0] = .9f;
                            if (r - 1 >= 0 && map[r - 1, c].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = prevBeliefs[r + 1, c];
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                        else
                        {
                            transitionSummation[0] = 0;
                            if (r - 1 >= 0 && map[r - 1, c].type != 0)
                            {
                                transitionSummation[1] = .1f;
                            }
                            else
                            {
                                transitionSummation[1] = 1f;
                            }
                            previousBelief[0] = 0;
                            previousBelief[1] = prevBeliefs[r, c];
                        }
                    }

                    float observationModel = 0;
                    int currentStateType = 0;
                    if (map[r, c].typeHighway >= 1)
                    {
                        currentStateType = 3;
                    }
                    else if (map[r, c].type == 1)
                    {
                        currentStateType = 1;
                    }
                    else if (map[r, c].type == 2)
                    {
                        currentStateType = 2;
                    }
                    //1:unblocked, 2: partially blocked, 3:highway

                    if (observations[i] == currentStateType)
                    {
                        observationModel = .9f;
                    }
                    else
                    {
                        observationModel = .05f;
                    }

                    //P(Xt|E1:t)=alpha*observation model*sum(transition model*prev)
                    float filterProb = observationModel * (transitionSummation[0] * previousBelief[0] + transitionSummation[1] * previousBelief[1]);


                    newBeliefs[r, c] = filterProb;
                    sumProbabilitiesBeliefs = sumProbabilitiesBeliefs + filterProb;
                }


            }

            //Debug.Log("--------------step " + i + "-----------------------");

            //we did all the probabilities, lets normalize
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    newBeliefs[r, c] = (1 / (sumProbabilitiesBeliefs)) * newBeliefs[r, c];
                    if(newBeliefs[r, c] > maxProb)
                    {
                        maxProb = newBeliefs[r, c];
                        maxProbLoc = new Vector2(r, c);
                    }


                }
            }
            prevBeliefs = newBeliefs;
            newBeliefs = new float[row, column];

            //error is distance between the biggest prob belief and the actual location at this point
            errorPlot[i] = (int)(locations[i].x-maxProbLoc.x+ locations[i].y - maxProbLoc.y);
        }

        //update the heatmap to what it was last
        makeHeatmap(prevBeliefs);
        

    }


    public GameObject percent10;
    public GameObject percent20;
    public GameObject percent30;
    public GameObject percent40;
    public GameObject percent50;
    public GameObject percent60;
    public GameObject percent70;
    public GameObject percent80;
    public GameObject percent90;


    void makeHeatmap(float[,] prevBeliefs)
    {
        int numRows = prevBeliefs.GetLength(0);
        int numCols = prevBeliefs.GetLength(1);

        

        //go through map and place the right squares in the right places
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                //Debug.Log("At " + r + "," + c + " the probability is " + prevBeliefs[r, c]);

                //.0052%
                if (prevBeliefs[r, c] < 1.0/(numRows*numCols))
                {
                    percent10.SetActive(true);
                    Object temp = Instantiate(percent10, new Vector3(r, numIterations+2, c), Quaternion.identity);
                    percent10.SetActive(false);
                }
                //.052%
                else if (prevBeliefs[r, c] < 10.0 / (numRows * numCols))
                {
                    percent20.SetActive(true);
                    Object temp = Instantiate(percent20, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent20.SetActive(false);
                }
                //.52%
                else if (prevBeliefs[r, c] < 100.0/ (numRows * numCols))
                {
                    percent30.SetActive(true);
                    Object temp = Instantiate(percent30, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent30.SetActive(false);
                }
                //5.2%
                else if (prevBeliefs[r, c] < 1000.0 / (numRows * numCols))
                {
                    percent40.SetActive(true);
                    Object temp = Instantiate(percent40, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent40.SetActive(false);
                }
                //10%
                else if (prevBeliefs[r, c] < .1)
                {percent50.SetActive(true);
                    Object temp = Instantiate(percent50, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent50.SetActive(false);

                }
                //40%
                else if (prevBeliefs[r, c] < .4)
                {
                    percent60.SetActive(true);
                    Object temp = Instantiate(percent60, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent60.SetActive(false);
                }
                //70%
                else if (prevBeliefs[r, c] < .7)
                {
                    percent70.SetActive(true);
                    Object temp = Instantiate(percent70, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent70.SetActive(false);
                }
                //90%
                else if (prevBeliefs[r, c] < .9)
                {
                    percent80.SetActive(true);
                    Object temp = Instantiate(percent80, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent80.SetActive(false);
                }
                //100%
                else if (prevBeliefs[r, c] <= 1)
                {
                    percent90.SetActive(true);
                    Object temp = Instantiate(percent90, new Vector3(r, numIterations + 2, c), Quaternion.identity);
                    percent90.SetActive(false);
                }

               

            }
        }
   


}



void setUpSmallMap()
    {
        k = 8; t = 4;
        T1 = new double[k, t + 1];
        T2 = new int[k, t + 1];


        //INPUT: 
        //observation space
        string[] O = { "N", "H", "T" };
        //state space
        string[] S = { "1,1", "1,2", "1,3", "2,1", "2,2", "2,3", "3,1", "3,3" };
        //sequence of observations
        int[] Y = { 0, 0, 1, 1 };//, "N", "H", "H]" };
        //transition matrix for right action
        double[,] A_right = { {.1,.9,0,0,0,0,0,0}, //1,1
            { 0, .1, .9, 0, 0, 0, 0, 0 }, //1,2
            { 0, 0, 1, 0, 0, 0, 0, 0 }, //1,3
            { 0, 0, 0, .1, .9, 0, 0, 0 }, //2,1
            { 0, 0, 0, 0, .1, .9, 0, 0 }, //2,2
            { 0, 0, 0, 0, 0, 1, 0, 0 },//2,3
            { 0, 0, 0, 0, 0, 0, 1, 0 }, //3,1
            { 0, 0, 0, 0, 0, 0, 0, 1 } }; //3,3
        double[,] A_down = {  { .1, 0, 0, .9, 0, 0, 0, 0 }, //1,1
            { 0, .1, 0, 0, .9, 0, 0, 0 }, //1,2
            { 0, 0, .1, 0, 0, .9, 0, 0 }, //1,3
            { 0, 0, 0, .1, 0, 0, .9, 0 }, //2,1
            { 0, 0, 0, 0, 1, 0, 0, 0 }, //2,2
            { 0, 0, 0, 0, 0, .1, 0, .9 }, //2,3
            { 0, 0, 0, 0, 0, 0, 1, 0 }, //3,1
            { 0, 0, 0, 0, 0, 0, 0, 1 } }; //3,3
        //emission matrix B
        double[,] B = { {.05,.9,.05}, //1,1
            {.05,.9,.05}, //1,2
            { .05, .05, .9}, //1,3
            { .9, .05, .05}, //2,1
            { .9, .05, .05}, //2,2
            { .9, .05, .05}, //2,3
            { .9, .05, .05}, //3,1
            {.05,.9,.05} }; //3,3
        //array of initial probabilities
        double[] p = { .125, .125, .125, .125, .125, .125, .125, .125 };//, "N", "H", "H]" };
        int[] x = viterbi(O, S, p, Y, A_right, B, A_down);
        for (int i = 0; i < x.Length; i++)
        {
            Debug.Log("state " + i + " path:" + S[x[i]]);
            Debug.Log("(1,1): " + T1[0, i] + ". (1,2): " + T1[1, i] + ". (1,3): " + T1[2, i]);
            Debug.Log("(2,1): " + T1[3, i] + ". (2,2): " + T1[4, i] + ". (2,3): " + T1[5, i]);
            Debug.Log("(3,1): " + T1[6, i] + "" + ". (3,3): " + T1[7, i]);
        }
    }

    void setUpSmallMapFiltering()
    {
        k = 8; t = 4;

        //INPUT: 
        //observation space
        string[] O = { "N", "H", "T" };
        //state space
        string[] S = { "1,1", "1,2", "1,3", "2,1", "2,2", "2,3", "3,1", "3,3" };
        //sequence of observations
        int[] Y = { 2, 2, 2, 2 };//, "N", "H", "H]" };
        //transition matrix for right action
        double[,] A_right = { {.1,.9,0,0,0,0,0,0}, //1,1
            { 0, .1, .9, 0, 0, 0, 0, 0 }, //1,2
            { 0, 0, 1, 0, 0, 0, 0, 0 }, //1,3
            { 0, 0, 0, .1, .9, 0, 0, 0 }, //2,1
            { 0, 0, 0, 0, .1, .9, 0, 0 }, //2,2
            { 0, 0, 0, 0, 0, 1, 0, 0 },//2,3
            { 0, 0, 0, 0, 0, 0, 1, 0 }, //3,1
            { 0, 0, 0, 0, 0, 0, 0, 1 } }; //3,3
        double[,] A_down = {  { .1, 0, 0, .9, 0, 0, 0, 0 }, //1,1
            { 0, .1, 0, 0, .9, 0, 0, 0 }, //1,2
            { 0, 0, .1, 0, 0, .9, 0, 0 }, //1,3
            { 0, 0, 0, .1, 0, 0, .9, 0 }, //2,1
            { 0, 0, 0, 0, 1, 0, 0, 0 }, //2,2
            { 0, 0, 0, 0, 0, .1, 0, .9 }, //2,3
            { 0, 0, 0, 0, 0, 0, 1, 0 }, //3,1
            { 0, 0, 0, 0, 0, 0, 0, 1 } }; //3,3
        //emission matrix B
        double[,] B = { {.05,.9,.05}, //1,1
            {.05,.9,.05}, //1,2
            { .05, .05, .9}, //1,3
            { .9, .05, .05}, //2,1
            { .9, .05, .05}, //2,2
            { .9, .05, .05}, //2,3
            { .9, .05, .05}, //3,1
            {.05,.9,.05} }; //3,3
        //array of initial probabilities
        double[] p = { .125, .125, .125, .125, .125, .125, .125, .125 };//, "N", "H", "H]" };
        int[] x = viterbi(O, S, p, Y, A_right, B, A_down);
        for (int i = 0; i < x.Length; i++)
        {
            Debug.Log("state " + i + " path:" + S[x[i]]);
            Debug.Log("(1,1): " + T1[0, i] + ". (1,2): " + T1[1, i] + ". (1,3): " + T1[2, i]);
            Debug.Log("(2,1): " + T1[3, i] + ". (2,2): " + T1[4, i] + ". (2,3): " + T1[5, i]);
            Debug.Log("(3,1): " + T1[6, i] + "" + ". (3,3): " + T1[7, i]);
        }
    }


    //test
    void setUpTextbookProb()
    {
        k = 2; t = 5;
        T1 = new double[k, t];
        T2 = new int[k, t];


        //INPUT: 
        //observation space
        string[] O = { "T", "F" };
        //state space
        string[] S = { "t", "f" };
        //sequence of observations
        int[] Y = { 0, 0, 1, 0, 0 };//, "N", "H", "H]" };
        //transition matrix for right action
        double[,] A_right = { {.7,.3},
            { .3, .7 },
             };
        double[,] A_down = {  { .1, 0, 0, .9, 0, 0, 0, 0 }, //1,1
            { 0, .1, 0, 0, .9, 0, 0, 0 }, //1,2
            { 0, 0, .1, 0, 0, .9, 0, 0 }, //1,3
            { 0, 0, 0, .1, 0, 0, .9, 0 }, //2,1
            { 0, 0, 0, 0, 1, 0, 0, 0 }, //2,2
            { 0, 0, 0, 0, 0, .1, 0, .9 }, //2,3
            { 0, 0, 0, 0, 0, 0, 1, 0 }, //3,1
            { 0, 0, 0, 0, 0, 0, 0, 1 } }; //3,3
        //emission matrix B
        double[,] B = { {.9,.1}, //1,1
            {.2,.8}, //1,2
             }; //3,3
        //array of initial probabilities
        double[] p = { .5, .5 };//, "N", "H", "H]" };
        int[] x = viterbi(O, S, p, Y, A_right, B, A_down);
        for (int i = 0; i < x.Length; i++)
        {
            Debug.Log("state " + i + " path:" + S[x[i]]);

        }
    }


    //pseudocode: https://en.wikipedia.org/wiki/Viterbi_algorithm
    int[] viterbi(string[] O, string[] S, double[] p, int[] Y, double[,] A_right, double[,] B, double[,] A_down)
    {
        double[,] properA;
        int[] X = new int[t + 1];
        for (int i = 0; i < S.Length; i++)
        {
            T1[i, 0] = p[i];
            T2[i, 0] = 0;
        }

        //go through all the timesteps
        for (int i = 1; i < t + 1; i++)
        {
            double normalizeSum = 0;
            if (i >= 0)
            {
                properA = A_down;
            }
            else
            {
                properA = A_right;
            }
            for (int j = 0; j < S.Length; j++)
            {
                double maxT1 = 0;
                for (int a = 0; a < k; a++)
                {
                    double value = T1[a, i - 1] * properA[a, j] * B[j, Y[i - 1]];
                    if (value > maxT1)
                    {
                        maxT1 = value;
                    }
                }
                T1[j, i] = maxT1;

                int maxT2 = 0;
                double maxT2Value = 0;
                for (int a = 0; a < k; a++)
                {
                    double value = T1[a, i - 1] * properA[a, j];
                    if (value > maxT2Value)
                    {
                        maxT2Value = value;
                        maxT2 = a;
                    }
                }
                T1[j, i] = maxT1;
                normalizeSum = normalizeSum + T1[j, i];
                T2[j, i] = maxT2;
            }
            //go through all the probabilities in T1 and normalize.
           
            for (int a=0; a<S.Length; a++)
            {
                T1[a, i]=T1[a,i]/normalizeSum;
            }
        }

        int[] Z = new int[t + 1];

        int maxZT = 0;
        double max2TValue = 0;
        for (int a = 0; a < k; a++)
        {
            double value = T1[a, t];
            if (value > max2TValue)
            {
                max2TValue = value;
                maxZT = a;
            }
        }
        Z[t] = maxZT;
        X[t] = Z[t];
        for (int i = t; i >= 1; i--)
        {
            Z[i - 1] = T2[Z[i], i];
            X[i - 1] = Z[i - 1];
        }
        return X;
    }








}

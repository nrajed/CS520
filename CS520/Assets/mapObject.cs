using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mapObject : MonoBehaviour {

    mapSquare[,] map;
    public Text displaySquareType;

    // Use this for initialization
    void Start () {
        Cursor.visible = true;

        //tempt test
       /* PriorityQueue fringe = new PriorityQueue();
        fringe.Insert(new Vector2(2, 2), 2);
        fringe.Insert(new Vector2(5, 5), 5);
        fringe.Insert(new Vector2(3, 3), 3);
        fringe.Insert(new Vector2(9, 9), 9);
        fringe.Insert(new Vector2(6.9f, 6.9f), 6.9f);
        fringe.Insert(new Vector2(11.2f, 11.2f), 11.2f);
        fringe.Insert(new Vector2(4, 4), 4);
        fringe.Insert(new Vector2(17, 17), 17);
        fringe.Insert(new Vector2(10, 10), 10);
        fringe.Insert(new Vector2(8, 8), 8);

        Vector2 test = fringe.Pop();


        fringe.Remove(new Vector2(6.9f, 6.9f));
        fringe.Remove(new Vector2(3, 3));
        fringe.Remove(new Vector2(11.2f, 11.2f));
        fringe.Remove(new Vector2(4, 4));
        fringe.Remove(new Vector2(2, 2));
        fringe.Remove(new Vector2(60, 60));
        fringe.Remove(new Vector2(17, 17));
        fringe.Remove(new Vector2(10, 10));
        fringe.Remove(new Vector2(5, 5));
        fringe.Remove(new Vector2(9, 9));
        fringe.Remove(new Vector2(8, 8));
        fringe.Remove(new Vector2(8, 8));
        fringe.Remove(new Vector2(8, 3));

        fringe.Insert(new Vector2(2, 2), 2);
         fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();
        fringe.Pop();

        Debug.Log("done");*/
    }
	
	// Update is called once per frame
	void Update () {
        //if raycast hits an object check:
        if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 1000000.0f))
            {
                //figure out point where it hit
                Vector3 point = hit.point;
                int r=(int)Mathf.Round(point.x);
                int c=(int)Mathf.Round(point.z);
                mapSquare[,] map1 = transform.gameObject.GetComponent<loadMap>().map;
                mapSquare[,] map2 = transform.gameObject.GetComponent<makeMap>().map;
                if (map1 != null)
                {
                    map = map1;
                }
                else
                {
                    map = map2;
                }
                displaySquareType.text = ""+map[r, c].type;
            }

        }


       


    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mapObject : MonoBehaviour {

    mapSquare[,] map;
    public Text displaySquareType;

    // Use this for initialization
    void Start () {
        Cursor.visible = true;

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

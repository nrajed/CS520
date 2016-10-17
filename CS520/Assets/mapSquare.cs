using UnityEngine;
using System.Collections;

public class mapSquare : MonoBehaviour {
    //public Vector3 position;
    public int type =0;
    //type:
    //0:blocked
    //1:unblocked
    //2:partially blocked
    //if other types then typeA, typeB also apply

    public int typeHighway = 0;
    //1:horizontal highway
    //2:vertical highway
    //3:_| upper left highway
    //4:|_ upper right highway
    //5:   lower left highway
    //6:   lower right highway

    public int count = 0;



    //FOR A*-----------------------------------------
    public Vector2 parent;
    public float g;
    //---------------------------------------------

    public mapSquare()
    {
        type = 1;
        typeHighway = 0;
        count = 0;

    }
}

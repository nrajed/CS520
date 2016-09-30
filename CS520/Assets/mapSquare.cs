using UnityEngine;
using System.Collections;

public class mapSquare : MonoBehaviour {
    //public Vector3 position;
    public int type =0;
    //type:
    //0:unblocked
    //1:partially blocked
    //2:blocked
    //if other types then typeA, typeB also apply

    public int typeHighway = 0;
    //1:horizontal highway
    //2:vertical highway
    //3:_| upper left highway
    //4:|_ upper right highway
    //5:   lower left highway
    //6:   lower right highway


    //for highways:
    public int typeA = 0;
    public int typeB = 0;

    public mapSquare()
    {
        type = 1;
        typeHighway = 0;
        typeA = 0;
        typeB = 0;
    }
}

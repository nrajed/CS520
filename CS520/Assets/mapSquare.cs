﻿using UnityEngine;
using System.Collections;

public class mapSquare : MonoBehaviour {
	//public Vector3 position;
	public int type =0;
	//type:
	//0:blocked
	//1:unblocked
	//2:partially blocked
	//if other types then typeA, typeB also apply

	public int n;

	public int typeHighway = 0;
	//1:horizontal highway
	//2:vertical highway
	//3:_| upper left highway
	//4:|_ upper right highway
	//5:   lower left highway
	//6:   lower right highway

	public int count = 0;


	//part 2
	public float[] SequentialG;
	public Vector2[] SequentialParent;

    //use for filtering....
    //0 there is a path
    //1 there is no path
    public int path;

	//FOR A*-----------------------------------------
	public Vector2 parent;
	public float g;
    public float f;
    public float h;
    //---------------------------------------------

    public mapSquare()
	{
		type = 1;
		typeHighway = 0;
		count = 0;
		n = 4;
		SequentialG = new float[n+1];
		SequentialParent = new Vector2[n+1];

	}
}
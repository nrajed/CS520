using UnityEngine;
using System.Collections;

public class PriorityQueue : MonoBehaviour {
    //using binary heap

    //has key and value
    //key=float--f-value
    //value=Vector2--vertex

        //vertex s with key x
    //identify or remove entry whose key is lowest(but no other entry)
    
    //      removeMin() -- PoP()
    //C      getMin() -- optional?
    //insert anything you want at any time: any key may be inserted at any time
    //      Insert(value, key)
    //      
    //Removes the value/vertex from priority queue fringe
    //Remove(value)



    //Binary Heap: An implementation of  Priority Queues
    //bnary heap: complete binary tree
    //every level of tree is full except possibly bottom row
    //entries in binary tree satisfy heap-order propery: no child has key less than its parent's key
    //stored in array of entries by level-order traversal

    public ArrayList keys = new ArrayList();
    public ArrayList values = new ArrayList();

    public PriorityQueue()
    {
        keys.Clear();
        keys.Add(0f);
        values.Clear();
        values.Add(0f);
    }

    //size of the fringe
    public int getSize()
    {
        return keys.Count-1;
    }

    //returns the value with minimum key
    //returns -1 if fringe size is 0
    public float getMin()
    {
        if (values.Count != 0)
        {
            return (float)values[0];
        }
        return -1;
    }

    //inserts vertex value with key into priority queue
    public void Insert(Vector2 value, float key)
    {
        //x=(key, value)
        //place x in bottom level of tree at first free spot
        keys.Add(key);
        values.Add(value);

        int position = keys.Count-1;
        int parentPosition = (int)(position / 2);

        while (parentPosition != 0)
        {
            //if hit 1st postion, break because that is always empty
            
            //compare x's key with its parents key
            if ((float)keys[position] < (float)keys[parentPosition])
            {
                //if x's key is less, exchange with parent
                keys[position] = keys[parentPosition];
                values[position] = values[parentPosition];
                keys[parentPosition] = key;
                values[parentPosition] = value;
                position = parentPosition;
                parentPosition = (int)(position / 2);
            }
            else
            {
                break;
            }


            //repeat until x cant move up

        }



    }

    //removes and returns value which has minimum key
    //if cant pop, returns (-1,-1)
    public Vector2 Pop()
    {
        if (values.Count < 2)
        {
            return new Vector2(-1,-1);
        }
        //remove entry at root; save for return value.
        Vector2 minimumValue = (Vector2)values[1];
        float minimumKey= (float)keys[1];

        //fill hole with last entry in tree: x
        values[1] = values[values.Count - 1];
        keys[1]= keys[keys.Count - 1];
        values.RemoveAt(values.Count - 1);
        keys.RemoveAt(keys.Count - 1);

        int position =  1;
        int children1Position = position * 2;
        int children2Position = position * 2+1;

        //bubble x down the heap
        while (children1Position< keys.Count)
        {
            if (children2Position >= keys.Count)
            {
                if ((float)keys[position] > (float)keys[children1Position])
                {
                    //swap x with its minimum child
                    int nextPosition = children1Position;

                    float x = (float)keys[position];
                    Vector2 xv = (Vector2)values[position];
                    keys[position] = keys[nextPosition];
                    values[position] = values[nextPosition];
                    keys[nextPosition] = x;
                    values[nextPosition] = xv;

                    position = nextPosition;
                    break;
                }else
                {
                    break;
                }
            }
            if ((float)keys[position] > (float)keys[children1Position] || (float)keys[position] > (float)keys[children2Position])
            {
                //swap x with its minimum child
                int nextPosition;
                if((float)keys[children1Position]< (float)keys[children2Position])
                {
                    nextPosition = children1Position;
                }else
                {
                    nextPosition = children2Position;
                }

                float x = (float)keys[position];
                Vector2 xv = (Vector2)values[position];
                keys[position] = keys[nextPosition];
                values[position] = values[nextPosition];
                keys[nextPosition] = x;
                values[nextPosition] = xv;

                position = nextPosition;
                children1Position = position * 2;
                children2Position = position * 2 + 1;
            }
            else
            {
                break;
            }
        }



        return minimumValue;
    }

    //removes value from priority queue
    public void Remove(Vector2 value)
    {
        int position = values.IndexOf(value);
        if (position == -1)
        {
            return;
        }
        //fill hole with last entry in tree: x
        values[position] = values[values.Count - 1];
        keys[position] = keys[keys.Count - 1];
        values.RemoveAt(values.Count - 1);
        keys.RemoveAt(keys.Count - 1);

        int children1Position = position * 2;
        int children2Position = position * 2 + 1;

        //bubble x down the heap
        while (children1Position < keys.Count)
        {
            if(children2Position>= keys.Count)
            {
                if ((float)keys[position] > (float)keys[children1Position])
                {
                    //swap x with its minimum child
                    int nextPosition = children1Position;

                    float x = (float)keys[position];
                    Vector2 xv = (Vector2)values[position];
                    keys[position] = keys[nextPosition];
                    values[position] = values[nextPosition];
                    keys[nextPosition] = x;
                    values[nextPosition] = xv;

                    position = nextPosition;
                    break;
                }else
                {
                    break;
                }
            }
            if ((float)keys[position] > (float)keys[children1Position] || (float)keys[position] > (float)keys[children2Position])
            {
                //swap x with its minimum child
                int nextPosition;
                if ((float)keys[children1Position] < (float)keys[children2Position])
                {
                    nextPosition = children1Position;
                }
                else
                {
                    nextPosition = children2Position;
                }

                float x = (float)keys[position];
                Vector2 xv = (Vector2)values[position];
                keys[position] = keys[nextPosition];
                values[position] = values[nextPosition];
                keys[nextPosition] = x;
                values[nextPosition] = xv;

                position = nextPosition;
                children1Position = position * 2;
                children2Position = position * 2 + 1;
            }
            else
            {
                break;
            }
        }
    }




}

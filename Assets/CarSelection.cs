using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelection : MonoBehaviour
{
    private GameObject[] carList;

    private void Start()
    {
        carList = new GameObject[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            carList[i] = transform.GetChild(i).gameObject;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCity : MonoBehaviour
{
    [SerializeField] string cityName;

    private void OnValidate()
    {
        var tm = GetComponentInChildren<TextMesh>();
        if (tm) 
            tm.text = cityName;

        name = cityName;
    }



}

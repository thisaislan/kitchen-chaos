using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{

    public bool IsFirstUpdate = true;

    void Update()
    {
        if (IsFirstUpdate)
        { 
            IsFirstUpdate = false;
            Loader.LoaderCallback();
        }
    }

}

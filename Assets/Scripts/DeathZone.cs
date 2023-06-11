using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IMagicObjectDirector temp = other.GetComponent<IMagicObjectDirector>();
        if(temp != null )
        {
            temp.InstaDestory();
        }
        else
        {
            Destroy( other.gameObject );
        }
    }
}

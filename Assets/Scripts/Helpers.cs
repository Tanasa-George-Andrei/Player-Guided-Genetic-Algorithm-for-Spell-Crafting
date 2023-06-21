using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Helpers
{
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        if(WaitDictionary.TryGetValue(time, out WaitForSeconds wait)) return wait;
        
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    public static readonly System.Random random = new System.Random(DateTime.Now.Millisecond);

    public static int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    public static float Range(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    //Leave the Create projectile with a grace period
    //private static Collider[] collisionsIntersect = new Collider[256];
    //public static bool CheckIfCollidersIntersect(Collider _target, Collider _obstruction)
    //{
    //    int hits = UnityEngine.Physics.OverlapSphereNonAlloc(_target.bounds.center, (_target.bounds.center - _target.bounds.max).magnitude, collisionsIntersect);
    //    for (int i = 0; i < hits; i++)
    //    {
    //        if (collisionsIntersect[i] == _obstruction)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    public static bool CheckExplosionObstruction(Collider _col, Vector3 _center, LayerMask _mask)
    {
        RaycastHit hit;
        return Physics.Raycast(_center, (_col.bounds.center - _center).normalized, out hit, (_col.bounds.center - _center).magnitude, _mask);
    }

    public static string WriteMagicHistory(SpellHistoryNode _node)
    {
        StringBuilder sb = new StringBuilder();
        SpellHistoryNode expNode = _node;
        while (expNode != null)
        {
            sb.AppendLine(expNode.target.GetName());
            if(expNode.target.GetActualDirector() != null)
            {
                sb.Append(" - ");
                sb.AppendLine(expNode.target.GetActualDirector().GetName());
            }
            expNode = expNode.prev;
        }
        return sb.ToString();
    }

}

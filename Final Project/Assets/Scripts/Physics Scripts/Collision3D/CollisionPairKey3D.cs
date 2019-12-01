using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;



public class CollisionPairKey3D
{
    private CollisionHullType3D[] _collTypes = new CollisionHullType3D[2];
    public CollisionPairKey3D(CollisionHullType3D collTypeA, CollisionHullType3D collTypeB)
    {

        if (collTypeA <= collTypeB)
        {
            _collTypes[0] = collTypeA;
            _collTypes[1] = collTypeB;
        }
        else
        {
            _collTypes[0] = collTypeB;
            _collTypes[1] = collTypeA;
        }
    }

    public class EqualityComparitor: IEqualityComparer<CollisionPairKey3D>
    {
        public bool Equals(CollisionPairKey3D x, CollisionPairKey3D y)
        {
            return x._collTypes[0] == y._collTypes[0] && x._collTypes[1] == y._collTypes[1];
        }

        public int GetHashCode(CollisionPairKey3D obj)
        {
            return (int)obj._collTypes[0] ^ (int)obj._collTypes[1];
        }
    }
}

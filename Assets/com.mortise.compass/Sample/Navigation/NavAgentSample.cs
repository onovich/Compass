using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class NavAgentSample : MonoBehaviour {

        public int ID;
        public bool IsStop;

        [SerializeField] Transform target;
        public Transform Target => target;
        public void SetTarget(Transform value) => target = value;

        Compass2D compass;
        public Compass2D Compass => compass;
        public void SetCompass(Compass2D value) => compass = value;

        Map2D map;
        public Map2D Map => map;
        public void SetMap(Map2D value) => map = value;

        Node2DPool node2DPool;
        public Node2DPool Node2DPool => node2DPool;
        public void SetNode2DPool(Node2DPool value) => node2DPool = value;

        List<Node2D> path;
        public List<Node2D> Path => path;
        public void SetPath(List<Node2D> value) => path = value;

    }

}
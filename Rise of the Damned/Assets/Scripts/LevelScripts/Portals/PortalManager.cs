using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField]
    public static PortalSet portals;

    public class Portal
    {
        public float xvalue;
        public float yvalue;
    }
    [System.Serializable]
    public class PortalSet
    {
        //will be ordered by y value
        public List<Portal> bottomportals;
        public List<Portal> topportals;

    }

    void Awake()
    {
        portals = new PortalSet();
        portals.bottomportals = new List<Portal>();
        portals.topportals = new List<Portal>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

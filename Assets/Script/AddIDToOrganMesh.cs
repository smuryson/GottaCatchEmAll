using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIDToOrganMesh : MonoBehaviour
{
    [SerializeField]
    public GameObject mesh;
    [SerializeField]
    public Organ organ;

    public int iD = 0;
    // Start is called before the first frame update
    void Start()
    {
        iD = organ.iD;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

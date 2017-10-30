using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

    public GameObject hitParticle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Shot Destroyer") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Terrain"))
        {
            Instantiate(hitParticle, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}

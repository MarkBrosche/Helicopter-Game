using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingController))]
public class OutpostController : MonoBehaviour {

    public Transform  smoke, gun, bulletSpawn;
    public GameObject player, explosion, enemy, smoking;
    private float lastShot = 0.0f;

    // Adjustable variables for setting the valid target area and rate of fire.
    public float range = 100f, gunSwivel=0, gunTilt=0, swivelMin = -60, swivelMax = 60, tiltMin = 250, tiltMax = 300;

    // These variables govern the life and sound effects of the enemy
    private int _life = 3;
    public AudioClip crack, collapse, shot;
    private AudioSource _crack, _collapse, _shot;
    private float dista;
    private ShootingController _gunner;

    private void Awake()
    {
        _gunner = GetComponent<ShootingController>();
        _crack = GetComponent<AudioSource>();
        _collapse = GetComponent<AudioSource>();
        _shot = GetComponent<AudioSource>();
    }    

    // Use this for initialization
    void Start () {
        SetLife();
    }
	
	// Update gets called every frame
    void Update()
    {
        // Aim the enemy's gun at the player within natural limitations of movement.        
        _gunner.Aim(gunSwivel, gunTilt, swivelMin, swivelMax, tiltMin, tiltMax, player, gun, range);

        // Get the distance between the enemy and player
        if (player == isActiveAndEnabled)
        {
            dista = Vector3.Distance(enemy.transform.position, player.transform.position);   
        }

    }

	void FixedUpdate()
    {
        // Set the distance from which the enemy can fire upon the player
        if (player == isActiveAndEnabled && (dista <= range))
        {
            _gunner.Fire(bulletSpawn);           
        }
  	}

    // Enemy object takes damage on collsion with bullets
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {            
            _crack.PlayOneShot(crack, 0.5f);
            _life--;
            SetLife();
        }
    }
    private void SetLife()
    {
        if (_life <= 0)
        {
            Instantiate(explosion, enemy.transform.localPosition, enemy.transform.localRotation);
            _collapse.PlayOneShot(collapse, 2.0f);
            Instantiate(smoking, smoke.transform.position, smoke.transform.rotation);
            Destroy(this.gameObject, 2f);
        }
    }
}

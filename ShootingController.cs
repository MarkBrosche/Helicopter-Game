using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour {

    public GameObject ammunition, hitParticle, muzFlashParticle;
    public AudioClip shot;
    private AudioSource _shot;
    private Rigidbody clone, clone2;
    private float lastShot = 0.0f, lastShot2 = 0.0f, dist;
    public float speed = 100.0f, frequency=5.0f;

    // Initialize sounds in Awake
    void Awake()
    {
        _shot = GetComponent<AudioSource>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Fire is called in FixedUpdate()
    public void Fire(Transform spawn)
    {
            if (Time.fixedTime > (1 / frequency + lastShot))
            {
                clone = Instantiate(ammunition.GetComponent<Rigidbody>(), spawn.transform.position, spawn.transform.rotation) as Rigidbody;
                clone.velocity = spawn.transform.TransformDirection(-Vector3.up * speed);
                Instantiate(muzFlashParticle, spawn.transform.position, spawn.transform.rotation);
                _shot.PlayOneShot(shot);
                lastShot = Time.fixedTime;
            }
    }

    // Fire is called in FixedUpdate()
    public void FireTank(Transform spawn)
    {
        if (Time.fixedTime > (1 / frequency + lastShot))
        {
            clone = Instantiate(ammunition.GetComponent<Rigidbody>(), spawn.transform.position, spawn.transform.rotation) as Rigidbody;
            clone.velocity = spawn.transform.TransformDirection(Vector3.forward * speed);
            Instantiate(muzFlashParticle, spawn.transform.position, spawn.transform.rotation);
            _shot.PlayOneShot(shot);
            lastShot = Time.fixedTime;
        }
    }

    // Fire2() is for firing both guns in the helicopter
    public void Fire2(Transform spawn, Transform spawn2)
    {
        if (Time.fixedTime > (1 / frequency + lastShot))
        {
            clone = Instantiate(ammunition.GetComponent<Rigidbody>(), spawn.transform.position, spawn.transform.rotation) as Rigidbody;
            clone.velocity = spawn.transform.TransformDirection(-Vector3.up * speed);
            Instantiate(muzFlashParticle, spawn.transform.position, spawn.transform.rotation);
            _shot.PlayOneShot(shot);
            clone2 = Instantiate(ammunition.GetComponent<Rigidbody>(), spawn2.transform.position, spawn2.transform.rotation) as Rigidbody;
            clone2.velocity = spawn2.transform.TransformDirection(-Vector3.up * speed);
            Instantiate(muzFlashParticle, spawn2.transform.position, spawn2.transform.rotation);
            _shot.PlayOneShot(shot);
            lastShot2 = Time.fixedTime;
        }
    }

    // Call Aim() in Update() - This function is for enemy guns
    public void Aim(float swivel, float tilt, float swivelMin, float swivelMax, float tiltMin, float tiltMax, GameObject target, Transform gun, float range)
    {
        if (target == isActiveAndEnabled)
        {
             dist = Vector3.Distance(gun.position, target.transform.position);
        }            
        if(target == isActiveAndEnabled && (dist <= range))
        {
            gun.LookAt(target.transform.position);

            swivel = gun.localEulerAngles.x;
            swivel = Mathf.Clamp(swivel, swivelMin, swivelMax);
            tilt = gun.localEulerAngles.y;
            tilt = Mathf.Clamp(tilt, tiltMin, tiltMax);
            gun.localEulerAngles = new Vector3(swivel, tilt, 0);
        }
    }
    // Call Aim() in Update() - This overload is for tank gun
    public void Aim(float tilt, float tiltMin, float tiltMax, GameObject target, Transform gun, Transform turret,float range)
    {
        if (target == isActiveAndEnabled)
        {
            dist = Vector3.Distance(gun.position, target.transform.position);
        }
        if (target == isActiveAndEnabled && (dist <= range))
        {
            gun.LookAt(target.transform.position);
            turret.LookAt(new Vector3(target.transform.position.x, 0, target.transform.position.z));

            tilt = gun.localEulerAngles.x;
            //tilt = Mathf.Clamp(tilt, tiltMin, tiltMax);
            gun.localEulerAngles = new Vector3(tilt, 0, 0);
        }
    }

    // Call Aim() in Update() - This overload is for the helicopter only
    public void Aim(float _portSwivel, float _portTilt, float _stbdSwivel, float _stbdTilt, float portSwivelMin, float portSwivelMax, float stbdSwivelMin, float stbdSwivelMax, float tiltMin, float tiltMax,
        Transform portGun, Transform stbdGun, Transform reticle, Transform portGunSwivel, Transform  stbdGunSwivel, Camera camera)
    {
        Vector3 point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Ray ray = camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            portGun.LookAt(hit.point);
            stbdGun.LookAt(hit.point);
            //draw a target on whatever the guns are aiming at
            reticle.position = hit.point + Vector3.up;

            _portSwivel = portGun.localEulerAngles.y;
            _portSwivel = Mathf.Clamp(_portSwivel, portSwivelMin, portSwivelMax);
            _portTilt = portGun.localEulerAngles.x;
            _portTilt = Mathf.Clamp(_portTilt, tiltMin, tiltMax);
            portGunSwivel.localEulerAngles = new Vector3(0, _portSwivel, 0);
            portGun.localEulerAngles = new Vector3(_portTilt, _portSwivel, 0);

            _stbdSwivel = stbdGun.localEulerAngles.y;
            _stbdSwivel = Mathf.Clamp(_stbdSwivel, stbdSwivelMin, stbdSwivelMax);
            _stbdTilt = stbdGun.localEulerAngles.x;
            _stbdTilt = Mathf.Clamp(_stbdTilt, tiltMin, tiltMax);
            stbdGunSwivel.localEulerAngles = new Vector3(0, _stbdSwivel, 0);
            stbdGun.localEulerAngles = new Vector3(_stbdTilt, _stbdSwivel, 0);
        }
    }
}

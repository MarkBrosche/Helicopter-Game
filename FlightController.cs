using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ShootingController))]
public class FlightController : MonoBehaviour {
    public float throttleSpeed = 0f, portSwivelMin = 240, portSwivelMax =330, stbdSwivelMin = 30, stbdSwivelMax= 120, tiltMin = -36, tiltMax = 50;
    public GameObject explosion, blackHawk, mainRotor, tailRotor, portGun, stbdGun, finish, followCam, pilotCam;
    public Transform portGunSwivel, stbdGunSwivel, portGunTilt, stbdGunTilt, reticle, bulletSpawn1, bulletSpawn2;
    [SerializeField] private Rigidbody _fuselage;

    [SerializeField] public AudioClip ding, boom, propeller, rescue;
    private AudioSource _ding, _boom, _propeller, _rescue, _music;
    [SerializeField] public Text lifeText, winText, missionText;
    private float _portSwivel, _stbdSwivel, _portTilt, _stbdTilt;
    [SerializeField] public Camera _camera;
    private ShootingController _gun;
    private int _life = 20;

    // Awake is called before Update() and initializes audio and class inheritance
    void Awake()
    {
        _music = GameObject.Find("Game Controller").GetComponent<AudioSource>();
        _gun = GetComponent<ShootingController>();
        _propeller = gameObject.AddComponent<AudioSource>();
        _propeller.playOnAwake = false;
        _propeller.clip = propeller;
        _propeller.loop = true;
        _propeller.pitch = 0f;

        _ding = gameObject.AddComponent<AudioSource>();
        _ding.playOnAwake = false;
        _ding.clip = ding;

        _boom = gameObject.AddComponent<AudioSource>();
        _boom.playOnAwake = false;
        _boom.clip = boom;

        _rescue = gameObject.AddComponent<AudioSource>();
        _rescue.playOnAwake = false;
        _rescue.clip = rescue;

        Update();

        _propeller.Play();
    }

    // Use this for initialization
    void Start () {
         _fuselage = blackHawk.GetComponent<Rigidbody>();
        SetLife();
    }

    // Update is called once per frame
    void Update()
    {
   
        _gun.Aim(_portSwivel, _portTilt, _stbdSwivel, _stbdTilt, portSwivelMin, portSwivelMax, stbdSwivelMin, stbdSwivelMax, tiltMin, tiltMax, portGun.transform, stbdGun.transform, reticle, portGunSwivel, stbdGunSwivel, _camera);

        SpinRotors(throttleSpeed);
    }

    // Fixed Update is called at fixed intervals for physics calculations
    void FixedUpdate ()
    {
        // Gun Controls ////////////////////////////////////////////////////////
        // Fire port gun
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _gun.Fire(bulletSpawn1);
        }

        // fire starboard gun
        if (Input.GetKey(KeyCode.Mouse1))
        {
            _gun.Fire(bulletSpawn2);
        }

        // Fire both guns
        if (Input.GetKey(KeyCode.Space))
        {
            _gun.Fire2(bulletSpawn1, bulletSpawn2);
        }

        // Flight Controls:////////////////////////////////////////////////////////////////////////////////////////////
        // Throttle Speed up
        if (Input.GetKey(KeyCode.Z))
        {
            throttleSpeed += 500 * Time.deltaTime;
            _propeller.pitch += 0.05f;
        }

        //Throttle Speed down
        if (Input.GetKey(KeyCode.C))
        {
            throttleSpeed -= 500 * Time.deltaTime;
            _propeller.pitch -= 0.1f;
        }

        // Make helicopter stable hover once enough lift is achieved from throttle up
        if (throttleSpeed >= 1000)
        {
            _fuselage.AddRelativeForce(Vector3.up *_fuselage.mass * Mathf.Abs(Physics.gravity.y));
        }

        // Cyclic Lateral Left (Strafe and Roll Left)
        if (Input.GetKey(KeyCode.A))
        {
            _fuselage.AddRelativeForce(Vector3.left * Time.fixedDeltaTime * throttleSpeed, ForceMode.Acceleration);
                //    _fuselage.AddRelativeTorque(Vector3.forward * Time.deltaTime * throttleSpeed, ForceMode.Impulse);
        }

        // Cyclic Lateral Right (Strafe and Roll Right)
        if (Input.GetKey(KeyCode.D))
        {
            _fuselage.AddRelativeForce(Vector3.right * Time.fixedDeltaTime * throttleSpeed, ForceMode.Acceleration);
                //    _fuselage.AddRelativeTorque(Vector3.back * Time.deltaTime * throttleSpeed, ForceMode.Impulse);
        }

        // Cyclic Longitudinal Pitch Forward
        if (Input.GetKey(KeyCode.W))
        {
            _fuselage.AddRelativeForce(Vector3.forward * Time.fixedDeltaTime * throttleSpeed, ForceMode.Acceleration);
                //    _fuselage.AddRelativeTorque(Vector3.right * Time.deltaTime * throttleSpeed, ForceMode.Acceleration);
        }

        // Cyclic Longitudinal Pitch Backward
        if (Input.GetKey(KeyCode.S))
        {
            _fuselage.AddRelativeForce(Vector3.back * Time.fixedDeltaTime * throttleSpeed, ForceMode.Acceleration);
                //    _fuselage.AddRelativeTorque(Vector3.left * Time.deltaTime * throttleSpeed, ForceMode.Acceleration);
        }

        // Collective Up
        if (Input.GetKey(KeyCode.UpArrow) && throttleSpeed > 1000)
        {
            _fuselage.AddForce(Vector3.up * throttleSpeed, ForceMode.Impulse);
        }

        // Collective Down
        if (Input.GetKey(KeyCode.DownArrow) && throttleSpeed > 0)
        {
            _fuselage.AddForce(Vector3.down * throttleSpeed, ForceMode.Impulse);
        }        
        
        // Anti-Torque Pedal Left (rotate left)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _fuselage.AddRelativeTorque(Vector3.down * 0.5f * throttleSpeed,ForceMode.Impulse);
        }

        // Anti-Torque Pedal right (rotate right)
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _fuselage.AddRelativeTorque(Vector3.up * 0.5f * throttleSpeed, ForceMode.Impulse);
        }
    }

    // SpinRotors() governs the main and tail rotors of the helicopter 
    // including max throttle and engine pitch.
    private void SpinRotors(float throttleSpeed)
    {
        throttleSpeed = Mathf.Clamp(throttleSpeed, 0, 2000);
        mainRotor.transform.Rotate(throttleSpeed * Vector3.up);
        tailRotor.transform.Rotate(throttleSpeed * Vector3.left);
        _propeller.pitch = Mathf.Clamp(_propeller.pitch, 0, 1);
    }

    void OnCollisionEnter(Collision other)
    {
        // Helicopter Damage
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Shell"))
        {
            _ding.PlayOneShot(ding, 0.5f);
            _life--;
            SetLife();
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            missionText.text = "Objective: Return to Base";
            Destroy(other.gameObject);
            //other.gameObject.SetActive(false);
            finish.SetActive(true);
            _rescue.PlayOneShot(rescue);
            
        }
        if (other.gameObject.CompareTag("Finish"))
        {
            winText.text = "Mission Complete! \n Bravo Zulu!";
            Time.timeScale = 0;
            _music.Pause();
        }
    }

    // SetLifeText holds and updates the UI with remaining Tank HP and destroys tank if <= 0.
    private void SetLife()
    {
        lifeText.text = "Health: " + _life.ToString();
        if (_life <= 0)
        {
            followCam.transform.SetParent(null);
            pilotCam.transform.SetParent(null);
            Instantiate(explosion, blackHawk.transform.localPosition, blackHawk.transform.localRotation);
            _boom.PlayOneShot(boom, 2.0f);
            throttleSpeed = 0;
            Destroy(this.gameObject, 2f);
            //blackHawk.SetActive(false);
        }
    }
}

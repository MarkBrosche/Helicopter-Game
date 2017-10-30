using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof (ShootingController))]
public class TankController : MonoBehaviour {

    public float tankSpeed = 10f, obstacleRange = 30f, shellRange = 100;
    public GameObject explosion, tank, turret, lfWheel, lSteer, rfWheel, rSteer, lbWheel, rbWheel, cannon, player;
    public float range = 100f, gunSwivel = 0, gunTilt = 0, swivelMin = -60, swivelMax = 60, tiltMin = 250, tiltMax = 300;
    public Transform shellSpawn;

    private float _frontWheelRadius = 100f, _backWheelRadius = 150f, _steerY = 0f, _cannonX = 0f, dista;
    private Vector3 _tankY;

    private int _life = 3;
    public AudioClip ding, boom;
    private AudioSource _ding, _boom;

    private ShootingController _shellcontrol;

    // Function used in setting up audio.
    private void Awake()
    {
        _ding = gameObject.AddComponent<AudioSource>();
        _ding.clip = ding;
        _ding.playOnAwake = false;
        _boom = gameObject.AddComponent<AudioSource>();
        _boom.clip = boom;
        _boom.playOnAwake = false;
        _shellcontrol = GetComponent<ShootingController>();
    }

    // Use this for initialization
    void Start () {
        SetLife();        
    }

    // Update is called once per frame
    void Update()
    {
        //MoveForward();

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.SphereCast(ray, 15f, out hit))
        {
            float angle = Random.Range(30, 110);
            Turn(angle);
        }
        // Aim the enemy's gun at the player within natural limitations of movement.        
        _shellcontrol.Aim(gunTilt, tiltMin, tiltMax, player, cannon.transform, turret.transform, range);

        // Get the distance between the enemy and player
        if (player == isActiveAndEnabled)
        {
            dista = Vector3.Distance(tank.transform.position, player.transform.position);
        }

    }

    void FixedUpdate()
    {
        // Set the distance from which the enemy can fire upon the player
        if (player == isActiveAndEnabled && (dista <= range))
        {
            _shellcontrol.FireTank(shellSpawn);
        }
    }

    // OnCollisionEnter is called when the tank collider makes contact with another gameObject's collider
    void OnCollisionEnter(Collision other)
    {
        // Tank Damage
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Shell"))
        {
            //Destroy(other.gameObject);
            _ding.PlayOneShot(ding, 0.5f);
            _life--;
            SetLife();
        }
    }

    // SetLifeText holds and updates the UI with remaining Tank HP and destroys tank if <= 0.
    private void SetLife()
    {
        if (_life <= 0)
        {
            Instantiate(explosion, tank.transform.localPosition, tank.transform.localRotation);
            _boom.PlayOneShot(boom, 2.0f);
            Destroy(tank.gameObject, 2f);
        }           
    }


    // Function that converts tank speed to wheel rotation speed.
    float RotSpeed(float radius)
    {
        return tankSpeed / (radius * 3.14159f) * 180;
    }

    // Turns the front wheels to the left and rotates the tank body
    private void Turn(float angle)
    {
        _steerY = Mathf.Clamp(angle, -45, 45);
        rSteer.transform.localEulerAngles = Vector3.RotateTowards(rSteer.transform.localEulerAngles, new Vector3(0, _steerY, 0), tankSpeed * Time.deltaTime, 0.0f);
        lSteer.transform.localEulerAngles = Vector3.RotateTowards(lSteer.transform.localEulerAngles, new Vector3(0, _steerY, 0), tankSpeed * Time.deltaTime, 0.0f);
        tank.transform.localEulerAngles = Vector3.RotateTowards(Vector3.forward, new Vector3(0, angle, 0), tankSpeed , 0.0f);
    }

    //// Turns the front wheels to the right and rotates the tank body
    //private void TurnRight()
    //{
    //    _steerY += 3;
    //    _steerY = Mathf.Clamp(_steerY, -45, 45);
    //    rightSteer.transform.localEulerAngles = new Vector3(0, _steerY, 0);
    //    leftSteer.transform.localEulerAngles = new Vector3(0, _steerY, 0);
    //}

    // Turns the wheels and translates the tank forward
    private void MoveForward()
    {
        tank.transform.Translate(Vector3.forward * tankSpeed * Time.deltaTime, Space.Self);
        lfWheel.transform.Rotate(Vector3.right, RotSpeed(_frontWheelRadius));
        rfWheel.transform.Rotate(Vector3.right, RotSpeed(_frontWheelRadius));
        lbWheel.transform.Rotate(Vector3.right, RotSpeed(_backWheelRadius));
        rbWheel.transform.Rotate(Vector3.right, RotSpeed(_backWheelRadius));
        //if (_steerY != 0)
        //{
        //    _tankY = new Vector3(0, _steerY * Time.deltaTime, 0); // Steer to turn ratio: 0.4478f
        //    tank.transform.Rotate(_tankY);
        //}
    }

    // Turns the wheels and tranlates the tank backward
    private void MoveBackward()
    {
        tank.transform.Translate(Vector3.back * tankSpeed * Time.deltaTime, Space.Self);
        lfWheel.transform.Rotate(Vector3.left, RotSpeed(_frontWheelRadius));
        rfWheel.transform.Rotate(Vector3.left, RotSpeed(_frontWheelRadius));
        lbWheel.transform.Rotate(Vector3.left, RotSpeed(_backWheelRadius));
        rbWheel.transform.Rotate(Vector3.left, RotSpeed(_backWheelRadius));
        if (_steerY != 0)
        {
            _tankY = new Vector3(0, -_steerY * Time.deltaTime, 0); // Steer to turn ratio: 0.4478f
            tank.transform.Rotate(_tankY);
        }
    }

    // Turns the turret to the left
    private void RotateTurretLeft()
    {
        turret.transform.Rotate(Vector3.down, 30 * Time.deltaTime);
    }

    // Turns the turret to the right
    private void RotateTurretRight()
    {
        turret.transform.Rotate(Vector3.up, 30 * Time.deltaTime);
    }

    // raises the tank cannon
    private void RaiseCannon()
    {
        _cannonX--;
        _cannonX = Mathf.Clamp(_cannonX, -90, 0);
        cannon.transform.localEulerAngles = new Vector3(_cannonX, 0, 0);
    }

    // lowers the tank cannon
    private void LowerCannon()
    {
        _cannonX++;
        _cannonX = Mathf.Clamp(_cannonX, -90, 0);
        cannon.transform.localEulerAngles = new Vector3(_cannonX, 0, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameObject followCam, overheadCam, pilotCam, blackHawk, menu;
    public Transform fuselage, followPoint;
    public AudioClip lose, win, music;
    private AudioSource _lose, _win, _music;
    public Text loseText, startText;
    private Vector3 offset, desiredPosition;
    private float angularSpeed;
    private int _pause = 0;

    // Use this for initialization
    void Start()
    {
        offset = followCam.transform.position-fuselage.transform.position;
        angularSpeed = blackHawk.GetComponent<Rigidbody>().angularVelocity.y;
        _music.PlayDelayed(14f);
        _music.volume = 0.5f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if ((overheadCam == isActiveAndEnabled) && (blackHawk == isActiveAndEnabled))
        {
            overheadCam.transform.position = new Vector3(fuselage.transform.position.x, overheadCam.transform.position.y, fuselage.transform.position.z);
        }

        if ((followCam == isActiveAndEnabled) && (blackHawk == isActiveAndEnabled))
        {
            followCam.transform.position = followPoint.transform.position;
            followCam.transform.LookAt(fuselage);
        }

        // Camera Selections /////////////////////////////////////////
        // '1' pilot / 1st person view
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Pilot Camera Selected");
            overheadCam.SetActive(false);
            followCam.SetActive(false);
            pilotCam.SetActive(true);
        }

        //'2' Camera follows behind the helicopter / 3rd person view
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Follow Camera Selected");
            followCam.SetActive(true);
            overheadCam.SetActive(false);
            pilotCam.SetActive(false);
        }

        // '3' Overhead / map view
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Overhead Camera Selected");
            followCam.SetActive(false);
            overheadCam.SetActive(true);
            pilotCam.SetActive(false);
        }
    }


     void Update()
    {
        // Remove intro text
        if (Input.anyKey)
        {
            //yield return new WaitForSeconds(2.0f);
            startText.text = "";
        }

        // Pause game and view controls
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");
            switch (_pause)
            {
                case 0:
                    Time.timeScale = 0;
                    menu.SetActive(true);
                    _pause = 1;

                    break;
                case 1:
                    Time.timeScale = 1;
                    menu.SetActive(false);
                    _pause = 0;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Debug.Log("User wants to quit.");
            Application.Quit();
        }

        //if lose game, press 'r' to replay or 'Esc' to quit
        if (blackHawk == null)
        {
            Debug.Log("Mission Failure");
            _music.Stop();
            _lose.PlayOneShot(lose);
            loseText.text = "Try again (Press 'r') \n Give Up (Press 'Del')";
        }

        //Reload game for any reason, press 'r'
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    private void Awake()
    {
        _lose = gameObject.AddComponent<AudioSource>();
        _lose.clip = lose;
        _lose.playOnAwake = true;
        _win = gameObject.AddComponent<AudioSource>();
        _music = gameObject.AddComponent<AudioSource>();
        _music.playOnAwake = true;
        _music.loop = true;
        _music.clip = music;

    }

}

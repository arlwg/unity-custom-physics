using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class UIController : Physics_Object
{
    [Header("UI Controls")]
    public GameObject panel;
    public Toggle gravityCheckBox;
    public Slider gravityScaleSlider;
    public InputField gravityScaleInputField;

    public GameObject activeObject;

    public Vector3 startPos1;
    public Vector3 startPos2;
    public Vector3 startPos3; 
    public Vector3 startPos4;
    public Vector3 startPos5;
    public Vector3 startPos6;

    public Vector3 pingpong;

    public Vector3 bowlingball;

    public Vector3 baseball;

    public Vector3 basketball;


    public Physics_Object objectV;
    public List<Physics_Object> physicsObjects = new List<Physics_Object>();

    private Vector3 StartPosition = new Vector3(-8.48000002f, 10.76999998f, -6.6100006f);
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        gravityScaleInputField.text = gravityScaleSlider.value.ToString();

        this.startPos1 = GameObject.Find("WoodenBlock").transform.position;
        this.startPos2 = GameObject.Find("WoodenBlock1").transform.position;
        this.startPos3 = GameObject.Find("WoodenBlock2").transform.position;
        this.startPos4 = GameObject.Find("WoodenBlock3").transform.position;
        this.startPos5 = GameObject.Find("WoodenBlock4").transform.position;
        this.startPos6 = GameObject.Find("WoodenBlock5").transform.position;
       this.pingpong = GameObject.Find("Ping Pong Ball").transform.position;
       this.baseball = GameObject.Find("Baseball").transform.position;
       this.basketball = GameObject.Find("Basketball").transform.position;
       this.bowlingball = GameObject.Find("Bowling Ball").transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            panel.SetActive(!panel.activeInHierarchy); // toggle

            Cursor.lockState = (panel.activeInHierarchy) ? CursorLockMode.None : CursorLockMode.Locked;
        }

    }

    public void OnOKButtonPressed()
    {
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnGravityToggled()
    {
        Debug.Log(gravityCheckBox.isOn ? "Gravity is On" : "Gravity is Off");

    }

    public void OnGravityScaleSliderValueChanged()
    {
     
    }

    public void OnGravityScaleTextFieldValueChanged()
    {
        gravityScaleSlider.value = Single.TryParse(gravityScaleInputField.text, out var number) ? number : 0.0f;
    }

    public void PingPongBall()
    {
        Debug.Log("PingPongBall Activate");
        this.activeObject = GameObject.Find("Ping Pong Ball");
        
        activeObject.transform.SetPositionAndRotation(this.StartPosition, this.activeObject.transform.rotation) ;
        this.objectV = activeObject.GetComponent<Physics_Object>();

        this.objectV.velocity = new Vector3(0, 0, 5);
    }
    public void BowlingBall()
    {
        Debug.Log("BowlingBall Activate");
        this.activeObject = GameObject.Find("Bowling Ball");
        activeObject.transform.SetPositionAndRotation(this.StartPosition, this.activeObject.transform.rotation) ;
        this.objectV = activeObject.GetComponent<Physics_Object>();

        this.objectV.velocity = new Vector3(0, 0, 5);
    }
    public void Baseball()
    {
        Debug.Log("Baseball Activate");
        this.activeObject = GameObject.Find("Baseball");
        activeObject.transform.SetPositionAndRotation(this.StartPosition, this.activeObject.transform.rotation) ;
        this.objectV = activeObject.GetComponent<Physics_Object>();

        this.objectV.velocity = new Vector3(0, 0, 5);
    }
    public void Basketball()
    {
        Debug.Log("Basketball Activate");
        this.activeObject = GameObject.Find("Basketball");
        activeObject.transform.SetPositionAndRotation(this.StartPosition, this.activeObject.transform.rotation) ;
        this.objectV = activeObject.GetComponent<Physics_Object>();

        this.objectV.velocity = new Vector3(0, 0, 5);
    }

    public void ResetBlocks()
    {

        GameObject.Find("WoodenBlock").transform.position = this.startPos1;
        this.objectV = GameObject.Find("WoodenBlock").GetComponent<Physics_Object>();
        this.objectV.velocity = Vector3.zero;

        GameObject.Find("WoodenBlock1").transform.position = this.startPos2;
        this.objectV = GameObject.Find("WoodenBlock1").GetComponent<Physics_Object>();
        this.objectV.velocity = Vector3.zero;
       GameObject.Find("WoodenBlock2").transform.position = this.startPos3;
       this.objectV = GameObject.Find("WoodenBlock2").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
       GameObject.Find("WoodenBlock3").transform.position = this.startPos4;
       this.objectV = GameObject.Find("WoodenBlock3").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
      GameObject.Find("WoodenBlock4").transform.position  = this.startPos5;
      this.objectV = GameObject.Find("WoodenBlock4").GetComponent<Physics_Object>();
      this.objectV.velocity = Vector3.zero;
       GameObject.Find("WoodenBlock5").transform.position = this.startPos6;
       this.objectV = GameObject.Find("WoodenBlock5").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
       GameObject.Find("Ping Pong Ball").transform.position = this.pingpong;
       this.objectV = GameObject.Find("WoodenBlock5").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
       GameObject.Find("Baseball").transform.position = this.baseball;
       this.objectV = GameObject.Find("WoodenBlock5").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
       GameObject.Find("Basketball").transform.position = this.basketball;
       this.objectV = GameObject.Find("WoodenBlock5").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
       GameObject.Find("Bowling Ball").transform.position = this.bowlingball;
       this.objectV = GameObject.Find("WoodenBlock5").GetComponent<Physics_Object>();
       this.objectV.velocity = Vector3.zero;
    }

    //Vector3(4.53117895,11.6386089,-32.2957077)
    //      5.77          3.8         -15

    //Vector3(-1.24000001,7.84000015,-17.4400005)
}

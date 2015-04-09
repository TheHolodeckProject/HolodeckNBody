using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LookControllerScript : MonoBehaviour {

    public GameObject NBodyDemoObject;
    private NBodyDemoMode demo;
    public Camera lookCamera;
    private GBody targetBody;
    private GameObject targetBodyObject;

    private float startMass;
    private float lookMass;
    public float massScaling = 1000;
    private Vector3 startScale;
    private Vector3 lookScale;
    public float sizeScaling = 4f;
    private ParticleSystem particles;

    public float lookRadius = 0.2f;
    private bool firstUpdate = true;
    public KeyCode resetKey = KeyCode.R;
    private bool isLooking = false;

    public GameObject accuracySlider;
    private Slider score;
    public Bounds scoreBounds;
    private NBodySimulation sim;

    public GameObject timer;
    private Text timerText;
    private DateTime startTime;

    public GameObject highScoreObject;
    private Text highScoreText;

    private float highScore = 120;

	// Update is called once per frame
	void Update () {
        if (firstUpdate)
        {
            demo = NBodyDemoObject.GetComponent<NBodyDemoMode>();

            sim = demo.GetDemoSimulation();

            targetBody = demo.GetTrailsBody();
            startMass = targetBody.Mass;
            lookMass = startMass * massScaling;

            targetBodyObject = demo.GetTrailsBodyObject();
            particles = targetBodyObject.GetComponent<ParticleSystem>();
            particles.enableEmission = false;
            startScale = targetBodyObject.transform.localScale;
            lookScale = targetBodyObject.transform.localScale * sizeScaling;
            score = accuracySlider.GetComponent<Slider>();
            score.maxValue = sim.GetGBodyObjects().Length;

            startTime = DateTime.Now;
            timerText = timer.GetComponent<Text>();
            highScoreText = highScoreObject.GetComponent<Text>();
            highScoreText.text = "\r\nBest Score\r\n" + highScore.ToString("F3") + "s";
            isLooking = false;
            firstUpdate = false;
        }
        Vector3 cameraCenter = lookCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, lookCamera.nearClipPlane));
        Ray r = new Ray(cameraCenter, lookCamera.transform.forward);
        float dist = Vector3.Cross(r.direction, targetBodyObject.transform.position - r.origin).magnitude;
        if (dist < lookRadius)
        {
            if(!isLooking)
            {
                isLooking = true;
                iTween.ScaleTo(targetBodyObject, lookScale, 1f);
            }
            targetBody.Mass = lookMass;
            particles.enableEmission = true;
        }
        else
        {
            if (isLooking)
            {
                isLooking = false;
                iTween.ScaleTo(targetBodyObject, startScale, 1f);
            }
            targetBody.Mass = startMass;
            particles.enableEmission = false;
        }

        if (Input.GetKey(resetKey))
        {
            firstUpdate = true;
            demo.Reset();
        }
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
	}

    void FixedUpdate()
    {
        if (sim != null)
        {
            int count = 0;
            foreach (Transform child in sim.gameObject.transform)
                if (scoreBounds.Contains(child.position))
                    count++;
            score.value = count;

            DateTime current = DateTime.Now;


            if(count == 0)
            {
                float scoreT = (float)current.Subtract(startTime).TotalMilliseconds / 1000f;
                if (scoreT < highScore)
                {
                    highScore = scoreT;
                    highScoreText.text = "\r\nBest Score\r\n" + highScore.ToString("F3") + "s";
                }
                
            }

            timerText.text = (current.Subtract(startTime).TotalMilliseconds / 1000).ToString("F3") + "s";
        }
    }
}

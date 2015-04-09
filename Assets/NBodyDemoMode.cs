using UnityEngine;
using System.Collections;

public class NBodyDemoMode : MonoBehaviour {

    private NBodySimulation demoSimulation;

    public bool enableDemo = true;
    public GameObject demoObject;
    public GameObject trailsDemoObject;
    public Vector3 demoGenerationRadii;
    public Vector3 demoGenerationVelocityRange;
    public float demoGenerationMinMass;
    public float demoGenerationMaxMass;
    public int demoNumberOfBodies;
    public float demoRenderScale = 0.1f;
    public bool demoPerpendicularVelocities;
    public float demoPerpendicularVelocitiesScaling;

    private int trailsBodyIndex = 0;

    private GBody trailsBody;
    private GameObject trailsBodyObject;

    private bool isDemoValid = false;

	// Use this for initialization
	void Start () {
        NBodySimulation[] sims = gameObject.GetComponents<NBodySimulation>();

        if (sims == null)
        {
            Debug.LogWarning("Warning: No NBodySimulation attached to object " + gameObject.name + " which has NBodyDemoMode component; demo will not run.");
            return;
        }

        if (sims.Length != 1)
        {
            Debug.LogWarning("Warning: More than one NBodySimulation attached to object " + gameObject.name + "; using first found.");
        }

        demoSimulation = sims[0];

        if (enableDemo)
        {
            GBody[] bodies = new GBody[demoNumberOfBodies];
            for (int i = 0; i < bodies.Length; i++)
            {
                GameObject renderObject;
                if (i == trailsBodyIndex)
                {
                    renderObject = (GameObject)GameObject.Instantiate(trailsDemoObject);
                    trailsBodyObject = renderObject;
                }
                else renderObject = (GameObject)GameObject.Instantiate(demoObject);
                renderObject.transform.parent = this.transform;
                Vector3 pos = Random.insideUnitSphere;
                pos.Scale(demoGenerationRadii);
                Vector3 vel = Random.insideUnitSphere;
                vel.Scale(demoGenerationVelocityRange);
                if (demoPerpendicularVelocities)
                    vel = new Vector3(pos.z / demoPerpendicularVelocitiesScaling, 0, -pos.x / demoPerpendicularVelocitiesScaling);
                float mass = Random.Range(demoGenerationMinMass, demoGenerationMaxMass);

                bodies[i] = new GBody(pos, vel, mass, renderObject, demoRenderScale);

                if (i == trailsBodyIndex) trailsBody = bodies[i];
            }

            demoSimulation.SetGBodyObjects(bodies);

            isDemoValid = true;
        }
	}

    public GBody GetTrailsBody()
    {
        return trailsBody;
    }

    public GameObject GetTrailsBodyObject()
    {
        return trailsBodyObject;
    }

    public NBodySimulation GetDemoSimulation()
    {
        return demoSimulation;
    }

    public void Reset()
    {
        foreach (Transform child in demoSimulation.gameObject.transform)
            GameObject.Destroy(child.gameObject);
        Start();
    }
}

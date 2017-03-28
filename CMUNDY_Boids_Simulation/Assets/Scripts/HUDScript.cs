using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    public static HUDScript singleton;

    public Text alignWeight;
    public Text cohesWeight;
    public Text separWeight;
    public Text boidTotalTxt;

    public Slider alignSlider;
    public Slider cohesSlider;
    public Slider separSlider;
    public Slider camSpeedSlider;
    public Slider boidTotalSlider;

    public Image helpBG;
    public Button defaultButton;
    public Image dropDown;
    public Text simulationInfo;
    public Text simulationName;
    public Text toggleCamView;
    public Button nextView, prevView;
    public Text camText;

    bool isActive;
    bool openHUD;
    Vector3 openPos;
    Vector3 closePos;

    void Awake()
    {
        singleton = this;
    }

	// Use this for initialization
	void Start ()
    {
        UpdateWeights();
        helpBG.enabled = false;
        defaultButton.gameObject.SetActive(false);
        simulationInfo.enabled = false;
        simulationName.enabled = false;
        openPos = new Vector3(-310, 8, 0);
        closePos = new Vector3(-310, 408.3f, 0);
        SetDefaults();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(openHUD)
        {
            dropDown.transform.localPosition = Vector3.Lerp(dropDown.transform.localPosition, openPos, Time.deltaTime * 19.2f);
        }
        else
        {
            dropDown.transform.localPosition = Vector3.Lerp(dropDown.transform.localPosition, closePos, Time.deltaTime * 19.2f);
        }
	}

    public void UpdateWeights()
    {
        alignWeight.text = "" + BoidsBehaviourScript.alignmentWeighting;
        cohesWeight.text = "" + BoidsBehaviourScript.cohesionWeighting;
        separWeight.text = "" + BoidsBehaviourScript.separationWeighting;
    }

    public void ControlAlignSlider()
    {
        alignSlider.wholeNumbers = true;
        BoidsBehaviourScript.alignmentWeighting = alignSlider.value;
        UpdateWeights();
    }

    public void ControlCohesSlider()
    {
        cohesSlider.wholeNumbers = true;
        BoidsBehaviourScript.cohesionWeighting = cohesSlider.value;
        UpdateWeights();
    }

    public void ControlSeparSlider()
    {
        separSlider.wholeNumbers = true;
        BoidsBehaviourScript.separationWeighting = separSlider.value;
        UpdateWeights();
    }

    public void ControlCamSpeed()
    {
        camSpeedSlider.wholeNumbers = true;
        CameraScript.singleton.camSpeed = (int)camSpeedSlider.value;
    }

    public void AdjustBoidTotal()
    {
        boidTotalSlider.wholeNumbers = true;
        BoidPoolManager.singleton.ActivateBoids((int)boidTotalSlider.value);
    }

    public void ResetScene()
    {
        BoidsBehaviourScript.alignmentWeighting = 1;
        BoidsBehaviourScript.cohesionWeighting = 1;
        BoidsBehaviourScript.separationWeighting = 1;
        UpdateWeights();
        SceneController.singleton.LoadScene(SceneController.singleton.currentScene);
    }

    public void ToggleHelp()
    {
        isActive = !isActive;

        helpBG.enabled = isActive;
        defaultButton.gameObject.SetActive(isActive);
        simulationName.enabled = isActive;
        simulationInfo.enabled = isActive;
    }

    public void SetDefaults()
    {
        switch(SceneController.singleton.currentScene)
        {
            case 0:
                BoidsBehaviourScript.alignmentWeighting = 8;
                BoidsBehaviourScript.cohesionWeighting = 4;
                BoidsBehaviourScript.separationWeighting = 7;
                UpdateWeights();
                simulationName.text = "Single Point Flocking Simulation:";
                simulationInfo.text = "This simulation demonstrates how birds flock with each other when stationery and whilst moving."
                                        + "\n\nControls: Click to place seeds for the birds to move to, use WASD to control the camera, "
                                        + "and use the dropdown menu to alter the behaviour of the boids. Use the button below to  " +
                                        "return to the default behaviour settings.";
                toggleCamView.enabled = false;
                prevView.gameObject.SetActive(false);
                nextView.gameObject.SetActive(false);
                camText.enabled = true;
                camSpeedSlider.gameObject.SetActive(true);
                boidTotalSlider.gameObject.SetActive(false);
                boidTotalTxt.enabled = false;
                break;
            case 1:
                BoidsBehaviourScript.alignmentWeighting = 8;
                BoidsBehaviourScript.cohesionWeighting = 4;
                BoidsBehaviourScript.separationWeighting = 6;
                UpdateWeights();
                simulationName.text = "In-Air Flocking Simulation:";
                simulationInfo.text = "This simulation demonstrates how birds flock with each other while in the air."
                        + "\n\nControls: Press SPACE to begin the simulation. Q + E control the camera zoom,"
                        + "and use the dropdown menu to alter the behaviour of the boids. Use the button below to  " +
                        "return to the default behaviour settings.";
                toggleCamView.enabled = true;
                prevView.gameObject.SetActive(true);
                nextView.gameObject.SetActive(true);
                camText.enabled = false;
                camSpeedSlider.gameObject.SetActive(false);
                boidTotalSlider.gameObject.SetActive(true);
                boidTotalTxt.enabled = true;
                break;
            case 2:
                BoidsBehaviourScript.alignmentWeighting = 8;
                BoidsBehaviourScript.cohesionWeighting = 4;
                BoidsBehaviourScript.separationWeighting = 6;
                UpdateWeights();
                simulationName.text = "Shoal Simulation with Predator:";
                simulationInfo.text = "This simulation demonstrates how fish shoal with each other and react to a predatory "
                        + "influence. \n\nControls: Rotate with WASD, and use the left mouse button to move forwards."
                        + "The dropdown menu can be used to alter the behaviour of the boids. Use the button below to  " +
                        "return to the default behaviour settings.";
                prevView.gameObject.SetActive(false);
                nextView.gameObject.SetActive(false);
                camText.enabled = false;
                camSpeedSlider.gameObject.SetActive(false);
                boidTotalSlider.gameObject.SetActive(false);
                boidTotalTxt.enabled = false;
                break;
        }
    }

    public void ToggleHUD()
    {
        openHUD = !openHUD;
    }

    public void NextSim()
    {
        SceneController.singleton.LoadScene(SceneController.singleton.currentScene + 1);
    }

    public void NextCameraView()
    {
        CameraScript.singleton.ChangeView(true);
    }

    public void PreviousCameraView()
    {
        CameraScript.singleton.ChangeView(false);
    }
}

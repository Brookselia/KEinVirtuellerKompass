using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurvatureGames.SpaceExtender;

public class Task : MonoBehaviour
{
    [SerializeField]
    private GameObject display;
    [SerializeField]
    private TextMesh textAttempts;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private List<GameObject> taskButton;
    [SerializeField]
    private GameObject room;
    [SerializeField]
    private GameObject table;

    [Header("Camera")]
    [SerializeField]
    private GameObject XRCamera;

    [Header("Textures and Rotation Gain")]
    [SerializeField]
    private List<Material> roomMaterials;
    [SerializeField]
    private List<Material> tableMaterials;
    [SerializeField]
    private List<Material> skyboxMaterials;
    [SerializeField]
    private List<RotationRedirector> rotationGains;

    [Header("Audio Files Instructions")]
    [SerializeField]
    private AudioClip intro;
    [SerializeField]
    private AudioClip virtuel;
    [SerializeField]
    private AudioClip physical;
    [SerializeField]
    private AudioClip thanks;

    [Header("Timer")]
    [SerializeField]
    [Range(10f, 120f)]
    private float intervalBetweenMessurements = 60f;
    [SerializeField]
    [Range(2, 20)]
    private int totalCountOfMessurements =  10;


    private bool triggerTasks = false;
    private int activeTasks = 0;
    private int maxTasks = 6;
    private float taskStartRate = 3f;
    private float nextTaskStart = 0f;
    private float nextIntervall = 0f;

    private List<int> orderOfMaterials;
    private List<int> orderOfRotationGains;
    private int cntAttempts = 0;
    private int cntOfMessurements = 0;

    private Renderer renderer;
    private AudioSource AudioSource;


    private void Awake()
    {
        OnButtonClicked.RegisterListener(ButtonInteraction);
    }

    private void OnDestroy()
    {
        OnButtonClicked.UnregisterListener(ButtonInteraction);
    }

    void Start()
    {
        nextTaskStart = taskStartRate;
        AudioSource = GetComponent<AudioSource>();
        orderOfMaterials = InitLists();
        orderOfRotationGains = InitLists();
    }

    void Update()
    {
        if(triggerTasks)
        {
            if ((Time.time >= nextTaskStart) && (activeTasks <= maxTasks))
            {
                nextTaskStart = Time.time + taskStartRate;
                StartTask();
            }

            if ((Time.time >= nextIntervall) && (nextIntervall != 0))
            {
                StartCoroutine(Messurement(virtuel));
            }
        }
    }

    /// <summary>
    /// Reacts to the OnButtonClicked event.
    /// Depending on the pressed button the corresponding actions are taken.
    /// </summary>
    /// <param name="buttonClick"></param>
    private void ButtonInteraction(OnButtonClicked buttonClick)
    {
        if (buttonClick.ButtonName.Equals(startButton.name) && (cntAttempts < 3))
        {
            InitAttempt();
            startButton.GetComponent<PushableBehavior>().ResetPushed();
        } else if (activeTasks>0)
        {
            activeTasks--;
        }
    }

    /// <summary>
    /// Initializes the order of the textures and the rotation gains. Only supports 3.
    /// </summary>
    /// <returns></returns>
    private List<int> InitLists()
    {
        List<int> possible = new List<int>();
        possible.Add(0);
        possible.Add(1);
        possible.Add(2);
        List<int> listNumbers = new List<int>();

        System.Random rnd = new System.Random();
        for (int i = 0; i < 3; i++)
        {
            int index = rnd.Next(0, possible.Count);
            listNumbers.Add(possible[index]);
            possible.RemoveAt(index);
        }

        return listNumbers;
    }

    /// <summary>
    /// Initializes the values of one attempt.
    /// </summary>
    private void InitAttempt()
    {
        display.SetActive(false);

        // init textures
        renderer = room.GetComponent<Renderer>();
        renderer.sharedMaterial = roomMaterials[orderOfMaterials[cntAttempts]];
        Debug.Log("Uses Room Material: " + roomMaterials[orderOfMaterials[cntAttempts]].name);
        renderer = table.GetComponent<Renderer>();
        renderer.sharedMaterial = tableMaterials[orderOfMaterials[cntAttempts]];
        Debug.Log("Uses Table Material: " + tableMaterials[orderOfMaterials[cntAttempts]].name);
        RenderSettings.skybox = skyboxMaterials[orderOfMaterials[cntAttempts]];
        Debug.Log("Uses Skybox: " + skyboxMaterials[orderOfMaterials[cntAttempts]].name);

        // init rotation gains
        rotationGains[orderOfRotationGains[cntAttempts]].gameObject.SetActive(true);
        Debug.Log("Used Rotation Gain: " + rotationGains[orderOfRotationGains[cntAttempts]].name);

        StartCoroutine(Messurement(intro));
    }

    /// <summary>
    /// Starts a task
    /// </summary>
    private void StartTask()
    {
        activeTasks++;
        System.Random rnd = new System.Random();
        GameObject button = taskButton[rnd.Next(0, taskButton.Count)];
        if (!button.gameObject.GetComponent<ButtonBehavior>().IsLit())
            button.GetComponent<ButtonBehavior>().StartButtonTask();
    }

    /// <summary>
    /// Plays the request for a messuring and starts the coroutine to wait before the angle is taken.
    /// </summary>
    /// <param name="clipToPlay"></param>
    private IEnumerator Messurement(AudioClip clipToPlay)
    {
        triggerTasks = false;

        AudioSource.clip = clipToPlay;
        Debug.Log("Started AudioClip: " + clipToPlay.name);
        AudioSource.Play();

        float time = 0f;

        switch (clipToPlay.name)
        {
            case "einführung":
                time = 39f;
                break;
            case "satz-ausgangsrichtung":
            case "satz-ausgangsrichtung2":
                time = 4f;
                break;
            case "dankefürteilnahme":
                time = 7f;
                break;
        }

        yield return new WaitForSeconds(time);

        Debug.Log(string.Format("Rotation for attempt nr.{0} and messurement nr.{1}: {2}", cntAttempts, cntOfMessurements, XRCamera.transform.rotation.eulerAngles.y));
        nextIntervall = Time.time + intervalBetweenMessurements;

        if (!(clipToPlay.Equals(thanks) && clipToPlay.Equals(virtuel)))
        {
            triggerTasks = true;
        }
        else if (clipToPlay.Equals(virtuel))
        {
            StartCoroutine(Messurement(physical));
        }

        if (clipToPlay.Equals(physical))
        {
            cntOfMessurements++;
        }

        if (cntOfMessurements == totalCountOfMessurements)
        {
            StartCoroutine(Messurement(thanks));
            FinishAttempt();
        }
    }

    /// <summary>
    /// Resets values after an attemt is finished.
    /// </summary>
    private void FinishAttempt()
    {
        // reset rotaition gain and counts and flags
        rotationGains[orderOfRotationGains[cntAttempts]].gameObject.SetActive(false);
        triggerTasks = false;
        cntOfMessurements = 0;
        nextIntervall = 0;
        activeTasks = 0;

        // rest lmps and buttons
        foreach(GameObject lamp in GameObject.FindGameObjectsWithTag("Lamp"))
        {
            lamp.GetComponent<ButtonBehavior>().ResetLamp();
        }
        foreach(GameObject button in GameObject.FindGameObjectsWithTag("Button"))
        {
            button.GetComponent<PushableBehavior>().ResetPushed();
        }

        // reset texures
        renderer = room.GetComponent<Renderer>();
        renderer.sharedMaterial = roomMaterials[3];
        renderer = table.GetComponent<Renderer>();
        renderer.sharedMaterial = tableMaterials[3];
        RenderSettings.skybox = skyboxMaterials[3];


        // Anzahl der durchgeführten Durchläufe erhöhen
        display.SetActive(true);
        cntAttempts++;
        textAttempts.text = cntAttempts + " von 3";
    }
}

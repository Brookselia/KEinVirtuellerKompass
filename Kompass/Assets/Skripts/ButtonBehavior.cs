using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject button;
    [SerializeField]
    private Material unlit;
    [SerializeField]
    private Material lit;

    private bool isLit = false;

    private Renderer renderer;
    private AudioSource audioSource;

    private void Awake()
    {
        OnButtonClicked.RegisterListener(FinishButtonTask);
    }

    private void OnDestroy()
    {
        OnButtonClicked.UnregisterListener(FinishButtonTask);
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.enabled = true;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartButtonTask()
    {
        //light on & sound
        renderer.sharedMaterial = lit;
        audioSource.Play();
        //button pressable
        button.GetComponent<PushableBehavior>().ResetPushed();
        isLit = true;
    }

    void FinishButtonTask(OnButtonClicked buttonClick)
    {
        if (buttonClick.ButtonName.Equals(button.name))
        {
            //light off
            ResetLamp();
        }
    }

    public void ResetLamp()
    {
        renderer.sharedMaterial = unlit;
        isLit = false;
    }

    public bool IsLit()
    {
        return isLit;
    }
}

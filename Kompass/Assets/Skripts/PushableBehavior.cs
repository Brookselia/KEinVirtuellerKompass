using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBehavior : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Initialer Wert")]
    private bool _isPushed = true;

    private AudioSource audioSource;

    private void Awake()
    {
        OnButtonClicked.RegisterListener(OnClickOfButton);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!_isPushed)
        {
            new OnButtonClicked(gameObject.name);
            audioSource.Play();
        }
    }

    public void ResetPushed()
    {
        _isPushed = false;
    }

    public void OnClickOfButton(OnButtonClicked buttonClick)
    {
        _isPushed = true;
    }

    private void OnDestroy()
    {
        OnButtonClicked.UnregisterListener(OnClickOfButton);
    }
}

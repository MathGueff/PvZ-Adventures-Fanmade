using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [Header("Values")]
    private int sunAmount = 25;
    private float sunCurrentTime;
    private float sunMaxTime = 10;
    private float fallSpeed = 2f;

    [Header("Components")]
    private SunController sunController;
    private Animator anim;
    [SerializeField] AudioClip collectSunSound;

    private void Start()
    {
        sunController = FindObjectOfType<SunController>();
        anim = GetComponent <Animator>();
    }

    private void Update()
    {
        sunCurrentTime += Time.deltaTime;
        if (sunCurrentTime >= sunMaxTime)
        {
            anim.SetInteger("transition", 2);
        }
    }

    public void moveSun(Vector2 distance, float? speed = null)
    {
        if (speed.HasValue)
            fallSpeed = speed.Value;
        StartCoroutine(MovingSun(distance));
    }

    public void setSunAmount(int amount)
    {
        sunAmount = amount;
    }
    private void CollectSun()
    {
        SoundManager.instance.PlaySound(collectSunSound);
        sunController.AddSun(sunAmount);
    }

    private void DestroySun()
    {
        Destroy(gameObject);
    }

    private void OnMouseEnter()
    {
        anim.SetInteger("transition", 1);
    }

    IEnumerator MovingSun(Vector2 targetPosition, float? speed = null)
    {
        if(speed == null) { 
            speed = fallSpeed;
        }

        while (Vector2.Distance(transform.position, targetPosition) > 0.01f) // Pequena margem para precisão
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed.Value * Time.deltaTime);
            yield return null;
        }

        // Ajuste final na posição para garantir que o objeto esteja exatamente no destino
        transform.position = targetPosition;
    }
}

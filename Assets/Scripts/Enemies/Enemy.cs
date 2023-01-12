using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameObject lineOfSight;
    [SerializeField] Transform eyes;

    private float time = 2.5f;
    public float timer;

    public enum Mode
    {
        PATROL,
        SUSPICIOUS,
        INVESTIGATING,
        AWARE
    }

    private int Awareness = 0;
    public int awareness { get { return Awareness; } set { Awareness = Mathf.Clamp(value, 0, 100);} }
    // Start is called before the first frame update
    void Start()
    {
        timer = time;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            awareness -= 5;
            timer = time;
        }
        print(awareness);
    }

    public void CheckSightlines(Vector3 playerDirection)
    {
        Physics.Raycast(eyes.position, playerDirection, out RaycastHit hit);
        if (hit.collider.gameObject.tag == "Player") awareness++;
    }
}

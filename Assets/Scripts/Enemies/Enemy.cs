using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameObject lineOfSight;
    [SerializeField] Transform eyes;

    private float time = 1f;
    public float timer;
    private float fullAwareDelay = 5.0f;
    public float fullAwareTimer;

    public SpiderController player;

    public enum Mode
    {
        PATROL,
        SUSPICIOUS,
        INVESTIGATING,
        AWARE
    }

    private float Awareness = 0;
    public float awareness { get { return Awareness; } set { Awareness = Mathf.Clamp(value, 0, 100);} }
    // Start is called before the first frame update
    void Start()
    {
        timer = time;
        fullAwareTimer = fullAwareDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (awareness == 100)
        {
            fullAwareTimer -= Time.deltaTime;
            if (fullAwareTimer <= 0)
            {
                awareness -= 5;
                fullAwareDelay = fullAwareTimer;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                awareness -= 5;
                timer = time;
            }
        }
        print(awareness);
    }

    public void CheckSightlines(Vector3 playerDirection)
    {
        Physics.Linecast(eyes.position, player.spiderCenter.transform.position, out RaycastHit hit, LayerMask.NameToLayer("Enemy"));
        if (hit.collider.gameObject.tag == "Player" && player.isVisible) awareness += 0.33f;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if(player != null) Gizmos.DrawLine(eyes.position, player.spiderCenter.transform.position);
    }
}

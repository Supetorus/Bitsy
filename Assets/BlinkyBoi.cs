using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyBoi : MonoBehaviour
{
	[SerializeField] List<Material> materials;
	[SerializeField] float interval;
	[SerializeField] float blinkLength;
	MeshRenderer render;
	private float timer;
    void Start()
    {
		timer = interval;
		render = GetComponent<MeshRenderer>();
		blinkLength = Mathf.Clamp(blinkLength, 0.1f, interval - 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
		{
			timer -= Time.deltaTime;
		} 
		else
		{
			StartCoroutine(Blink());
			timer = interval;
		}
    }

	IEnumerator Blink()
	{
		render.material = materials[1];
		yield return new WaitForSeconds(blinkLength);
		render.material = materials[0];
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class DebugMovement : MonoBehaviour
{
	[SerializeField] float speed = 1.0f;
	Vector2 input = Vector2.zero;


	void Start()
    {
        
    }

    void Update()
    {
		transform.Translate(speed * Time.deltaTime * new Vector3(input.x, 0, input.y));
	}

	private void OnMove(InputValue inputValue)
	{
		input = inputValue.Get<Vector2>();
	}
}

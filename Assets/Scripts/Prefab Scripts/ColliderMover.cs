using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMover : MonoBehaviour
{
	[SerializeField] GameObject emptyObject;

    public void Move()
	{
		var transforms = GetComponentsInChildren<Transform>();
		foreach (var transform in transforms)
		{

			if (transform.gameObject == gameObject || !transform.gameObject.GetComponent<Collider>()) continue;
			MoveOnObject(transform.gameObject);
		}
	}

	public void MoveOnObject(GameObject gameObject)
	{
		var newObject = Instantiate(emptyObject, gameObject.transform);
		Collider colliderOnObject = gameObject.GetComponent<Collider>();

		Type colliderType = colliderOnObject.GetType();
		if (colliderType.IsAssignableFrom(typeof(MeshCollider)))
		{
			MeshCollider meshCollider = (MeshCollider) colliderOnObject;
			MeshCollider objectCollider = newObject.AddComponent<MeshCollider>();
			objectCollider.convex = meshCollider.convex;
			objectCollider.isTrigger = meshCollider.isTrigger;
			objectCollider.cookingOptions = meshCollider.cookingOptions;
			objectCollider.material = meshCollider.material;
			objectCollider.sharedMaterial = meshCollider.sharedMaterial;
			objectCollider.sharedMesh = meshCollider.sharedMesh;
		}else if (colliderType.IsAssignableFrom(typeof(BoxCollider)))
		{
			BoxCollider boxCollider = (BoxCollider)colliderOnObject;
			BoxCollider objectCollider = newObject.AddComponent<BoxCollider>();
			objectCollider.isTrigger = boxCollider.isTrigger;
			objectCollider.material = boxCollider.material;
			objectCollider.sharedMaterial = boxCollider.sharedMaterial;
			objectCollider.center = boxCollider.center;
			objectCollider.size = boxCollider.size;
		}
		else if (colliderType.IsAssignableFrom(typeof(CapsuleCollider)))
		{
			CapsuleCollider capsuleCollider = (CapsuleCollider)colliderOnObject;
			CapsuleCollider objectCollider = newObject.AddComponent<CapsuleCollider>();
			objectCollider.isTrigger = capsuleCollider.isTrigger;
			objectCollider.material = capsuleCollider.material;
			objectCollider.sharedMaterial = capsuleCollider.sharedMaterial;
			objectCollider.center = capsuleCollider.center;
			objectCollider.radius = capsuleCollider.radius;
			objectCollider.height = capsuleCollider.height;
			objectCollider.direction = capsuleCollider.direction;
		}
		else if (colliderType.IsAssignableFrom(typeof(SphereCollider)))
		{
			SphereCollider sphereCollider = (SphereCollider)colliderOnObject;
			SphereCollider objectCollider = newObject.AddComponent<SphereCollider>();
			objectCollider.isTrigger = sphereCollider.isTrigger;
			objectCollider.material = sphereCollider.material;
			objectCollider.sharedMaterial = sphereCollider.sharedMaterial;
			objectCollider.center = sphereCollider.center;
			objectCollider.radius = sphereCollider.radius;
		}
		DestroyImmediate(colliderOnObject);
	}
}

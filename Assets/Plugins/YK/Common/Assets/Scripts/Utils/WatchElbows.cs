using moveen.core;
using UnityEngine;

namespace moveen.utils {
	public class WatchElbows : MonoBehaviour {
		public float radius1 = 5;
		public float radius2 = 5;
		public float radius3 = 5;


		// Use this for initialization
		void Start () {
		
		}

		// Update is called once per frame
		void Update () {
		
		}

		public void OnDrawGizmos() {
//        LegSkel3 ls = LegSkel3.leg(1, transform.GetChild(0).transform.position, transform.GetChild(1).transform.position, transform.GetChild(2).transform.position, 1, 1, 1);

			JunctionElbow3 ej3 = new JunctionElbow3();
			ej3.basisAbs = new P<Vector3>(transform.GetChild(0).transform.position);
			ej3.targetAbs = new P<Vector3>(transform.GetChild(1).transform.position);
			ej3.normalAbs = new P<Vector3>(Vector3.up);
			ej3.radius1 = radius1;
			ej3.radius2 = radius2 / 2;
			ej3.radius3 = radius3;


			ej3.tick(0);


			Gizmos.DrawLine(ej3.basisAbs.v, ej3.smthng1);
			Gizmos.DrawLine(ej3.smthng1, ej3.smthng2);
//        Debug.Log(ej3.smthng1.dist(ej3.smthng2));
			Gizmos.DrawLine(ej3.smthng2, ej3.targetAbs.v);


			for (var i = 0; i < transform.childCount; i++) {
				UnityEditorUtils.circle3d(transform.GetChild(i).position, Quaternion.AngleAxis(0, Vector3.up), 0.5f, 20);
			}

//        ls.calcAbs(0, Vector3.zero, Quaternion.identity);


			Gizmos.color = Color.grey;
			UnityEditorUtils.circle3d(ej3.basisAbs.v, Quaternion.AngleAxis(0, Vector3.up), ej3.radius1, 50);
			UnityEditorUtils.circle3d(ej3.targetAbs.v, Quaternion.AngleAxis(0, Vector3.up), ej3.radius3, 50);
			UnityEditorUtils.circle3d(ej3.midCirclePos, Quaternion.AngleAxis(0, Vector3.up), ej3.radius2, 50);

//        UnityEditorUtils.circle3d(ej3.smthng1, Quaternion.AngleAxis(0, Vector3.up), 1, 50);
//        UnityEditorUtils.circle3d(ej3.smthng2, Quaternion.AngleAxis(0, Vector3.up), 1, 50);

		}


	}
}
using UnityEngine;

namespace moveen.example {
    public class FireControl1 : MonoBehaviour {
        public GameObject gun;
        public GameObject explosion;
        public Transform body;
        public string key;
        private Rigidbody bodyRigid;


        public void Start() {
            if (body == null) return;
            bodyRigid = body.GetComponent<Rigidbody>();
        }

        public void FixedUpdate() {
            if (bodyRigid == null) return;
            if (Input.GetKeyDown(key)) {
                Animator animator = gun.GetComponent<Animator>();
                animator.SetTrigger("shootTrigger");

                bodyRigid.AddForceAtPosition(transform.TransformDirection(new Vector3(-20, 0, 0)), transform.position, ForceMode.Force);
                var expl = Instantiate(explosion, transform.position, transform.rotation);
                Destroy(expl, 0.5f);


            }
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
        }
    }
}
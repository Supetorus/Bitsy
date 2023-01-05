using UnityEngine;

namespace moveen.descs {
    public interface CharacterInput {
        void setLookDirection(Vector3 dir);
        void setWalkDirection(Vector3 pos);
        void setAimTo(Vector3 pos);

    }
}
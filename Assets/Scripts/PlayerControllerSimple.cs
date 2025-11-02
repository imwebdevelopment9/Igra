using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerSimple : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 10f;
    public Transform cameraPivot;   // prevuci Main Camera ili CM kameru

    CharacterController cc;
    float yVel;
    const float gravity = -9.81f;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        // Ulaz sa tastature
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Pravac kretanja na osnovu kamere
        Vector3 camF = Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
        Vector3 camR = Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;
        Vector3 dir = (camF * v + camR * h);

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion to = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, to, rotateSpeed * Time.deltaTime);
        }

        // Gravitacija
        if (cc.isGrounded && yVel < 0) yVel = -2f;
        yVel += gravity * Time.deltaTime;

        // Kretanje
        Vector3 vel = dir.normalized * moveSpeed;
        vel.y = yVel;

        cc.Move(vel * Time.deltaTime);
    }
}

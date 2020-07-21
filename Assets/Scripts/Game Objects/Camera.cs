using UnityEngine;

public class Camera : MonoBehaviour
{
	public static Camera main;
	public Transform target;
	public float smoothSpeed = 0.125f;
	public Vector3 offset;

    private void Awake()
    {
		main = this;
    }
    // Start is called before the first frame update
    void Start()
    {
		target = Zero.current.transform;
    }
	void FixedUpdate()
	{
		Vector3 desiredPosition = target.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;
	}

}
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
	[SerializeField] private float followHeight;
	[SerializeField] private bool rotateWithTarget = false;

	public void SetTarget(Transform newTarget) {
		target = newTarget;
		target.gameObject.GetComponent<Health>().OnDeath.AddListener(DeleteWaypoint);
	}

	private void Update() {
		if (target == null)
			return;

		transform.position = new Vector3(target.position.x, followHeight, target.position.z);

		if (rotateWithTarget) {
			transform.rotation = Quaternion.Euler(new Vector3(90f, target.rotation.eulerAngles.y, 0f));
		}
	}

	private void DeleteWaypoint() {
		Destroy(gameObject);
	}
}
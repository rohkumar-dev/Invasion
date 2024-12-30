using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Events;

public class BasicRigidBodyPush : MonoBehaviour
{
	public UnityEvent OnPunch;

	public LayerMask pushLayers;
	public bool canPush;
	private bool punchAnimationHasPlayed = false;

	[SerializeField] private float strengthWhileDisabled;
	[SerializeField] private float strengthWhileEnabled;

	[SerializeField] private List<AudioClip> whooshAudioClips;
	[SerializeField] private AudioMixerGroup mixerGroup;

	private TPSController tpsController;

	private void Awake() {
		tpsController = GetComponent<TPSController>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (canPush && !punchAnimationHasPlayed && !tpsController.canShoot) StartCoroutine(PushRigidBodies(collider));
	}

	private IEnumerator PushRigidBodies(Collider collider)
	{

		Rigidbody rb = collider.attachedRigidbody;
		NavMeshAgent agent = collider.gameObject.GetComponent<NavMeshAgent>();
		Health health = collider.gameObject.GetComponent<Health>();

		if (agent == null || rb == null || !health.IsAlive()) yield break;

		agent.enabled = false;
		rb.isKinematic = false;
		rb.useGravity = true;

		Vector3 backDir = (collider.transform.position - transform.position).normalized;
		backDir.y = 0f;

        Vector3 force = (backDir + Vector3.up).normalized * (tpsController.canShoot ? strengthWhileEnabled : strengthWhileDisabled);

		OnPunch.Invoke();
		yield return new WaitUntil(
			() => punchAnimationHasPlayed == true
		);

        rb.AddForce(force, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.1f);

		float startTime = Time.time;

        yield return new WaitUntil(
            () => collider == null || rb == null || agent == null || (rb.velocity.magnitude <= 2f && IsGrounded(collider.transform.position)) || Time.time > startTime + 3.5f
        );

        yield return new WaitForSeconds(0.1f);

		if (rb == null || agent == null || collider == null) yield break;
		
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        NavMesh.SamplePosition(collider.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        collider.transform.position = hit.position;
        agent.enabled = true;
	}

	private void ShoveEnemy() {
		punchAnimationHasPlayed = true;
		SoundManager.shared.PlayRandomSoundClip(whooshAudioClips, mixerGroup, transform, 1f);
	}

	private void FinishShoveAnimation() {
		punchAnimationHasPlayed = false;
	}

	private bool IsGrounded(Vector3 pos) {
        return Physics.Raycast(pos, Vector3.down, 0.1f);
    }
}
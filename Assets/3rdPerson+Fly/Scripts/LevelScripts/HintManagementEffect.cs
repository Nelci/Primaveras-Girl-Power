using UnityEngine;

// This class is created for the example scene. There is no support for this script.
public class HintManagementEffect : MonoBehaviour
{
	public string message = "";
	public string message2 = "";
	public KeyCode changeMessageKey;
	public ParticleSystem getting;
	
	public string skill = "";
	
	private GameObject player;
	private bool used = false;

	private ControlsTutorial manager;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControlsTutorial> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if((other.gameObject == player) && !used)
		{
			manager.SetShowMsg(true);
			manager.SetMessage(message);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject == player)
		{
			manager.SetShowMsg(false);
			if (!used) {
				return;
			}
			manager.AddScore(1);
			manager.SetAcquiredSkill(skill, true);
			Destroy(gameObject);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if(message2 != "" && other.gameObject == player && Input.GetKeyDown(changeMessageKey))
		{
			manager.SetMessage(message2);
			gettingSkill();
		}
	}

    private void gettingSkill()
    {
		used = true;
		getting.gameObject.SetActive(true);
    }

	
}

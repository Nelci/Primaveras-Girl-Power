using System.Collections;
using System.Collections.Generic;
using ModelShark;
using UnityEngine;

[RequireComponent(typeof(TooltipTrigger))]
public class ShowToolTipColide : MonoBehaviour
{
    public string requiredSkill;
    public string withSkillText;
    public string withoutSkillText;
	private ControlsTutorial manager;

    // Start is called before the first frame update
    void Start()
    {
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControlsTutorial> ();
    }

    void OnTriggerEnter(Collider colliderInfo)
    {
        TooltipTrigger tooltipTrigger = gameObject.GetComponent<TooltipTrigger>();

        tooltipTrigger.isRemotelyActivated = true;
        tooltipTrigger.staysOpen = true;
        // Set the tooltip text.
        if (manager.GetAcquiredSkill(requiredSkill))
        {
            tooltipTrigger.SetText("BodyText", withSkillText);
        } else {
            
            tooltipTrigger.SetText("BodyText", withoutSkillText);
        }

        // Popup the tooltip (Note: duration doesn't matter, since StaysOpen is True)
        tooltipTrigger.Popup(1f, gameObject);
    }

    void OnTriggerExit(Collider colliderInfo)
    {
        TooltipTrigger tooltipTrigger = gameObject.GetComponent<TooltipTrigger>();
        tooltipTrigger.ForceHideTooltip();
    }
}

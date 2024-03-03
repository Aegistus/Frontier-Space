using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class InteractState
    {
        public InteractState(AgentAction action) : base(action) { }

        public override void Before()
        {
            RaycastHit rayHit;
            Physics.Raycast(action.eyeTransform.position, action.eyeTransform.forward, out rayHit, action.interactDistance);
            if (rayHit.collider != null)
            {
                IInteractable interactable = rayHit.collider.GetComponentInChildren<IInteractable>();
                interactable?.Interact(gameObject);
            }
        }

        public override Type CheckTransitions()
        {
            return typeof(IdleState);
        }
    }
}

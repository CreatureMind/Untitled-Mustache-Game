using UnityEngine;

    public class Pickup_Heal : Pickup_Base
    {
        protected sealed override void OnTriggerEnter(Collider other)
        {
            Debug.Log("Pickup Heal");
            base.OnTriggerEnter(other);
        }

        protected override void DoAction()
        {
            Player_Manager.Instance.StatHandler.Heal();
        }
    }

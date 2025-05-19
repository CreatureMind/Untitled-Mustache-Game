using UnityEngine;

    public class Pickup_Heal : Pickup_Base
    {
        protected override void DoAction()
        {
            Player_Manager.Instance.StatHandler.Heal();
        }
    }

using UnityEngine;

    public class Pickup_Heal : Pickup_Base
    {
        protected override void DoAction()
        {
            Player_Manager.Instance.MovementHandler.StatHandler.Heal();
        }
        
        public new void RandomDropRange(GameObject currentObj)
        {
            //base.RandomDropRange(currentObj);
        }
    }

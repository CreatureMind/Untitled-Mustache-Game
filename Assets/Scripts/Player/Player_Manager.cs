
    using System;
    using UnityEngine;

    public class Player_Manager : MonoBehaviour
    {
        private static Player_Manager instance;
        public static Player_Manager Instance => instance;
        
        
        [SerializeField] private Movement_Handler _movementHandler;
        public Movement_Handler MovementHandler => _movementHandler;
        
        
        private Stat_Handler _statHandler;
        public Stat_Handler StatHandler => _statHandler;
        

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            _statHandler = new Stat_Handler(_movementHandler.UnitData);
        }
    }

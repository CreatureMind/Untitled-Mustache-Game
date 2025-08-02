
    public static class Pickup_Util
    {
        // this class is a helper static util class
        
        public static PoolType RandomizePickup()
        {
            var random = UnityEngine.Random.Range(0, 100);
            switch (random)
            {
                // extend to create a full randomizing pickup system
                default:
                    return PoolType.PickupHealth;
            }
        }

        public static bool RandomizeCrate()
        {
            var random = UnityEngine.Random.Range(0, 100);
            switch (random)
            {
                case < 20:
                    return true;
            }
            return false;
        }
    }

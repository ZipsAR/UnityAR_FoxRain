namespace Structs
{
    public struct StatChangeCriteria
    {
        // Stat values that vary every unitTime
        public int fluctuatingValuePerTime; 
        // Stat value that changes each time the pet moves a certain distance
        public int fluctuatingValuePerDistance;
        
        // Time elapsed and distance traveled since stat value was updated
        public float curTime;
        public float curDistance;
        
        // Stat changes at this time and distance
        public float unitTime;
        public float unitDistance;
        
        public StatChangeCriteria(
            int fluctuatingValuePerTime,
            int fluctuatingValuePerDistance,
            
            float curTime,
            float curDistance,
            
            float unitTime,
            float unitDistance)
        {
            this.fluctuatingValuePerTime = fluctuatingValuePerTime;
            this.fluctuatingValuePerDistance = fluctuatingValuePerDistance;

            this.curTime = curTime;
            this.curDistance = curDistance;
            
            this.unitTime = unitTime;
            this.unitDistance = unitDistance;
        }
    }
}
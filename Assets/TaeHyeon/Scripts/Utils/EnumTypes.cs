namespace EnumTypes
{
    #region Hand

        public enum HandSide
        {
            Left,
            Right,
        }

    #endregion

    public enum PlayMode
    {
        None = 0,
        InteractMode = 1,
        StrollMode = 2,
        AgilityMode = 3,
        StoreMode = 4,
        HousingMode = 5
    }
    
    public enum Cmd
    {
        Move = 0,
        Look = 1,
        Sit = 2,
        Eat = 3,
        Brush = 4,
        Bite = 5,
        Spit = 6,
    }

    public enum TutorialType
    {
        Toy,
        Snack,
        Money,
    }

    public enum DialogOrient
    {
        Center,
        Left,
        Right,
    }
    
    #region Pet
    
        public enum PetType
        {
            None,
            Corgi,
            Husky,
            Shiba,
            White,
            Cat,
            BellCat,
        }
        
        public enum PetParts
        {
            None,
            Head,
            Jaw,
            Body,
            HandDetection,
        }
        
        // If this order is changed, the order of sound in the inspector window must be changed accordingly
        public enum PetSounds
        {
            Bark1,
            Bark2,
            Bark3,
            Gasps,
            Sniff,
            Whines,
            Eat,
        }

        public enum PetStates
        {
            Idle,
            Walk,
            Sit,
        }
        
        public enum PetStatNames
        {
            Fullness,
            Tiredness,
            Cleanliness,
            Exp,
            Level,
            Money,
        }
        
    #endregion
}
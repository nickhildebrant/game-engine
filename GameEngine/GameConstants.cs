namespace CPI311.GameEngine
{
    public class GameConstants
    {
        // player constants
        public const float PlayerSpeed = 100f;
        public const float PlayerRotationSpeed = 3.0f;

        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;

        //asteroid constants
        public const int NumAsteroids = 15;
        public const float AsteroidMinSpeed = 1500.0f;
        public const float AsteroidMaxSpeed = 3000.0f;
        public const float AsteroidSpeedAdjustment = 5.0f;

        //bullets constants
        public const int NumBullets = 30;
        public const float BulletSpeedAdjustment = 5000.0f;

        //game constants
        public const int ShotPenalty = 1;
        public const int KillBonus = 25;
        public const int DeathPenalty = 100;

        public const float AsteroidBoundingSphereScale = 0.95f;  //95% size
        public const float ShipBoundingSphereScale = 0.5f;  //50% size
    }
}

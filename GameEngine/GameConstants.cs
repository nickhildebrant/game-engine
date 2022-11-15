namespace CPI311.GameEngine
{
    public class GameConstants
    {
        // player constants
        public const float PlayerSpeed = 2500f;
        public const float PlayerRotationSpeed = 3.0f;

        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;

        //asteroid constants
        public const int NumAsteroids = 10;
        public const float AsteroidMinSpeed = 1200.0f;
        public const float AsteroidMaxSpeed = 2000.0f;
        public const float AsteroidSpeedAdjustment = 5.0f;

        //bullets constants
        public const int NumBullets = 20;
        public const float BulletSpeedAdjustment = 5000.0f;

        //game constants
        public const int ShotPenalty = 5;
        public const int KillBonus = 100;
    }
}

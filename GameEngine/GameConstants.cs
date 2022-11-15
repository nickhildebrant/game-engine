namespace CPI311.GameEngine
{
    public class GameConstants
    {
        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;

        //asteroid constants
        public const int NumAsteroids = 10;
        public const float AsteroidMinSpeed = 20;
        public const float AsteroidMaxSpeed = 100;

        //bullets constants
        public const int NumBullets = 30;
        public const float BulletSpeedAdjustment = 1.0f;

        //game constants
        public const int ShotPenalty = 10;
        public const int KillBonus = 5;
    }
}

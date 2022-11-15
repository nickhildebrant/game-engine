namespace CPI311.GameEngine
{
    public class GameConstants
    {
        //camera constants
        public const float CameraHeight = 2500.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;

        //asteroid constants
        public const int NumAsteroids = 10;
        public const float AsteroidMinSpeed = 100.0f;
        public const float AsteroidMaxSpeed = 300.0f;

        //bullets constants
        public const int NumBullets = 30;
        public const float BulletSpeedAdjustment = 100.0f;

        //game constants
        public const int ShotPenalty = 10;
        public const int KillBonus = 5;

        public const float AsteroidBoundingSphereScale = 0.95f;  //95% size
        public const float ShipBoundingSphereScale = 0.5f;  //50% size
    }
}

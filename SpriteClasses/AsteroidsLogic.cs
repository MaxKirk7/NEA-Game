using System;

namespace NEAGameObjects;
partial class Asteroids
{
        private void Move(float delta){
        //using the unit circle I can change the proportion of verticle and horizontle movement
        //The value will be between -1 and 1 so by multiplying by 10 when converting to an int we get a more accurate rounding
        XPos += (int)((int)(Math.Sin(UnitCircleValue) *100 * delta) * SpeedScale);
        YPos += (int)((int)(Math.Cos(UnitCircleValue) *100 * delta) * SpeedScale);
    }
    private static bool SpawnAsteroids(){
        return asteroids.Count == 0;
    }
    private static void CreateAsteroids(int score){
        if (score > 500){
            for (int i = 0; i < score /100; i++){
            asteroids.Add(new Asteroids()); // more asteroids the higher the score gets
            }
        }
        for (int i = 0; i < 5; i++){
            asteroids.Add(new Asteroids()); // minimum 5 asteroids drawn
        }
    }
}
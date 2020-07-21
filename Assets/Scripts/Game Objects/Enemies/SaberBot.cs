public class SaberBot : Enemy
{

    public override void Initialize(float runSpeed, float maxHealth, int points, float damage)
    {
        RunSpeed = runSpeed;
        minimumRange = 0.3f;
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        Points = points;
        Damage = damage;
    }
    public override void Initialize()
    {
        RunSpeed = 2;
        minimumRange = 0.3f;
        MaxHealth = 30;
        CurrentHealth = MaxHealth;
        Points = 5;
        Damage = 10;
    }
}

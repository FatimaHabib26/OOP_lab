namespace GameProject.Models
{
    public class Monster
    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int AttackPower { get; private set; }
        public int ScoreValue { get; private set; }

        public Monster(string name, int health, int attackPower, int scoreValue)
        {
            Name = name;
            Health = health;
            AttackPower = attackPower;
            ScoreValue = scoreValue;
        }

        public void TakeDamage(int damage)
        {
            Health = Health - damage;
            if (Health < 0) Health = 0;
        }

        public bool IsAlive() { return Health > 0; }
    }
}

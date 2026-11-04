using GameProject.Models;
using GameProject.Repositories;
using System;
using System.Collections.Generic;

namespace GameProject.Services
{
    // The service contains rules and calculations, but no Console code.
    public class GameService
    {
        private readonly ScoreRepository _repository;
        private readonly Random _random = new Random();

        public GameService(ScoreRepository repository) { _repository = repository; }
        public Player CreatePlayer(string name) { return new Player(name); }

        public List<Monster> CreateMonsters()
        {
            List<Monster> monsters = new List<Monster>();
            monsters.Add(new Monster("Slime", 40, 8, 10));
            monsters.Add(new Monster("Goblin", 65, 12, 20));
            monsters.Add(new Monster("Dragon", 100, 16, 40));
            return monsters;
        }

        public int PlayerAttack(Player player, Monster monster)
        {
            int damage = CalculateDamage(player.AttackPower);
            monster.TakeDamage(damage);
            return damage;
        }

        public int MonsterAttack(Monster monster, Player player)
        {
            int damage = CalculateDamage(monster.AttackPower);
            player.TakeDamage(damage);
            return damage;
        }

        private int CalculateDamage(int attackPower)
        {
            return _random.Next(attackPower - 3, attackPower + 4);
        }

        public void GiveMonsterReward(Player player, Monster monster)
        {
            player.AddScore(monster.ScoreValue);
        }

        public int CalculateVictoryBonus(Player player)
        {

            return 0;
        }

        public void SaveResult(Player player, bool won)
        {
            string result = won ? "Won" : "Lost";
            _repository.Save(new GameResult(player.Name, player.Score, result));
        }

        public List<GameResult> GetScores() { return _repository.GetAll(); }

        public List<GameResult> GetTopScores(int numberOfScores)
        {
            // DIFFICULT STUDENT TODO (service task): build a leaderboard.
            // Requirements:
            // 1. Read all results 
            // 2. Sort them from highest score to lowest score
            // 3. If two players have the same score, order their names A-Z.
            // 4. Return only the requested number of results.
            // 5. Do not fail when the file contains fewer results than requested.
            // 6. Use nested loops to compare items, and use a temporary variable when swapping two items.

            // Fallback: the project stays runnable until students implement sorting.
            return _repository.GetAll();
        }
    }
}

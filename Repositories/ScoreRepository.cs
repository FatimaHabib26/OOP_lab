using GameProject.Models;
using System.Collections.Generic;
using System.IO;

namespace GameProject.Repositories
{
    // Only this layer reads and writes files.
    public class ScoreRepository
    {
        private readonly string _filePath;
        public ScoreRepository(string filePath) { _filePath = filePath; }

        public void Save(GameResult result)
        {
            string line = result.PlayerName + "|" + result.Score + "|" + result.Result;
            File.AppendAllText(_filePath, line + System.Environment.NewLine);
        }

        public List<GameResult> GetAll()
        {
            List<GameResult> results = new List<GameResult>();
            if (!File.Exists(_filePath)) return results;

            foreach (string line in File.ReadAllLines(_filePath))
            {
                string[] parts = line.Split('|');
                int score;
                // Ignore invalid file lines instead of crashing.
                if (parts.Length == 3 && int.TryParse(parts[1], out score))
                    results.Add(new GameResult(parts[0], score, parts[2]));
            }
            return results;
        }
    }
}

namespace GameProject.Models
{
    public class GameResult
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public string Result { get; set; }

        public GameResult(string playerName, int score, string result)
        {
            PlayerName = playerName;
            Score = score;
            Result = result;
        }
    }
}

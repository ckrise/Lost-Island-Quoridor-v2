using System;
using System.Collections.Generic;
using Board;

namespace ArtificialInteligence.GeneticLearning
{
    class AITraining
    {
        public static Random rnd = new Random();

        public AIForTraining champion;
        int numChampWins;
        public AIForTraining challenger;
        int numChallengerWins;
        int numDraws;

        public AITraining() {
            champion = new AIForTraining();
            challenger = champion.GetMutatedAI();
        }

        public void PlayGames(int numGames) {
            for (int i = 0; i < numGames; i++) {
                int winner = PlayGame();
                if (winner == 1)
                {
                    numChampWins++;
                }
                else if (winner == 2)
                {
                    numChallengerWins++;
                }
                else {
                    numDraws++;
                }
            }
        }

        private int PlayGame() {
            challenger.ResetBoard();
            champion.ResetBoard();

            int numMovesMade = 0;
            int winner = 0;
            AIBoard board = new AIBoard();
            
            string champMove;
            string challengerMove;

            champMove = champion.GetHardMove("");
            board.MakeMove(champMove);
            numMovesMade++;
            while (winner == 0) {
                challengerMove = challenger.GetHardMove(champMove);
                board.MakeMove(challengerMove);
                numMovesMade++;
                winner = board.GetWinner();
                if (winner != 0 || numMovesMade > 150) {
                    break;
                }

                champMove = champion.GetHardMove(challengerMove);
                board.MakeMove(champMove);
                numMovesMade++;
                winner = board.GetWinner();
                if (winner != 0 || numMovesMade > 150)
                {
                    break;
                }
            }
            return winner;
        }

        public void LogTheMatch() {
            using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(@"C:\Users\Jackson's HP\Desktop\AIBoardTesting\AIBattles.txt", true))
            {
                file.WriteLine("Game Datetime: " + DateTime.Now);
                file.Write("Champion weights: ");
                foreach (int weight in champion.EvalWeights) {
                    file.Write(weight.ToString());
                }
                file.WriteLine();
                file.Write("Challenger weights: ");
                foreach (int weight in challenger.EvalWeights)
                {
                    file.Write(weight.ToString() + " : ");
                }
                file.WriteLine();
                file.WriteLine("Champion Wins: " + numChampWins.ToString() + "    Challenger Wins: " +
                    numChallengerWins.ToString() + "     Draws: " + numDraws.ToString());
            }
        }

        public void DeclareVictor() {
            LogTheMatch();
            int max = Math.Max(Math.Max(numDraws, numChampWins), numChallengerWins);
            if (max == numChallengerWins)
            {
                PrepForNextShowdown(champion);
            }
            else if (max == numChampWins)
            {
                PrepForNextShowdown(challenger);
            }
            else {
                PrepForNextShowdown(new AIForTraining());
            }
        }

        private void PrepForNextShowdown(AIForTraining champ) {
            champion = new AIForTraining(champ);
            challenger = champion.GetMutatedAI();
            numChallengerWins = 0;
            numChampWins = 0;
            numDraws = 0;
        }
    }
}

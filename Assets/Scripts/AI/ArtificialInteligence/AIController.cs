namespace ArtificialInteligence
{
    /**
     * This class is used for all interactions with AI.
     **/
    public class AIController
    {
        private string difficulty;
        private AI computer;

        //Default constructor 
        public AIController()
        {
            computer = new AI();
            difficulty = "hard";
        }
        /**
         * Returns a move in the following format.
         * If wall, indicates the bottom left square then horizontal or vertical. Ex. B5h
         * If moving a piece, indicates only the tile moved to. Ex. A7
        **/
        public string GetMove(string playerMove)
        {
            string AImove;
            if (difficulty == "easy")
            {
                AImove = computer.GetEasyMove(playerMove);
            }
            else
            {
                AImove = computer.GetHardMove(playerMove);
            }
            return AImove;
        }

        public void SetAIHard()
        {
            difficulty = "hard";
        }

        public void SetAIEasy()
        {
            difficulty = "easy";
        }
    }
}

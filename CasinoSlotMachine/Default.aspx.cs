using System;
using System.Linq;

namespace CasinoSlotMachine
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState.Add("PlayersMoney", 1000);
                int[] spinArray = new int[3];
                ReelControl(1, out spinArray);  // Out parameter required for later but not used here
                PlayersMoneyControl(1);
            }
        }

        ///*** 1.  App Control***///
        protected void LeverButton_Click(object sender, EventArgs e)
        {
            // Initialize the two primary game values
            int playersMoney = 0;  // VS wants me to declare this variable inline with the BetControl call but doing that broke my code twice with no error messages
            int bet = 0;

            // Take in bet and determine that it is an int > 0 && <= playersMoney and instruct user accordingly
            bet = BetControl(bet, out playersMoney);
            if (!((bet > 0) && (playersMoney > 0) && (playersMoney >= bet))) return;

            // If all conditions are met, determine results
            int winnings = 0;
            playersMoney = DetermineResults(bet, playersMoney, out winnings);

            // Pass playersMoney back into ViewState and inform user of results
            PlayersMoneyControl(2, playersMoney, bet, winnings);
        }



        ///*** 2.  Results Determination Control (1 method) ***///
        private int DetermineResults(int bet, int playersMoney, out int winnings)
        {
            int winLoseMultiplier = SpinResultMultiplierControl();
            winnings = bet * winLoseMultiplier;
            playersMoney = playersMoney - bet + winnings;
            return playersMoney;
        }



        ///*** 3.  Bet Control (3 methods) ***///
        private int BetControl(int bet, out int playersMoney)
        {
            bet = TakeBet(bet, out playersMoney);
            if (bet <= 0)
            {
                ResultLabelControl(1);  // Instruct user to place bet
                return bet;
            }

            if ((bet > playersMoney) || (playersMoney <= 0)) ResultLabelControl(2);  // Instruct user there's not enough money to make this bet
            return bet;
        }

        private int TakeBet(int bet, out int playersMoney)
        {
            playersMoney = 0;
            playersMoney = PlayersMoney(1, playersMoney);  // How much money does player have left?
            string betString = betTextBox.Text.Trim();
            ParseBet(betString, out bet);  // Ensure bet text is an int
            return bet;
        }

        private void ParseBet(string betString, out int bet)  // Sends out bet if it's an int, regardless if it's > 0
        {
            if (!int.TryParse(betString, out bet)) return;
        }



        ///*** 4.  Player's Money Control (3 methods) ***///
        private void PlayersMoneyControl(int key, int playersMoney = 0, int bet = 0, int winnings = 0)
        {
            switch (key)
            {
                case 1: // From Page_Load: set initial state of 
                    PlayersMoneyLabel(PlayersMoney(1));
                    break;
                case 2: // Finalize labels and money
                    ResultLabelControl(3, bet, winnings);
                    PlayersMoneyLabel(playersMoney);
                    PlayersMoney(2, playersMoney);
                    break;
            }
        }

        private void PlayersMoneyLabel(int playersMoney)
        {
            playersMoneyLabel.Text = String.Format("{0:C}", playersMoney);
        }

        private int PlayersMoney(int key, int playersMoney = 0) 
        {
            switch (key)
            {
                case 1: // From Page_Load -> PlayersMoneyControl: Pulls initial bankroll from viewstate and sends it to PlayersMoneyLabel()
                    playersMoney = (int)ViewState["PlayersMoney"];
                    break;
                case 2: // From AppControl -> PlayersMoneyControl: Passes playersMoney + winnings back into ViewState
                    ViewState["PlayersMoney"] = playersMoney;
                    break;
            }

            return playersMoney;
        }



        ///*** 5.  Result Label Control (1 method) ***///
        private void ResultLabelControl(int key, int bet = 0, int winnings = 0)
        {
            switch (key)
            {
                case 1:
                    resultLabel.Text = "<span style=\"color: red;\">Please place your bet</span>";
                    break;
                case 2:
                    resultLabel.Text = "<span style=\"color: red;\">You don't have enough money to make this bet</span>";
                    break;
                case 3:
                    if (winnings == 0) resultLabel.Text = string.Format("Sorry, you lost {0:C}. Better luck next time", bet);
                    else resultLabel.Text = string.Format("You bet {0:C} and won {1:C}", bet, winnings);
                    break;
            }
        }



        ///*** 6.  Spin Result Multiplier Control (5 methods) ***///
        // Receives image indexes from ReelControl as codes to determine winnings
        private int SpinResultMultiplierControl()
        {
            int[] spinArray = new int[3];
            ReelControl(2, out spinArray);
            int multiplier = GetMultiplier(spinArray);
            return multiplier;
        }

        private int GetMultiplier(int[] spinArray)
        {
            // BAR checker
            if (CheckForBar(spinArray)) return 0;

            // Cherry checker
            int cherryMultiplier = CheckForCherries(spinArray);
            if (cherryMultiplier > 1) return cherryMultiplier;

            // Three sevens checker
            if (CheckForSevens(spinArray)) return 100;

            return 0;
        }

        private bool CheckForBar(int[] spinArray)
        {
            if (spinArray.Contains(0)) return true;
            else return false;
        }

        private int CheckForCherries(int[] spinArray)
        {
            int cherryMultiplier = 1;  // 1 cherry: x2; 2 cherries: x3; 3 cherries: x4;
            for (int i = 0; i < 3; i++)
            {
                if (spinArray[i] == 2) cherryMultiplier++;  // x2, x3, x4
            }
            return cherryMultiplier;
        }

        private bool CheckForSevens(int[] spinArray)
        {
            if ((spinArray[0] == 9) && (spinArray[1] == 9) && (spinArray[2] == 9))
                return true;
            else
                return false;
        }



        ///*** 7.  Reels Control (4 methods) ***///
        // Key 1 = initial spin only for page lode
        // Key 2 = populates spinArray[] with random image indexes for determining results
        private void ReelControl(int key, out int[] spinArray) // This method could be overloaded to avoid unnecessary declaraation in Page_Load
        {
            spinArray = new int[3];

            switch (key)
            {
                case 1:
                    Spin();
                    break;
                case 2:
                    spinArray = Spin();
                    break;
            }
        }


        Random random = new Random();
        int RandomNumber()
        {
            int randomNumber = random.Next(12);
            return randomNumber;
        }

        // In case 2, spinReels splits into ReelImage once for each index,
        // and is returned into spinArray for use in SpinResultsMultiplierControl()
        private int[] Spin()
        {
            int[] spinReels = new int[] { RandomNumber(), RandomNumber(), RandomNumber() };
            leftReelImage.ImageUrl = ReelImage(spinReels[0]);
            middleReelImage.ImageUrl = ReelImage(spinReels[1]);
            rightReelImage.ImageUrl = ReelImage(spinReels[2]);
            return spinReels;
        }

        string ReelImage(int randomNumber)
        {
            string[] Images = new string[12]
            {
                "Bar", "Bell", "Cherry", "Clover", "Diamond", "HorseShoe",
                "Lemon", "Orange", "Plum", "Seven", "Strawberry", "Watermellon"
            };

            string reelImage = "/Images/" + Images[randomNumber] + ".png";
            return reelImage;
        }
    }
}
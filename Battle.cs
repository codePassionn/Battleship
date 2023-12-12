using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Security;

namespace BattleShip
{
    public class Battle
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String[,] p1Board = new String[10, 10];
        private String[,] p2Board = new String[10, 10];

        private String[,] p2Hit = new String[10, 10];// the performance of player 2
        private String[,] p1Hit = new String[10, 10]; // the performance of player 1 

        private bool isPlayer1Disconnect;
        //p2Board = ship location record for player 2. same for player 1
        //p2 hit = where the player is aiming respectively to hit the opposite board. 

        private int hitCounterp1 = 0;
        private int hitCounterp2 = 0;

        private Boolean hasWon = false;

        public short winStatus = 0;

        private User player1, player2;

        public Battle(User u1, User u2)
        {
            isPlayer1Disconnect = false;
            player1 = u1;
            player2 = u2;

            initilizeVariables();
            helloPlayers();
        }

        public void beginGame()
        {
            // Get user 1 to deploy the ships
            sendMessageToPlayer(player2, "Info,Waiting for player 1 to deploy ships.");
            deployShips(player1, p1Board, "P1");
            sendMessageToPlayer(player2, "Info,Player 1 has deployed the ships.");

            // Get user 2 to deploy the ships
            sendMessageToPlayer(player1, "Info,Waiting for player 2 to deploy ships.");
            deployShips(player2, p2Board, "P2");
            sendMessageToPlayer(player1, "Info,Player 2 has deployed the ships.");

            sendMessageToPlayer(player1, "Info,Let's start hitting.");
            sendMessageToPlayer(player2, "Info,Let's start hitting.");

            try
            {
                while (!hasWon)
                {
                    playHitGames();
                }
                if (hitCounterp1 == 17 && hitCounterp2 == 17)
                {
                    // It's a draw
                    sendMessageToPlayer(player1, "Info,The game ended in a draw.");
                    sendMessageToPlayer(player2, "Info,The game ended in a draw.");
                }
                if (hitCounterp1 == 17)
                {
                    // Player 1 won
                    winStatus = 1;
                    sendMessageToPlayer(player1, "Info,YOU WON!!!");
                    sendMessageToPlayer(player2, "Info,YOU LOST!!! Player 1 wins.");
                }
                if (hitCounterp2 == 17)
                {
                    // Player 2 won
                    winStatus = 2;
                    sendMessageToPlayer(player2, "Info,YOU WON!!!");
                    sendMessageToPlayer(player1, "Info,YOU LOST!!! Player 2 wins.");
                }
            }
            catch (Exception ex)
            { // Someone got disconnected
                log.Error(ex.ToString());
                if (isPlayer1Disconnect) // player 2 wins
                {
                    winStatus = 2;
                }
                else // player 1 wins
                {
                    winStatus = 1;
                }
            }
            finally
            {

            }
        }

        private void deployShips(User p, string[,] board, string pl)
        {
            //Console.Clear();
            //Console.WriteLine("This is the turn for Player 1!  Player 2: Leave!");
            //Console.ReadLine();
            //Console.Clear();
            int rowN = 0;
            int colN = 0;
            string selection;
            string direction; // save the direction of the ship, v for vertical, h for horizontal.


            //-----------------------------------------------
            sendMessageToPlayer(p, "Info," + showBoard(pl));
            sendMessageToPlayer(p,"Read,Please select the location of your ship patrol based on the graph");
            selection = ReadMessage(p);
            sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
            direction = ReadMessage(p);

            while (!(inputValidation(selection, 2, direction) && shipSizeValidate(selection, 2, direction, board))) //-input validation process 
            {
                sendMessageToPlayer(p, "Read,Invalid Input - Please input in the format of two character string - the first character is from A to I - the second is from 0 to 9 for the location!!");
                selection = ReadMessage(p);
                sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
                direction = ReadMessage(p);
            }

            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                board[rowN, colN] = "S";
                board[rowN + 1, colN] = "S";
            }
            else
            {
                board[rowN, colN] = "S";
                board[rowN, colN + 1] = "S";
            }

            //------------------------------------------------------------------------------------------------------------
            sendMessageToPlayer(p, "Info," + showBoard(pl));
            sendMessageToPlayer(p, "Read,Please select the location of your Destroyer ship  based on the graph");
            selection = ReadMessage(p);
            sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
            direction = ReadMessage(p);

            while (!(inputValidation(selection, 3, direction) && shipSizeValidate(selection, 3, direction, board))) //-input validation process 
            {
                sendMessageToPlayer(p, "Read,Invalid Input - Please input in the format of two character string - the first character is from A to H - the second is from 0 to 9!! for the location");
                selection = ReadMessage(p);
                sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
                direction = ReadMessage(p);
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                board[rowN, colN] = "S";
                board[rowN + 1, colN] = "S";
                board[rowN + 2, colN] = "S";
            }
            else
            {
                board[rowN, colN] = "S";
                board[rowN, colN + 1] = "S";
                board[rowN, colN + 2] = "S";
            }

            //--------------------------
            sendMessageToPlayer(p, "Info," + showBoard(pl));
            sendMessageToPlayer(p, "Read,Please select the location of your Submarine ship based on the graph");            
            selection = ReadMessage(p);
            sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
            direction = ReadMessage(p);
            while (!(inputValidation(selection, 3, direction) && shipSizeValidate(selection, 3, direction, board))) //-input validation process 
            {
                sendMessageToPlayer(p, "Read,Invalid Input - Please input in the format of two character string - the first character is from A to H - the second is from 0 to 9 for the location!!");
                selection = ReadMessage(p);
                sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
                direction = ReadMessage(p);
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                board[rowN, colN] = "S";
                board[rowN + 1, colN] = "S";
                board[rowN + 2, colN] = "S";
            }
            else
            {
                board[rowN, colN] = "S";
                board[rowN, colN + 1] = "S";
                board[rowN, colN + 2] = "S";
            }


            //--------------------------
            sendMessageToPlayer(p, "Info," + showBoard(pl));
            sendMessageToPlayer(p, "Read,Please select the location of your BattleShip ship based on the graph");
            selection = ReadMessage(p);
            sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
            direction = ReadMessage(p);
            while (!(inputValidation(selection, 4, direction) && shipSizeValidate(selection, 4, direction, board))) //-input validation process 
            {
                sendMessageToPlayer(p, "Read,Invalid Input - Please input in the format of two character string - the first character is from A to G - the second is from 0 to 9!!");
                selection = ReadMessage(p);
                sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
                direction = ReadMessage(p);
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                board[rowN, colN] = "S";
                board[rowN + 1, colN] = "S";
                board[rowN + 2, colN] = "S";
                board[rowN + 3, colN] = "S";
            }
            else
            {
                board[rowN, colN] = "S";
                board[rowN, colN + 1] = "S";
                board[rowN, colN + 2] = "S";
                board[rowN, colN + 3] = "S";
            }

            //---------------------------------------------------
            sendMessageToPlayer(p, "Info," + showBoard(pl));
            sendMessageToPlayer(p, "Read,Please select the location of your Carrier ship based on the graph");
            selection = ReadMessage(p);
            sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
            direction = ReadMessage(p);
            while (!(inputValidation(selection, 5, direction) && shipSizeValidate(selection, 5, direction, board))) //-input validation process 
            {
                sendMessageToPlayer(p, "Read,Invalid Input - Please input in the format of two character string - the first character is from A to F - the second is from 0 to 9!!");
                selection = ReadMessage(p);
                sendMessageToPlayer(p, "Read,Please select the direction of your ship - V for vertical - H for horizontal");
                direction = ReadMessage(p);
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                board[rowN, colN] = "S";
                board[rowN + 1, colN] = "S";
                board[rowN + 2, colN] = "S";
                board[rowN + 3, colN] = "S";
                board[rowN + 4, colN] = "S";
            }
            else
            {
                board[rowN, colN] = "S";
                board[rowN, colN + 1] = "S";
                board[rowN, colN + 2] = "S";
                board[rowN, colN + 3] = "S";
                board[rowN, colN + 4] = "S";
            }
            sendMessageToPlayer(p, "Info," + showBoard(pl));
        }

        private bool shipSizeValidate(string selection, int v, string direction, string[,] board)
        {
            int rowN = Convert.ToInt32(selection[0]) - 65;
            int colN = Convert.ToInt32(selection[1]) - 48;
            switch (direction)
            {
                case "H":
                    {
                        for (int i = colN; i < (colN + v); i++)
                        {
                            if (i >= 10 || !board[rowN, i].Equals(" "))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                case "V":
                    {
                        for (int i = rowN; i < (rowN + v); i++)
                        {
                            if (i >= 10 || !board[i, colN].Equals(" "))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
            }
            return false;
        }

        void initilizeVariables()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    p1Board[i, j] = " ";
                    p2Board[i, j] = " ";
                    p1Hit[i, j] = " ";
                    p2Hit[i, j] = " ";
                }
            }
        }

        void Main(string[] args)
        {

            // showBoard("P1");
            initilizeVariables();

            helloPlayers();
            queryInputData();
            seeYouPlayers();
        }

        void helloPlayers()
        {
            sendMessageToPlayer(player1, "Info,Welcome to Battleship.");
            sendMessageToPlayer(player2, "Info,Welcome to Battleship.");
            //Console.Title = "Battleship";
            //Console.WriteLine("Welcome to Battleship.\n\nPress any key to continue");
            //Console.ReadLine();

        }

        void queryInputData()
        {
            inputShipLocation();
            while (!hasWon)
            {
                playHitGames();
            }

        }

        void seeYouPlayers()
        {
            Console.Clear();
            Console.WriteLine("Thank you for playing Battleship!");
            Console.WriteLine("The winner is :");
            if (hitCounterp1 == 17)
                Console.WriteLine("Player 1!");
            else
                Console.WriteLine("Player 2!");
            Console.WriteLine("Thank you for playing.");
            Console.ReadLine();
        }


        //----------Initilization and input deploit for 2 players. 
        void inputShipLocation()
        {
            Console.Clear();
            Console.WriteLine("This is the turn for Player 1!  Player 2: Leave!");
            Console.ReadLine();
            Console.Clear();
            int rowN = 0;
            int colN = 0;
            string selection;
            string direction; // save the direction of the ship, v for vertical, h for horizontal.


            //-----------------------------------------------
            Console.WriteLine("Please select the location of your ship patrol based on the graph");
            showBoard("P1");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();

            while (!inputValidation(selection, 2, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to I, the second is from 0 to 9 for the location!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }

            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN + 1, colN] = "S";
            }
            else
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN, colN + 1] = "S";
            }

            //------------------------------------------------------------------------------------------------------------
            Console.WriteLine("Please select the location of your Destroyer ship  based on the graph");
            showBoard("P1");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();

            while (!inputValidation(selection, 3, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to H, the second is from 0 to 9!! for the location");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN + 1, colN] = "S";
                p1Board[rowN + 2, colN] = "S";
            }
            else
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN, colN + 1] = "S";
                p1Board[rowN, colN + 2] = "S";
            }

            //--------------------------
            Console.WriteLine("Please select the location of your Submarine ship based on the graph");
            showBoard("P1");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 3, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to H, the second is from 0 to 9 for the location!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN + 1, colN] = "S";
                p1Board[rowN + 2, colN] = "S";
            }
            else
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN, colN + 1] = "S";
                p1Board[rowN, colN + 2] = "S";
            }


            //--------------------------
            Console.WriteLine("Please select the location of your BattleShip ship based on the graph");
            showBoard("P1");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 4, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to G, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN + 1, colN] = "S";
                p1Board[rowN + 2, colN] = "S";
                p1Board[rowN + 3, colN] = "S";
            }
            else
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN, colN + 1] = "S";
                p1Board[rowN, colN + 2] = "S";
                p1Board[rowN, colN + 3] = "S";
            }

            //---------------------------------------------------
            Console.WriteLine("Please select the location of your Carrier ship based on the graph");
            showBoard("P1");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 5, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to F, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN + 1, colN] = "S";
                p1Board[rowN + 2, colN] = "S";
                p1Board[rowN + 3, colN] = "S";
                p1Board[rowN + 4, colN] = "S";
            }
            else
            {
                p1Board[rowN, colN] = "S";
                p1Board[rowN, colN + 1] = "S";
                p1Board[rowN, colN + 2] = "S";
                p1Board[rowN, colN + 3] = "S";
                p1Board[rowN, colN + 4] = "S";
            }


            //---------------------Initilization for Player 2
            //-------------------------------------------------------
            Console.Clear();
            Console.WriteLine("Player 2! it is your turn! Press any key to continue.");
            Console.ReadLine();

            Console.WriteLine("Please select the location of your Patrol ship bsed on the graph");
            showBoard("P2");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 2, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to I, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN + 1, colN] = "S";
            }
            else
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN, colN + 1] = "S";
            }
            //-----------------------------------------------------
            Console.WriteLine("Please select the location of your Destroyer ship based on the graph");
            showBoard("P2");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 3, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to H, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN + 1, colN] = "S";
                p2Board[rowN + 2, colN] = "S";
            }
            else
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN, colN + 1] = "S";
                p2Board[rowN, colN + 2] = "S";
            }


            //---------------------------------------------------
            Console.WriteLine("Please select the location of your Submarine ship based on the graph");
            showBoard("P2");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 3, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to H, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN + 1, colN] = "S";
                p2Board[rowN + 2, colN] = "S";
            }
            else
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN, colN + 1] = "S";
                p2Board[rowN, colN + 2] = "S";
            }

            //---------------------------------------------------
            Console.WriteLine("Please select the location of your BattleShip ship based on the graph");
            showBoard("P2");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 4, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to G, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN + 1, colN] = "S";
                p2Board[rowN + 2, colN] = "S";
                p2Board[rowN + 3, colN] = "S";
            }
            else
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN, colN + 1] = "S";
                p2Board[rowN, colN + 2] = "S";
                p2Board[rowN, colN + 3] = "S";
            }


            //------------------------------------------------------------

            Console.WriteLine("Please select the location of your Carrier ship based on the graph");
            showBoard("P2");
            selection = Console.ReadLine();
            Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
            direction = Console.ReadLine();
            while (!inputValidation(selection, 5, direction)) //-input validation process 
            {
                Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to F, the second is from 0 to 9!!");
                selection = Console.ReadLine();
                Console.WriteLine("Please select the direction of your ship, V for vertical, H for horizontal");
                direction = Console.ReadLine();
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            if (direction == "V")
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN + 1, colN] = "S";
                p2Board[rowN + 2, colN] = "S";
                p2Board[rowN + 3, colN] = "S";
                p1Board[rowN + 4, colN] = "S";

            }
            else
            {
                p2Board[rowN, colN] = "S";
                p2Board[rowN, colN + 1] = "S";
                p2Board[rowN, colN + 2] = "S";
                p2Board[rowN, colN + 3] = "S";
                p1Board[rowN, colN + 4] = "S";

            }





        }

        void playHitGames()
        {
            //Console.Clear();
            string selection;
            int rowN = 0;
            int colN = 0;
            //-----------------------------

            //Console.WriteLine("Player 1: please enter your seledtion based on the previous hit choice.");
            //showBoard("P1Hit");
            sendMessageToPlayer(player2, "Info,Player 1 is making his move. Player 1 current state is: \n" + showBoard("P1Hit"));
            sendMessageToPlayer(player2, "Info,Your current state is: \n" + showBoard("P2Hit"));

            
            sendMessageToPlayer(player1, "Info,Player 2 current state is: \n" + showBoard("P2Hit"));
            sendMessageToPlayer(player1, "Info,Your current state is: \n" + showBoard("P1Hit"));

            sendMessageToPlayer(player1, "Read,Please enter your selection based on the previous hit choice.");

            selection = ReadMessage(player1);
            while (!(validateMove(selection) && !alreadySelected(p1Hit, selection))) //-input validation process 
            {
                //Console.WriteLine("Invalid Input, Please input in the format of two character string, the first character is from A to J, the second is from 0 to 9!!");
                //selection = Console.ReadLine();
                sendMessageToPlayer(player1, "Read,Invalid Input - Please input in the format of two character string - the first character is from A to J - the second is from 0 to 9!!");
                selection = ReadMessage(player1);
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            sendMessageToPlayer(player2, "Info, Player 1 attacked: " + selection);
            if (p2Board[rowN, colN] != " " && p1Hit[rowN, colN] != "H")
            {
                sendMessageToPlayer(player1, "Info,HIT!");
                sendMessageToPlayer(player2, "Info,Player 1 HIT!");
                //Console.WriteLine("HIT!");
                p1Hit[rowN, colN] = "H";
                hitCounterp1++;
                if (hitCounterp1 == 17)
                {
                    hasWon = true;
                }
            }
            else
            {
                sendMessageToPlayer(player1, "Info,MISS!");
                sendMessageToPlayer(player2, "Info,Player 1 MISS!");
                //Console.WriteLine("MISS!");
                p1Hit[rowN, colN] = "M";
            }

            //Console.WriteLine("\nPress any key to continue.");
            //Console.ReadLine();
            //Console.Clear();
            
            sendMessageToPlayer(player1, "Info,Player 2 is making his move. Player 2 current state is: \n" + showBoard("P2Hit"));
            sendMessageToPlayer(player1, "Info,Your current state is: \n" + showBoard("P1Hit"));

            sendMessageToPlayer(player2, "Info,Player 1 current state is: \n" + showBoard("P1Hit"));
            sendMessageToPlayer(player2, "Info,Your current state is: \n" + showBoard("P2Hit"));
            
            sendMessageToPlayer(player2, "Read,Please enter your selection based on the previous hit choice.");
            //Console.WriteLine("Player 2: please enter your selection based on the previous hit choice.");
            //showBoard("P2Hit");
            //selection = Console.ReadLine();
            selection = ReadMessage(player2);
            while (!(validateMove(selection) && !alreadySelected(p2Hit, selection))) //-input validation process 
            {
                sendMessageToPlayer(player2, "Info,Invalid Input - Please input in the format of two character string - the first character is from A to J - the second is from 0 to 9!!");
                selection = ReadMessage(player2);
            }
            rowN = Convert.ToInt32(selection[0]) - 65;
            colN = Convert.ToInt32(selection[1]) - 48;
            sendMessageToPlayer(player1, "Info, Player 2 attacked: " + selection);
            if (p1Board[rowN, colN] != " " && p2Hit[rowN, colN] != "H")
            {
                sendMessageToPlayer(player2, "Info,HIT!");
                sendMessageToPlayer(player1, "Info,Player 2 HIT!");
                //Console.WriteLine("HIT!");
                p2Hit[rowN, colN] = "H";
                hitCounterp2++;
                if (hitCounterp2 == 17)
                {
                    hasWon = true;
                }
            }
            else
            {
                sendMessageToPlayer(player2, "Info,MISS!");
                sendMessageToPlayer(player1, "Info,Player 2 MISS!");
                //Console.WriteLine("MISS!");
                p2Hit[rowN, colN] = "M";
            }
            //Console.WriteLine("\nPress any key to continue.");
            //Console.ReadLine();
        }

        private bool alreadySelected(string[,] p1Board, string selection)
        {
            int rowN = Convert.ToInt32(selection[0]) - 65;
            int colN = Convert.ToInt32(selection[1]) - 48;
            if (p1Board[rowN, colN].Equals(" "))
                return false;
            return true;
        }

        private string showBoard(String selection)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("  ");
            for (int i = 0; i < 10; i++)
            {
                sb.Append(" " + i);
            }

            sb.AppendLine();
            char startRow = 'A';
            if (selection.Equals("P1"))
            {
                for (int x = 0; x < 10; x++)
                {
                    sb.Append(startRow + "  ");
                    for (int i = 0; i < 10; i++)
                    {
                        sb.Append(p1Board[x, i] + "|");
                        //Console.Write(p1Board[x, i] + "|");
                    }
                    sb.AppendLine();
                    startRow++;
                    //Console.WriteLine();
                }
            }
            else if (selection.Equals("P2"))
            {
                for (int x = 0; x < 10; x++)
                {
                    sb.Append(startRow + "  ");
                    for (int i = 0; i < 10; i++)
                    {
                        sb.Append(p2Board[x, i] + "|");
                        //Console.Write(p2Board[x, i] + "|");
                    }
                    sb.AppendLine();
                    startRow++;
                    //Console.WriteLine();
                }
            }
            else if (selection.Equals("P1Hit"))
            {
                for (int x = 0; x < 10; x++)
                {
                    sb.Append(startRow + "  ");
                    for (int i = 0; i < 10; i++)
                    {
                        sb.Append(p1Hit[x, i] + "|");
                        //Console.Write(p1Hit[x, i] + "|");
                    }
                    sb.AppendLine();
                    startRow++;
                    //Console.WriteLine();
                }
            }
            else
            {
                for (int x = 0; x < 10; x++)
                {
                    sb.Append(startRow + "  ");
                    for (int i = 0; i < 10; i++)
                    {
                        sb.Append(p2Hit[x, i] + "|");
                        //Console.Write(p2Hit[x, i] + "|");
                    }
                    sb.AppendLine();
                    startRow++;
                    //Console.WriteLine();
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }
        
        /*----------------------------------------------
         * this is validation for input when deploying and hitting the ships.  
         * there are 2 parameters. 
         * option is for choosing input circumstance. 
         *patternV1 is for regular validation while hitting the ships
         * patternV2 is validation of placing patrol ship during ship choicde at the beginning of the game
         * patternV3 is for placing destroyer and submarine ship
         * patternV4 is for placing battleship validation
         * patternV5 is for validation while placing air carrier. 
         * patternH2 is for validation of horizontal placing patrol ship
         * patternH3  is for validation of horizontal placing desstroy and submarine ship
         * patternH4 is for validation of horizontal placing battleship ship
         * patternH5 is for validation of horizontal placing carrier ship
         * --------------------------------------------*/
        bool inputValidation(string input, short option, string direction)
        {

            string patternV1 = @"^[A-I][0-9]$";
            string patternV2 = @"^[A-H][0-9]$";
            string patternV3 = @"^[A-H][0-9]$";
            string patternV4 = @"^[A-G][0-9]$";
            string patternV5 = @"^[A-F][0-9]$";

            string patternH1 = @"^[A-J][0-8]$";
            string patternH2 = @"^[A-J][0-7]$";
            string patternH3 = @"^[A-J][0-7]$";
            string patternH4 = @"^[A-J][0-6]$";
            string patternH5 = @"^[A-J][0-5]$";

            string patternDiren = @"^[VH]$";

            if (Regex.IsMatch(direction, patternDiren) == false)
                return false;

            bool flag = false;
            if (direction == "V")
            {
                switch (option)
                {
                    case 1:
                        flag = Regex.IsMatch(input, patternV1);
                        break;
                    case 2:
                        flag = Regex.IsMatch(input, patternV2);
                        break;
                    case 3:
                        flag = Regex.IsMatch(input, patternV3);
                        break;
                    case 4:
                        flag = Regex.IsMatch(input, patternV4);
                        break;
                    case 5:
                        flag = Regex.IsMatch(input, patternV5);
                        break;
                }

            }
            else
            {
                switch (option)
                {
                    case 1:
                        flag = Regex.IsMatch(input, patternH1);
                        break;
                    case 2:
                        flag = Regex.IsMatch(input, patternH2);
                        break;
                    case 3:
                        flag = Regex.IsMatch(input, patternH3);
                        break;
                    case 4:
                        flag = Regex.IsMatch(input, patternH4);
                        break;
                    case 5:
                        flag = Regex.IsMatch(input, patternH5);
                        break;
                }
            }
            return flag;
        }

        bool validateMove(string input)
        {
            string pattern = @"^[A-J][0-9]$";
            return Regex.IsMatch(input, pattern);
        }

        private bool sendMessageToPlayer(User u, string message)
        {
            try
            {
                message += "<EOF>";
                u.sslStream.Write(Encoding.UTF8.GetBytes(message));
            }
            catch (System.IO.IOException ex)
            {
                log.Error(ex.ToString());
                if (u == player1)
                    isPlayer1Disconnect = true;
                else
                    isPlayer1Disconnect = false;
                throw ex;
            }
            return true;
        }

        private string ReadMessage(User u)
        {
            try
            {
                // Read the  message sent by the client.
                // The client signals the end of the message using the
                // "<EOF>" marker.
                byte[] buffer = new byte[2048];
                StringBuilder messageData = new StringBuilder();
                int bytes = -1;
                do
                {
                    // Read the client's test message.
                    bytes = u.sslStream.Read(buffer, 0, buffer.Length);

                    // Use Decoder class to convert from bytes to UTF8
                    // in case a character spans two buffers.
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                    // Check for EOF or an empty message.
                    if (messageData.ToString().IndexOf("<EOF>") != -1)
                    {
                        break;
                    }
                } while (bytes != 0);

                return messageData.ToString().Replace("<EOF>","");
            }
            catch (System.IO.IOException ex)
            {
                log.Error(ex.ToString());
                if (u == player1)
                    isPlayer1Disconnect = true;
                else
                    isPlayer1Disconnect = false;
                throw ex;
            }

        }
    }
}
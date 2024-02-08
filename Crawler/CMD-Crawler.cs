using System;
using System.Windows.Input;
using System.IO;

namespace Crawler
{
    /**
     * The main class of the Dungeon Crawler Application
     * 
     * You may add to your project other classes which are referenced.
     * Complete the templated methods and fill in your code where it says "Your code here".
     * Do not rename methods or variables which already exist or change the method parameters.
     * You can do some checks if your project still aligns with the spec by running the tests in UnitTest1
     * 
     * For Questions do contact us!
     */
    public class CMDCrawler
    {
        /**
         * use the following to store and control the next movement of the yser
         */
        public enum PlayerActions { NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, QUIT };
        bool running = false; //Shows if game is active or not
        public PlayerActions action = PlayerActions.NOTHING; // Determines what the player is currently doing
        public string[] textmap; // String array of each line of the map
        public char[][] currentMap; // char jagged array of the currentmap
        public string mapName = string.Empty; //mapName loads in the map
        public int[] position = { 0, 0 }; // determines player position
        public int[] monsterpos = { 0, 0 }; // determines monster position
        public bool originalPlayerPos = false; //shows whether the player is in the original position
        public bool originalMonsterPos = true; //determines whether the monster is in its original position
        public bool mapLoaded = false; // determines whether the map is loaded
        public int goldCount = 0; // keeps track of gold
        bool onMenu = true; //determines whether you are on the menu
        public bool playPressed = false; //determines whether you have typed play
        public char[][] emptyMap = new char[0][]; // this is returned when a map isnt loaded

        /**
         * tracks if the game is running
         */
        public StreamReader sr;
        private bool active = true; // game is set to active

        /**
         * Reads user input from the Console
         * 
         * Please use and implement this method to read the user input.
         * 
         * Return the input as string to be further processed
         * 
         */
        private string ReadUserInput() // All the user input reads need to be hear as they're not called from the tests.
        {
            string inputRead = string.Empty; //input is initialized to zero
            if (currentMap != emptyMap) // if the map isnt empty ask for user input to load the map
            {
                if (onMenu)
                {
                    Console.Write("Enter Load, followed by the map you want to select: "); // asks for user input
                    ProcessUserInput(Console.ReadLine());  // Processes user input for menu.
                }
                else
                {
                    ProcessUserInput(Convert.ToString(Console.ReadKey().KeyChar));
                }
                return inputRead;
            }
            return inputRead;

           
        }

        /**
         * Processed the user input string
         * 
         * takes apart the user input and does control the information flow
         *  * initializes the map ( you must call InitializeMap)
         *  * starts the game when user types in Play
         *  * sets the correct playeraction which you will use in the GameLoop
         */

        public void ProcessUserInput(string input)
        {
            // Processes each input and assigns the appropriate player actions
            if (mapLoaded)
            {
                if (active) {
                    if (input == "play") // if the input is play go to gameloop
                    {
                        GameLoop(true);
                    }
                }
            }
            if (input.ToLower() == "load simple.map" || input.ToLower() == "load simple")
            {
                mapName = "Simple.map";
                active = true;
                onMenu = false;
                mapLoaded = true;
                InitializeMap(mapName); // initializes simple
            }
            else if (input.ToLower() == "load advanced.map" || input.ToLower() == "load advanced")
            {
                mapName = "Advanced.map";
                active = true;
                onMenu = false;
                mapLoaded = true;
                InitializeMap(mapName); //initializes advanced
            }
            else
            {
                if (input.Length < 1)
                {
                    handleKeyStroke(input);
                }
                else
                {
                    action = PlayerActions.NOTHING;
                    handleKeyStroke(input);
                }
            }

        }

        private void handleKeyStroke(string s)
        {
            position = GetPlayerPosition();
            int x = position[1];
            position[1] = position[0];
            position[0] = x;
            char temp;
            // Remove this and all Console.Read*
            if (active)
            { 
                if (running)
                {
                    try {
                        if (s.ToLower() == "w")// Converts the capital w from the conversion of the ConsoleKey
                        {
                            action = PlayerActions.NORTH;
                            try
                            {
                                if (currentMap[position[0] - 1][position[1]] == '.')
                                {
                                    temp = currentMap[position[0] - 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] - 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = temp; // above line and current swaps the player position character with the character above
                                }
                                else if (currentMap[position[0] - 1][position[1]] == 'G')
                                {
                                    temp = currentMap[position[0] - 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] - 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.'; // above line and current swaps the player position character with the character above
                                    goldCount = goldCount + 10;
                                }
                                else if (currentMap[position[0] - 1][position[1]] == 'E')
                                {
                                    temp = currentMap[position[0] - 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1]] = 'E';
                                    currentMap[position[0] - 1][position[1]] = '@';
                                    Console.WriteLine("You finished with, " + goldCount + ". Well Done. ");
                                    running = false;
                                    action = PlayerActions.QUIT;
                                }
                                else if (currentMap[position[0] - 1][position[1]] == 'M')
                                {
                                    temp = currentMap[position[0] - 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] - 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.';
                                    Console.WriteLine("You Died with, " + goldCount + ". Better Luck Next time. ");
                                    running = false;
                                    action = PlayerActions.QUIT;
                                }
                                else
                                {
                                    Console.WriteLine("You have reached the edge of the map.");
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Edge of map");
                            }
                        }
                        else if (s.ToLower() == "a")  // Converts the capital a from the conversion of the ConsoleKey
                        {
                            action = PlayerActions.WEST;
                            try
                            {
                                if (currentMap[position[0]][position[1] - 1] == '.')
                                {
                                    temp = currentMap[position[0]][position[1] - 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] - 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = temp;
                                }
                                else if (currentMap[position[0]][position[1] - 1] == 'G')
                                {
                                    temp = currentMap[position[0]][position[1] - 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] - 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.';
                                    goldCount = goldCount + 10;
                                }
                                else if (currentMap[position[0]][position[1] - 1] == 'E')
                                {
                                    temp = currentMap[position[0]][position[1] - 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] - 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1] + 1] = 'E';
                                    running = false;
                                    Console.WriteLine("You finished with, " + goldCount + ". Well Done. ");
                                    action = PlayerActions.QUIT;
                                }
                                else if (currentMap[position[0]][position[1] - 1] == 'M')
                                {
                                    temp = currentMap[position[0]][position[1] - 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] - 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.';
                                    Console.WriteLine("You Died with, " + goldCount + ". Better Luck Next time. ");
                                    running = false;
                                    action = PlayerActions.QUIT;
                                }
                                else
                                {
                                    Console.WriteLine("You are at the edge of the map.");
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Edge of map");
                            }
                        }
                        else if (s.ToLower() == "s") // Converts the capital S from the conversion of the ConsoleKey
                        {
                            action = PlayerActions.SOUTH;
                            try
                            {
                                if (currentMap[position[0] + 1][position[1]] == '.')
                                {
                                    temp = currentMap[position[0] + 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] + 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = temp;
                                }
                                else if (currentMap[position[0] + 1][position[1]] == 'G')
                                {
                                    temp = currentMap[position[0] + 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] + 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.';
                                    goldCount = goldCount + 10;
                                }
                                else if (currentMap[position[0] + 1][position[1]] == 'E')
                                {
                                    temp = currentMap[position[0] + 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] + 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0] + 1][position[1]] = 'E';
                                    running = false;
                                    action = PlayerActions.QUIT;
                                }
                                else if (currentMap[position[0] + 1][position[1]] == 'M')
                                {
                                    temp = currentMap[position[0] + 1][position[1]]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0] + 1][position[1]] = currentMap[position[0]][position[1]];
                                    currentMap[position[0] + 1][position[1]] = '.';
                                    Console.WriteLine("You Died with, " + goldCount + ". Better Luck Next time. ");
                                    running = false;
                                    Console.WriteLine("You finished with, " + goldCount + ". Well Done. ");
                                    action = PlayerActions.QUIT;
                                }
                                else
                                {
                                    Console.WriteLine("You have reached the edge of the map.");
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Edge of map");
                            }
                        }
                        else if (s.ToLower() == "d")
                        {
                            action = PlayerActions.EAST;
                            try
                            {
                                if (currentMap[position[0]][position[1] + 1] == '.')
                                {
                                    temp = currentMap[position[0]][position[1] + 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] + 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = temp;
                                }
                                else if (currentMap[position[0]][position[1] + 1] == 'G')
                                {
                                    temp = currentMap[position[0]][position[1] + 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] + 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.';
                                    goldCount = goldCount + 10;
                                }
                                else if (currentMap[position[0]][position[1] + 1] == 'E')
                                {
                                    temp = currentMap[position[0]][position[1] + 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1]] = '.';
                                    currentMap[position[0]][position[1] + 1] = 'E';
                                    running = false;
                                    Console.WriteLine("You finished with, " + goldCount + ". Well Done. ");
                                    action = PlayerActions.QUIT;
                                }
                                else if (currentMap[position[0]][position[1] + 1] == 'M')
                                {
                                    temp = currentMap[position[0]][position[1] + 1]; // CurrentMap is the jagged array, position is the player position in an array 
                                    currentMap[position[0]][position[1] + 1] = currentMap[position[0]][position[1]];
                                    currentMap[position[0]][position[1]] = '.';
                                    Console.WriteLine("You Died with, " + goldCount + ". Better Luck Next time. ");
                                    running = false;
                                    action = PlayerActions.QUIT;
                                }
                                else
                                {
                                    Console.WriteLine("You have reached the edge of the map.");
                                    action = PlayerActions.NOTHING;
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Edge of map");
                            }
                        }
                    }
                    catch
                    {
                        action = PlayerActions.NOTHING;
                    }
                }
            }
        }

        /**
         * The Main Game Loop. 
         * It updates the game state.
         * 
         * This is the method where you implement your game logic and alter the state of the map/game
         * use playeraction to determine how the character should move/act
         * the input should tell the loop if the game is active and the state should advance
         */
        public void GameLoop(bool active)
        {
            running = true;
            // Your code here
            if (onMenu != true)
            {
                try
                {
                    Console.Clear();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("Amount of gold: " + goldCount);
                printJaggedArray(currentMap);
            }
        }
        public void printJaggedArray(char[][] arr)
        {
            foreach (char[] first in arr)
            {
                foreach (char second in first)
                {
                    Console.Write(second);
                }
                Console.WriteLine("");
            }
        }

        /**
        * Map and GameState get initialized
        * mapName references a file name 
        * 
        * Create a private object variable for storing the map in Crawler and using it in the game.
        */
        public bool InitializeMap(String mapName)
        {
            bool initSuccess = false;
            // Your code here
            if (mapName != String.Empty)
            {
                textmap = System.IO.File.ReadAllLines(@"..\..\..\maps\" + mapName);
                initSuccess = true;
                GetOriginalMap();
            }
            else
            {
                initSuccess = false;
            }

            return initSuccess;
        }
        public char[][] createCharacter(char[][] map)
        {
            int i = 0;
            int x = 0;
            if (currentMap != null)
            {
                try
                {
                    for (; i < currentMap.Length; i++)
                    {
                        for (; x < currentMap[i].Length - 1; x++)
                        {
                            if (currentMap[i][x].ToString().Contains('S'))
                            {
                                currentMap[i][x] = '@';
                                map = currentMap;
                            }
                        }
                        x = 0;
                    }
                }
                catch
                {
                    return emptyMap;
                }
                return map;
            }
            else
            {
                InitializeMap(mapName);
            }
            return map;
        }
        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         */
        public char[][] GetOriginalMap()
        {
            if (textmap != null)
            {
                char[][] map = new char[textmap.Length][];
                Console.WriteLine(textmap);
                // Your code here
                for (int i = 0; i < textmap.Length; i++)
                {
                    char[] currentline = textmap[i].ToCharArray();
                    map[i] = currentline;
                }
                currentMap = map;
                Console.WriteLine(map);
                return map;
            }
            else
            {
                InitializeMap(mapName);
                emptyMap = new char[0][];
                return emptyMap;
            }
        }

        /*
         * Returns the current map state 
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            // the map should be map[y][x]
            // Your code here
            if (mapLoaded == false)
            {
                char[][] map = GetOriginalMap(); // gets the 
                return map;
            }
            else
            {
                mapLoaded = true;
                return currentMap;
            }
        }

        /**
         * Returns the current position of the player on the map
         * 
         * The first value is the x corrdinate and the second is the y coordinate on the map
         */
        public int[] GetPlayerPosition()
        {
            createCharacter(currentMap);
            //initializes the position to 0,0
            int[] position = { 0, 0 };
            // Your code here
            int i = 0;
            int x = 0;
            if (currentMap != null)
            {
                for (; i < currentMap.Length; i++)
                {
                    for (; x < currentMap[i].Length - 1; x++)
                    {
                        // Looks for @ in each line of the jagged array and returns the position in the array 
                        if (currentMap[i][x].ToString().Contains('@'))
                        {
                            position = new int[] { x, i };
                            originalPlayerPos = true;
                            return position;
                        }
                    }
                    // Looks for @ in each row and makes sure that no value is missed during iteration
                    if (currentMap[i][x].ToString().Contains('@'))
                    {
                        position = new int[] { i, x };
                        originalPlayerPos = true;
                        return position;
                    }
                    //resets the second dimension search when the first dimension changes
                    x = 0;
                }
            }
            return position;
        }

        /**
        * Returns the next player action
        * 
        * This method does not alter any internal state
        */
        public int GetPlayerAction()
        {

            // Your code here

            return (int)action;
        }


        public bool GameIsRunning()
        {
            // Your code here 
            return running;
        }

        /**
         * Main method and Entry point to the program
         * ####
         * Do not change! 
        */
        static void Main(string[] args)
        {
            CMDCrawler crawler = new CMDCrawler();
            string input = string.Empty;
            Console.WriteLine("Welcome to the Commandline Dungeon!" + Environment.NewLine +
                "May your Quest be filled with riches!" + Environment.NewLine);

            // Loops through the input and determines when the game should quit
            while (crawler.active && crawler.action != PlayerActions.QUIT)
            {
                Console.WriteLine("Your Command: ");
                input = crawler.ReadUserInput();
                Console.WriteLine(Environment.NewLine);

                crawler.ProcessUserInput(input);

                crawler.GameLoop(crawler.active);
            }

            Console.WriteLine("See you again" + Environment.NewLine +
                "In the CMD Dungeon! ");
        }


    }
}

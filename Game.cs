using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace _3DFPS
{
    public class Map
    {
        private int mapSize;
        private int mapFill;
        private int mapRnd;
        private char[,] map;

        public Map(int mapSize)
        {
            this.mapSize = mapSize;
            this.mapFill = 0;
            mapRnd = 0;
            GenerateSkyFar();
        }
        public Map(int mapSize, int mapRnd, int mapFill)
        {
            this.mapSize = mapSize;
            this.mapFill = mapFill;
            this.mapRnd = mapRnd;
            GenerateSkyFar();
        }

        public char[,] GetMap { get { return map; } }

        private void GenerateSkyFar()
        {
            map = new char[mapSize, mapSize];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                map[i, 0] = 'W';
                map[i, map.GetLength(1) - 2] = 'W';
            }
            for (int i = 1; i < map.GetLength(1) - 1; i++)
            {
                map[0, i] = 'W';
                map[map.GetLength(0) - 2, i] = 'W';
            }
            for (int i = 1; i < map.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < map.GetLength(1) - 1; j++)
                {
                    if (i > 1 && j > 1 && i < map.GetLength(0) - 2 && j < map.GetLength(1) - 2 && map[i, j] != 'W' && (map[i - 1, j] == 'W'))
                    {
                        map[i, j] = Game.rand.Next(0, 100) < mapFill ? 'W' : 'F';
                    }
                    else if (i > 1 && j > 1 && i < map.GetLength(0) - 2 && j < map.GetLength(1) - 2 && map[i, j] != 'W' && (map[i, j - 1] == 'W'))
                    {
                        map[i, j] = Game.rand.Next(0, 100) < mapFill ? 'W' : 'F';
                    }
                    else if (map[i, j] != 'W')
                    {
                        map[i, j] = Game.rand.Next(0, 100) < mapRnd ? 'W' : 'F';
                    }
                }
            }
            for (int i = 1; i < map.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < map.GetLength(1) - 1; j++)
                {
                    if (i > 1 && j > 1 && i < map.GetLength(0) - 2 && j < map.GetLength(1) - 2 && map[i, j] != 'W' &&
                       (map[i - 1, j] == 'W' && map[i + 1, j] == 'W' && map[i, j - 1] == 'W' && map[i, j + 1] == 'W'))
                    {
                        map[i, j] = Game.rand.Next(0, 100) < mapFill ? 'W' : 'F';
                    }
                }
            }
        }
    }

    public class Player
    {
        private float playerX;
        private float playerY;
        private double playerZ;
        private float playerAngleV;
        private float playerAngleH;
        private float playerSpeed;
        private float playerRotationSpeedV;
        private float playerRotationSpeedH;
        public Player(float playerX, float playerY, float playerZ, float playerAngleV, int playerAngleH, float playerSpeed, float playerRotationSpeedV, float playerRotationSpeedH)
        {
            this.playerX = playerX;
            this.playerY = playerY;
            this.playerZ = playerZ;
            this.playerAngleV = playerAngleV;
            this.playerAngleH = (float)(playerAngleH * Math.PI / 180);
            this.playerSpeed = playerSpeed;
            this.playerRotationSpeedV = playerRotationSpeedV;
            this.playerRotationSpeedH = playerRotationSpeedH;
        }

        public float GetX { get { return playerX; } }
        public float GetY { get { return playerY; } }
        public double GetZ { get { return playerZ; } }
        public float GetAngleV { get { return playerAngleV; } }
        public float GetAngleH { get { return playerAngleH; } }


        public void Move(float angle, float speed)
        {
            playerX += (float)(Math.Sin(playerAngleH + angle) * playerSpeed * speed);
            playerY += (float)(Math.Cos(playerAngleH + angle) * playerSpeed * speed);
            if (Game.map.GetMap[(int)playerY, (int)playerX] == 'W')
            {
                playerX -= (float)(Math.Sin(playerAngleH + angle) * playerSpeed * speed);
                playerY -= (float)(Math.Cos(playerAngleH + angle) * playerSpeed * speed);
            }
        }
        public void Falling()
        {
            while (true)
            {
                if (playerZ > 0)
                {
                    playerZ -= 0.05;   // Падение игрока на землю
                }
                Thread.Sleep(1);
            }
        }
        public void Action()
        {
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Spacebar:
                        {
                            if (Math.Round(playerZ) == -10)
                            {
                                playerZ = 0;
                                continue;
                            }
                            if (Math.Round(playerZ) == 0) playerZ = 15; // Прыжок игрока (пока без анимации прыжка, только анимация падения)
                            break;
                        }
                    case ConsoleKey.C:
                        {
                            if (Math.Round(playerZ) == -10)
                            {
                                playerZ = 0;
                                continue;
                            }
                            if (Math.Round(playerZ) == 0) playerZ = -10;
                            break;
                        }
                    case ConsoleKey.W:
                        {
                            Move(0, 1);
                            break;
                        }
                    case ConsoleKey.S:
                        {
                            Move(0, -1);
                            break;
                        }
                    case ConsoleKey.A:
                        {
                            Move((float)Math.PI / 2, -0.5f);
                            break;
                        }
                    case ConsoleKey.D:
                        {
                            Move((float)Math.PI / 2, 0.5f);
                            break;
                        }
                    case (ConsoleKey.LeftArrow):
                        {
                            if (playerAngleH - playerRotationSpeedV > 0) playerAngleH = playerAngleH - playerRotationSpeedV;
                            else playerAngleH = playerAngleH - playerRotationSpeedV + (360 * (float)Math.PI / 180);
                            break;
                        }
                    case (ConsoleKey.RightArrow):
                        {
                            if (playerAngleH + playerRotationSpeedV < (360 * (float)Math.PI / 180)) playerAngleH = playerAngleH + playerRotationSpeedV;
                            else playerAngleH = playerAngleH + playerRotationSpeedV - (360 * (float)Math.PI / 180);
                            break;
                        }
                    case (ConsoleKey.UpArrow):
                        {
                            if (playerAngleV + playerRotationSpeedV < Game.renderer.GetWindowHeight) playerAngleV = playerAngleV + playerRotationSpeedH * 100f;
                            break;
                        }
                    case (ConsoleKey.DownArrow):
                        {
                            if (playerAngleV - playerRotationSpeedV > - Game.renderer.GetWindowHeight) playerAngleV = playerAngleV - playerRotationSpeedH * 100f;
                            break;
                        }
                    case (ConsoleKey.Add):
                        {
                            Game.renderer.SetFOV = 0.01f;
                            Game.renderer.SetVertFOV = 0.015f * (Console.WindowWidth / Console.WindowHeight);
                            break;
                        }
                    case (ConsoleKey.Subtract):
                        {
                            Game.renderer.SetFOV = -0.01f;
                            Game.renderer.SetVertFOV = -0.015f * (Console.WindowWidth / Console.WindowHeight);
                            break;
                        }
                    case (ConsoleKey.Home):
                        {
                            Game.renderer.SwitchFishEye = !Game.renderer.SwitchFishEye;
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            }
        }
    }

    public class Renderer
    {
        protected class P
        {
            public float d;
            public float dot;
            public P(float d, float dot)
            { 
                this.d = d;
                this.dot = dot;
            }
        }

        private char[,] frame;
        private char[,] skyTextureFar;
        private char[,] skyTextureNear;
        private char[,] skyTextureFarTemp;
        private char[,] skyTextureNearTemp;
        private int skyFarRotateSleepTime;
        private int skyNearRotateSleepTime;
        private float FOV;
        private float vertFOV;
        private float renderDistance;
        private float renderQuality;
        private float shadeDistance;
        private int maxFPS;
        private int windowWidth;
        private int windowHeight;
        private string screenBuffer;
        private bool hasChanged;
        private bool hasChangedForSkyFar;
        private bool hasChangedForSkyNear;
        private float rayStep;
        private float rayAngle;
        private float distanceToWall;
        private bool isHitWall;
        private bool boundary;
        private float eyeX;
        private float eyeY;
        private int testX;
        private int testY;
        private float vx;
        private float vy;
        private float d;
        private float dot;
        private float bound;
        private List<P> p;
        private List<P> sortedP;
        private char shade;
        private char borderShade;
        private int sky;
        private int floor;
        private float b;
        private bool fishEye;

        public Renderer(int skyFarRotateSleepTime, int skyNearRotateSleepTime, int FOV, float renderDistance, float renderQuality, float shadeDistance, int maxFPS)
        {
            FixWindowSize();
            hasChanged = true;
            hasChangedForSkyFar = true;
            hasChangedForSkyNear = true;
            this.skyFarRotateSleepTime = skyFarRotateSleepTime;
            this.skyNearRotateSleepTime = skyNearRotateSleepTime;
            this.FOV = (float)(FOV * Math.PI / 180) * (Console.WindowWidth / Console.WindowHeight);
            vertFOV = 0;
            this.shadeDistance = shadeDistance;
            this.renderDistance = renderDistance;
            this.renderQuality = renderQuality;
            this.maxFPS = 1000 / maxFPS;
            fishEye = false;
            frame = new char[windowHeight, windowWidth];
            borderShade = 'X';
        }

        public Renderer(int skyFarRotateSleepTime, int skyNearRotateSleepTime, int FOV, float renderDistance, float renderQuality, float shadeDistance)
        {
            FixWindowSize();
            hasChanged = true;
            hasChangedForSkyFar = true;
            hasChangedForSkyNear = true;
            this.skyFarRotateSleepTime = skyFarRotateSleepTime;
            this.skyNearRotateSleepTime = skyNearRotateSleepTime;
            this.FOV = (float)(FOV * Math.PI / 180) * (Console.WindowWidth / Console.WindowHeight);
            vertFOV = 0;
            this.shadeDistance = shadeDistance;
            this.renderDistance = renderDistance;
            this.renderQuality = renderQuality;
            maxFPS = 0;
            fishEye = false;
            frame = new char[windowHeight, windowWidth];
            borderShade = 'X';
        }

        public int GetWindowHeight { get { return windowHeight; } }
        public float SetFOV { set { FOV += value; } }
        public float SetVertFOV { set { vertFOV += value; } }
        public bool SwitchFishEye { get { return fishEye; } set { fishEye = value; } }

        private void FixWindowSize()
        {
            if (windowWidth != Console.WindowWidth - 1)
            {
                Console.Clear();
                windowWidth = Console.WindowWidth - 1;
                Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                hasChanged = true;
                hasChangedForSkyFar = true;
                hasChangedForSkyNear = true;
            }
            if (windowHeight != Console.WindowHeight - 1)
            {
                Console.Clear();
                windowHeight = Console.WindowHeight - 1;
                Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                hasChanged = true;
                hasChangedForSkyFar = true;
                hasChangedForSkyNear = true;
            }
        }

        private void GenerateSkyFar()
        {
            skyTextureFar = new char[windowHeight * 2, windowWidth * 4];
            for (int y = 0; y < skyTextureFar.GetLength(0); y++)
            { 
                for (int x = 0; x < skyTextureFar.GetLength(1); x++)
                {
                    if (Game.rand.Next(0, 100) == 1)
                        skyTextureFar[y, x] = '·';
                    else if (Game.rand.Next(0, 1000) == 1)
                        skyTextureFar[y, x] = '*';
                    else
                        skyTextureFar[y, x] = ' ';
                }
            }
        }

        private void GenerateSkyNear()
        {
            skyTextureNear = new char[windowHeight * 2, windowWidth * 4];
            for (int y = 0; y < skyTextureNear.GetLength(0); y++)
            {
                for (int x = 0; x < skyTextureNear.GetLength(1); x++)
                {
                    skyTextureNear[y, x] = ' ';
                }
            }

            char[,] moon = {{' ',' ',' ',' ',' ','▒','▒','▒','▒','▒','▒',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' '},
                            {' ',' ',' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' '},
                            {' ',' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' '},
                            {' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' '},
                            {'▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' ',' '},
                            {'▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' ',' '},
                            {'▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' ',' '},
                            {'▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' ',' '},
                            {'▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' ',' '},
                            {'▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ','▒','█','█','█','█','█','█','▒',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ','▒','▒','▒','▒','▒','▒',' ',' ',' ',' ',' '}};

            int i = Game.rand.Next((int)(skyTextureNear.GetLength(0) * 0.25), (int)(skyTextureNear.GetLength(0) * 0.75) - 16);
            int j = Game.rand.Next(0, skyTextureNear.GetLength(1) - 16);
            for (int y = i; y < i + 16; y++)
            {
                for (int x = j; x < j + 16; x++)
                {
                    skyTextureNear[y, x] = moon[y - i, x - j];
                }
            }
        }

        public void SkyRotateFar()
        {
            while (true)
            {
                FixWindowSize();
                if (hasChangedForSkyFar)
                {
                    GenerateSkyFar();
                    hasChangedForSkyFar = false;
                }
                skyTextureFarTemp = new char[windowHeight * 2, windowWidth * 4];
                for (int y = 0; y < skyTextureFarTemp.GetLength(0); y++)
                {
                    for (int x = 0; x < skyTextureFarTemp.GetLength(1); x++)
                    {
                        if (x + 1 < skyTextureFar.GetLength(1)) skyTextureFarTemp[y, x] = skyTextureFar[y, x + 1];
                        else skyTextureFarTemp[y, x] = skyTextureFar[y, 0];
                    }
                }
                skyTextureFar = skyTextureFarTemp;
                Thread.Sleep(skyFarRotateSleepTime);
            }
        }
        public void SkyRotateNear()
        {
            while (true)
            {
                FixWindowSize();
                if (hasChangedForSkyNear)
                {
                    GenerateSkyNear();
                    hasChangedForSkyNear = false;
                }
                skyTextureNearTemp = new char[windowHeight * 2, windowWidth * 4];
                for (int y = 0; y < skyTextureNearTemp.GetLength(0); y++)
                {
                    for (int x = 0; x < skyTextureNearTemp.GetLength(1); x++)
                    {
                        if (x + 1 < skyTextureNear.GetLength(1)) skyTextureNearTemp[y, x] = skyTextureNear[y, x + 1];
                        else skyTextureNearTemp[y, x] = skyTextureNear[y, 0];
                    }
                }
                skyTextureNear = skyTextureNearTemp;
                Thread.Sleep(skyNearRotateSleepTime);
            }
        }

        public void RayCast()
        {
            while (true)
            {
                FixWindowSize();
                if (hasChanged)
                {
                    frame = new char[windowHeight, windowWidth];
                    hasChanged = false;
                }
                for (int x = 0; x < windowWidth; x++)
                {
                    rayAngle = (Game.player.GetAngleH - FOV / 2.0f) + ((float)x / (float)windowWidth) * FOV;
                    rayStep = renderQuality;
                    distanceToWall = 0.0f;
                    isHitWall = false;
                    boundary = false;
                    eyeX = (float)Math.Sin(rayAngle);
                    eyeY = (float)Math.Cos(rayAngle);

                    while (!isHitWall && distanceToWall < renderDistance)
                    {
                        distanceToWall += rayStep;
                        testX = (int)(Game.player.GetX + eyeX * distanceToWall);
                        testY = (int)(Game.player.GetY + eyeY * distanceToWall);
                        
                        if (testX < 0 || testX >= Game.map.GetMap.GetLength(1) || testY < 0 || testY >= Game.map.GetMap.GetLength(0))
                        {
                            isHitWall = true;
                            distanceToWall = renderDistance;
                        }
                        else
                        {
                            if (Game.map.GetMap[testY, testX] == 'W')
                            {
                                isHitWall = true;
                                if (!fishEye) distanceToWall *= (float)Math.Cos(Game.player.GetAngleH > rayAngle ? rayAngle - Game.player.GetAngleH : Game.player.GetAngleH - rayAngle);

                                p = new List<P>();
                                for (int ix = 0; ix < 2; ix++)
                                {
                                    for (int iy = 0; iy < 2; iy++)
                                    {
                                        vx = (float)testX + ix - Game.player.GetX;  // Вектор
                                        vy = (float)testY + iy - Game.player.GetY;
                                        d = (float)Math.Sqrt(vx * vx + vy * vy); // Модуль
                                        dot = (eyeX * vx / d) + (eyeY * vy / d); // Скалярное произведение
                                        p.Add(new P(d, dot));
                                    }
                                }
                                sortedP = p.OrderBy(o => o.d).ToList(); // Сортировка по возростанию модуля
                                bound = (float)(renderQuality * 0.5);
                                if ((float)(Math.Acos(sortedP[0].dot)) < bound) boundary = true;
                                else if ((float)(Math.Acos(sortedP[1].dot)) < bound) boundary = true;
                                //else if ((float)(Math.Acos(p[2].dot)) < bound) boundary = true;
                            }
                        }
                    }

                    if (distanceToWall >= renderDistance)
                    {
                        distanceToWall = renderDistance;
                    }

                    shade = ' ';

                    if (shadeDistance * 0.25 > distanceToWall)
                    {
                        if (boundary & x != 0)
                        {
                            if (frame[windowHeight / 2, x - 1] != borderShade) shade = borderShade;
                            else shade = '█';
                        }
                        else shade = '█';
                    }
                    else if (shadeDistance * 0.35 > distanceToWall)
                    {
                        if (boundary & x != 0)
                        {
                            if (frame[windowHeight / 2, x - 1] != borderShade) shade = borderShade;
                            else shade = '▓';
                        }
                        else shade = '▓';
                    }
                    else if (shadeDistance * 0.5 > distanceToWall)
                    {
                        if (boundary & x != 0)
                        {
                            if (frame[windowHeight / 2, x - 1] != borderShade) shade = borderShade;
                            else shade = '▒';
                        }
                        else shade = '▒';
                    }
                    else if (shadeDistance >= distanceToWall)
                    {
                        if (boundary & x != 0)
                        {
                            if (frame[windowHeight / 2, x - 1] != borderShade) shade = borderShade;
                            else shade = '▒';
                        }
                        else shade = '▒';
                    }
                    else if (shadeDistance <= renderDistance & renderDistance == distanceToWall)
                    {
                        shade = ' ';
                    }
                    else shade = '░';

                    sky = (int)((windowHeight / 2.0) - (windowHeight / distanceToWall)) + (int)Game.player.GetAngleV + (int)vertFOV + (int)Game.player.GetZ; // Конец неба
                    floor = windowHeight - sky + 2 * (int)Game.player.GetAngleV + (int)(Game.player.GetZ * 2); // Начало пола

                    if (sky >= (int)(windowHeight / 2.0 + 1 + Game.player.GetAngleV + Game.player.GetZ))
                    {
                        sky = (int)(windowHeight / 2.0 + Game.player.GetAngleV);
                        floor = sky;
                    }

                    for (int y = 0; y < windowHeight; y++)
                    {
                        if (y < sky) // Небо
                        {
                            float angle = Game.player.GetAngleH * 180 / (float)Math.PI - 45 > 0 ? Game.player.GetAngleH * 180 / (float)Math.PI - 45
                                                                                                : Game.player.GetAngleH * 180 / (float)Math.PI - 45 + 360;
                            int xx = x + (int)((skyTextureFar.GetLength(1)) * angle / 360);
                            int yy = y + windowHeight - (int)Game.player.GetAngleV;

                            if (xx > skyTextureFar.GetLength(1) - 1) xx = xx - skyTextureFar.GetLength(1);

                            if (skyTextureNear[yy, xx] != ' ')
                            {
                                frame[y, x] = skyTextureNear[yy, xx];
                            }
                            else
                            {
                                switch (skyTextureFar[yy, xx])
                                {
                                    case '·':
                                        {
                                            if (Game.rand.Next(0, 100) < 1) frame[y, x] = ' ';
                                            else frame[y, x] = skyTextureFar[yy, xx];
                                            break;
                                        }
                                    case '*':
                                        {
                                            if (Game.rand.Next(0, 100) < 1) frame[y, x] = '·';
                                            else frame[y, x] = skyTextureFar[yy, xx];
                                            break;
                                        }
                                    default:
                                        {
                                            frame[y, x] = skyTextureFar[yy, xx];
                                            break;
                                        }
                                }
                            }
                        }

                        else if (y == sky) // Верхняя граница стены
                        {
                            if (renderDistance != distanceToWall)
                                frame[y, x] = borderShade;
                            else
                                frame[y, x] = shade = ' ';
                        }

                        else if (y > sky & y < floor) // Стена
                            frame[y, x] = shade;

                        else if (y == floor) // Нижняя граница стены
                        {
                            if (renderDistance != distanceToWall)
                                frame[y, x] = borderShade;
                            else
                                frame[y, x] = shade = ' ';
                        }

                        else // Пол                          // !!! Нужно сделать смещение пола 
                        {
                            b = 1.0f - (((float)y - windowHeight / 2.0f) / ((float)windowHeight / 2.0f));
                            if (b < 0.5 - (Game.player.GetZ * 0.0315 + Game.player.GetAngleV * 0.0305)) shade = '=';  // !!! Для корреции наклона и смещения пола - И - для одинакового удаления стены и пола
                            else if (b < 0.75 - (Game.player.GetZ * 0.0315 + Game.player.GetAngleV * 0.0305)) shade = '-'; // !!! Нужно сделать зависимость от разрешения окна консоли
                            else shade = '·';
                            frame[y, x] = shade;
                        }
                    }
                }
                screenBuffer = "";
                for (int y = 0; y < windowHeight; y++)
                {
                    for (int x = 0; x < windowWidth; x++)
                    {
                        screenBuffer += frame[y, x];
                    }
                    screenBuffer += "\n";
                }

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
                Console.Write(screenBuffer);
                Thread.Sleep(maxFPS);
            }
        }
    }

    class Game
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static Random rand;
        public static Map map;
        public static Player player;
        public static Renderer renderer;
        public static Thread RayCast;
        public static Thread SkyRotateFar;
        public static Thread SkyRotateNear;
        public static Thread Movement;
        public static Thread Falling;

        public static void Main(string[] args)
        {
            ShowWindow(ThisConsole, 3);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.Black;

            rand = new Random();
            map = new Map(96, 1, 10);
            player = new Player(rand.Next(1, map.GetMap.GetLength(1) - 2), rand.Next(1, map.GetMap.GetLength(0) - 2), 35, 0, rand.Next(0, 360), 0.1f, 0.05f, 0.02f);
            renderer = new Renderer(1000, 166, 30, 128f, 0.01f, 16f);

            RayCast = new Thread(renderer.RayCast);
            SkyRotateFar = new Thread(renderer.SkyRotateFar);
            SkyRotateNear = new Thread(renderer.SkyRotateNear);
            Movement = new Thread(player.Action);
            Falling = new Thread(player.Falling);
            RayCast.Start();
            SkyRotateFar.Start();
            SkyRotateNear.Start();
            Movement.Start();
            Falling.Start();
        }
    }
}

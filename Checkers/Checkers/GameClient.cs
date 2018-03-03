﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Navigation;

namespace Checkers
{
    public class GameClient
    {
        private Boolean inGame = false;
        private Boolean isConnected = false;

        private String userId;
        private IPEndPoint remote;
        private Socket client;
        private byte[] r_buff = new byte[16384];

        private GameState game = null;

        private static GameClient gcInstance = null;

        public static GameClient init()
        {
            // TODO

            return GameClient.getInstance();
        }

        public static GameClient getInstance()
        {
            if (GameClient.gcInstance == null)
                GameClient.gcInstance = new GameClient();
            return GameClient.gcInstance;
        }

        // TODO: make these part of GameState
        private static GameState parseFromString(String gString)
        {
            GameState ret = new GameState();

            return ret;
        }

        private static String parseToString(GameState gState)
        {
            return "";
        }

        public int Connect(String netAddress, String userName)
        {
            if (isConnected)
            {
                userId = userName;
                remote = new IPEndPoint(IPAddress.Parse(netAddress), 8080);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    int r_sz;
                    String r;

                    client.Connect(remote);

                    client.Send(Encoding.ASCII.GetBytes(userId + ": HELLO"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.Equals("S: WELCOME", StringComparison.OrdinalIgnoreCase))
                    {
                        isConnected = true;
                        return 0;
                    }
                    else if (r.Equals("S: E:USER EXISTS", StringComparison.OrdinalIgnoreCase))
                    {
                        return -2;
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return -1;
        }

        public int disconnect()
        {
            if (!isConnected)
                return -1;

            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                isConnected = false;
            }
            catch (Exception e) { Console.Write(e.ToString()); }

            return 0;
        }

        public List<String> listPlayers()
        {
            List<String> ret = null;

            if (isConnected)
            {
                try
                {
                    int r_sz;
                    String r;

                    client.Send(Encoding.ASCII.GetBytes(userId + ": LIST PLAYERS"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 8, r_sz);
                        ret = r.Split('|').ToList();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return ret;
        }

        public List<GameState> listGames()
        {
            List<GameState> ret = null;

            if (isConnected)
            {
                try
                {
                    int r_sz;
                    String r;

                    client.Send(Encoding.ASCII.GetBytes(userId + ": LIST GAMES"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 8, r_sz);
                        ret = r.Split('|').Select(gString => parseFromString(gString)).ToList();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return ret;
        }

        public int joinGame()
        {
            if (isConnected && !inGame)
            {
                try
                {
                    int r_sz;
                    String r;

                    // TODO: OtherPlayerName
                    client.Send(Encoding.ASCII.GetBytes(userId + ": NEW"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 8, r_sz);
                        this.game = parseFromString(r);
                        inGame = true;
                        return 0;
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return -1;
        }

        public int joinGame(GameState remote)
        {
            if (isConnected && !inGame)
            {
                try
                {
                    int r_sz;
                    String r;

                    // TODO: OtherPlayerName
                    client.Send(Encoding.ASCII.GetBytes(userId + ": JOIN " + remote.player1Name));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 8, r_sz);
                        this.game = parseFromString(r);
                        inGame = true;
                        return 0;
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return -1;
        }

        public int quitGame()
        {
            if (isConnected && inGame)
            {
                try
                {
                    int r_sz;
                    String r;

                    // TODO: OtherPlayerName
                    client.Send(Encoding.ASCII.GetBytes(userId + ": QUIT"));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        this.game = null;
                        this.inGame = false;

                        return 0;
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return -1;
        }

        public GameState getGameState()
        {
            return this.game;
        }

        public int sendState(GameState game)
        {
            if (isConnected && inGame)
            {
                try
                {
                    int r_sz;
                    String r;

                    // TODO: OtherPlayerName
                    client.Send(Encoding.ASCII.GetBytes(userId + ": SEND " + parseToString(game)));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                        return 0;
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return -1;
        }

        public int receiveState(GameState game)
        {
            int ret = -1;

            if (isConnected && inGame)
            {
                try
                {
                    int r_sz;
                    String r;

                    // TODO: OtherPlayerName
                    client.Send(Encoding.ASCII.GetBytes(userId + ": SEND " + parseToString(game)));

                    r_sz = client.Receive(r_buff);
                    r = Encoding.ASCII.GetString(r_buff, 0, r_sz);

                    if (r.StartsWith("S: OKAY", StringComparison.OrdinalIgnoreCase))
                    {
                        r = Encoding.ASCII.GetString(r_buff, 8, r_sz);
                        this.game = parseFromString(r);
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            return ret;
        }
    }
}

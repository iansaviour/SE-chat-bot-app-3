using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatExchangeDotNet;

namespace SE_chat_bot_app_3
{
    public class ChatApi
    {
        public ChatApi(string login, string pass, string botname, int botuserid, int mainRoomID, int debugRoomID)
        {
            this.login = login;
            this.pass = pass;
            this.botname = botname;
            this.userID = botuserid;
            this.mainRoomID = mainRoomID;
            this.debugRoomID = debugRoomID;
        }
        public string login, pass, botname;
        public int userID, mainRoomID, debugRoomID;

        public DateTime startTime;
        private Client client;

        public bool exceptionOccurred;
        public TimeSpan LastEventArrivalMaximumDelta = new TimeSpan(0, 1, 0);
        public TimeSpan ChatApiReinitializationInterval = new TimeSpan(0, 5, 0);
        public DateTime LastChatApiInitializationAttempt = DateTime.MinValue;

        public Dictionary<int, int> processedMessageIDEditsDic = new Dictionary<int, int>();

        public static int maxRoomTranscriptLength = 500;

        public Dictionary<int, List<ChatMessage>> roomTranscriptDic = new Dictionary<int, List<ChatMessage>>();

        public Dictionary<int, int> commandResponseMessageIDsDic = new Dictionary<int, int>(); // remove where response is older than 2 minutes coz it's only used for editing own responses

        public void Start()
        {
            Log("[…] Starting chat api…");

            exceptionOccurred = false;
            LastChatApiInitializationAttempt = DateTime.Now;

            startTime = DateTime.UtcNow;

            client = new Client(login, pass);

            if (mainRoomID > 0)
            {
                if (!roomTranscriptDic.ContainsKey(mainRoomID))
                    roomTranscriptDic[mainRoomID] = new List<ChatMessage>();
                JoinRoom(mainRoomID);
                Log("[…] Joined main room.");
                if (client.Rooms.Count > 0)
                {
                    botname = client.Rooms[0].Me.Name;
                    userID = client.Rooms[0].Me.ID;
                    client.Rooms[0].WebSocketRecoveryTimeout = new TimeSpan(10, 0, 0, 0);
                    Log("[…] Found bot name in main room.");
                }
                else
                {
                    exceptionOccurred = true;
                    Log("[X] Failed to join main room.");
                }
            }

            if (debugRoomID > 0)
            {
                if (!roomTranscriptDic.ContainsKey(debugRoomID))
                    roomTranscriptDic[debugRoomID] = new List<ChatMessage>();
                JoinRoom(debugRoomID);
                Log("[…] Joined debug room.");
                if (client.Rooms.Count > 1)
                    client.Rooms[0].WebSocketRecoveryTimeout = new TimeSpan(10, 0, 0, 0);
                else
                {
                    exceptionOccurred = true;
                    Log("[X] Failed to join debug room.");
                }
            }

            Log("[.] Started chat api.");
        }
        public void Stop() { exceptionOccurred = true; client.Dispose(); Log("[×] Chat client disposed. New messages should stop arriving now."); }
        void JoinRoom(int roomid)
        {
            try
            {
                var url = @"http://chat.stackexchange.com/rooms/" + roomid;
                var room = client.JoinRoom(url);
                room.IgnoreOwnEvents = false;
                AddRoomEventListeners(room);
                Log("[…] Successfully joined room with id \"" + roomid + "\".");
            }
            catch (Exception ex) { Log("[X] Could not join room with id \"" + roomid + "\":\n" + ex); }
        }
        void AddRoomEventListeners(Room room)
        {
            room.EventManager.ConnectListener(EventType.DataReceived, new Action<string>(jsonData => ChatApiMeaningfulDataReceived(jsonData)));
            room.EventManager.ConnectListener(EventType.InternalException, new Action<Exception>(ex => ChatApiInternalException(ex)));
            room.EventManager.ConnectListener(EventType.MessagePosted, new Action<Message>(message => ChatApiMessageReceived(message)));
        }

        void ChatApiMeaningfulDataReceived(string jsonData)
        {
            try { lastEventArrivalTime = DateTime.Now; Log("[»] " + jsonData); }
            catch (Exception ex) { Log("[X] " + ex); }
        }
        void ChatApiInternalException(Exception ex)
        {
            exceptionOccurred = true;
            Log("[X] Chat api internal exception:\n" + ex);
        }
        void ChatApiMessageReceived(Message message)
        {
            string str = message.Content;

            try
            {
                var v = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                str = v;
            }
            catch (Exception ex) { Log("[X] " + ex); }

            Log("[>] " + str);
            ProcessMessage(message);
        }

        void ProcessMessage(Message msg)
        {
            var cm = new ChatMessage
            {
                Edits = msg.EditCount,
                MessageID = msg.ID,
                PostedAtUTC = DateTime.UtcNow,
                ReplyMessageID = msg.ParentID,
                RoomID = msg.RoomID,
                Text = msg.Content,
                UserID = msg.Author.ID,
                UserName = msg.Author.Name
            };

            var roomTranscript = roomTranscriptDic[msg.RoomID];

            try
            {
                roomTranscript.Add(cm);
                lastReceivedMessage = cm;
                if (roomTranscript.Count > maxRoomTranscriptLength)
                    while (roomTranscript.Count > maxRoomTranscriptLength)
                        roomTranscript.RemoveAt(0);
            }
            catch (Exception ex) { Log("[X] Unexpected exception: \n" + ex); }

            lastMessageArrivalTime = DateTime.Now;

            if (msg.Author.ID != this.userID) // don't process bot's own messages
            {
                if (cm.MessageID != 0)
                {
                    unprocessedMessages.Add(cm);
                    Log("[+] Enqueued message for processing.");
                }
                else
                    Log("[X] Somehow messageID turned out to be zero");
            }
            else // add own message to transcript without processing
            {
                //roomTranscript.Add(cm);
                Log("[−] Own message received. No processing required.");
            }
        }
        public ChatMessage lastReceivedMessage;
        public DateTime lastMessageArrivalTime = DateTime.MinValue;
        public DateTime lastEventArrivalTime = DateTime.MinValue;
        public List<ChatMessage> unprocessedMessages = new List<ChatMessage>();




        public void DeleteMessage(int roomid, int messageId)
        {
            var room = GetRoomByID(roomid);
            if (room == null)
                return;

            try
            {
                var res = room.DeleteMessage(messageId);
                Log("[«] SendMessage result was: " + res);
            }
            catch (Exception ex) { Log("[X] " + ex); }
        }
        public Room GetRoomByID(int roomid)
        {
            foreach (var r in client.Rooms)
                if (r.ID == roomid)
                    return r;

            return null;
        }

        static void Log(string text) { DebugLogManager.Log(text); }

    }
}

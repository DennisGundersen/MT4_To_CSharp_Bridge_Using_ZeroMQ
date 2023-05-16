//+----------------------------------------------------------------------+
//|                                                     BridgeClient.mq4 |
//|                                    Copyright 2023, Dennis Gundersen. |
//| https://github.com/DennisGundersen/MT4_To_CSharp_Bridge_using_ZeroMQ |
//+----------------------------------------------------------------------+

#property copyright "Copyright 2023, Dennis Gundersen AS"
#property link      "https://github.com/DennisGundersen/MT4_To_CSharp_Bridge_using_ZeroMQ"
#property version   "1.00"
#property strict

#include <Zmq/Zmq.mqh>

input string   address="tcp://127.0.0.1:9001";
input int      connectTimeout=1000; //[ms]
input int      receiveTimeout=3000; //[ms]
input int      sendTimeout=4000; //[ms]
input int      reconnectInterval=2000; //[ms]
input int      reconnectIntervalMax=30000; //[ms]


Context context("bridgeClientSimpleTick");
Socket client(context, ZMQ_REQ);

const string CONNECT_MSG = "HELO";
const string CONNECT_MSG_ACK = "EHLO";
bool connected = false;
int tick = 0;

bool ReceiveLazyPirate(ZmqMsg &msg, int retries = 3, int retryInterval = 1000)
{
   bool received = false;
   while(retries > 0)
   {
      --retries;
      PrintFormat("Waiting for response");
      received = client.recv(msg);
      PrintFormat("(#%d) Response available: %d", retries, received);
      if(received)
      {
         PrintFormat("(#%d) Response size: `%d`, data `%s`", retries, msg.size(), msg.getData());
         return true;
      }
      Sleep(retryInterval);
   }
   return false;
}

bool InitializeConnectionToServer()
{
   PrintFormat("Connecting to server… %s", address);
   return client.connect(address);
}

bool SendCommand(string command, string data, ZmqMsg &response)
{
   ZmqMsg cmd(command);
   ZmqMsg cmdData(data);
   bool res = client.sendMore(cmd);
   client.send(cmdData);
   return ReceiveLazyPirate(response);
}

bool Welcome()
{
   ZmqMsg msg;
   PrintFormat("Sending Connect Message… `%s`", CONNECT_MSG);
   bool res = SendCommand(CONNECT_MSG, "", msg);
   return res && msg.getData() == CONNECT_MSG_ACK;
}

bool InitializeSocket()
{
   client.setTimeout(connectTimeout); //timeout during connection
   client.setReceiveTimeout(receiveTimeout); //timeout during receiving
   client.setSendTimeout(sendTimeout); //timeout during sending
   client.setReconnectInterval(reconnectInterval); //interval at which try to reconnect to server
   client.setReconnectIntervalMax(reconnectIntervalMax); //when total time exceeds this value stop retrying connect
   client.setLinger(0);
   //When Relaxed is set to true Correlated should be also enabled to avoid receiving incorrect responses
   client.setRequestCorrelated(true); //add id of message, response is ignored if values don't match
   client.setRequestRelaxed(true); //allows to send another frame even when didn't received response - should be used with above setting 
   return true;
}

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
{
   InitializeSocket();
   PrintFormat("Using ZeroMQ version: %s", Zmq::getVersion());
   InitializeConnectionToServer();
   connected = Welcome();
   return(INIT_SUCCEEDED);
}


//+------------------------------------------------------------------+
//| Tick received function                                           |
//+------------------------------------------------------------------+
void OnTick()
{
   ++tick;
   PrintFormat("OnTick #%d (connected %d)", tick, connected);
	if(!connected)
	{
	   connected = Welcome();
	}
	ZmqMsg msg;
	PrintFormat("OnTick #%d: Sending message `%s`", tick, msg.getData()); 
	bool res = SendCommand("tic", IntegerToString(tick), msg);
	PrintFormat("OnTick #%d: Received succeed=%d, size=%d, message=`%s`", tick, res, msg.size(), msg.getData());
}



//+------------------------------------------------------------------+
//| EA closing function call                                         |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
{
   client.disconnect(address);
}

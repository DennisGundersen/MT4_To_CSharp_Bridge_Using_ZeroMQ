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
input int      connectTimeout=1000; //connect timeout [ms]
input int      receiveTimeout=3000; //receive timeout [ms]
input int      sendTimeout=4000; //send timeout [ms]
input int      reconnectInterval=2000; //reconnect interval [ms]
input int      reconnectIntervalMax=30000; //max reconnect interval [ms]

bool useTick = false;

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

bool SendCommand(ZmqMsg& response, string& data[])
{
   int dataLen = ArraySize(data) - 1; //number of elements except the last one
   for(int i = 0; i < dataLen; ++i) //all except last send with more=true
   {
      client.sendMore(data[i]);
   }
   bool res = client.send(data[dataLen]); //last element send with more=false
   return ReceiveLazyPirate(response);
}

void ShowResponse(ZmqMsg& msg)
{
   PrintFormat("Received Response: size=%d, message=`%s`", msg.size(), msg.getData());
}

bool Welcome()
{
   ZmqMsg msg;
   PrintFormat("Sending Connect Message… `%s`", CONNECT_MSG);
   string payload[1];
   payload[0] = CONNECT_MSG;
   bool res = SendCommand(msg, payload);
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
   ZmqMsg msg;
   
   string a[1] = {"A"};
   SendCommand(msg, a);
   ShowResponse(msg);
   
   string af[1] = {"Library2Expose_InstanceExport_PublicMethod1"};
   SendCommand(msg, af);
   ShowResponse(msg);
   
   string b[2] = {"B", "123"};
   SendCommand(msg, b);
   ShowResponse(msg);
   
   string bf[2] = {"Library2Expose_InstanceExport_PublicMethod2", "123"};
   SendCommand(msg, bf);
   ShowResponse(msg);
   
   return(INIT_SUCCEEDED);
}


//+------------------------------------------------------------------+
//| Tick received function                                           |
//+------------------------------------------------------------------+
void OnTick()
{
   if(!useTick) return;
   ++tick;
   PrintFormat("OnTick #%d (connected %d)", tick, connected);
	if(!connected)
	{
	   connected = Welcome();
	}
	PrintFormat("OnTick #%d: Sending command Tic(%d)", tick, tick); 
	string payload[2] = {"Tic", ""};
	payload[1] = IntegerToString(tick);
	ZmqMsg msg;
	bool res = SendCommand(msg, payload);
	PrintFormat("OnTick #%d: Received succeed=%d, size=%d, message=`%s`", tick, res, msg.size(), msg.getData());
}



//+------------------------------------------------------------------+
//| EA closing function call                                         |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
{
   client.disconnect(address);
}

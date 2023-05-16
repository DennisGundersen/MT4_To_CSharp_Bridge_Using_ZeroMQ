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

string message = "Hello from MT4 (MQL4)";

bool ReceiveLazyPirate(Socket &socket, ZmqMsg &msg, int retries = 3, int retryInterval = 1000)
{
   bool received = false;
   while(retries > 0)
   {
      --retries;
      PrintFormat("Waiting for response");
      received = socket.recv(msg);
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

bool InitializeSocket(Socket &socket)
{
   socket.setTimeout(connectTimeout); //timeout during connection
   socket.setReceiveTimeout(receiveTimeout); //timeout during receiving
   socket.setSendTimeout(sendTimeout); //timeout during sending
   socket.setReconnectInterval(reconnectInterval); //interval at which try to reconnect to server
   socket.setReconnectIntervalMax(reconnectIntervalMax); //when total time exceeds this value stop retrying connect
   socket.setLinger(0);
   //When Relaxed is set to true Correlated should be also enabled to avoid receiving incorrect responses
   socket.setRequestCorrelated(true); //add id of message, response is ignored if values don't match
   socket.setRequestRelaxed(true); //allows to send another frame even when didn't received response - should be used with above setting 
   return true;
}

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
{
   bool res;
   Context context("bridgeClientSimple");
   Socket socket(context, ZMQ_REQ);
   InitializeSocket(socket);
   
   Print(Zmq::getVersion());
   PrintFormat("Connecting to server… %s", address);
   socket.connect(address);
   ZmqMsg request(message);
   PrintFormat("Sending Message… `%s`", message);
   res = socket.send(request);
   PrintFormat("Send result succeed=%d", res);
   if(!res)
   {
      PrintFormat("Send failed #%d: %s", Zmq::errorNumber(), Zmq::errorMessage(Zmq::errorNumber()));
      return(INIT_FAILED);
   }
   PrintFormat("Waiting for response…");
   ZmqMsg response;
   res = ReceiveLazyPirate(socket, response);
   PrintFormat("Response succeed=%d, size=%d, data=`%s`", res, response.size(), response.getData());  
   return(INIT_SUCCEEDED);
}


//+------------------------------------------------------------------+
//| Tick received function                                           |
//+------------------------------------------------------------------+
void OnTick()
{
	
}



//+------------------------------------------------------------------+
//| EA closing function call                                         |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
{

}
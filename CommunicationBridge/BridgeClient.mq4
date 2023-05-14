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

string message = "Hello from MT4 (MQL4)";

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
{
   Context context("bridgeClient");
   Socket socket(context, ZMQ_REQ);
   
   Print(Zmq::getVersion());
   PrintFormat("Connecting to server… %s", address);
   socket.connect(address);
   ZmqMsg request(message);
   PrintFormat("Sending Message… `%s`", message);
   socket.send(request);
   PrintFormat("Waiting for response…");
   ZmqMsg reply;
   socket.recv(reply);
   PrintFormat("Response size: `%d`", reply.size());
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
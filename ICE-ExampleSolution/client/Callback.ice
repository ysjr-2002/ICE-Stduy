#pragma once


module Demo
{
     interface CallbackReceiver
     {
         void callback();
     };	

     interface CallbackSender
     {
     
         void initializeCallback(CallbackReceiver* proxy);


         void shutdownDynamicDetect();
     };
};
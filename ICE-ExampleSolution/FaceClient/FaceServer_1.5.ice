// **********************************************************************
//
// Copyright (c) 2003-2016 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

#pragma once

module FaceRecognitionModule
{ 

    // ============== callback interface ==============
    interface ConnectionListener
    {
        void onRecv(string xmlContent);
    };


    // ============== server interface ==============
    interface FaceRecognition
    {
        //发送请求
        //params xml : xml格式的字符串
        string send(string xml);

        // 初始化回调函数
        void initConnectionListener(ConnectionListener* listener);
        
    };

};

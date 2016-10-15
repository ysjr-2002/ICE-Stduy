module demo
{
    //字节数组定义
    sequence<byte> ByteSeq;

	interface  Printer
	{
	    void printString(string s);
		void sendImage(ByteSeq seq,string name);
	};
};
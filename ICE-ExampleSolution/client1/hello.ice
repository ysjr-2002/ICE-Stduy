module demo
{
    //�ֽ����鶨��
    sequence<byte> ByteSeq;

	interface  Printer
	{
	    void printString(string s);
		void sendImage(ByteSeq seq,string name);
	};
};
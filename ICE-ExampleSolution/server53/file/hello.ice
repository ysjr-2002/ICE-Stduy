module demo
{
    //�ֽ����鶨��
    sequence<byte> ByteSeq;

	interface ICallback
	{
	    void doback(string date);
	};

	interface Printer
	{
	    void InitCallback(ICallback* callback);
	    void printString(string s);
		void sendImage(ByteSeq seq,string name);
	};

	interface Dgl
	{
	    void Where(string name);
	};
};
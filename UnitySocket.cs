namespace LSocket.Net 
{
  	using UnityEngine; 
	using System; 
	using System.Net.Sockets; 
	using System.Net; 
	using System.Collections; 
	using System.Collections.Generic;
	using System.Text;
    using System.Threading;
	using LSocket.Type; 
	using LSocket.cmd;
	using System.Linq;

    class SocketThread
    {
		UnitySocket socket;
		SocketDemo demo;
		int idx;
		public SocketThread(UnitySocket socket, SocketDemo demo, int idx){
			this.socket = socket;
			this.demo = demo;
			this.idx = idx;
		}
		public void run(){
			while (true){
				try{
				    String s = socket.ReceiveString();
				    demo.textAreaString[idx * 3] = demo.textAreaString[idx * 3 + 1];
				    demo.textAreaString[idx * 3 + 1] = demo.textAreaString[idx * 3 + 2];
				    demo.textAreaString[idx * 3 + 2] = s;
				    Debug.Log(s + " " + idx);
				}
				catch (Exception e){
				    Debug.Log(e.ToString());
				    socket.t.Abort();
				}
			}
		}
    }
    public class UnitySocket{ 
		public Socket mSocket = null;
		public Thread t=null;
		private SocketThread st=null;
		public SocketDemo demo=null;
		public UnitySocket(){} 
		public void SocketConnection(string LocalIP, int LocalPort,SocketDemo demo,int idx){
			this.demo=demo;
			mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
			try{
				IPAddress ip = IPAddress.Parse(LocalIP); 
				IPEndPoint ipe = new IPEndPoint(ip, LocalPort); 
				mSocket.Connect(ipe);
				st =new SocketThread(this, demo, idx);
				t = new Thread(new ThreadStart(st.run));
				t.Start();
			} 
			catch (Exception e){
				Debug.Log(e.ToString());
			} 
		}
		public void close(){
			mSocket.Close(0);
			mSocket=null;
		}
		public void DoLogin(String userName){
			try{
				SendPacket(CommandID.LOGIN,userName,0);
			}catch(Exception e){
				Debug.Log(e.ToString());
			}
		}
		public void SendPacket(short ID,params object[] p){
			var list = new List<byte[]>();
			list.Add(Getbyte(ID));
			for (int i = 0; i < p.Length; i++){
				if(i%2!=0)continue;
				switch ((int)p[i+1]) {
				case 16:
					list.Add(Getbyte((short)p[i]));		break;
				case 32:
					list.Add(Getbyte((int)p[i]));		break;
				case 64:
					list.Add(Getbyte((long)p[i]));		break;
				case 0:
					list.Add(Getbyte((string)p[i]));	break;
				default:
					break;
				}
			}
			byte[] all = list.SelectMany(a=>a).ToArray();
			int len = all.Length;
			mSocket.Send(Getbyte(len));		//pack len
			mSocket.Send(all);				//pack body(commandID,data)
		}
		public byte[] Getbyte(short data){	//16
			return TypeConvert.getBytes(data,true); 
		}
		public byte[] Getbyte(float data){	//32
			return TypeConvert.getBytes(data, true);
		}
		public byte[] Getbyte(int data){	//32
			return TypeConvert.getBytes(data,true); 
		}		
		public byte[] Getbyte(long data){	//64
			return TypeConvert.getBytes(data,true); 
		}
		public byte[] Getbyte(string data){	//16+N
			var len = Encoding.UTF8.GetByteCount(data);
			var s = Encoding.UTF8.GetBytes(data);
			return byteAdd(Getbyte((short)len),s);
		}
		public static byte[] byteAdd(byte[] a , byte[] b){
			var output = new byte[a.Length+b.Length];
			Buffer.BlockCopy(a,0, output,0,a.Length);
			Buffer.BlockCopy(b,0, output,a.Length,b.Length);
			return output;
		}
		public float ReceiveFloat(){
			byte[] recvBytes = new byte[4];
			mSocket.Receive(recvBytes, 4, 0);//从服务器端接受返回信息 
			float data = TypeConvert.getFloat(recvBytes, true);
			return data;
		}
		public short ReceiveShort(){
			 byte[] recvBytes = new byte[2]; 
			 mSocket.Receive(recvBytes,2,0);//从服务器端接受返回信息 
			 short data=TypeConvert.getShort(recvBytes,true); 
			 return data; 
		}
		public int ReceiveInt(){
			 byte[] recvBytes = new byte[4]; 
			 mSocket.Receive(recvBytes,4,0);//从服务器端接受返回信息 
			 int data=TypeConvert.getInt(recvBytes,true); 
			 return data; 
		} 
		public long ReceiveLong(){ 
			 byte[] recvBytes = new byte[8]; 
			 mSocket.Receive(recvBytes,8,0);//从服务器端接受返回信息 
			 long data=TypeConvert.getLong(recvBytes,true); 
			 return data; 
		}
		public String ReceiveString(){ 
			 int length = ReceiveShort();
			 Debug.Log("Stringlen="+length);
			 byte[] recvBytes = new byte[length]; 
			 mSocket.Receive(recvBytes,length,0);	
			 String data = Encoding.UTF8.GetString(recvBytes); 
			 return data; 
		} 
	}
} 

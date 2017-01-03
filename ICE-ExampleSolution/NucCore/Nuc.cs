using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace NucCore
{
    public class Nuc
    {
        private WebSocket _socket = null;
        private Constrant _constrant = null;

        public Nuc(Constrant constrant)
        {
            _constrant = constrant;
        }

        /// <summary>
        /// 获取所有摄像机
        /// </summary>
        /// <returns></returns>
        public List<Camera> GetCameras()
        {
            var url = _constrant.camera;
            var list = HttpMethod.Get<List<Camera>>(url);
            return list;
        }

        /// <summary>
        /// 创建摄像机
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rtsp"></param>
        /// <returns></returns>
        public Camera CreateCamera(string name, string rtsp)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "name", name },
                { "url", rtsp },
            };

            var url = _constrant.camera;
            var camera = HttpMethod.PostNoImage<Camera>(url, dictionary);
            return camera;
        }

        /// <summary>
        /// 删除摄像机
        /// </summary>
        /// <param name="camera_id"></param>
        public void DeleteCamera(string camera_id)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "camera_id", camera_id },
            };

            var url = _constrant.camera + "/" + camera_id;
            HttpMethod.Delete(url);
        }

        /// <summary>
        /// 返回所有Handle
        /// </summary>
        /// <returns></returns>
        public List<Handle> GetHandles()
        {
            var url = _constrant.handle;
            var list = HttpMethod.Get<List<Handle>>(url);
            return list;
        }

        public Handle CreateHandle(string camera_id, string group_id)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "camera_id", camera_id },
                { "group_id", group_id },
            };

            var url = _constrant.handle;
            var handle = HttpMethod.PostNoImage<Handle>(url, dictionary);
            return handle;
        }

        public void DeleteHandle(string handle_id)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "handle_id", handle_id }
            };

            //var url = _constrant.handle;
            //HttpMethod.PostNoImage<Empty>(url, dictionary);

            var url_del = _constrant.handle + "/" + handle_id;
            HttpMethod.Delete(url_del);
        }

        private Action<RecognizeResult> _callback = null;
        /// <summary>
        /// 获取识别结果
        /// </summary>
        /// <param name="handle_id"></param>
        public void Live(string handle_id, Action<RecognizeResult> callback)
        {
            _callback = callback;
            var url = _constrant.handle_websocket + "/" + handle_id + "/live";
            _socket = new WebSocket(url);
            _socket.OnOpen += Socket_OnOpen;
            _socket.OnError += _socket_OnError;
            _socket.OnClose += _socket_OnClose;
            _socket.OnMessage += Socket_OnMessage;
            _socket.Connect();
        }

        private void _socket_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("websocket close");
        }

        private void _socket_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("websocket error");
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("websocket open");
        }

        private void Socket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.IsText)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var entity = serializer.Deserialize<RecognizeResult>(e.Data);
                if (entity.data.status == TractStatus.recognized.ToString())
                {
                    //已识别
                    _callback?.Invoke(entity);
                }
            }
        }

        public void LiveStop()
        {
            _socket?.Close();
        }

        /// <summary>
        /// 获取所有组
        /// </summary>
        /// <returns></returns>
        public List<Group> GetGroups()
        {
            var url = _constrant.group;
            var list = HttpMethod.Get<List<Group>>(url);
            return list;
        }

        /// <summary>
        /// 创建用户组
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Group CreateGroup(string tag)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "tag", tag }
            };

            var url = _constrant.group;
            var group = HttpMethod.PostNoImage<Group>(url, dictionary);
            return group;
        }

        /// <summary>
        /// 添加用户到用户组
        /// </summary>
        /// <param name="group_Id"></param>
        /// <param name="user_Id"></param>
        public void AddUserToGroup(string group_Id, string user_Id)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"group_id", group_Id},
                {"user_id", user_Id}
            };

            var url = _constrant.group;
            HttpMethod.PostNoImage<Empty>(url, dictionary);
        }

        /// <summary>
        /// 删除用户组中的用户
        /// </summary>
        /// <param name="group_Id"></param>
        /// <param name="user_Id"></param>
        public void DeleteGroup(string group_Id)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"group_id", group_Id},
            };

            //var url = _constrant.group;
            //HttpMethod.PostNoImage<Empty>(url, dictionary);
            var url_del = _constrant.group + "/" + group_Id;
            HttpMethod.Delete(url_del);
        }

        /// <summary>
        /// 删除组中的用户
        /// </summary>
        /// <param name="group_Id"></param>
        /// <param name="user_Id"></param>
        public void DeleteGroupUser(string group_Id, string user_Id)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"group_id", group_Id},
                {"user_id", user_Id}
            };

            var url = _constrant.group;
            //HttpMethod.PostNoImage<Empty>(url, dictionary);
            var url_del = _constrant.group + "/" + group_Id + "/" + user_Id;
            HttpMethod.Delete(url_del);
        }

        /// <summary>
        /// 查询所有用户
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsers()
        {
            var url = _constrant.user;
            var list = HttpMethod.Get<List<User>>(url);
            return list;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public User CreatUser(string tag)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"tag", tag }
            };

            var url = _constrant.user;
            var user = HttpMethod.PostNoImage<User>(url, dictionary);
            return user;
        }

        /// <summary>
        /// 向用户加入图片
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="image"></param>
        public void PostUserImage(string userId, byte[] image)
        {
            var url = _constrant.user + "/" + userId + "/image";
            HttpMethod.Post<User>(url, image, null);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user_Id"></param>
        public void DeleteUser(string user_Id)
        {
            var url = _constrant.user + "/" + user_Id;
            HttpMethod.Delete(url);
        }
    }
}

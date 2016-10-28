using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognitionModule;
using Ice;
using System.Threading;

namespace server
{
    class MyFace : FaceRecognitionDisp_
    {
        public override CompareResult compare(byte[] srcImg, byte[] destImg, Current current__)
        {
            Console.Out.WriteLine("compare---->");
            Console.Out.WriteLine(string.Format("Image size:{0} {1}", srcImg.Length, destImg.Length));

            //var similarity = CloudAPI.MegviiCloud.Compare(srcImg, destImg);

            var similarity = 0.3f;
            CompareResult cr = new CompareResult();
            cr.similarity = (float)similarity;
            cr.code = 200;
            cr.message = "";
            return cr;
        }

        public override byte[] convertSignatureCode(byte[] img, Current current__)
        {
            Console.Out.WriteLine("convertSignatureCode---->");
            byte[] buffer = new byte[new Random().Next(100, 999)];
            return buffer;
        }

        private ClientCallbackReceiverPrx proxy = null;
        public override OperationResult dynamicDetect(string rtspPath, ClientCallbackReceiverPrx proxy, float threshold, int maxImageCount, long interval, Current current__)
        {
            Console.Out.WriteLine("dynamicDetect---->");

            Console.Out.WriteLine("rtspPath->" + rtspPath);
            Console.Out.WriteLine("threshold->" + threshold);
            Console.Out.WriteLine("maxImageCount->" + maxImageCount);
            Console.Out.WriteLine("interval->" + interval);

            if (proxy == null)
            {
                Console.Out.WriteLine("为什么要传入一个空引用啊");
            }
            else
            {
                Console.Out.WriteLine("终于等到你");
                this.proxy = proxy;
                StartCallback();
            }

            OperationResult result = new FaceRecognitionModule.OperationResult();
            result.code = 200;
            result.message = "execute ok";
            return result;
        }

        private void StartCallback()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    List<PersonInfo> persons = new List<FaceRecognitionModule.PersonInfo>();
                    persons.Add(new PersonInfo
                    {
                        id = Guid.NewGuid().ToString("N"),
                        uuid = Guid.NewGuid().ToString("N"),
                        code = "code",
                        name = "ysj",
                        race = "汉", //种族
                        nationality = "中国",
                        gender = "男",
                        img1 = new byte[1],
                        img2 = new byte[2],
                        img3 = new byte[3],
                        signatureCode1 = new byte[1],
                        signatureCode2 = new byte[2],
                        signatureCode3 = new byte[3],
                        tagList = new string[100]
                    });
                    proxy.detectCallback(persons.ToArray());
                    Console.Out.WriteLine("do callback");
                    Thread.Sleep(2000);
                }
            });
        }

        public override OperationResult shutdownDynamicDetect(Current current__)
        {
            Console.Out.WriteLine("shutdownDynamicDetect");
            OperationResult or = new FaceRecognitionModule.OperationResult();
            or.code = 200;
            or.message = string.Empty;
            return or;
        }

        public override OperationResult createOrUpdatePerson(string uuid, string name, string code, byte[] img1, byte[] signatureCode1, byte[] img2, byte[] signatureCode2, byte[] img3, byte[] signatureCode3, Current current__)
        {
            Console.Out.WriteLine("createOrUpdatePerson");

            Console.Out.WriteLine("uuid->" + uuid);
            Console.Out.WriteLine("name->" + name);
            Console.Out.WriteLine("code->" + code);
            Console.Out.WriteLine("img1->" + img1?.Length);
            Console.Out.WriteLine("signatureCode1->" + signatureCode1?.Length);

            Console.Out.WriteLine("img2->" + img2?.Length);
            Console.Out.WriteLine("signatureCode2->" + signatureCode2?.Length);

            Console.Out.WriteLine("img3->" + img3?.Length);
            Console.Out.WriteLine("signatureCode3->" + signatureCode3?.Length);

            Console.Out.WriteLine("save ok");

            OperationResult or = new FaceRecognitionModule.OperationResult();
            or.code = 200;
            or.message = string.Empty;
            return or;
        }

        public override OperationResult updatePersonTags(string uuid, string[] tagList, Current current__)
        {
            Console.Out.WriteLine("updatePersonTags");
            Console.Out.WriteLine("uuid->" + uuid);
            Console.Out.WriteLine("tagList->" + tagList?.Length);
            OperationResult or = new FaceRecognitionModule.OperationResult();
            or.code = 200;
            or.message = string.Empty;
            return or;
        }

        public override OperationResult deletePersonTags(string uuid, string[] tagList, Current current__)
        {
            Console.Out.WriteLine("deletePersonTags");
            Console.Out.WriteLine("uuid->" + uuid);
            Console.Out.WriteLine("tagList->" + tagList?.Length);
            OperationResult or = new FaceRecognitionModule.OperationResult();
            or.code = 200;
            or.message = string.Empty;
            return or;
        }

        public override QueryPersonResult queryPerson(string id, string uuid, string code, string[] tagList, int offset, int size, Current current__)
        {
            Console.Out.WriteLine("queryPerson");
            Console.Out.WriteLine("id->" + id);
            Console.Out.WriteLine("uuid->" + uuid);
            Console.Out.WriteLine("code->" + code);
            Console.Out.WriteLine("tagList->" + tagList.Length);
            Console.Out.WriteLine("offset->" + offset);
            Console.Out.WriteLine("size->" + size);

            List<PersonInfo> persons = new List<PersonInfo>();

            QueryPersonResult qr = new QueryPersonResult();
            qr.personInfoList = persons.ToArray();
            qr.totalCount = 10;

            return qr;
        }

        public override OperationResult removePerson(string uuid, Current current__)
        {
            Console.Out.WriteLine("removePerson");
            Console.Out.WriteLine("uuid->" + uuid);
            OperationResult or = new FaceRecognitionModule.OperationResult();
            or.code = 200;
            or.message = string.Empty;
            return or;
        }

        public override FaceInfoResult staticDetect(byte[] img, float threshold, int maxImageCount, Current current__)
        {
            Console.Out.WriteLine("staticDetect---->");

            List<PersonInfo> persons = new List<PersonInfo>();
            persons.Add(new PersonInfo
            {
                id = Guid.NewGuid().ToString("N"),
                uuid = Guid.NewGuid().ToString("N"),
                code = "code",
                name = "ysj",
                race = "汉", //种族
                nationality = "中国",
                gender = "男",
                img1 = new byte[1],
                img2 = new byte[2],
                img3 = new byte[3],
                signatureCode1 = new byte[1],
                signatureCode2 = new byte[2],
                signatureCode3 = new byte[3],
                tagList = new string[100]
            });

            persons.Add(new PersonInfo
            {
                id = Guid.NewGuid().ToString("N"),
                uuid = Guid.NewGuid().ToString("N"),
                code = "code",
                name = "dgl",
                race = "汉", //种族
                nationality = "中国",
                gender = "女",
                img1 = new byte[1],
                img2 = new byte[2],
                img3 = new byte[3],
                signatureCode1 = new byte[1],
                signatureCode2 = new byte[2],
                signatureCode3 = new byte[3],
                tagList = new string[100]
            });

            FaceInfoResult result = new FaceRecognitionModule.FaceInfoResult();
            result.faceInfoList = persons.ToArray();
            result.code = 200;
            result.message = string.Empty;
            return result;
        }

        public override VerifySignatureCodeResult verifySignatureCode(byte[] signatureCode, float threshold, int offset, int size, Current current__)
        {
            Console.Out.WriteLine("verifySignatureCode");

            Console.Out.WriteLine("signaturecode->" + signatureCode?.Length);
            Console.Out.WriteLine("threshold->" + threshold);
            Console.Out.WriteLine("offset->" + offset);
            Console.Out.WriteLine("size->" + size);

            List<VerifySignatureCodeInfo> signutres = new List<VerifySignatureCodeInfo>();
            signutres.Add(new VerifySignatureCodeInfo
            {
                similarity = 0.6f,
                personInfo = new PersonInfo
                {
                    id = Guid.NewGuid().ToString("N"),
                    uuid = Guid.NewGuid().ToString("N"),
                    code = "code",
                    name = "dgl",
                    race = "汉", //种族
                    nationality = "中国",
                    gender = "女",
                    img1 = new byte[1],
                    img2 = new byte[2],
                    img3 = new byte[3],
                    signatureCode1 = new byte[1],
                    signatureCode2 = new byte[2],
                    signatureCode3 = new byte[3],
                    tagList = new string[100]
                }
            });
            VerifySignatureCodeResult result = new VerifySignatureCodeResult();
            result.totalCount = new Random().Next(100, 200);
            result.verifySignatureCodeInfoList = signutres.ToArray();
            return result;
        }
    }
}

using PetsWorldWebservice.XuLy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace PetsWorldWebservice
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://buiphucminhtam.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        //POST FIND OWNER
        //Get Post
        //Get max id in db
        [WebMethod]
        public string GetPostMaxIdFindOwner()
        {
             try
             {
                 //Đầu tiên sẽ query để lấy ra id lớn nhất của bài trong ngày trước
                string query = "select TOP 1 (id) from FindOwner where DAY(datecreated) = DAY(GETDATE()) order by id desc";
                clsDB db = new clsDB();
                double maxId = 0;
                maxId = db.getFirstDoubleValueSqlCatchException(query);
                if(maxId==0){ //Trường hợp này là ko có bài viết nào của ngày hiện tại nên lấy id lớn nhất trong 3 ngày tiếp theo
                   query = "select TOP 1 (id) from FindOwner"+ 
                       " where DAY(datecreated) <= (DAY(GETDATE())-1) AND DAY(datecreated) > (DAY(GETDATE())-4)"+ 
                       " order by id desc";
                    maxId = db.getFirstDoubleValueSqlCatchException(query);
                    if(maxId==0){//Trường hợp này vẫn không có bài viết nào trong 3 ngày tiếp theo nên sẽ lấy id max của tất cả các bài viết đang có
                        query = "SELECT TOP 1(id) FROM FindOwner order by id desc";
                        maxId = db.getFirstDoubleValueSqlCatchException(query);
                    }
                }
                db.CLose_Connection();
                return maxId.ToString();
             }
            catch(Exception e){
                return "0";
            }
        }
        
        //Get 10 new post for load more
         [WebMethod]
        public string GetPostFindOwnerNewest(int id)// gửi lên cái id và sẽ nhận được 10 bài viết cũ, ban đầu sẽ gửi lên id 0 (sử dụng để load more)
        {
            try
            {
                string query = String.Format("select * from FindOwner where id>{0} and id<={0}+10",id);
                clsDB db = new clsDB();
                DataTable dt = db.getDataTable(query);
                string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                db.CLose_Connection();
                return s;
            }
             catch(Exception e)
            {
                 return "0";
            }
            
            
        }
        
        //Get 10 old post for load more
         [WebMethod]
        public string GetPostFindOwnerOlder(int id)// gửi lên cái id và sẽ nhận được 10 bài viết cũ, ban đầu sẽ gửi lên id 0 (sử dụng để load more)
        {
            if(id>0){
                if(id-10>=0) // kiểm tra xem còn đủ 10 bài để lấy ko nếu ko thì lấy hết
                {
                    string query = String.Format("select * from FindOwner where id>={0}-10 and id<{0} order by id desc",id);
                    clsDB db = new clsDB();
                    DataTable dt = db.getDataTable(query);
                    string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                    db.CLose_Connection();
                    return s;
                }
                else
                {
                    string query = String.Format("select * from FindOwner where id<{0}",id);
                    clsDB db = new clsDB();
                    DataTable dt = db.getDataTable(query);
                    string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                    db.CLose_Connection();
                    return s;
                }
                
            }
            
            else return "0";
         }
        
        //Update Post
        [WebMethod]
        public int UpdatePostFindOwner(string jsonPost)
        {
            try{
            JObject post = JObject.Parse(jsonPost);
            string query = "UPDATE FindOwner"
                    +" SET description = @description,"
                    +" requirement = @requirement"
                    +" WHERE id = @id";
            clsDB db = new clsDB();
            SqlCommand cmd = db.pSqlCmd;
            SqlConnection con = db.pConnection;
            cmd.CommandText = query;
            cmd.Connection = con;
            
            //Add values to query
            cmd.Parameters.AddWithValue("@description",post.GetValue("description").ToString());
            cmd.Parameters.AddWithValue("@requirement",post.GetValue("requirement").ToString());
            cmd.Parameters.AddWithValue("@id",Convert.ToInt64(post.GetValue("id")));

            cmd.ExecuteNonQuery();
            con.Close();

            return 1;
            }
            catch(Exception e){
                return 0;
            }
   
        }
           
        //Delete Post
         [WebMethod]
        public int DeletePostFindOwner(int id)
        {
             try{
                string query = String.Format("DELETE FROM FindOwner Where id = {0}",id);
                clsDB db = new clsDB();

                SqlCommand cmd = db.pSqlCmd;
                SqlConnection con = db.pConnection;
                cmd.CommandText = query;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
                con.Close();
                return 1;
             }
             catch(Exception e){
                return 0;
             }
        }

        //Insert Post
         [WebMethod]
        public int InsertPostFindOwner(string jsonPost)
        {
            try{
                JObject post = JObject.Parse(jsonPost);
                string query = "Insert into FindOwner"
                        +" values(@userid,@petid,@description,@requirement,GETDATE())";
                clsDB db = new clsDB();
                SqlCommand cmd = db.pSqlCmd;
                SqlConnection con = db.pConnection;
                cmd.CommandText = query;
                cmd.Connection = con;
            
                //Add values to query
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(post.GetValue("userid").ToString()));
                cmd.Parameters.AddWithValue("@name", post.GetValue("name").ToString());
                cmd.Parameters.AddWithValue("@type", post.GetValue("type").ToString());
                cmd.Parameters.AddWithValue("@vacines",post.GetValue("id").ToString());

                cmd.ExecuteNonQuery();
                con.Close();

                return 1;
            }
            catch(Exception e){
                return 0;
            }
            
        }


        //USER INFO
        //Get User Info
         [WebMethod]
         public String GetUserInfo(int id) {
             try
             {
                 string query = String.Format("select * from UserInfo where id={0}", id);
                 clsDB db = new clsDB();
                 DataTable dt = db.getDataTable(query);
                 string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                 db.CLose_Connection();
                 return s;
             }
             catch (Exception e)
             {
                 return "0";
             }
         }
         //Update User Info
         [WebMethod]
         public int UpdateUserInfo(string jsonUserInfo)
         {
             try
             {
                 JObject userinfo = JObject.Parse(jsonUserInfo);

                 string query = "UPDATE UserInfo"
                        + " SET fullname = @fullname,"
                        + " phone = @phone,"
                        + " address = @address"
                        + " WHERE id = @id";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@fullname", userinfo.GetValue("fullname").ToString());
                 cmd.Parameters.AddWithValue("@phone", userinfo.GetValue("phone").ToString());
                 cmd.Parameters.AddWithValue("@address", userinfo.GetValue("address").ToString());
                 cmd.Parameters.AddWithValue("@id", Int64.Parse(userinfo.GetValue("id").ToString()));
       
                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }

             return 0;
         }


      
        //PET INFO
        //Get Pet Info
         [WebMethod]
         public String GetPetInfo(int id)
         {
             try
             {
                 string query = String.Format("select * from PetInfo where id={0}", id);
                 clsDB db = new clsDB();
                 DataTable dt = db.getDataTable(query);
                 string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                 db.CLose_Connection();
                 return s;
             }
             catch (Exception e)
             {
                 return "0";
             }
         }

        //Insert Pet Info
         [WebMethod]
        public int InsertPetInfo(string jsonPetInfo)
        {

           try{
                JObject post = JObject.Parse(jsonPetInfo);
                string query = "Insert into PetInfo"
                        +" values(@userid,@name,@type,@vacines,GETDATE())";
                clsDB db = new clsDB();
                SqlCommand cmd = db.pSqlCmd;
                SqlConnection con = db.pConnection;
                cmd.CommandText = query;
                cmd.Connection = con;
            
                //Add values to query
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(post.GetValue("userid").ToString()));
                cmd.Parameters.AddWithValue("@name", post.GetValue("name").ToString());
                cmd.Parameters.AddWithValue("@type", Convert.ToInt64(post.GetValue("typeid").ToString()));
                cmd.Parameters.AddWithValue("@vacines",Convert.ToInt64(post.GetValue("id").ToString()));

                cmd.ExecuteNonQuery();
                con.Close();

                return 1;
            }
            catch(Exception e){
                return 0;
            }
        }
        
        //Update Pet Info
         [WebMethod]
         public int UpdatePetInfo(string jsonPetInfo) {
             try{
                 JObject petInfo = JObject.Parse(jsonPetInfo);

                 string query = "UPDATE PetInfo"
                        + " SET name = @name,"
                        + " type = @type,"
                        + " vaccines = @vaccines"
                        + " WHERE id = @id";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@name", petInfo.GetValue("name").ToString());
                 cmd.Parameters.AddWithValue("@type", Convert.ToInt64(petInfo.GetValue("typeid").ToString()));
                 cmd.Parameters.AddWithValue("@vaccines", Convert.ToInt64(petInfo.GetValue("id").ToString()));
                 cmd.Parameters.AddWithValue("@id", Convert.ToInt64(petInfo.GetValue("id")));

                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1; 
              }
             catch(Exception e){
                return 0;
              }

             return 0;
         }

        //Delete Pet Info
         public int DeletePetInfo(int id)
         {
             try
             {
                 string query = String.Format("DELETE FROM PetInfo Where id = {0}", id);
                 clsDB db = new clsDB();

                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;
                 cmd.ExecuteNonQuery();
                 con.Close();
                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }

       

         //PET TYPE
         //Get PetType
         [WebMethod]
         public String GetPetType(int id)
         {
             try
             {
                 string query = String.Format("select * from PetType where id={0}", id);
                 clsDB db = new clsDB();
                 DataTable dt = db.getDataTable(query);
                 string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                 db.CLose_Connection();
                 return s;
             }
             catch (Exception e)
             {
                 return "0";
             }
         }

         //Insert PetType
         [WebMethod]
         public int InsertPetType(string jsonPetType)
         {

             try
             {
                 JObject post = JObject.Parse(jsonPetType);
                 string query = "Insert into PetType"
                         + " values(@typename,GETDATE())";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
           
                 cmd.Parameters.AddWithValue("@name", post.GetValue("name").ToString());
              
                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }

         //Update PetType
         [WebMethod]
         public int UpdatePetType(string jsonPetType)
         {
             try
             {
                 JObject petInfo = JObject.Parse(jsonPetType);

                 string query = "UPDATE PetType"
                        + " SET name = @name,"
                        + " WHERE id = @id";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@name", petInfo.GetValue("name").ToString());
                 cmd.Parameters.AddWithValue("@id", Convert.ToInt64(petInfo.GetValue("id")));

                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }

             return 0;
         }
            
         //Delete PetType
         public int DeletePetType(int id)
         {
             try
             {
                 string query = String.Format("DELETE FROM PetType Where id = {0}", id);
                 clsDB db = new clsDB();

                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;
                 cmd.ExecuteNonQuery();
                 con.Close();
                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }


        //REGISTER AND LOGIN
        //Đăng kí
        [WebMethod]
        public int Register(String userinfo)
        {
             JObject user = JObject.Parse(userinfo);

            //Check user name have the same in db ?
             clsDB db = new clsDB();
    
             //query
             String queryCheck = String.Format("Select id from UserInfo where username = {0}", user.GetValue("username"));
             double id = db.getFirstDoubleValueSqlCatchException(queryCheck);

             if (id > 0)
             {
                 return 0;
             }
             else
             {
                 SqlConnection con = db.pConnection;
                 SqlCommand cmd = db.pSqlCmd;
                 try
                 {

                     String query = "INSERT INTO UserInfo"
                         + " (username,password,fullname,phone,address,datecreated,userimage)"
                         + " VALUES(@username,PWDENCRYPT(@password),@fullname,@phone,@address,@datecreated,@userimage)";

                     cmd.CommandText = query;

                     //Add value to query
                     cmd.Parameters.AddWithValue("@username", user.GetValue("username").ToString());
                     cmd.Parameters.AddWithValue("@password", user.GetValue("password").ToString());
                     cmd.Parameters.AddWithValue("@fullname", user.GetValue("fullname").ToString());
                     cmd.Parameters.AddWithValue("@phone", user.GetValue("phone").ToString());
                     cmd.Parameters.AddWithValue("@address", user.GetValue("address").ToString());
                     cmd.Parameters.AddWithValue("@datecreated", DateTime.Parse(user.GetValue("datecreated").ToString()));
                     cmd.Parameters.AddWithValue("@userimage", user.GetValue("userimage").ToString());

                     cmd.ExecuteNonQuery();
                     con.Close();
                 }
                 catch (Exception e)
                 {
                     return 0;
                 }
             }
             return 1;
        }

        //Đăng nhập
        [WebMethod]
        public string Login(string userName, string passWord)
        {
            DataTable dtResult = new DataTable("DangNhap");
            dtResult.Columns.Add(new DataColumn("RESULT", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("MSG", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("fullname", Type.GetType("System.String")));
            dtResult.Columns.Add(new DataColumn("id", Type.GetType("System.Int32")));
            dtResult.Columns.Add(new DataColumn("userimage", Type.GetType("System.String")));
            DataRow dong = dtResult.NewRow();
            dtResult.Rows.Add(dong);


            dong["RESULT"] = "0";
            dong["MSG"] = "";
            dong["fullname"] = "";
            dong["id"] = "0";
            clsDB db = null;
            try
            {
                db = new clsDB();
                string query = "";
                query = " select ISNULL( PWDCOMPARE(   RTRIM(LTRIM('" + passWord + "')),  ( select  password from UserInfo where username ='" + userName + "' ) )  ,0)  checked  ";

                double kqua = db.getFirstDoubleValueSqlCatchException(query);
                if (kqua <= 0)
                {
                    db.CLose_Connection();
                    dong["RESULT"] = "0";
                    dong["MSG"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    return XuLy.XuLy.ParseDataTableToJSon(dtResult);
                }

                //get fullname and userId
                string fullname = "";
                int id = 0;
                string userimage = "";

                query = " select fullname,id from UserInfo  where username=N'" + userName + "' ";
                DataTable kh = db.getDataTable(query);

                if (kh.Rows.Count > 0)
                {
                    fullname = kh.Rows[0]["fullname"].ToString();
                    id = int.Parse(kh.Rows[0]["id"].ToString());
                }

          
                dong["RESULT"] = "1";
                dong["MSG"] = "Đăng nhập thành công!";
                dong["fullname"] = fullname;
                dong["id"] = id;
                db.CLose_Connection();

                return XuLy.XuLy.ParseDataTableToJSon(dtResult);
            }
            catch (Exception e)
            {
                db.CLose_Connection();
                dong["RESULT"] = "-1";
                dong["MSG"] = "Xảy ra lỗi server khi đăng nhập! (" + e.Message + ")";
                return XuLy.XuLy.ParseDataTableToJSon(dtResult);
            }

        }

  
        //UPLOAD IMAGE
        [WebMethod]
        public string UploadUserImage(string byteArray, int userid,string url)
        {
            try
               {
                 url = SaveImageToStorage(byteArray, userid,url);
                   if (!url.Equals("error"))
                    {
                        string query = "Update UserInfo Set userimage = @url where id = @id";
                        clsDB db = new clsDB();
                        SqlCommand cmd = db.pSqlCmd;
                        SqlConnection con = db.pConnection;
                        cmd.Connection = con;
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@url", url);
                        cmd.Parameters.AddWithValue("@id", userid);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return url;
                    }
                    else return "0";
                }
                catch (Exception e)
                {
                    return "0";
                }
           
            
        }
        [WebMethod]
        public string UploadPetImage(string byteArray,int userid,int petid,string url)
        {
            try
            {
                url = SaveImageToStoragePetImage(byteArray, userid, petid, url);
                if (!url.Equals("error"))
                {
                    string query = "Insert into Photo values(@url,@userid,@petid,GETDATE())";
                    clsDB db = new clsDB();
                    SqlCommand cmd = db.pSqlCmd;
                    SqlConnection con = db.pConnection;
                    cmd.Connection = con;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@petid", petid);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return "1";
                }
                else return url ;
            }
            catch (Exception e)
            {
                return "0";
            }         
        }
      
        public string SaveImageToStoragePetImage(string byteArray,int userid,int petid,string url)
        {
            byte[] image_byte = Convert.FromBase64String(byteArray);
            Int64 unixTimestamp = (Int64)DateTime.Now.Millisecond;
            Guid guid = Guid.NewGuid();
            
            if (url.Equals("None"))
            {
                try
                {// the byte array argument contains the content of the file 
                    // the string argument contains the name and extension 
                    // of the file passed in the byte array 
                    // instance a memory stream and pass the 
                    // byte array to its constructor 
                    MemoryStream ms = new MemoryStream(image_byte);
                    // instance a filestream pointing to the 
                    // storage folder, use the original file name 
                    // to name the resulting file 
                    string path = Server.MapPath
                                ("~/UploadPhoto/") + userid + petid + guid + ".png";
                    FileStream fs = new FileStream(path, FileMode.Create);
                    // write the memory stream containing the original 
                    // file as a byte array to the filestream 
                    ms.WriteTo(fs);
                    // clean up 
                    ms.Close();
                    fs.Close();
                    fs.Dispose();
                    // return OK if we made it this far 
                    return path;
                }
                catch (Exception ex)
                {
                    // return the error message if the operation fails 
                    return "error";
                } 
            }
            else
            {
                try
                {// the byte array argument contains the content of the file 
                    // the string argument contains the name and extension 
                    // of the file passed in the byte array 
                    // instance a memory stream and pass the 
                    // byte array to its constructor 
                    MemoryStream ms = new MemoryStream(image_byte);
                    // instance a filestream pointing to the 
                    // storage folder, use the original file name 
                    // to name the resulting file 
             
                    FileStream fs = new FileStream(url, FileMode.Create);
                    // write the memory stream containing the original 
                    // file as a byte array to the filestream 
                    ms.WriteTo(fs);
                    // clean up 
                    ms.Close();
                    fs.Close();
                    fs.Dispose();
                    // return OK if we made it this far 
                    return url;
                }
                catch (Exception ex)
                {
                    // return the error message if the operation fails 
                    return "error";
                } 
            }
            
        }
        public string SaveImageToStorage(string byteArray, int userid, string url)
        {
            byte[] image_byte = Convert.FromBase64String(byteArray);
            Int64 unixTimestamp = (Int64)DateTime.Now.Millisecond;
            Guid guid = Guid.NewGuid();

            if (url.Equals("None"))
            {
                try
                {// the byte array argument contains the content of the file 
                    // the string argument contains the name and extension 
                    // of the file passed in the byte array 
                    // instance a memory stream and pass the 
                    // byte array to its constructor 
                    MemoryStream ms = new MemoryStream(image_byte);
                    // instance a filestream pointing to the 
                    // storage folder, use the original file name 
                    // to name the resulting file 
                    string path = "UploadPhoto/" + userid + guid + ".png";
                    string pathtosave = Server.MapPath
                                ("~/"+path);
                    FileStream fs = new FileStream(pathtosave, FileMode.Create);
                    // write the memory stream containing the original 
                    // file as a byte array to the filestream 
                    ms.WriteTo(fs);
                    // clean up 
                    ms.Close();
                    fs.Close();
                    fs.Dispose();
                    // return OK if we made it this far 
                    return path;
                }
                catch (Exception ex)
                {
                    // return the error message if the operation fails 
                    return "error";
                }
            }
            else
            {
                try
                {// the byte array argument contains the content of the file 
                    // the string argument contains the name and extension 
                    // of the file passed in the byte array 
                    // instance a memory stream and pass the 
                    // byte array to its constructor 
                    MemoryStream ms = new MemoryStream(image_byte);
                    // instance a filestream pointing to the 
                    // storage folder, use the original file name 
                    // to name the resulting file 

                    FileStream fs = new FileStream(url, FileMode.Create);
                    // write the memory stream containing the original 
                    // file as a byte array to the filestream 
                    ms.WriteTo(fs);
                    // clean up 
                    ms.Close();
                    fs.Close();
                    fs.Dispose();
                    // return OK if we made it this far 
                    return url;
                }
                catch (Exception ex)
                {
                    // return the error message if the operation fails 
                    return "error";
                }
            }

        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

    }

}
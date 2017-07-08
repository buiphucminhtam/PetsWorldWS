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
        public string GetPostFindOwnerNewest(int id)// gửi lên cái id và sẽ nhận được 10 bài viết mới, ban đầu sẽ gửi lên id 0 (sử dụng để load more)
        {
            try
            {
                string query = String.Format("select FindOwner.id,UserInfo.fullname,convert(varchar,FindOwner.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindOwner.userid as [userId], FindOwner.description, FindOwner.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                +" from FindOwner join UserInfo on FindOwner.userid = UserInfo.id" 
                +" join Petinfo on FindOwner.petid = Petinfo.id"
                + " join PetType on PetInfo.typeid = PetType.id where FindOwner.id>{0} and FindOwner.id<={0}+10", id);
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

                    string query = String.Format("select FindOwner.id,UserInfo.fullname,convert(varchar,FindOwner.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindOwner.userid as [userId], FindOwner.description, FindOwner.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                + " from FindOwner join UserInfo on FindOwner.userid = UserInfo.id"
                + " join Petinfo on FindOwner.petid = Petinfo.id"
                + " join PetType on PetInfo.typeid = PetType.id where FindOwner.id>={0}-10 and FindOwner.id<={0} order by id desc", id);
                    clsDB db = new clsDB();
                    DataTable dt = db.getDataTable(query);
                    string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                    db.CLose_Connection();
                    return s;
                }
                else
                {
                    string query = String.Format("select FindOwner.id,UserInfo.fullname,convert(varchar,FindOwner.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindOwner.userid as [userId], FindOwner.description, FindOwner.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                + " from FindOwner join UserInfo on FindOwner.userid = UserInfo.id" 
                + " join Petinfo on FindOwner.petid = Petinfo.id"
                + " join PetType on PetInfo.typeid = PetType.id where FindOwner.id<{0}", id);
                    clsDB db = new clsDB();
                    DataTable dt = db.getDataTable(query);
                    string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                    db.CLose_Connection();
                    return s;
                }
                
            }
            
            else return "0";
         }
        

        //Get post with user id
         [WebMethod]
        public string GetPostFindOwnerByUserId(int userId){
            try
            {
                string query = String.Format("select FindOwner.id,UserInfo.fullname,convert(varchar,FindOwner.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindOwner.userid as [userId], FindOwner.description, FindOwner.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                + " from FindOwner join UserInfo on FindOwner.userid = UserInfo.id"
                + " join Petinfo on FindOwner.petid = Petinfo.id"
                + " join PetType on PetInfo.typeid = PetType.id where FindOwner.userid = {0}", userId);
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
          [WebMethod]
         public string GetPostFindOwnerByAndPetId(int userid, int petid)
         {
             try
             {
                 string query = String.Format("select FindOwner.id,UserInfo.fullname,convert(varchar,FindOwner.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindOwner.userid as [userId], FindOwner.description, FindOwner.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                 + " from FindOwner join UserInfo on FindOwner.userid = UserInfo.id"
                 + " join Petinfo on FindOwner.petid = Petinfo.id"
                 + " join PetType on PetInfo.typeid = PetType.id where FindOwner.userid = {0} and FindOwner.petid = {1}", userid, petid);
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
                string query = "Insert into FindOwner " + "OUTPUT INSERTED.ID"
                        +" values(@userid,@petid,@description,@requirement,GETDATE())";
                clsDB db = new clsDB();
                SqlCommand cmd = db.pSqlCmd;
                SqlConnection con = db.pConnection;
                cmd.CommandText = query;
                cmd.Connection = con;
            
                //Add values to query
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(post.GetValue("userid").ToString()));
                cmd.Parameters.AddWithValue("@petid", Convert.ToInt64(post.GetValue("petid").ToString()));
                cmd.Parameters.AddWithValue("@description", post.GetValue("description").ToString());
                cmd.Parameters.AddWithValue("@requirement", post.GetValue("requirement").ToString());

                Int32 id = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                return id;
            }
            catch(Exception e){
                return 0;
            }
            
        }

        //POST FIND PET
         [WebMethod]
         public string GetPostMaxIdFindPet()
         {
             try
             {
                 //Đầu tiên sẽ query để lấy ra id lớn nhất của bài trong ngày trước
                 string query = "select TOP 1 (id) from FindPet where DAY(datecreated) = DAY(GETDATE()) order by id desc";
                 clsDB db = new clsDB();
                 double maxId = 0;
                 maxId = db.getFirstDoubleValueSqlCatchException(query);
                 if (maxId == 0)
                 { //Trường hợp này là ko có bài viết nào của ngày hiện tại nên lấy id lớn nhất trong 3 ngày tiếp theo
                     query = "select TOP 1 (id) from FindPet" +
                         " where DAY(datecreated) <= (DAY(GETDATE())-1) AND DAY(datecreated) > (DAY(GETDATE())-4)" +
                         " order by id desc";
                     maxId = db.getFirstDoubleValueSqlCatchException(query);
                     if (maxId == 0)
                     {//Trường hợp này vẫn không có bài viết nào trong 3 ngày tiếp theo nên sẽ lấy id max của tất cả các bài viết đang có
                         query = "SELECT TOP 1(id) FROM FindPet order by id desc";
                         maxId = db.getFirstDoubleValueSqlCatchException(query);
                     }
                 }
                 db.CLose_Connection();
                 return maxId.ToString();
             }
             catch (Exception e)
             {
                 return "0";
             }
         }

         //Get 10 new post for load more
         [WebMethod]
         public string GetPostFindPetNewest(int id)// gửi lên cái id và sẽ nhận được 10 bài viết cũ, ban đầu sẽ gửi lên id 0 (sử dụng để load more)
         {
             try
             {
                 string query = String.Format("select FindPet.id,UserInfo.fullname,convert(varchar,FindPet.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindPet.userid as [userId], FindPet.description, FindPet.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                 + " from FindPet join UserInfo on FindPet.userid = UserInfo.id"
                 + " join Petinfo on FindPet.petid = Petinfo.id"
                 + " join PetType on PetInfo.typeid = PetType.id where FindPet.id>{0} and FindPet.id<={0}+10", id);
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

         //Get 10 old post for load more
         [WebMethod]
         public string GetPostFindPetOlder(int id)// gửi lên cái id và sẽ nhận được 10 bài viết cũ, ban đầu sẽ gửi lên id 0 (sử dụng để load more)
         {
             if (id > 0)
             {
                 if (id - 10 >= 0) // kiểm tra xem còn đủ 10 bài để lấy ko nếu ko thì lấy hết
                 {

                     string query = String.Format("select FindPet.id,UserInfo.fullname,convert(varchar,FindPet.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindPet.userid as [userId], FindPet.description, FindPet.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                 + " from FindPet join UserInfo on FindPet.userid = UserInfo.id"
                 + " join Petinfo on FindPet.petid = Petinfo.id"
                 + " join PetType on PetInfo.typeid = PetType.id where FindPet.id>={0}-10 and FindPet.id<={0} order by id desc", id);
                     clsDB db = new clsDB();
                     DataTable dt = db.getDataTable(query);
                     string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                     db.CLose_Connection();
                     return s;
                 }
                 else
                 {
                     string query = String.Format("select FindPet.id,UserInfo.fullname,convert(varchar,FindPet.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindPet.userid as [userId], FindPet.description, FindPet.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                 + " from FindPet join UserInfo on FindPet.userid = UserInfo.id"
                 + " join Petinfo on FindPet.petid = Petinfo.id"
                 + " join PetType on PetInfo.typeid = PetType.id where FindPet.id<{0}", id);
                     clsDB db = new clsDB();
                     DataTable dt = db.getDataTable(query);
                     string s = XuLy.XuLy.ParseDataTableToJSon(dt);
                     db.CLose_Connection();
                     return s;
                 }

             }

             else return "0";
         }


         //Get post with user id
         [WebMethod]
         public string GetPostFindPetByUserId(int userId)
         {
             try
             {
                 string query = String.Format("select FindPet.id,UserInfo.fullname,convert(varchar,FindPet.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindPet.userid as [userId], FindPet.description, FindPet.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                 + " from FindPet join UserInfo on FindPet.userid = UserInfo.id"
                 + " join Petinfo on FindPet.petid = Petinfo.id"
                 + " join PetType on PetInfo.typeid = PetType.id where FindPet.userid = {0}", userId);
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
            [WebMethod]
         public string GetPostFindPetByUserIdAndPetId(int userid, int petid)
         {
             try
             {
                 string query = String.Format("select FindPet.id,UserInfo.fullname,convert(varchar,FindPet.datecreated,100) as [datecreated],PetInfo.id as [petId],PetInfo.name as [petname],PetType.typename,PetInfo.vaccine, FindPet.userid as [userId], FindPet.description, FindPet.requirement, convert(varchar,PetInfo.vaccinedate,103) as [vaccinedate]"
                 + " from FindPet join UserInfo on FindPet.userid = UserInfo.id"
                 + " join Petinfo on FindPet.petid = Petinfo.id"
                 + " join PetType on PetInfo.typeid = PetType.id where FindPet.userid = {0} and FindPet.petid = {1}",userid,petid);
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

         //Update Post
         [WebMethod]
         public int UpdatePostFindPet(string jsonPost)
         {
             try
             {
                 JObject post = JObject.Parse(jsonPost);
                 string query = "UPDATE FindPet"
                         + " SET description = @description,"
                         + " requirement = @requirement"
                         + " WHERE id = @id";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@description", post.GetValue("description").ToString());
                 cmd.Parameters.AddWithValue("@requirement", post.GetValue("requirement").ToString());
                 cmd.Parameters.AddWithValue("@id", Convert.ToInt64(post.GetValue("id")));

                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }

         }

         //Delete Post
         [WebMethod]
         public int DeletePostFindPet(int id)
         {
             try
             {
                 string query = String.Format("DELETE FROM FindPet Where id = {0}", id);
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

         //Insert Post
         [WebMethod]
         public int InsertPostFindPet(string jsonPost)
         {
             try
             {
                 JObject post = JObject.Parse(jsonPost);
                 string query = "Insert into FindPet " + "OUTPUT INSERTED.ID"
                         + " values(@userid,@petid,@description,@requirement,GETDATE())";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(post.GetValue("userid").ToString()));
                 cmd.Parameters.AddWithValue("@petid", Convert.ToInt64(post.GetValue("petid").ToString()));
                 cmd.Parameters.AddWithValue("@description", post.GetValue("description").ToString());
                 cmd.Parameters.AddWithValue("@requirement", post.GetValue("requirement").ToString());

                 Int32 id = Convert.ToInt32(cmd.ExecuteScalar());
                 con.Close();

                 return id;
             }
             catch (Exception e)
             {
                 return 0;
             }

         }


        //USER INFO
        //Get User Info
         [WebMethod]
         public String GetUserInfo(int id) {
             try
             {
                 string query = String.Format("select id,username,fullname,phone,address,datecreated,userimage,state from UserInfo where id={0}", id);
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
         //ChangeUserPassword
         [WebMethod]
         public int ChangePassword(string newpassword,int id) {
             try
             {
                  string query = "UPDATE UserInfo"
                        + " SET password = PWDENCRYPT(@password)"
                        + " WHERE id = @id";

                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
               
                 cmd.Parameters.AddWithValue("@id", id);
                 cmd.Parameters.AddWithValue("@password",newpassword);
       
                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1;
             }catch(Exception e)
             {
                 return 0;
             }
         }

         [WebMethod]
         public int LockUser(int id)
         {
             try
             {
                 string query = "Update UserInfo set state = 2 where id = @id";

                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query

                 cmd.Parameters.AddWithValue("@id", id);

                 cmd.ExecuteNonQuery();
                 con.Close();
                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }
         [WebMethod]
         public int UnlockUser(int id)
         {
             try
             {
                 string query = "Update UserInfo set state = 1 where id = @id";

                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query

                 cmd.Parameters.AddWithValue("@id", id);

                 cmd.ExecuteNonQuery();
                 con.Close();
                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }
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
                string query = "Insert into PetInfo " + "OUTPUT INSERTED.ID"
                        + " values(@userid,@name,@type,@vaccine,@vaccinedate)";
                clsDB db = new clsDB();
                SqlCommand cmd = db.pSqlCmd;
                SqlConnection con = db.pConnection;
                cmd.CommandText = query;
                cmd.Connection = con;
            
                //Add values to query
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(post.GetValue("userid").ToString()));
                cmd.Parameters.AddWithValue("@name", post.GetValue("name").ToString());
                cmd.Parameters.AddWithValue("@type", Convert.ToInt64(post.GetValue("typeid").ToString()));
                cmd.Parameters.AddWithValue("@vaccine", Convert.ToInt64(post.GetValue("vaccine").ToString()));
                cmd.Parameters.AddWithValue("@vaccinedate", post.GetValue("vaccinedate").ToString());

                 Int32 id = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                return id;
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
                        + " typeid = @typeid,"
                        + " vaccine = @vaccine,"
                        + " vaccinedate = @vaccinedate"
                        + " WHERE id = @id";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@name", petInfo.GetValue("name").ToString());
                 cmd.Parameters.AddWithValue("@typeid", Convert.ToInt64(petInfo.GetValue("typeid").ToString()));
                 cmd.Parameters.AddWithValue("@vaccine", Convert.ToInt64(petInfo.GetValue("vaccine").ToString()));
                 cmd.Parameters.AddWithValue("@vaccinedate", petInfo.GetValue("vaccinedate").ToString());
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
         [WebMethod]
         public int DeletePetInfo(int id)
         {
             try
             {
                 if (DeletePhotoById(id) == 1)
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
                 else
                     return 0;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }

         private int DeletePhotoById(int petId) {
             try
             {
                 string query = String.Format("Select * from Photo Where petid = {0}", petId);
                 clsDB db = new clsDB();
                 DataTable dt = db.getDataTable(query);
                 String result = XuLy.XuLy.ParseDataTableToJSon(dt);
                 JArray JArray = JArray.Parse(result);
                 foreach (JObject o in JArray.Children<JObject>())
                 {
                     string url = o.GetValue("url").ToString();
                     DeleteFileFromFolder(url);
                 }
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
         public String GetPetType()
         {
             try
             {
                 string query = String.Format("select * from PetType");
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
                         + " values(@name,GETDATE())";
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
                        + " SET typename = @typename,"
                        + " WHERE id = @id";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@typename", petInfo.GetValue("typename").ToString());
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

        //REPORT
        //GET REPORT
         [WebMethod]
         public String GetReport()
         {
             try
             {
                 string query = String.Format("select id,userid,petid,msg,convert(varchar,Report.datecreated,100) as [datecreated],typepost from Report");
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

        //GET REPORT BY DATE
         [WebMethod]
         public String GetReportByDate(string date, int type)
         {
             try
             {
                 string query = "select id,userid,petid,msg,convert(varchar,Report.datecreated,100) as [datecreated],typepost from Report"
                 + " where convert(varchar,Report.datecreated,103) = @date and Report.typepost = @type";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query
                 cmd.Parameters.AddWithValue("@date", date);
                 cmd.Parameters.AddWithValue("@type", type);

                 DataTable dt = new DataTable("Table");
                 dt.Load(cmd.ExecuteReader());
                 string s = XuLy.XuLy.ParseDataTableToJSon(dt);

                 con.Close();
                 return s;
             }
             catch (Exception e)
             {
                 return "0";
             }
         }

        //INSERT REPORT
         [WebMethod]
         public int InsertReport(string jsonReport)
         {

             try
             {
                 JObject report = JObject.Parse(jsonReport);
                 string query = "Insert into Report"
                         + " values(@userid,@petid,@msg,GETDATE(),@typepost)";
                 clsDB db = new clsDB();
                 SqlCommand cmd = db.pSqlCmd;
                 SqlConnection con = db.pConnection;
                 cmd.CommandText = query;
                 cmd.Connection = con;

                 //Add values to query

                 cmd.Parameters.AddWithValue("@userid", int.Parse(report.GetValue("userid").ToString()));
                 cmd.Parameters.AddWithValue("@petid", int.Parse(report.GetValue("petid").ToString()));
                 cmd.Parameters.AddWithValue("@msg", report.GetValue("msg").ToString());
                 cmd.Parameters.AddWithValue("@typepost", int.Parse(report.GetValue("typepost").ToString()));

                 cmd.ExecuteNonQuery();
                 con.Close();

                 return 1;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }

        //DELETE REPORT
         public int DeleteReport(int id)
         {
             try
             {
                 string query = String.Format("DELETE FROM Report Where id = {0}", id);
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
        //Lock Account
        //Unlock Account
        //Đăng kí
        [WebMethod]
        public int Register(String userinfo)
        {
             JObject user = JObject.Parse(userinfo);

            //Check user name have the same in db ?
             clsDB db = new clsDB();
    
             //query
             String queryCheck = String.Format("Select id from UserInfo where UserInfo.username = {0}", user.GetValue("username").ToString());
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
                         + " (username,password,fullname,phone,address,datecreated,userimage,state)"
                         + " VALUES(@username,PWDENCRYPT(@password),@fullname,@phone,@address,GETDATE(),@userimage,1)";

                     cmd.CommandText = query;

                     //Add value to query
                     cmd.Parameters.AddWithValue("@username", user.GetValue("username").ToString());
                     cmd.Parameters.AddWithValue("@password", user.GetValue("password").ToString());
                     cmd.Parameters.AddWithValue("@fullname", user.GetValue("fullname").ToString());
                     cmd.Parameters.AddWithValue("@phone", user.GetValue("phone").ToString());
                     cmd.Parameters.AddWithValue("@address", user.GetValue("address").ToString());
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
            dtResult.Columns.Add(new DataColumn("state", Type.GetType("System.Int32")));
            DataRow dong = dtResult.NewRow();
            dtResult.Rows.Add(dong);


            dong["RESULT"] = "0";
            dong["MSG"] = "";
            dong["fullname"] = "";
            dong["id"] = "0";
            dong["state"] = "0";

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
                int state = 0;


                query = " select fullname,id,state from UserInfo  where username=N'" + userName + "' ";
                DataTable kh = db.getDataTable(query);

                if (kh.Rows.Count > 0)
                {
                    fullname = kh.Rows[0]["fullname"].ToString();
                    id = int.Parse(kh.Rows[0]["id"].ToString());
                    state = int.Parse(kh.Rows[0]["state"].ToString());
                }

          
                dong["RESULT"] = "1";
                dong["MSG"] = "Đăng nhập thành công!";
                dong["fullname"] = fullname;
                dong["id"] = id;
                dong["state"] = state;

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
        //GET IMAGE 
        [WebMethod]
        public string GetPhotoById(int petId) {
            try
            {
                string query = String.Format("select * from Photo where petid = {0}",petId);
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
        public string UploadPetImage(string byteArray,int userid,int petid)
        {
            try
            {
                String url = SaveImageToStoragePetImage(byteArray, userid, petid);
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
      
        public string SaveImageToStoragePetImage(string byteArray,int userid,int petid)
        {
            byte[] image_byte = Convert.FromBase64String(byteArray);
            Int64 unixTimestamp = (Int64)DateTime.Now.Millisecond;
            Guid guid = Guid.NewGuid();
            
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
                    string path = "UploadPhoto/" + userid + petid + guid + ".png";
                    string pathtosave = Server.MapPath
                                ("~/" + path);
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
                    string pathtosave = Server.MapPath
                                 ("~/" + url);
                    FileStream fs = new FileStream(pathtosave, FileMode.Create);
                    
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

        public void DeleteFileFromFolder(string StrFilename)
        {

            string strPhysicalFolder = Server.MapPath("~/");

            string strFileFullPath = strPhysicalFolder + StrFilename;

            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
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
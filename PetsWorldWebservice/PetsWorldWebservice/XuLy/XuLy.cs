using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using Newtonsoft.Json;

namespace PetsWorldWebservice.XuLy
{
    public class XuLy
    {


        public XuLy()
        {

        }
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0,
                                             imageBytes.Length))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }
        public static int Songaytrongthang(int month, int year)
        {
            int ngay = 0;
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    {
                        ngay = 31;
                        break;
                    }
                case 4:
                case 6:
                case 9:
                case 11:
                    {
                        ngay = 30;
                        break;
                    }
                case 2:
                    {
                        if (year % 4 == 0)
                            ngay = 29;
                        else
                            ngay = 28;
                        break;
                    }
            }

            return ngay;
        }
        public static string ThuTrongTuan()
        {
            DayOfWeek ngay = DateTime.Now.DayOfWeek;
            string ngaythang = "";

            switch (ngay)
            {
                case DayOfWeek.Monday:
                    ngaythang = "2";
                    break;
                case DayOfWeek.Tuesday:
                    ngaythang = "3";
                    break;
                case DayOfWeek.Wednesday:
                    ngaythang = "4";
                    break;
                case DayOfWeek.Thursday:
                    ngaythang = "5";
                    break;
                case DayOfWeek.Friday:
                    ngaythang = "6";
                    break;
                case DayOfWeek.Saturday:
                    ngaythang = "7";
                    break;
                case DayOfWeek.Sunday:
                    ngaythang = "8";
                    break;
            }

            return ngaythang;
        }


        public static string BoDau(string ip_str_change)
        {
            ip_str_change = ip_str_change.Trim();
            Regex v_reg_regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string v_str_FormD = ip_str_change.Normalize(NormalizationForm.FormD);
            string kq = v_reg_regex.Replace(v_str_FormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            return kq.Trim();
            //return Regex.Replace(kq.Trim(), " ", "-");
        }

        public static int GetWeekOfMonth()
        {
            DateTime date = DateTime.Now;
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;


        }


        public static double getKhoangCachHaversine(double lat1, double long1, double lat2, double long2)
        {
            double R = 6371.00;
            double dLat, dLon, a, c;

            dLat = toRad(lat2 - lat1);
            dLon = toRad(long2 - long1);
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);
            a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c * 1000; //m
        }

        public static double getKhoangCachGoogle(double lat1, double long1, double lat2, double long2)
        {
            CultureInfo culture = new CultureInfo("en-US");
            try
            {
                //Đo khoảng cách
                string result = "";
                string sURL;
                sURL = "http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + lat1 + "," + long1 + "&destinations=" + lat2 + "," + long2 + "&mode=walking&sensor=false";

                WebRequest wrGETURL = WebRequest.Create(sURL);
                Stream objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                string sLine = "";
                int i = 0;
                while (sLine != null)
                {
                    i++;
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        result += sLine;
                }

                // Create an XmlReader
                using (XmlReader reader = XmlReader.Create(new StringReader(result)))
                {
                    result = "-1";
                    reader.ReadToFollowing("distance");
                    reader.ReadToFollowing("value");
                    result = reader.ReadElementContentAsString(); //Khoảng cách lấy được
                }

                return Double.Parse(result, culture);

            }
            catch (Exception ex)
            {
                return -1;

            }
        }


        private static double toRad(double value)
        {
            return value * Math.PI / 180;
        }


        public static int LaySoNgay(int sothang)
        {


            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths(((-1) * sothang) + 1);
            string thangbatdau = ngay_thang_bd.Month.ToString();
            if (thangbatdau.Length < 2)
                thangbatdau = "0" + thangbatdau;


            DateTime ngay_thang_kt = DateTime.Now.AddMonths(-1);
            string thangketthuc = ngay_thang_kt.Month.ToString();
            if (thangketthuc.Length < 2)
                thangketthuc = "0" + thangketthuc;

            return ngay_thang_kt.Subtract(ngay_thang_bd).Days;
        }

        public static int LaySoNgayDS(int sothang)
        {

            DateTime ngay_thang_ht = DateTime.Now;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths((-1) * sothang);

            int tongsongay = 0;

            tongsongay = ngay_thang_ht.Subtract(ngay_thang_bd).Days;

            return tongsongay;
        }

        public static string[] getCurent()
        {
            string[] thanght = new string[2];
            string thang = DateTime.Now.Month.ToString();
            if (thang.Length < 2)
                thang = "0" + thang;

            string ngay = DateTime.Now.Day.ToString();
            if (ngay.Length < 2)
                ngay = "0" + ngay;

            thanght[0] = DateTime.Now.Year.ToString() + "-" + thang + "-01";
            thanght[1] = DateTime.Now.Year.ToString() + "-" + thang + "-" + ngay;
            return thanght;

        }

        public static string[] ThangBD_KT(int sothang)
        {
            string[] thang = new string[2];

            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths(((-1) * sothang) + 1);
            string thangbatdau = ngay_thang_bd.Month.ToString();
            if (thangbatdau.Length < 2)
                thangbatdau = "0" + thangbatdau;

            thang[0] = ngay_thang_bd.Year.ToString() + "-" + thangbatdau + "-01";


            DateTime ngay_thang_kt = DateTime.Now.AddMonths(-1);
            string thangketthuc = ngay_thang_kt.Month.ToString();
            if (thangketthuc.Length < 2)
                thangketthuc = "0" + thangketthuc;

            //thang[0] = iYear.ToString() + "-" + thangketthuc + "-01";
            //thang[1] = iYear.ToString() + "-" + thangbatdau + "-" + LastDayOfMonth(iMonth - 1, iYear);


            thang[1] = ngay_thang_kt.Year.ToString() + "-" + thangketthuc + "-" + LastDayOfMonth(ngay_thang_kt.Month, ngay_thang_kt.Year);
            return thang;
        }

        public static string[] ThangBD_KT_TinhDS(int sothang)
        {
            string[] thang = new string[2];

            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;

            DateTime ngay_thang_bd = DateTime.Now.AddMonths(-sothang);
            string thangbatdau = ngay_thang_bd.Month.ToString();
            if (thangbatdau.Length < 2)
                thangbatdau = "0" + thangbatdau;

            thang[0] = ngay_thang_bd.Year.ToString() + "-" + thangbatdau + "-01";


            DateTime ngay_thang_kt = DateTime.Now.AddMonths(-1);
            string thangketthuc = ngay_thang_kt.Month.ToString();
            if (thangketthuc.Length < 2)
                thangketthuc = "0" + thangketthuc;

            //thang[0] = iYear.ToString() + "-" + thangketthuc + "-01";
            //thang[1] = iYear.ToString() + "-" + thangbatdau + "-" + LastDayOfMonth(iMonth - 1, iYear);


            thang[1] = ngay_thang_kt.Year.ToString() + "-" + thangketthuc + "-" + LastDayOfMonth(ngay_thang_kt.Month, ngay_thang_kt.Year);
            return thang;
        }

        public static string LaySoNgayToiHienTai()
        {
            return DateTime.Now.Day.ToString();
        }

        public static string LastDayOfMonth(int month, int year)
        {
            string ngay = "";
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    {
                        ngay = "31";
                        break;
                    }
                case 4:
                case 6:
                case 9:
                case 11:
                    {
                        ngay = "30";
                        break;
                    }
                case 2:
                    {
                        if (year % 4 == 0)
                            ngay = "29";
                        else
                            ngay = "28";
                        break;
                    }
            }

            return ngay;
        }

        public static string getDateTime()
        {
            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;
            int iDay = DateTime.Now.Day;

            return iYear + "-" + (iMonth < 10 ? ("0" + iMonth.ToString()) : iMonth.ToString()) + "-" + (iDay < 10 ? ("0" + iDay.ToString()) : iDay.ToString());
        }

        public static string getDateTimeVN()
        {
            int iYear = DateTime.Now.Year;
            int iMonth = DateTime.Now.Month;
            int iDay = DateTime.Now.Day;

            return (iDay < 10 ? ("0" + iDay.ToString()) : iDay.ToString()) + "-" + (iMonth < 10 ? ("0" + iMonth.ToString()) : iMonth.ToString()) + "-" + iYear;
        }

        /**
         * Kiểm tra tọa độ có đang nằm trong đất nước Việt Nam hay không (tương đối)
         */
        public static bool dangOVietNam(string lat, string lon)
        {
            CultureInfo culture = new CultureInfo("en-US");
            double dlat = double.Parse(lat, culture);
            double dlon = double.Parse(lon, culture);
            return 8.5 < dlat && dlat < 23.4 //vĩ độ cực bắc, cực nam
                && 102.14 < dlon && dlon < 109.5; //kinh độ cực bắc, cực nam
        }

        public static bool dangOVietNam(double lat, double lon)
        {
            return 8.5 < lat && lat < 23.4 //vĩ độ cực bắc, cực nam
                && 102.14 < lon && lon < 109.5; //kinh độ cực bắc, cực nam
        }

        public static string quyen_sanpham(string userId)
        {
            return "( select sanpham_fk from nhanvien_sanpham where nhanvien_fk ='" + userId + "')";
        }

        public static string quyen_npp(string userId, int loai, string loaiId)
        {
            string today = XuLy.getDateTime();
            if (loai == 3)
            {
                //GSBH
                return " ( SELECT NPP_FK FROM NHAPP_GIAMSATBH WHERE GSBH_FK = '" + loaiId + "' AND NGAYBATDAU <= '" + today + "' AND '" + today + "' <= NGAYKETTHUC ) ";
            }
            else if (loai == 2)
            {
                //ASM
                return " ( SELECT NPP_FK FROM NHAPP_GIAMSATBH " +
                        "   WHERE GSBH_FK IN (  " +
                        " 	    SELECT PK_SEQ FROM GIAMSATBANHANG  " +
                        " 	    WHERE KHUVUC_FK IN ( SELECT KHUVUC_FK FROM ASM_KHUVUC WHERE ASM_FK = '" + loaiId + "' ) " +
                        " 		    AND TRANGTHAI = 1 " +
                        "   ) AND NGAYBATDAU <= '" + today + "' AND '" + today + "' <= NGAYKETTHUC " +
                        " ) ";
            }
            else if (loai == 1)
            {
                //BM
                return " ( SELECT NPP_FK FROM NHAPP_GIAMSATBH " +
                        "   WHERE GSBH_FK IN (  " +
                        " 	    SELECT PK_SEQ FROM GIAMSATBANHANG  " +
                        " 	    WHERE KHUVUC_FK IN ( SELECT PK_SEQ FROM KHUVUC WHERE VUNG_FK IN ( SELECT VUNG_FK FROM BM_CHINHANH WHERE BM_FK = '" + loaiId + "' ) ) " +
                        " 		    AND TRANGTHAI = 1 " +
                        "   ) AND NGAYBATDAU <= '" + today + "' AND '" + today + "' <= NGAYKETTHUC " +
                        " ) ";
            }

            return " ( select npp_fk from phamvihoatdong where nhanvien_fk ='" + userId + "') ";
        }

        public static string quyen_kenh(string userId)
        {
            return "( select kenh_fk as kbh_fk from nhanvien_kenh where nhanvien_fk ='" + userId + "' )";
        }

        public static string getIdNpp(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM NHAPHANPHOI " +
                    "   WHERE TRANGTHAI = 1 AND PK_SEQ IN " + quyen_npp(nvId, loai, loaiId) +
                //"       AND PK_SEQ IN ( " +
                //"           SELECT NPP_FK FROM NHAPP_KBH WHERE KBH_FK IN " + quyen_kenh(nvId) +
                //"       ) " +
                    " ) ";
        }

        public static string getIdDdkd(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM DAIDIENKINHDOANH WHERE TRANGTHAI = 1 AND NPP_FK IN " + getIdNpp(nvId, loai, loaiId) + " ) ";
        }

        public static string getIdGsbh(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM GIAMSATBANHANG WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT GSBH_FK FROM GSBH_KHUVUC WHERE KHUVUC_FK IN ( SELECT KHUVUC_FK FROM NHAPHANPHOI WHERE PK_SEQ IN " + getIdNpp(nvId, loai, loaiId) + " ) ) ) ";
        }

        public static string getIdAsm(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM ASM WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT ASM_FK FROM ASM_KHUVUC WHERE KHUVUC_FK IN ( SELECT KHUVUC_FK FROM NHAPHANPHOI WHERE PK_SEQ IN " + getIdNpp(nvId, loai, loaiId) + " ) ) ) ";
        }

        public static string getIdBm(string nvId, int loai, string loaiId)
        {
            return " ( SELECT PK_SEQ FROM BM WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT BM_FK FROM BM_CHINHANH WHERE VUNG_FK IN ( SELECT VUNG_FK FROM KHUVUC WHERE TRANGTHAI = 1 AND PK_SEQ IN ( SELECT KHUVUC_FK FROM NHAPHANPHOI WHERE PK_SEQ IN " + getIdNpp(nvId, loai, loaiId) + " ) ) ) ) ";
        }

        public static int parseInt(string value, int exceptionValue)
        {
            int so = exceptionValue;
            try { so = int.Parse(value); }
            catch { }
            return so;
        }
        public static double parseDouble(string value, double exceptionValue)
        {
            double so = exceptionValue;
            try { so = double.Parse(value); }
            catch { }
            return so;
        }

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        public static bool checkDateFormat(string inputString, string formatString)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(inputString, formatString, System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string remote = "Server=dms.hoathien.vn;UID=salesup;PWD=htp@2016;";
        // public static string remote = "Server=.;Trusted_Connection=yes;";

        public static void CreateTonHienTai(clsDB db, string @ma_dvcs, string @ma_kho, string @ma_vi_tri, string @masp)
        {
            string @ma_vi_triCondition = @ma_vi_tri;
            string query = "";


            if (@ma_vi_triCondition.Equals("00"))
                @ma_vi_triCondition = "";

            string sql = "\n IF OBJECT_ID('tempdb.dbo.#TonHienTai') IS NOT NULL DROP TABLE #TonHienTai  " +
                                "\n CREATE TABLE #TonHienTai ( ma_dvcs nvarchar(500),ma_kho nvarchar(500),ma_vi_tri nvarchar(500),ma_vt nvarchar(500), ton float ) " +
                                "\n if isnull((select dongboERP from nhaphanphoi where mafast ='" + ma_dvcs + "'),0) = 1 " +
                                "\n     insert #TonHienTai (ma_dvcs,ma_kho,ma_vi_tri,ma_vt,ton)   " +
                                "\n     select distinct ma_dvcs,ma_kho, case ma_vi_tri when '' then '00' else ma_vi_tri end ma_vi_tri ,ma_vt,ton from  OPENROWSET('SQLNCLI', '" + remote + "','SET FMTONLY OFF; " +
                                "\n     exec [FAST].[HTP_FBOR2_A].dbo.Fast$DMSGESO$CurrentStock ''" + ma_dvcs + "'',''" + ma_kho + "'',''" + ma_vi_triCondition + "'',''" + masp + "'' ') where 1=1 ";

            int sodo = db.updateQueryReturnInt(sql);



            query =
                             "\n if isnull((select dongboERP from nhaphanphoi where mafast ='" + ma_dvcs + "'),0) = 0 " +
                             "\n     insert #TonHienTai (ma_dvcs,ma_kho,ma_vi_tri,ma_vt,ton)   " +
                             "\n     select distinct ma_dvcs,ma_kho, ma_vi_tri ,ma_vt,ton from  Kho_Test_HTP x " +
                             "\n     where 1= 1 and not exists (select 1 from #TonHienTai y where  y.ma_dvcs = x.ma_dvcs and y.ma_kho = x.ma_kho and y.ma_vi_tri = x.ma_vi_tri  and y.ma_vt = x.ma_vt )   ";
            if (@ma_dvcs.Trim().Length > 0)
                query += " and ma_dvcs = '" + @ma_dvcs + "'";
            if (@ma_kho.Trim().Length > 0)
                query += " and ma_kho  ='" + @ma_kho + "' ";
            if (@ma_vi_tri.Trim().Length > 0)
                query += " and ma_vi_tri =  '" + @ma_vi_tri + "' ";
            if (@masp.Trim().Length > 0)
                query += " and masp =  '" + @masp + "'";

            int sodo2 = db.updateQueryReturnInt(query);


        }


        public static void CreateTonDau(clsDB db, string @ma_dvcs, string @ma_kho, string @ma_vi_tri, string @masp)
        {
            string @ma_vi_triCondition = @ma_vi_tri;
            string query = "";


            if (@ma_vi_triCondition.Equals("00"))
                @ma_vi_triCondition = "";

            string sql = "\n IF OBJECT_ID('tempdb.dbo.#TonDau') IS NOT NULL DROP TABLE #TonDau  " +
                                "\n CREATE TABLE #TonDau ( ma_dvcs nvarchar(500),ma_kho nvarchar(500),ma_vi_tri nvarchar(500),ma_vt nvarchar(500), ton float ) " +
                                "\n if isnull((select dongboERP from nhaphanphoi where mafast ='" + ma_dvcs + "'),0) = 1 " +
                                "\n     insert #TonHienTai (ma_dvcs,ma_kho,ma_vi_tri,ma_vt,ton)   " +
                                "\n     select distinct ma_dvcs,ma_kho, case ma_vi_tri when '' then '00' else ma_vi_tri end ma_vi_tri ,ma_vt,ton from  OPENROWSET('SQLNCLI', '" + remote + "','SET FMTONLY OFF; " +
                                "\n     exec [FAST].[HTP_FBOR2_A].dbo.Fast$DMSGESO$Stock ''" + ma_dvcs + "'',''" + ma_kho + "'',''" + ma_vi_triCondition + "'',''" + masp + "'' ') where 1=1 ";

            int sodo = db.updateQueryReturnInt(sql);






        }



        public static string ParseDataTableToJSon(DataTable dt)
        {
            string json = JsonConvert.SerializeObject(dt);
            return json;

        }

    }
}

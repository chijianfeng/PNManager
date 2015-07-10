using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipeNetManager.Login
{
    class AuthControl
    {
        //定义权限，root：可对数据进行修改等全部操作，标记为 0
        //定义权限，admin:只能对数据进行查看，标记为 1
        public static int AUTH_ROOT = 0;
        public static int AUTH_ADMIN = 1;

        private static AuthControl mAuthcontrol = null;

        private int mAuth;

        private AuthControl() { }

        public static AuthControl getInstance()
        {
            if(null==mAuthcontrol){
                mAuthcontrol = new AuthControl();
            }
            return mAuthcontrol;
        }

        public int getAuth() {
            return mAuth;
        }

        public void setAuth(int auth) {
            mAuth = auth;
        }

        //登陆用户名
        private string mUsername;
        public string UserName {
            set { mUsername = value; }
            get { return mUsername; }
        }

        //登陆当前时间
        private DateTime mLoginTime = DateTime.Now;
        public DateTime LoginTime {
            set { mLoginTime = value; }
            get {
                return mLoginTime; }
        }

        public string getLoginTime() {
            return mLoginTime.ToString("yyyy年MM月dd日,HH:mm");
        }
    }
}

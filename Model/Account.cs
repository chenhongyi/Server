﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Account
    {
        public Account() { }
        public Account(string imei, byte mobileType, string ipAddress)
        {
            SetIMEI(imei);
            SetIP(ipAddress);
            SetMobileType(mobileType);

        }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid AccountID { get; set; }
        public string UserName { get; set; }    //登录名
        public string RetailID { get; set; }    //第三方登录id
        public string Password { get; set; }    //密码
        public int RoleNumber { get; set; } = 0;     //已有角色数量
        public List<string> _imei { get; set; } //设备码  保存5个设备

        private void SetIMEI(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (_imei == null)
                _imei = new List<string>();
            if (_imei.Any())
            {
                foreach (var item in _imei)
                {
                    if (item == value)
                    {
                        return;
                    }
                }

                if (_imei.Count == 5)
                {
                    _imei.RemoveAt(0);
                }
                _imei.Add(value);
            }
        }
        public List<string> GetIMEI()
        {
            return _imei ?? null;
        }

        DateTime RegisterTime { get; set; } = DateTime.Now; //注册时间

        public List<string> _ipAddress { get; set; }//登录ip      保存10条

        private void SetIP(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            if (_ipAddress == null)
            { _ipAddress = new List<string>(); }
            if (_ipAddress.Any())
            {
                foreach (var item in _ipAddress)
                {
                    if (value == item)
                    {
                        return;
                    }
                }
            }
            if (_ipAddress.Count == 10)
            {
                _ipAddress.RemoveAt(0);
            }
            _ipAddress.Add(value);
        }
        public List<string> GetIp()
        {
            if (_ipAddress.Any())
            {
                return _ipAddress;
            }
            return null;
        }

        public DateTime LastLoginTime { get; set; } = DateTime.Now;//上次登录时间

        public bool IsDelete { get; set; } = false;     //逻辑删除

        public bool IsBan { get; set; } = false;    //逻辑禁用

        public long Gold { get; set; }  //代币

        public List<int> _mobileType { get; set; }
        private void SetMobileType(int value)
        {
            if (value >= 0)
            {
                if (_mobileType == null)
                {
                    _mobileType = new List<int>();
                }
                if (_mobileType.Any())
                {
                    foreach (var item in _mobileType)
                    {
                        if (item == value)
                        {
                            return;
                        }
                    }
                }
            }
            else return;
            if (_mobileType.Count == 5)
            {
                _mobileType.RemoveAt(0);
            }
            _mobileType.Add(value);
        }


        public List<int> GetMobileType()
        {
            if (_mobileType.Any())
            {
                return this._mobileType;
            }
            return null;
        }

    }
}

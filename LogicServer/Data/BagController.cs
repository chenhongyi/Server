using Microsoft.ServiceFabric.Data;
using Model;
using Model.Data.General;
using Model.Data.Npc;
using Model.ResponseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoData;
using GameEnum;
using Model.ViewModels;
using Model.MsgQueue;
using Shared.Serializers;
using Shared;
using static AutoData.Item;
using Shared.Websockets;
using PublicGate.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Model.Data.Business;

namespace LogicServer.Data
{
    public class BagController:BaseInstance<BagController>
    {

        /// <summary>
        /// 获取用户背包信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Bag> GetRoleBagItemInfoAsync(IReliableStateManager sm, Guid roleId)
        {
            return await DataHelper.GetRoleBagByRoleIdAsync(sm, roleId);
        }

        public async Task UpdateRoleBagItemInfoAsync(IReliableStateManager sm, Guid roleId, Bag bag, Bag old)
        {
            await DataHelper.UpdateRoleBagByRoleIdAsync(sm, roleId, bag, old);
        }


        public async Task<ChangeAvatarResult> ChangeAvatar(IReliableStateManager sm, UserRole role, int[] id, ChangeAvatarResult result)
        {
            var oldRole = role;
            #region 检查时装部位
            var parts = GetAvatarParts(role.Avatar);
            List<int> oldAvatar = new List<int>();  //换下来的时装id
            #endregion

            foreach (var i in id)
            {
                if (!(await CheckItemInBag(sm, role.Id, i, 1))) { result.Result = WsResult.NotItemInBag; return result; }
                var tmp = GetForId(i);
                if (tmp == null) { result.Result = WsResult.NotItemConfig; return result; }
                oldAvatar.AddRange(GetOldAvatarItemId(parts, tmp.Parts));
                foreach (var j in tmp.Attribute)    //增加属性
                {
                    var attr = role.UserAttr.First(a => a.UserAttrID == j.AttributeID);
                    checked
                    {
                        try
                        {
                            attr.Count += j.Count;
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

                }
            }
            foreach (var i in oldAvatar.Distinct())   //移除旧的属性
            {
                var tmp = GetForId(i);
                foreach (var item in tmp.Attribute)
                {
                    var attr = role.UserAttr.First(a => a.UserAttrID == item.AttributeID);
                    checked
                    {
                        try
                        {
                            attr.Count -= item.Count;
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            //检查背包剩余空间是否可以存放卸下来的时装
            foreach (var oldItem in oldAvatar)
            {
                if (!await CheckBagSpace(sm, role.Id, oldItem, 1))
                {
                    result.Result = WsResult.NotEnoughBagSpace;
                    return result;
                }
            }
            DataHelper.UpdateRoleInfoByRoleIdAsync(sm, role.Id, role, oldRole).Wait();   //保存玩家数据


            //移除背包中的时装
            foreach (var item in id)
            {
                await RemoveItemsAsync(sm, role.Id, item, 1);
            }
            foreach (var item in oldAvatar)
            {
                await AddItemToRoleBag(sm, role.Id, item, 1);  //增加旧时装到背包中
            }
            //更换时装到身上
            await UpdateAvaterAsync(sm, role, id, parts);

            ///构造返回
            var roleAttr = await DataHelper.GetRoleAttrAsync(sm, role.Id);
            if (roleAttr != null)
            {
                foreach (var art in roleAttr)
                {
                    result.ChangeAttr.Add(new Model.ResponseData.UserAttr()
                    {
                        Count = art.Count,
                        UserAttrID = art.UserAttrID
                    });
                }
            }

            var bgInfo = await GetRoleBagItemInfoAsync(sm, role.Id);
            result.BagInfo.CurUsedCell = bgInfo.CurUsedCell;
            result.BagInfo.MaxCellNumber = bgInfo.MaxCellNumber;
            foreach (var item in bgInfo.Items)
            {
                result.BagInfo.Items.Add(new LoadRoleBagInfo()
                {
                    CurCount = item.CurCount,
                    Id = item.Id,
                    OnSpace = item.OnSpace
                });
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="role"></param>
        /// <param name="id">道具集合</param>
        /// <param name="oldAvatar">key=部位  value=道具id</param>
        /// <returns></returns>
        private async Task UpdateAvaterAsync(IReliableStateManager sm, UserRole role, int[] id, Dictionary<int, int> oldAvatar)
        {
            var old = role;
            UpdateAvatarResult result = new UpdateAvatarResult();
            foreach (var i in oldAvatar)
            {
                foreach (var j in id)
                {
                    var tmp = GetItemById(j);
                    if (tmp.Parts.Contains(i.Key))
                    {
                        role.Avatar.Remove(i.Value);
                        if (!role.Avatar.Contains(j))
                        {
                            role.Avatar.Add(j);
                            result.Id.Add(j);
                        }
                    }
                }
            }
            await DataHelper.UpdateRoleInfoByRoleIdAsync(sm, role.Id, role, old);
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.UpdateAvatarResult, 1, role.Id, sm, data);
        }

        /// <summary>
        /// 拿到即将被替换的部位的时装的id
        /// </summary>
        /// <param name="dic">存储部位对应时装道具id的字典（1对多）  部位 ----  itemId </param>
        /// <param name="parts">道具的部位int集合</param>
        /// <returns></returns>
        private static List<int> GetOldAvatarItemId(Dictionary<int, int> dic, List<int> parts)
        {
            List<int> result = new List<int>();
            foreach (var k in parts)
            {
                if (dic.ContainsKey(k))
                {
                    var oldAvatarId = 0;
                    if (dic.TryGetValue(k, out oldAvatarId))
                    {
                        result.Add(oldAvatarId);
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 检查身上时装所占用的部位
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        private static Dictionary<int, int> GetAvatarParts(List<int> avatar)
        {
            Dictionary<int, int> parts = new Dictionary<int, int>(); //key=部位    value = 对应物品id 1对多
            for (int i = 0; i < avatar.Count; i++)
            {
                var tmp = GetForId(avatar[i]);
                if (tmp.Parts.Count > 1)    //时装对应多个部位
                {
                    foreach (var partId in tmp.Parts)
                    {
                        parts.Add(partId, tmp.Id);
                    }
                }
                else
                {
                    parts.Add(tmp.Parts[0], tmp.Id);
                }
            }
            return parts;
        }


        /// <summary>
        /// 增加金钱
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<BagInfo> GoldCtrl(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            BagInfo result = new BagInfo();
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            var money = item.Count * count;
            var bgInfo = await GetRoleBagItemInfoAsync(sm, roleId);
            var old = bgInfo;
            if (bgInfo != null)
            {
                var userMoney = bgInfo.Items.FirstOrDefault(p => p.Id == item.Id);
                if (userMoney != null)
                {
                    checked
                    {
                        try
                        {
                            userMoney.CurCount += money;    //更新值
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    await DataHelper.UpdateRoleBagByRoleIdAsync(sm, roleId, bgInfo, old);    //保存

                   

                    result.CurUsedCell = bgInfo.CurUsedCell;
                    result.MaxCellNumber = bgInfo.MaxCellNumber;
                    foreach (var i in bgInfo.Items)
                    {
                        result.Items.Add(new LoadRoleBagInfo()
                        {
                            CurCount = i.CurCount,
                            Id = i.Id,
                            OnSpace = i.OnSpace
                        });
                    }
                    return result;
                }

            }
            return null;
        }



        /// <summary>
        /// 多次使用同一个物品
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<BaseResponseData> UseItemsAsync(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            UseItemResult result = new UseItemResult();
            if (itemId <= 0) throw new ArgumentNullException();
            //1 检查传过来的道具类型是否都是可以使用的类型
            if (!(CheckItemType(itemId)))
            {
                result.Result = WsResult.ItemTypeError;
                return result;   //道具不可使用
            }
            if (!(await CheckItemInBag(sm, roleId, itemId, count)))
            {
                result.Result = WsResult.NotItemInBag;
                return result;
            }
            else
            {
                var item = GetItemById(itemId); //获取物品
                if (item != null)
                { //检查玩家级别是否可以使用该物品
                    if (await CheckItemLevel(sm, roleId, itemId))
                    {
                        switch (item.Type)
                        {
                            case 1://普通物品 可以使用
                                result = await UseGeneralItem(sm, roleId, itemId, count);
                                break;
                            case (int)GameEnum.Currency.Coin://金钱类物品
                            case (int)GameEnum.Currency.Gold://金钱类物品
                                                             //  case (int)GameEnum.Currency.Money://金钱类物品
                                result.BagInfo = await GoldCtrl(sm, roleId, itemId, count);
                                FinanceLogData loginfo = new FinanceLogData()
                                {
                                    Count = item.Sell.Count* count,
                                    EventName = item.Name,
                                    MoneyType = item.Sell.CurrencyID,
                                    Type = FinanceLogType.SellItem
                                };
                                await FinanceLogController.Instance.UpdateFinanceLog(sm, roleId, loginfo);
                                break;
                        }
                        return result;
                    }
                    else
                    {
                        result.Result = WsResult.NeedLevel;
                    }
                }
                else
                {
                    result.Result = WsResult.NotItemConfig;
                }
            }
            return result;
        }


        /// <summary>
        /// 检查道具类型是否是可以使用的类型
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private bool CheckItemType(int itemId)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return false;
            }
            if (item.Type != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查玩家级别是否达到使用要求
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private async Task<bool> CheckItemLevel(IReliableStateManager sm, Guid roleId, int itemId)
        {
            var item = GetItemById(itemId); //获取物品
            if (item != null)
            {
                int level = await DataHelper.GetRoleLevelAsync(sm, roleId);
                if (level <= 0 || item.Level > level)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查单个物品在背包中是否存在
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns>true 存在 false 不存在</returns>
        private async Task<bool> CheckItemInBag(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            var bgInfo = await GetRoleBagItemInfoAsync(sm, roleId);    //获取包中的道具
            var item = bgInfo.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return false;
            if (item.CurCount < count) return false;
            return true;
        }

        /// <summary>
        /// 使用普通道具
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<UseItemResult> UseGeneralItem(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            UseItemResult result = new UseItemResult();
            //检查背包空间
            if (!await CheckBagSpace(sm, roleId, itemId, count))
            {
                result.Result = WsResult.NotEnoughBagSpace;
                return result;
            }
            var itemCount = GetUsedItems(itemId, count);    //拿到将要造成的影响 道具 id  道具数量

            //删除使用的道具
            if (!(await RemoveItemsAsync(sm, roleId, itemId, count)))
            {
                result.Result = WsResult.NotEnoughItem;
                return result;
            }

            //区分一下 是增加属性  还是增加道具
            return await UseItemResult(sm, roleId, itemCount);
        }


        /// <summary>
        /// 使用道具的结果 加道具/属性/经验/金钱/打开UI
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="dicItem"></param>
        /// <returns></returns>
        public async Task<UseItemResult> UseItemResult(IReliableStateManager sm, Guid roleId, Dictionary<int, ItemTypeAndCount> dicItem)
        {
            if (dicItem == null) throw new ArgumentNullException();
            UseItemResult result = new UseItemResult();
            long exp = 0;
            foreach (var item in dicItem)
            {
                if (item.Key <= 0) continue;
                var itemId = item.Key;
                var itemCount = item.Value.Count;   //数量
                switch (item.Value.ItemType)
                {
                    case UseEffet.GetItem://获得道具
                        await AddItemToRoleBag(sm, roleId, itemId, itemCount);
                        break;
                    case UseEffet.AddAttribute://增加属性
                        var attr = await AddRoleAttrByUseItemAsync(sm, roleId, itemId, itemCount);
                        result.ChangeAttr.Add(new Model.ResponseData.UserAttr()
                        {
                            Count = attr.Count,
                            UserAttrID = attr.UserAttrID
                        });
                        break;
                    case UseEffet.AddCurrency:  //增加金钱
                        await AddMoneyByUseItemAsync(sm, roleId, itemId, itemCount);

                        break;
                    case UseEffet.AddExp://增加经验值
                        exp = await AddExpByUseItemAsync(sm, roleId, itemCount);    //只要最终数据 所以不是+=
                        break;
                }
            }
            var roleBag = await GetRoleBagItemInfoAsync(sm, roleId);
            foreach (var b in roleBag.Items)
            {
                result.BagInfo.Items.Add(new LoadRoleBagInfo()
                {
                    CurCount = b.CurCount,
                    Id = b.Id,
                    OnSpace = b.OnSpace
                });
            }
            result.BagInfo.CurUsedCell = roleBag.CurUsedCell;
            result.BagInfo.MaxCellNumber = roleBag.MaxCellNumber;
            result.Exp = exp;
            //把改变的数值返回给用户
            return result;
        }


        /// <summary>
        /// 增加经验值
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>

        private async Task<long> AddExpByUseItemAsync(IReliableStateManager sm, Guid roleId, long itemCount)
        {
            var role = await DataHelper.GetRoleInfoByRoleIdAsync(sm, roleId);
            if (role == null)
                throw new NullReferenceException();
            var old = role;
            role.Exp += itemCount;


            var level = Level.GetForLv(role.Level);
            //TODO
            //检查级别是否可以升级
            await DataHelper.UpdateRoleInfoByRoleIdAsync(sm, roleId, role, old);
            if (role.Exp >= level.Exp)
            {
                await DataHelper.RoleLevelUpAsync(sm, roleId);
            }
            return role.Exp;

        }




        /// <summary>
        /// 使用物品后增加金钱
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="moneyId"></param>
        /// <param name="moneyCount"></param>
        /// <returns></returns>
        private async Task<LoadRoleBagInfo> AddMoneyByUseItemAsync(IReliableStateManager sm, Guid roleId, int moneyId, long moneyCount)
        {
            LoadRoleBagInfo result = new LoadRoleBagInfo();
            var roleBg = await GetRoleBagItemInfoAsync(sm, roleId);
            var oldbg = roleBg;
            if (roleBg == null) throw new NullReferenceException();

            var money = roleBg.Items.First(p => p.Id == moneyId);
            if (money != null)
            {
                checked
                {
                    try
                    {
                        money.CurCount += moneyCount;
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            await UpdateRoleBagItemInfoAsync(sm, roleId, roleBg, oldbg);   //保存
            FinanceLogData loginfo = new FinanceLogData()
            {
                Count = moneyCount,
                EventName = "",
                MoneyType = moneyId,
                Type = FinanceLogType.SellItem
            };
            await FinanceLogController.Instance.UpdateFinanceLog(sm, roleId, loginfo);
            result.CurCount = money.CurCount;
            result.OnSpace = 0;
            result.Id = money.Id;
            return result;
        }


        /// <summary>
        /// 使用道具后增加用户属性
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<Model.Data.Npc.UserAttr> AddRoleAttrByUseItemAsync(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            var role = await DataHelper.GetRoleInfoByRoleIdAsync(sm, roleId);   //获取数据
            var old = role;
            Model.Data.Npc.UserAttr attr = new Model.Data.Npc.UserAttr();
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            foreach (var j in item.Attribute)
            {
                var tmp = role.UserAttr.FirstOrDefault(p => p.UserAttrID == j.AttributeID);
                if (tmp != null)
                {
                    checked
                    {
                        try
                        {
                            tmp.Count += j.Count;
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    attr = tmp;
                }
            }
            await DataHelper.UpdateRoleInfoByRoleIdAsync(sm, roleId, role, old); //更新数据
            //调用方法  通知客户端
            return attr;
        }



        /// <summary>
        /// 添加道具进背包
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<AddItemResult> AddItemToRoleBag(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            AddItemResult result = new AddItemResult();
            if (!await CheckBagSpace(sm, roleId, itemId, count))
            {
                result.Result = WsResult.NotEnoughBagSpace;
                return result;
            }
            var itemCount = new Dictionary<int, long>();
            itemCount.Add(itemId, count);
            result = await AddItemToRoleBag(sm, roleId, itemCount);
            //TODO
            //产生一个通知  通知下发给用户
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.AddItemResult, 1, roleId, sm, data);    //写入队列
            return result;

        }


        /// <summary>
        /// 添加道具进用户背包
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        private async Task<AddItemResult> AddItemToRoleBag(IReliableStateManager sm, Guid roleId, Dictionary<int, long> itemCount)
        {
            AddItemResult result = new AddItemResult();
            var roleInfo = await DataHelper.GetRoleInfoByRoleIdAsync(sm, roleId);
            var roleBg = await GetRoleBagItemInfoAsync(sm, roleId);
            if (roleBg == null || roleInfo == null) throw new NullReferenceException();
            var old = roleBg;
            var oldRoleInfo = roleInfo;
            long oldSocialStatus = 0;
            foreach (var i in roleBg.Items)
            {
                var item = GetItemById(i.Id);
                if (item != null)
                {
                    checked
                    {
                        try
                        {
                            if (item.Superposition == 0)  //身价值不叠加
                            {

                                oldSocialStatus += item.Status;
                            }
                            else if (item.Superposition == 1)  //叠加
                            {
                                oldSocialStatus += (item.Status * i.CurCount);
                            }
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            long socialStatus = roleInfo.SocialStatus - oldSocialStatus;    //除去背包中所有物品带来的身价值

            var curBagCell = roleBg.CurUsedCell;    //当前背包所用格子
            foreach (var b in itemCount)
            {
                var tmpItem = roleBg.Items.FirstOrDefault(p => p.Id == b.Key);
                var itemTemplate = GetItemById(b.Key);
                checked
                {
                    try
                    {
                        if (tmpItem != null)    //存在
                        {

                            curBagCell -= tmpItem.OnSpace;  //去除原先占用的格子
                            if (itemTemplate.Count == 1)
                            {
                                tmpItem.Id = b.Key;

                                tmpItem.OnSpace = tmpItem.CurCount += b.Value;  //当前数量和空间
                            }
                            else
                            {
                                tmpItem.Id = b.Key;
                                tmpItem.CurCount += b.Value;    //当前数量
                                tmpItem.OnSpace = (tmpItem.CurCount / itemTemplate.Count) + 1;
                            }
                            curBagCell += tmpItem.OnSpace;  //更新加入物品后的格子
                        }
                        else
                        {
                            Model.Data.General.Item i = new Model.Data.General.Item();
                            if (itemTemplate.Count == 1)
                            {
                                i.Id = b.Key;
                                i.OnSpace = i.CurCount = b.Value;   //当前数量和空间
                            }
                            else
                            {
                                i.Id = b.Key;
                                i.CurCount += b.Value;
                                i.OnSpace = (i.CurCount / itemTemplate.Count) + 1;
                            }
                            curBagCell += i.OnSpace;  //更新加入物品后的格子
                            roleBg.Items.Add(i);
                        }
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

            }
            roleBg.CurUsedCell = curBagCell;

            await DataHelper.UpdateRoleBagByRoleIdAsync(sm, roleId, roleBg, old);    //保存
            //TODO  
            foreach (var status in roleBg.Items)
            {
                var item = GetItemById(status.Id);
                if (item != null)
                {
                    checked
                    {
                        try
                        {
                            if (item.Superposition == 0)
                            {
                                socialStatus += item.Status;
                            }
                            else
                            {
                                socialStatus += (item.Status * status.CurCount);
                            }
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            roleInfo.SocialStatus = socialStatus;   //身价值更新
            await DataHelper.UpdateRoleInfoByRoleIdAsync(sm, roleId, roleInfo, oldRoleInfo); //保存


            result.ShenJia = socialStatus;
            result.BagInfo.CurUsedCell = roleBg.CurUsedCell;
            foreach (var i in roleBg.Items)
            {
                result.BagInfo.Items.Add(new LoadRoleBagInfo()
                {
                    OnSpace = i.OnSpace,
                    CurCount = i.CurCount,
                    Id = i.Id
                });
            }
            return result;
        }

        /// <summary>
        /// 检查背包空间
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="items">增加的道具</param>
        /// <returns>true 空间足够  false 空间不够</returns>
        private async Task<bool> CheckBagSpace(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            var roleBg = await GetRoleBagItemInfoAsync(sm, roleId);
            if (roleBg.CurUsedCell >= roleBg.MaxCellNumber)
            {
                return false;
            }
            Int64 newOnSpace = 0;
            var item = GetItemById(itemId);
            if (item == null)
            {
                return false;
            }
            var isExists = roleBg.Items.FirstOrDefault(i => i.Id == itemId);
            if (isExists != null)
            {//有同类物品
                if (item.Count == 1)    //不可叠加
                {
                    newOnSpace += count;
                }
                else
                {   //可叠加
                    var oldSpace = isExists.OnSpace;    //旧的占用空间
                    var curSpace = (isExists.CurCount + count) / item.Count;   //物品增加后占用空间
                    if (curSpace > oldSpace)
                    {
                        newOnSpace += (curSpace - oldSpace);
                    }
                }
            }
            else
            {
                if (item.Count == 1)
                {
                    newOnSpace += count;
                }
                else
                {
                    newOnSpace = count / item.Count;   //物品增加后占用空间
                }
            }
            if ((newOnSpace + roleBg.CurUsedCell) > roleBg.MaxCellNumber)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获得使用道具后得到属性/物品/经验/金钱/打开ui 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>

        private Dictionary<int, ItemTypeAndCount> GetUsedItems(int itemId, long count)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            Dictionary<int, ItemTypeAndCount> dic = new Dictionary<int, ItemTypeAndCount>();
            foreach (var effet in item.UseEffet)
            {
                ItemTypeAndCount typeCount = new ItemTypeAndCount();
                typeCount.ItemType = UseEffet.GetItem;
                checked
                {
                    try
                    {
                        typeCount.Count = effet.Value2 * count;
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                dic.Add(effet.Value1, typeCount);
            }
            return dic;
        }




        /// <summary>
        /// 检查背包是否已达到空间上限
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns>true达到上限  false未达到上限</returns>
        private async Task<bool> CheckItemSpace(IReliableStateManager sm, Guid roleId)
        {
            var bgInfo = await DataHelper.GetRoleBagByRoleIdAsync(sm, roleId);
            if (bgInfo.CurUsedCell >= bgInfo.MaxCellNumber)
                return true;
            return false;
        }



        /// <summary>
        /// 出售商品
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>

        public async Task<SellItemResult> SellItemsAsync(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            try
            {
                SellItemResult result = new SellItemResult();
                var bgInfo = await DataHelper.GetRoleBagByRoleIdAsync(sm, roleId);
                if (bgInfo == null)
                {
                    throw new NullReferenceException();
                }
                var old = bgInfo;
                var item = GetItemById(itemId);
                if (item == null) { result.Result = WsResult.NotItemConfig; return result; }

                var money = item.Sell;

                if (!await RemoveItemsAsync(sm, roleId, itemId, count)) { result.Result = WsResult.RemoveItemErr; return result; }

                long financeMoney = 0;
                var curMoney = bgInfo.Items.FirstOrDefault(p => p.Id == money.CurrencyID);
                checked
                {
                    try
                    {
                        financeMoney += (money.Count * count);
                        curMoney.CurCount += (money.Count * count);
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                await DataHelper.UpdateRoleBagByRoleIdAsync(sm, roleId, bgInfo, old);     //保存数值
                                                                                          //金钱变动消息
                                                                                          // await MsgSender.Instance.GoldUpdate(sm, roleId, curMoney.CurCount, (Currency)money.CurrencyID);

                FinanceLogData loginfo = new FinanceLogData()
                {
                    Count = financeMoney,
                    EventName = item.Name,
                    MoneyType = money.CurrencyID,
                    Type = FinanceLogType.SellItem
                };
                await FinanceLogController.Instance.UpdateFinanceLog(sm, roleId, loginfo);

                var retBg = await DataHelper.GetRoleBagByRoleIdAsync(sm, roleId);
                var retRoInfo = await DataHelper.GetRoleInfoByRoleIdAsync(sm, roleId);
                result.BagInfo.CurUsedCell = retBg.CurUsedCell;
                result.BagInfo.MaxCellNumber = retBg.MaxCellNumber;
                foreach (var i in retBg.Items)
                {
                    result.BagInfo.Items.Add(new LoadRoleBagInfo()
                    {
                        CurCount = i.CurCount,
                        Id = i.Id,
                        OnSpace = i.OnSpace
                    });
                }

                result.ShenJia = retRoInfo.SocialStatus;
                return result;

            }
            catch (Exception ex)
            {
                //日志
                throw ex;
            }

        }


        /// <summary>
        /// 移除背包中的道具
        /// </summary>s
        /// <param name="sm"></param>
        /// <param name="item"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveItemsAsync(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            try
            {
                var item = GetItemById(itemId);
                if (item == null)
                    return false;
                //检查包中是否有以上道具
                var roleInfo = await DataHelper.GetRoleInfoByRoleIdAsync(sm, roleId);
                if (roleInfo != null)
                {
                    var oldRoleInfo = roleInfo;
                    var bgInfo = await DataHelper.GetRoleBagByRoleIdAsync(sm, roleId);
                    if (bgInfo != null)
                    {
                        //计算身价值
                        var old = bgInfo;

                        var i = bgInfo.Items.FirstOrDefault(p => p.Id == itemId);
                        if (i == null)
                        {
                            return false;
                        }
                        else
                        {
                            //减少数量  减少到0的时候  删除 暂时无此需求
                            if (i.CurCount > count)
                            {
                                if (item.Superposition == 0)
                                {
                                    roleInfo.SocialStatus -= item.Status;   //减少身价值
                                }
                                else
                                {
                                    roleInfo.SocialStatus -= (item.Status * count);
                                }

                                var newCount = i.CurCount - count;
                                i.CurCount = newCount;         //重新计算数量
                            }
                            else if (i.CurCount == count)
                            {
                                bgInfo.Items.Remove(i);
                                if (item.Superposition == 0)
                                {
                                    roleInfo.SocialStatus -= item.Status;   //减少身价值
                                }
                                else
                                {
                                    roleInfo.SocialStatus -= (item.Status * count);
                                }
                            }
                            else
                            {
                                return false;   //数量不够 无法删除
                            }
                            await DataHelper.UpdateRoleBagByRoleIdAsync(sm, roleId, bgInfo, old); //更新背包信息
                            await DataHelper.UpdateRoleInfoByRoleIdAsync(sm, roleId, roleInfo, oldRoleInfo);

                            //道具变动消息
                            await MsgSender.Instance.ItemUpdate(sm, roleId, itemId, i.CurCount);
                            //身价变动消息
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //TODO日志
                throw ex;
            }
        }


        public AutoData.Item GetItemById(int id)
        {
            return AutoData.Item.GetForId(id);
        }
    }
}

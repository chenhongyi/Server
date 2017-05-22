using Microsoft.ServiceFabric.Data;
using Model.Data.General;
using Model.Data.Npc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data
{
    public class BagController
    {
        private Bag _box;   //当前角色的背包

        /// <summary>
        /// 查看背包中所有道具
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Bag> GetItemsAsync(IReliableStateManager sm, Guid roleId)
        {
            return await DataHelper.GetUserBagByRoleIdAsync(sm, roleId);
        }

        /// <summary>
        /// 移除背包中的道具
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="item"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task RemoveItemsAsync(IReliableStateManager sm, List<Item> item, Guid roleId)
        {
            try
            {
                if (item != null)
                {
                    if (item.Any())
                    {
                        //检查包中是否有以上道具
                        this._box = await DataHelper.GetUserBagByRoleIdAsync(sm, roleId);
                        if (this._box != null)
                        {
                            int count = 0;
                            foreach (var a in this._box.Items)
                            {
                                foreach (var b in item)
                                {
                                    //TODO
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO日志
                throw ex;
            }
        }

        /// <summary>
        /// 添加道具
        /// </summary>
        /// <param name="item"></param>
        public async Task AddItemsAsync(IReliableStateManager sm, List<Item> item, Guid roleId)
        {
            try
            {
                if (item != null)
                {
                    if (item.Any())
                    {   //检查格子数量够不够
                        this._box = await DataHelper.GetUserBagByRoleIdAsync(sm, roleId);
                        if (this._box != null)
                        {
                            if (this._box.MaxCellNumber < this._box.CurUsedCell + item.Count) //简单粗暴的检查方法 不考虑叠加问题  //TODO 需要修改
                            {
                                //格子不够
                                throw new Exception("背包格子不够");
                            }
                            //检查背包中是否有相同id商品
                            Dictionary<long, int> sameDic = new Dictionary<long, int>();//相同的 id 数量
                            Dictionary<long, int> differentDic = new Dictionary<long, int>();  //不同的 id 数量
                            foreach (var i in this._box.Items)//循环背包中的所有物品
                            {
                                foreach (var j in item) //循环要加入背包中的所有物品
                                {
                                    if (i.Id == j.Id)
                                    {
                                        sameDic.Add(j.Id, j.CurCount);
                                        continue;
                                    }
                                    differentDic.Add(j.Id, j.CurCount);
                                }
                            }
                            if (sameDic.Any())    //背包中有相同物品
                            {
                                foreach (var i in sameDic)  //添加背包中有的相同商品
                                {
                                    //相同物品数量++
                                    var existItem = this._box.Items.First(p => p.Id == i.Key);
                                    //检查是否可以叠加
                                    if (existItem.MaxCount == 1)
                                    {
                                        //不可以叠加 道具加1 占用空间+1
                                        if (this._box.MaxCellNumber < (this._box.CurUsedCell + existItem.OnSpace + i.Value))
                                        {
                                            throw new Exception("背包格子不够");
                                        }
                                        this._box.Items.Remove(existItem);   //删除旧的 
                                        existItem.CurCount += i.Value;
                                        existItem.OnSpace += i.Value;
                                        this._box.Items.Add(existItem);      //增加新的 更新
                                    }
                                    else
                                    {
                                        //可以叠加
                                        if (existItem.CurCount + i.Value <= existItem.MaxCount)
                                        {
                                            //占用空间 //333/999 = 0;  1000/999=1
                                            int space = ((existItem.CurCount + i.Value) / existItem.MaxCount) + 1;
                                            if (this._box.CurUsedCell + space >= this._box.MaxCellNumber)       //背包空间不足
                                            {
                                                throw new Exception("背包格子不够");
                                            }
                                            else
                                            {
                                                this._box.Items.Remove(existItem);  //移除旧的  
                                                existItem.CurCount += i.Value;
                                                existItem.OnSpace = space;
                                                this._box.Items.Add(existItem);     //加新的
                                            }
                                        }
                                    }
                                }
                                //把不同的商品单独取出来 加进去
                                List<Item> differentItems = new List<Item>();
                                foreach (var a in item)
                                {
                                    foreach (var b in differentDic)
                                    {
                                        if (a.Id == b.Key)
                                        {
                                            differentItems.Add(a);
                                            continue;
                                        }
                                    }
                                }

                                this._box.Items.AddRange(differentItems);//添加背包中没有的物品
                                await DataHelper.UpdateUserBagByRoleIdAsync(sm, roleId, this._box);
                            }
                            else
                            {
                                this._box.Items.AddRange(item);
                                //保存到数据库中
                                await DataHelper.UpdateUserBagByRoleIdAsync(sm, roleId, this._box);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }
    }
}

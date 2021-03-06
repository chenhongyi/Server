namespace GameEnum
{
    /// <summary>
    /// 性别
    /// <summary>
    public enum Sex
    {
       /// <summary>
       /// 男性
       /// <summary>
       Male = 1,
       /// <summary>
       /// 女性
       /// <summary>
       Female = 2,
    }
    /// <summary>
    /// avater部位
    /// <summary>
    public enum AvaterPart
    {
       /// <summary>
       /// 头部
       /// <summary>
       Head = 1,
       /// <summary>
       /// 上身
       /// <summary>
       Body = 2,
       /// <summary>
       /// 下身
       /// <summary>
       Pants = 3,
    }
    /// <summary>
    /// 天赋
    /// <summary>
    public enum Talent
    {
       /// <summary>
       /// 体质
       /// <summary>
       Vit = 1,
       /// <summary>
       /// 智力
       /// <summary>
       Int = 2,
       /// <summary>
       /// 专注
       /// <summary>
       Dex = 3,
       /// <summary>
       /// 亲和
       /// <summary>
       Kind = 4,
       /// <summary>
       /// 魅力
       /// <summary>
       Cha = 5,
    }
    /// <summary>
    /// 行业
    /// <summary>
    public enum Industry
    {
       /// <summary>
       /// 贸易
       /// <summary>
       Trade = 1,
       /// <summary>
       /// 科技
       /// <summary>
       Techonology = 2,
       /// <summary>
       /// 金融
       /// <summary>
       Financial = 3,
       /// <summary>
       /// 主播
       /// <summary>
       Anchor = 4,
       /// <summary>
       /// 模特
       /// <summary>
       Model = 5,
       /// <summary>
       /// 影视
       /// <summary>
       Film = 6,
       /// <summary>
       /// 快餐
       /// <summary>
       FastFood = 7,
       /// <summary>
       /// 料理
       /// <summary>
       Cooking = 8,
       /// <summary>
       /// 酒店
       /// <summary>
       Hotel = 9,
       /// <summary>
       /// 食品
       /// <summary>
       Food = 10,
       /// <summary>
       /// 服装
       /// <summary>
       Cloth = 11,
       /// <summary>
       /// 家电
       /// <summary>
       Applicance = 12,
       /// <summary>
       /// 采集
       /// <summary>
       Collection = 13,
       /// <summary>
       /// 冶炼
       /// <summary>
       Smelt = 14,
       /// <summary>
       /// 建筑
       /// <summary>
       Build = 15,
    }
    /// <summary>
    /// 家具
    /// <summary>
    public enum Furniture
    {
       /// <summary>
       /// 地板
       /// <summary>
       Floor = 1,
       /// <summary>
       /// 墙纸
       /// <summary>
       Wallpaper = 2,
       /// <summary>
       /// 床
       /// <summary>
       Bed = 3,
       /// <summary>
       /// 茶几
       /// <summary>
       TeaTable = 4,
       /// <summary>
       /// 沙发
       /// <summary>
       Sofa = 5,
       /// <summary>
       /// 电视柜
       /// <summary>
       TVStands = 6,
       /// <summary>
       /// 衣柜
       /// <summary>
       Wardrobe = 7,
       /// <summary>
       /// 墙面装饰
       /// <summary>
       WallDecoration = 8,
       /// <summary>
       /// 地毯
       /// <summary>
       Rug = 9,
       /// <summary>
       /// 桌子
       /// <summary>
       Table = 10,
       /// <summary>
       /// 椅子
       /// <summary>
       Chair = 11,
       /// <summary>
       /// 摆件
       /// <summary>
       Decoration = 12,
       /// <summary>
       /// 窗
       /// <summary>
       Window = 13,
       /// <summary>
       /// 门
       /// <summary>
       Door = 14,
    }
    /// <summary>
    /// 货币
    /// <summary>
    public enum Currency
    {
       /// <summary>
       /// 钞票
       /// <summary>
       Money = 1,
       /// <summary>
       /// 金砖
       /// <summary>
       Gold = 2,
       /// <summary>
       /// 创客币
       /// <summary>
       Coin = 3,
    }
    /// <summary>
    /// 建筑
    /// <summary>
    public enum Build
    {
       /// <summary>
       /// 公司
       /// <summary>
       Company = 1,
       /// <summary>
       /// 公园
       /// <summary>
       Park = 2,
       /// <summary>
       /// 银行
       /// <summary>
       Bank = 3,
       /// <summary>
       /// 商会
       /// <summary>
       Shanghui = 4,
       /// <summary>
       /// 公寓
       /// <summary>
       Apartment = 5,
       /// <summary>
       /// 交易中心
       /// <summary>
       Tarding = 6,
    }
    /// <summary>
    /// 使用效果
    /// <summary>
    public enum UseEffet
    {
       /// <summary>
       /// 获得道具：道具ID，数量
       /// <summary>
       GetItem = 1,
       /// <summary>
       /// 增加属性：属性ID，值
       /// <summary>
       AddAttribute = 2,
       /// <summary>
       /// 增加货币：货币ID，值
       /// <summary>
       AddCurrency = 3,
       /// <summary>
       /// 增加经验
       /// <summary>
       AddExp = 4,
       /// <summary>
       /// 打开UI
       /// <summary>
       OpenUI = 5,
    }
    /// <summary>
    /// 道具类型
    /// <summary>
    public enum ItemKing
    {
       /// <summary>
       /// 道具
       /// <summary>
       Item = 1,
       /// <summary>
       /// 时装
       /// <summary>
       Avater = 2,
       /// <summary>
       /// 家具
       /// <summary>
       Furniture = 3,
       /// <summary>
       /// 货币
       /// <summary>
       Money = 99,
    }
    /// <summary>
    /// 部门类型
    /// <summary>
    public enum DepartMentType
    {
       /// <summary>
       /// 财务部
       /// <summary>
       Finance = 1,
       /// <summary>
       /// 人事部
       /// <summary>
       Personnel = 2,
       /// <summary>
       /// 市场部
       /// <summary>
       Market = 3,
       /// <summary>
       /// 投资部
       /// <summary>
       Investment = 4,
    }
    /// <summary>
    /// 财务部日志
    /// <summary>
    public enum FinanceLog
    {
       /// <summary>
       /// 捐献金砖
       /// <summary>
       DonateGold = 1,
       /// <summary>
       /// 领取店铺收益
       /// <summary>
       GetGain = 2,
       /// <summary>
       /// 够买时装
       /// <summary>
       BuyClothes = 3,
       /// <summary>
       /// 卖出物品
       /// <summary>
       SellItem = 4,
       /// <summary>
       /// 购买物品
       /// <summary>
       BuyItems = 5,
       /// <summary>
       /// 升级公司
       /// <summary>
       LvUpCompany = 6,
       /// <summary>
       /// 升级部门
       /// <summary>
       LvUpDepartment = 7,
       /// <summary>
       /// 购买土地
       /// <summary>
       BuyLand = 8,
       /// <summary>
       /// 创建店铺
       /// <summary>
       CreateBuild = 9,
       /// <summary>
       /// 销毁店铺
       /// <summary>
       DestoryBuild = 10,
    }
    /// <summary>
    /// 部门升级效果类型
    /// <summary>
    public enum DepartMentLvUpType
    {
       /// <summary>
       /// 员工上限
       /// <summary>
       StaffLimit = 1,
       /// <summary>
       /// 人才市场上限
       /// <summary>
       TalentLvLimit = 2,
       /// <summary>
       /// 地产上限
       /// <summary>
       RealestateLimit = 3,
       /// <summary>
       /// 店铺上限
       /// <summary>
       StoreLimit = 4,
       /// <summary>
       /// 扩建上限
       /// <summary>
       ExtensionLimit = 5,
       /// <summary>
       /// 店铺加成
       /// <summary>
       StoreAddtion = 6,
       /// <summary>
       /// 宣传次数
       /// <summary>
       PropagandaCounts = 7,
       /// <summary>
       /// 策略打击次数
       /// <summary>
       StrategicCounts = 8,
       /// <summary>
       /// 公司采购
       /// <summary>
       PurchaseCounts = 9,
       /// <summary>
       /// 贷款等级
       /// <summary>
       loanLvLimit = 10,
       /// <summary>
       /// 兑换次数
       /// <summary>
       MakerCoinCounts = 11,
    }
    /// <summary>
    /// 店铺类型
    /// <summary>
    public enum ShopType
    {
       /// <summary>
       /// 烧烤店
       /// <summary>
       BBQ = 1,
       /// <summary>
       /// 披萨店
       /// <summary>
       Pizza = 2,
       /// <summary>
       /// 西餐厅
       /// <summary>
       WestFood = 3,
       /// <summary>
       /// 中餐厅
       /// <summary>
       ChinaFood = 4,
       /// <summary>
       /// 手机店
       /// <summary>
       Phone = 5,
       /// <summary>
       /// 电玩店
       /// <summary>
       ElectricPlay = 6,
       /// <summary>
       /// 电脑店
       /// <summary>
       Computer = 7,
       /// <summary>
       /// 音响城
       /// <summary>
       Sound = 8,
       /// <summary>
       /// 皮具店
       /// <summary>
       Leather = 9,
       /// <summary>
       /// 服装店
       /// <summary>
       Clothe = 10,
       /// <summary>
       /// 婚纱店
       /// <summary>
       WeddingDress = 11,
       /// <summary>
       /// 旗袍
       /// <summary>
       Cheongsam = 12,
       /// <summary>
       /// 棋牌店
       /// <summary>
       MahJong = 13,
       /// <summary>
       /// 台球店
       /// <summary>
       Snooker = 14,
       /// <summary>
       /// 酒吧
       /// <summary>
       Bar = 15,
       /// <summary>
       /// KTV
       /// <summary>
       KTV = 16,
       /// <summary>
       /// 珠宝店
       /// <summary>
       Jewelry = 17,
       /// <summary>
       /// 名表店
       /// <summary>
       Watch = 18,
       /// <summary>
       /// 文玩店
       /// <summary>
       Culture = 19,
       /// <summary>
       /// 古董店
       /// <summary>
       Antique = 20,
    }
    /// <summary>
    /// 土地状态
    /// <summary>
    public enum MapStatus
    {
       /// <summary>
       /// 空白土地
       /// <summary>
       Empty = 0,
       /// <summary>
       /// 已被购买土地
       /// <summary>
       Saled = 1,
       /// <summary>
       /// 已有建筑土地
       /// <summary>
       Building = 2,
       /// <summary>
       /// 罚没土地
       /// <summary>
       BanningLand = 3,
       /// <summary>
       /// 罚没建筑
       /// <summary>
       BanningBuild = 4,
    }
    /// <summary>
    /// 返回码
    /// <summary>
    public enum WsResult
    {
       /// <summary>
       /// 正常
       /// <summary>
       Success = 0,
       /// <summary>
       /// 重名
       /// <summary>
       DuplicationName = 1,
       /// <summary>
       /// 过多角色
       /// <summary>
       MoreRoles = 2,
       /// <summary>
       /// 参数错误
       /// <summary>
       ParamsError = 3,
       /// <summary>
       /// Token不存在
       /// <summary>
       TokenNotExists = 4,
       /// <summary>
       /// Token过期
       /// <summary>
       TokenTimeOut = 5,
       /// <summary>
       /// 账号不存在
       /// <summary>
       NotAccount = 6,
       /// <summary>
       /// 角色id为空
       /// <summary>
       RoleIdIsNull = 7,
       /// <summary>
       /// 角色不存在
       /// <summary>
       RoleNotExists = 8,
       /// <summary>
       /// 账号中无角色
       /// <summary>
       NotRole = 9,
       /// <summary>
       /// 道具类型不符
       /// <summary>
       ItemTypeError = 10,
       /// <summary>
       /// 背包中没有该道具
       /// <summary>
       NotItemInBag = 11,
       /// <summary>
       /// 没有道具配置信息
       /// <summary>
       NotItemConfig = 12,
       /// <summary>
       /// 级别不够
       /// <summary>
       NeedLevel = 13,
       /// <summary>
       /// 背包空间不够
       /// <summary>
       NotEnoughBagSpace = 14,
       /// <summary>
       /// 用户背包错误
       /// <summary>
       RoleBagErr = 15,
       /// <summary>
       /// 道具数量不足
       /// <summary>
       NotEnoughItem = 16,
       /// <summary>
       /// 删除道具出错
       /// <summary>
       RemoveItemErr = 17,
       /// <summary>
       /// 公司升级失败(条件不足)
       /// <summary>
       CompanyLvUpFailed = 18,
       /// <summary>
       /// 没有找到公司
       /// <summary>
       NotFoundCompany = 19,
       /// <summary>
       /// 部门无效(没有找到部门)
       /// <summary>
       DepartmentInvalid = 20,
       /// <summary>
       /// 部门升级失败
       /// <summary>
       DepartmentLvUpErr = 21,
       /// <summary>
       /// 部门升级失败(条件没满足)
       /// <summary>
       DepartmentLvUpFailed = 22,
       /// <summary>
       /// 配置文件错误
       /// <summary>
       ConfigErr = 23,
       /// <summary>
       /// 不能创建过多公司
       /// <summary>
       MoreCompany = 24,
       /// <summary>
       /// 不能创建过多部门
       /// <summary>
       MoreDepartment = 25,
       /// <summary>
       /// 需要大于0的正整数
       /// <summary>
       PositiveInteger = 26,
       /// <summary>
       /// 数字错误
       /// <summary>
       NumberErr = 27,
       /// <summary>
       /// 财务部日志不存在
       /// <summary>
       FinanceLogNotExists = 28,
       /// <summary>
       /// 空的地图信息
       /// <summary>
       NullMapCell = 29,
       /// <summary>
       /// 购买土地出错
       /// <summary>
       BuyLandErr = 30,
       /// <summary>
       /// 土地已被卖出
       /// <summary>
       LandIsSaled = 31,
       /// <summary>
       /// 未定义错误
       /// <summary>
       Error = 10000,
       /// <summary>
       /// 没有处理程序
       /// <summary>
       NoneActionFunc = 10001,
       /// <summary>
       /// 没有足够的金钱
       /// <summary>
       NotEnoughMoney = 32,
       /// <summary>
       /// 创建店铺失败
       /// <summary>
       CreateShopFailed = 33,
       /// <summary>
       /// 非空的土地
       /// <summary>
       UnNullLand = 34,
       /// <summary>
       /// 无主的土地
       /// <summary>
       UnOwnLand = 35,
       /// <summary>
       /// 罚没的土地
       /// <summary>
       BanningLand = 36,
       /// <summary>
       /// 罚没的建筑
       /// <summary>
       BanningBuild = 37,
       /// <summary>
       /// 非你拥有的土地
       /// <summary>
       NotYourLand = 38,
       /// <summary>
       /// 名下没有建筑
       /// <summary>
       NoneBuilds = 39,
       /// <summary>
       /// 建筑不存在
       /// <summary>
       BuildIsNotExists = 40,
       /// <summary>
       /// 建筑不属于角色
       /// <summary>
       BuildIdNotOwnRole = 41,
       /// <summary>
       /// 扩建等级不够
       /// <summary>
       NeedExtendLevel = 42,
    }
}
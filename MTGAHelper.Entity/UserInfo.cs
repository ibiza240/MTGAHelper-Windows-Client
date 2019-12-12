using AutoMapper;
using MTGAHelper.Lib.Config;

namespace MTGAHelper.Lib.Model
{
    public class UserInfo : ConfigModelUser
    {
        //public bool CollectionOnDisk { get; set; }
        //public int CollectionSizeInMemory { get; set; }
        public double LastLoginHours { get; set; }
        public int NbDecksMonitored { get; set; }

        public UserInfo()
        {
        }

        //public UserInfo(ConfigModelUser configModel)
        //{
        //    this.CollectionDate = configModel.CollectionDate;
        //    this.CustomDecks = configModel.CustomDecks;
        //    this.Id = configModel.Id;
        //    this.LastLogin = configModel.LastLogin;
        //    this.NbLogin = configModel.NbLogin;
        //    this.PriorityByDeckId = configModel.PriorityByDeckId;
        //    this.ScrapersActive = configModel.ScrapersActive;
        //    this.ThemeIsDark = configModel.ThemeIsDark;
        //    this.Weights = configModel.Weights;
        //    this.MtgaUserProfile = configModel.MtgaUserProfile;
        //}

        public static UserInfo FromConfigModelUser(ConfigModelUser configModel)
        {
            var res = Mapper.Map<UserInfo>(configModel);
            return res;
        }

        ////public UserInfo With(int collectionCardsTotal, int count, double totalHours, bool isCollectionOnDisk)
        //public UserInfo With(int count, double totalHours)
        //{
        //    //CollectionSizeInMemory = collectionCardsTotal;
        //    NbDecksMonitored = count;
        //    LastLoginHours = totalHours;
        //    //CollectionOnDisk = isCollectionOnDisk;
        //    return this;
        //}
    }
}
